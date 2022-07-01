using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
public class KamikaseController : BaseEnemyController
{
    private enum states
    {
        Patrol,
        Chase,
        Explode
    }
    [SerializeField] private Transform[] wayPoints;

    private FSM<states> _fsm;
    private KamikaseModel _model;

    public event Action<Vector3> onRotate;
    public event Action<Vector3, float> onMove;
    public event Action<bool> onDetect;
    public event Action onExplode;
    protected override void Awake()
    {
        _model = GetComponent<KamikaseModel>();
        base.Awake();
    }
    protected override void Start()
    {
        _model.SuscribeEvents(this);
        base.Start();
    }
    protected override void InitBehaviours()
    {
        var chase = new Chase(_actualTarget.transform ,transform, _actualTarget, obstacleAvoidanceSO.PredictionTime);
        behaviours.Add(Steerings.Chase, chase);
        var seek = new Seek(_actualTarget.transform, transform);
        behaviours.Add(Steerings.Seek, seek);
    }

    protected override void InitDesitionTree()
    {
        INode chase = new ActionNode(() => _fsm.Transition(states.Chase));
        INode patrol = new ActionNode(() => _fsm.Transition(states.Patrol));
        INode explode = new ActionNode(() => _fsm.Transition(states.Explode));

        INode QCanExplode = new QuestionNode(CheckForExplode,explode, chase);
        INode QCanChase = new QuestionNode(DetectCommand,QCanExplode, patrol);
        INode QIsPlayerAlive = new QuestionNode(CheckForPlayer, QCanChase, patrol);
        _root = QIsPlayerAlive;
    }

    protected override void InitFSM()
    {
        var chase = new KamikaseChaseState<states>(MovementCommand,RotateCommand,CheckForExplode, _root, _obstacleAvoidance, Steerings.Chase, DetectCommand, transform, _model.Stats.ChaseSpeed);
        var patrol = new EnemyPatrolState<states>(MovementCommand, RotateCommand,HasTakenDamage, transform, _root, wayPoints, Steerings.Seek, _obstacleAvoidance, DetectCommand, _model.Stats.PatrolSpeed);
        var explode = new KamikaseExplodeState<states>(CheckForExplode, ExplodeCommand, _root);

        chase.AddTransition(states.Explode, explode);
        chase.AddTransition(states.Patrol, patrol);

        patrol.AddTransition(states.Chase, chase);

        _fsm = new FSM<states>(patrol);

    }
    private bool CheckForExplode()
    {
        var distance = Vector3.Distance(transform.position, _actualTarget.transform.position);
        if (distance > obstacleAvoidanceSO.ExplodeDistance)
        {
            return false;
        }

        return true;
    }
    private void MovementCommand(Vector3 moveDir, float desiredSpeed)
    {

        onMove?.Invoke(moveDir, desiredSpeed);
    }
    private void RotateCommand(Vector3 rotationDir)
    {
        onRotate?.Invoke(_obstacleAvoidance.GetFixedDir(rotationDir));

    }
    private bool DetectCommand()
    {
        if (!_lineOfSight.CheckForOneTarget())
        {
            onDetect?.Invoke(false);

            return false;
        }

        onDetect?.Invoke(true);
        return true;

    }
    private void ExplodeCommand()
    {
        onExplode?.Invoke();
        onMove?.Invoke(Vector3.zero, 0);
    }
    protected override void DieActions()
    {
        ExplodeCommand();
    }
    private void Update()
    {
        _fsm.UpdateState();
    }

}
