using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class EvadeRobotController : BaseEnemyController
{
    enum enemyStates
    {
        Idle,
        Patrol,
        Seek,
        Evade,
    }

    [SerializeField] private Transform[] wayPoints;
    private EvadeRobotModel _enemyModel;
    //private SecurityEnemyView _enemyView;
    private FSM<enemyStates> _fsm;

    public event Action<Vector3> onRotate;
    public event Action<Vector3, float> onMove;
    public event Action<bool> onDetect;
    public event Action onDie;
    public event Action onEvade;
    protected override void Awake()
    {
        base.Awake();
        _enemyModel = GetComponent<EvadeRobotModel>();
       // _enemyView = GetComponent<SecurityEnemyView>();
        lifeController = GetComponent<LifeController>();
        lifeController.actionToDo += DieActions;
    }
    protected override void Start()
    {
        _enemyModel.SuscribeEvents(this);
        //_enemyView.SuscribeEvents(this);
        base.Start();

    }
    protected override void InitBehaviours()
    {
        var seek = new Seek(_actualTarget.transform, transform);
        behaviours.Add(Steerings.Seek, seek);
        var evade = new Evade(_actualTarget.transform, transform, _actualTarget, obstacleAvoidanceSO.PredictionTime);
        behaviours.Add(Steerings.Evade, evade);
    }
    protected override void InitFSM()
    {
        var patrol = new EnemyPatrolState<enemyStates>(MovementCommand, RotateCommand, HasTakenDamage, transform, _root, wayPoints, Steerings.Seek, _obstacleAvoidance, DetectCommand, _enemyModel.Stats.PatrolSpeed);
        var evade = new EnemyEvadeState<enemyStates>(MovementCommand,RotateCommand, _root, _obstacleAvoidance, Steerings.Evade, DetectCommand, transform, _enemyModel.Stats.SeekSpeed);

        evade.AddTransition(enemyStates.Patrol, patrol);

        patrol.AddTransition(enemyStates.Evade, evade);


        _fsm = new FSM<enemyStates>(patrol);
    }
    protected override void InitDesitionTree()
    {

        INode evade = new ActionNode(() => _fsm.Transition(enemyStates.Evade));
        INode patrol = new ActionNode(() => _fsm.Transition(enemyStates.Patrol));

        INode QOnSight = new QuestionNode(DetectCommand, evade, patrol);
        INode QTakeDamage = new QuestionNode(HasTakenDamage, evade, QOnSight);
        INode QPlayerAlive = new QuestionNode(CheckForPlayer, QTakeDamage, patrol);
        _root = QPlayerAlive;
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

    private void IdleCommand()
    {
        onMove(transform.forward, 0);
    }
    private bool CheckForShooting()
    {
        var distance = Vector3.Distance(transform.position, _actualTarget.transform.position);
        if (distance > obstacleAvoidanceSO.ShootDistance)
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
    public void OnDrawGizmosSelected()
    {

        Gizmos.color = Color.red;
        if (_obstacleAvoidance != null && _obstacleAvoidance.ActualBehaviour != null)
        {
            var dir = _obstacleAvoidance.ActualBehaviour.GetDir();
            Gizmos.DrawRay(transform.position, dir * 2);
        }
        Gizmos.DrawWireSphere(transform.position, obstacleAvoidanceSO.Radius);
        Gizmos.DrawRay(transform.position, Quaternion.Euler(0, obstacleAvoidanceSO.Angle / 2, 0) * transform.forward * obstacleAvoidanceSO.Radius);
        Gizmos.DrawRay(transform.position, Quaternion.Euler(0, -obstacleAvoidanceSO.Angle / 2, 0) * transform.forward * obstacleAvoidanceSO.Radius);
    }
    private void Update()
    {
        _fsm.UpdateState();
    }
}
