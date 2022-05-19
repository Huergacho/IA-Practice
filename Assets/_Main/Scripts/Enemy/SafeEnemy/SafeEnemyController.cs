using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class SafeEnemyController : BaseEnemyController
{
    enum enemyStates
    {
        Patrol,
        Chase,
        Evade,
        Shoot
    }
    [SerializeField]private Transform[] _wayPoints;
    private SafeEnemyModel _model;
    private SafeEnemyView _view;

    private FSM<enemyStates> _fsm;

    public event Action<bool> _onDetect;
    public event Action<Vector3, float> _onMove;
    public event Action<Vector3> _onRotate;
    public event Action onShoot;

    protected override void Awake()
    {
        _model = GetComponent<SafeEnemyModel>();
        _view = GetComponent<SafeEnemyView>();
        base.Awake();
    }
    protected override void Start()
    {
        _model.SuscribeEvents(this);
        InitDesitionTree();
        InitFSM();
    }
    protected override void InitBehaviours()
    {
        var seek = new Seek(_actualTarget.transform, transform);
        behaviours.Add(Steerings.Seek, seek);

        var chase = new Chase(_actualTarget.transform, transform, _actualTarget, obstacleAvoidanceSO.PredictionTime);
        behaviours.Add(Steerings.Chase, chase);

        var evade = new Evade(_actualTarget.transform, transform, _actualTarget, obstacleAvoidanceSO.PredictionTime);
        behaviours.Add(Steerings.Evade, evade);
    }
    protected override void InitFSM()
    {
        var patrol = new SafeEnemyPatrolState<enemyStates>(MoveCommand, transform, _root, _wayPoints,Steerings.Seek,_obstacleAvoidance,DetectCommand,_model.Stats.SeekSpeed);
        var evade = new SafeEnemyEvadeState<enemyStates>(MoveCommand, CheckForShooting, _root, _obstacleAvoidance, Steerings.Evade, DetectCommand, transform, _model.Stats.ChaseSpeed);
        var shoot = new SafeEnemyShootState<enemyStates>(Shoot,MoveCommand,_root,_obstacleAvoidance,Steerings.Chase, DetectCommand,CheckForShooting,transform,_model.Stats.SeekSpeed);

        patrol.AddTransition(enemyStates.Evade, evade);
        patrol.AddTransition(enemyStates.Shoot, shoot);

        evade.AddTransition(enemyStates.Patrol, patrol);
        evade.AddTransition(enemyStates.Shoot, shoot);

        shoot.AddTransition(enemyStates.Evade, evade);
        shoot.AddTransition(enemyStates.Patrol, patrol);

        _fsm = new FSM<enemyStates>(patrol);

    }
    protected override void InitDesitionTree()
    {
        INode evade = new ActionNode(() => _fsm.Transition(enemyStates.Evade));
        INode patrol = new ActionNode(() => _fsm.Transition(enemyStates.Patrol));
        INode shoot = new ActionNode(() => _fsm.Transition(enemyStates.Shoot));

        INode QCanShoot = new QuestionNode(CheckForShooting, shoot, evade);
        INode QOnSight = new QuestionNode(DetectCommand, QCanShoot, patrol);
        INode QPlayerAlive = new QuestionNode(CheckForPlayer, QOnSight, patrol);
        _root = QPlayerAlive;
    }
    private void Update()
    {
        _fsm.UpdateState();
    }
    private bool CheckForShooting()
    {
        var distance = Vector3.Distance(transform.position, _actualTarget.transform.position);
        if (distance < obstacleAvoidanceSO.ShootDistance)
        {
            return false;
        }

        return true;
    }

    private void MoveCommand(Vector3 dir, float desiredSpeed, Vector3 rotation)
    {
        _onMove?.Invoke(dir, desiredSpeed);
        _onRotate?.Invoke(rotation);
    }
    private void Shoot()
    {
        onShoot?.Invoke();
    }
    private bool DetectCommand()
    {
        if (!_lineOfSight.CheckForOneTarget())
        {
            _onDetect?.Invoke(false);

            return false;
        }

        _onDetect?.Invoke(true);
        return true;

    }
    protected override void DieActions()
    {
        Destroy(gameObject);
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
}
