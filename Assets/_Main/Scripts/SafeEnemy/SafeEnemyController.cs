using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class SafeEnemyController : MonoBehaviour, IFSMController
{
    enum enemyStates
    {
        Patrol,
        Chase,
        Evade,
        Shoot
    }
    [SerializeField] private PlayerModel player;
    [SerializeField]private Transform[] _wayPoints;
    [SerializeField]private ObstacleAvoidanceSO _obstacleAvoidanceStats;
    private SafeEnemyModel _model;
    private SafeEnemyView _view;

    private FSM<enemyStates> _fsm;
    private LineOfSight _lineOfSight;
    private ObstacleAvoidance _obstacleAvoidance;
    private INode _root;
    private LifeController _lifeController;
    private Dictionary<Steerings, ISteering> behaviours = new Dictionary<Steerings, ISteering>();

    public event Action<bool> _onDetect;
    public event Action<Vector3, float> _onMove;
    public event Action<Vector3> _onRotate;
    public event Action onShoot;

    private void Awake()
    {
        _lifeController = GetComponent<LifeController>();
        _lineOfSight = GetComponent<LineOfSight>();
        _model = GetComponent<SafeEnemyModel>();
        _view = GetComponent<SafeEnemyView>();
        _obstacleAvoidance = new ObstacleAvoidance(transform, player, _obstacleAvoidanceStats, behaviours);
        InitBehaviours();
    }
    private void Start()
    {
        _model.SuscribeEvents(this);
        InitDesitionTree();
        InitFSM();
    }
    public void InitBehaviours()
    {
        var seek = new Seek(player.transform, transform);
        behaviours.Add(Steerings.Seek, seek);

        var chase = new Persuit(player.transform, transform, player, _obstacleAvoidanceStats.PredictionTime);
        behaviours.Add(Steerings.Persuit, chase);

        var evade = new Evade(player.transform, transform, player, _obstacleAvoidanceStats.PredictionTime);
        behaviours.Add(Steerings.Evade, evade);
    }
    public void InitFSM()
    {
        var patrol = new SafeEnemyPatrolState<enemyStates>(MoveCommand, transform, _root, _wayPoints,Steerings.Seek,_obstacleAvoidance,DetectCommand,_model.Stats.SeekSpeed);
        var evade = new SafeEnemyEvadeState<enemyStates>(MoveCommand, CheckForShooting, _root, _obstacleAvoidance, Steerings.Evade, DetectCommand, transform, _model.Stats.ChaseSpeed);
        var shoot = new SafeEnemyShootState<enemyStates>(Shoot,MoveCommand,_root,_obstacleAvoidance,Steerings.Persuit,DetectCommand,CheckForShooting,transform,_model.Stats.SeekSpeed);

        patrol.AddTransition(enemyStates.Evade, evade);
        patrol.AddTransition(enemyStates.Shoot, shoot);

        evade.AddTransition(enemyStates.Patrol, patrol);
        evade.AddTransition(enemyStates.Shoot, shoot);

        shoot.AddTransition(enemyStates.Evade, evade);
        shoot.AddTransition(enemyStates.Patrol, patrol);

        _fsm = new FSM<enemyStates>(patrol);

    }
    public void InitDesitionTree()
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
        var distance = Vector3.Distance(transform.position, player.transform.position);
        if (distance < _obstacleAvoidanceStats.ShootDistance)
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
    private bool CheckForPlayer()
    {
        return GameManager.Instance.IsPlayerAlive;
    }
    private void DieActions()
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
        Gizmos.DrawWireSphere(transform.position, _obstacleAvoidanceStats.Radius);
        Gizmos.DrawRay(transform.position, Quaternion.Euler(0, _obstacleAvoidanceStats.Angle / 2, 0) * transform.forward * _obstacleAvoidanceStats.Radius);
        Gizmos.DrawRay(transform.position, Quaternion.Euler(0, -_obstacleAvoidanceStats.Angle / 2, 0) * transform.forward * _obstacleAvoidanceStats.Radius);
    }
}
