using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
public class EnemyController : MonoBehaviour
{
    enum enemyStates
    {
        Idle,
        Patrol,
        Seek,
        Chase,
        Shoot
    }
    [SerializeField]private PlayerModel _actualTarget = null;
    [SerializeField] private ObstacleAvoidanceSO obstacleAvoidanceSO;
    [SerializeField] private Transform[] wayPoints;
    private EnemyModel _enemyModel;
    private EnemyView _enemyView;
    private FSM<enemyStates> _fsm;

    private LineOfSight _lineOfSight;
    private INode _root;
    private ObstacleAvoidance _obstacleAvoidance;
    private Dictionary<Steerings, ISteering> behaviours = new Dictionary<Steerings,ISteering>();
    private LifeController lifeController;

    public event Action<Vector3> onRotate;
    public event Action<Vector3, float> onMove;
    public event Action<bool> onDetect;
    public event Action onDie;
    public event Action onShoot;
    private void Awake()
    {
        _enemyModel = GetComponent<EnemyModel>();
        _enemyView = GetComponent<EnemyView>();
        _lineOfSight = GetComponent<LineOfSight>();
        InitBehaviours();
        lifeController = GetComponent<LifeController>();
        lifeController.actionToDo += DieActions;
        _obstacleAvoidance = new ObstacleAvoidance(transform, _actualTarget,obstacleAvoidanceSO,behaviours);
    }
    private void Start()
    {
        _enemyModel.SuscribeEvents(this);
        _enemyView.SuscribeEvents(this);
        InitDesitionTree();
        InitFSM();
    }
    private void InitBehaviours()
    {
        var seek =  new Seek(_actualTarget.transform, transform);
        behaviours.Add(Steerings.Seek,seek);
        var persuit = new Persuit(_actualTarget.transform, transform, _actualTarget, obstacleAvoidanceSO.PredictionTime);
        behaviours.Add(Steerings.Persuit, persuit);
    }
    private void InitFSM()
    {
        var idle = new EnemyIdleStates<enemyStates>(IdleCommand,_root);
        var patrol = new EnemyPatrolState<enemyStates>(MovementCommand,transform,_root,wayPoints,Steerings.Seek,_obstacleAvoidance,DetectCommand,_enemyModel.Stats.PatrolSpeed);
        var chase = new EnemyChaseState<enemyStates>(MovementCommand,CheckForShooting, _root, _obstacleAvoidance,Steerings.Persuit,DetectCommand,transform,_enemyModel.Stats.ChaseSpeed);
        var shoot = new EnemyShootState<enemyStates>(Shoot, MovementCommand,_root,_obstacleAvoidance, Steerings.Seek,DetectCommand,CheckForShooting,transform,_enemyModel.Stats.SeekSpeed);

        idle.AddTransition(enemyStates.Patrol, patrol);
        idle.AddTransition(enemyStates.Chase, chase);


        chase.AddTransition(enemyStates.Idle, idle);
        chase.AddTransition(enemyStates.Shoot, shoot);
        chase.AddTransition(enemyStates.Patrol, patrol);

        patrol.AddTransition(enemyStates.Idle, idle);
        patrol.AddTransition(enemyStates.Chase, chase);
        patrol.AddTransition(enemyStates.Shoot, shoot);

        shoot.AddTransition(enemyStates.Chase, chase);
        shoot.AddTransition(enemyStates.Patrol, patrol);



        _fsm = new FSM<enemyStates>(idle);
    }
    private void InitDesitionTree() 
    { 

        INode chase = new ActionNode(() => _fsm.Transition(enemyStates.Chase));
        INode patrol = new ActionNode(() => _fsm.Transition(enemyStates.Patrol));
        INode idle = new ActionNode(() => _fsm.Transition(enemyStates.Idle));
        INode shoot = new ActionNode(() => _fsm.Transition(enemyStates.Shoot));

        INode QCanShoot = new QuestionNode(CheckForShooting, shoot, chase);
        INode QOnSight = new QuestionNode(DetectCommand, QCanShoot, patrol);
        INode QPlayerAlive = new QuestionNode(CheckForPlayer, QOnSight, patrol);
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
    private bool CheckForPlayer()
    {
        return GameManager.Instance.IsPlayerAlive;
    }
    private void IdleCommand()
    {
        onMove(transform.forward, 0);
    }
    private bool CheckForShooting()
    {
        var distance = Vector3.Distance(transform.position, _actualTarget.transform.position);
        if(distance > obstacleAvoidanceSO.ShootDistance)
        {
            return false;
        }

        return true;
    }

    private void Shoot()
    {
        onShoot?.Invoke();
    }
    private void MovementCommand(Vector3 moveDir, float desiredSpeed, Vector3 rotation)
    {

        onMove?.Invoke(moveDir, desiredSpeed);
        onRotate?.Invoke(_obstacleAvoidance.GetFixedDir(rotation));
    }
    public void OnDrawGizmosSelected()
    {
 
        Gizmos.color = Color.red;
        if(_obstacleAvoidance != null && _obstacleAvoidance.ActualBehaviour != null)
        {
            var dir = _obstacleAvoidance.ActualBehaviour.GetDir(); 
            Gizmos.DrawRay(transform.position, dir * 2);
        }
        Gizmos.DrawWireSphere(transform.position, obstacleAvoidanceSO.Radius);
        Gizmos.DrawRay(transform.position, Quaternion.Euler(0, obstacleAvoidanceSO.Angle / 2, 0) * transform.forward * obstacleAvoidanceSO.Radius);
        Gizmos.DrawRay(transform.position, Quaternion.Euler(0, -obstacleAvoidanceSO.Angle / 2, 0) * transform.forward * obstacleAvoidanceSO.Radius);
    }

    private void DieActions()
    {
        Destroy(gameObject);
    }
    private void Update()
    {
        _fsm.UpdateState();
    }
}
