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
    private EnemyModel _enemyModel;
    private EnemyView _enemyView;
    private FSM<enemyStates> _fsm;

    private LineOfSight _lineOfSight;
    private INode _root;
    [SerializeField]private PlayerModel _actualTarget = null;
    private ObstacleAvoidance _obstacleAvoidance;
    [SerializeField] private ObstacleAvoidanceSO obstacleAvoidanceSO;
    private Dictionary<Steerings, ISteering> behaviours = new Dictionary<Steerings,ISteering>();
    [SerializeField] private Transform[] wayPoints;
    private Transform currWayPoint;

    public event Action<Vector3> onRotate;
    public event Action<Vector3, float> onMove;
    public event Action<bool> onDetect;
    public event Action onDie;
    private LifeController lifeController;
    private void Awake()
    {
        _enemyModel = GetComponent<EnemyModel>();
        _enemyView = GetComponent<EnemyView>();
        _lineOfSight = GetComponent<LineOfSight>();
        InitBehaviours();
        lifeController = GetComponent<LifeController>();
        lifeController.actionToDo = DieActions;
        _obstacleAvoidance = new ObstacleAvoidance(transform, _actualTarget,obstacleAvoidanceSO,behaviours);
    }
    private void Start()
    {
        _enemyModel.SuscribeEvents(this);
        _enemyView.SuscribeEvents(this);
        currWayPoint = wayPoints[0];
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
        var patrol = new EnemyPatrolState<enemyStates>(PatrolCommand, _root);
        var seek = new EnemySeekState<enemyStates>(SeekCommand, _root, _obstacleAvoidance,Steerings.Seek);
      //  var chase = new EnemyChaseState<enemyStates>(ChaseCommand, _root, _obstacleAvoidance,Steerings.Persuit);
        var shoot = new EnemyShootState<enemyStates>(Shoot, _root,_obstacleAvoidance, Steerings.Seek);

        idle.AddTransition(enemyStates.Patrol, patrol);
        idle.AddTransition(enemyStates.Seek,seek);
        // idle.AddTransition(enemyStates.Chase, chase);

        seek.AddTransition(enemyStates.Idle, idle);
     //   seek.AddTransition(enemyStates.Chase, chase);
        seek.AddTransition(enemyStates.Patrol, patrol);
        seek.AddTransition(enemyStates.Shoot, shoot);

        //chase.AddTransition(enemyStates.Idle, idle);
        //chase.AddTransition(enemyStates.Shoot, shoot);
        //chase.AddTransition(enemyStates.Seek, seek);

        patrol.AddTransition(enemyStates.Idle, idle);
        patrol.AddTransition(enemyStates.Patrol, patrol);
        patrol.AddTransition(enemyStates.Seek, seek);
       // patrol.AddTransition(enemyStates.Chase, chase);

        shoot.AddTransition(enemyStates.Seek,seek);
       // shoot.AddTransition(enemyStates.Chase, chase);



        _fsm = new FSM<enemyStates>(idle);
    }
    private void InitDesitionTree() 
    { 

        INode seek = new ActionNode(() => _fsm.Transition(enemyStates.Seek));
       // INode chase = new ActionNode(() => _fsm.Transition(enemyStates.Chase));
        INode patrol = new ActionNode(() => _fsm.Transition(enemyStates.Patrol));
        INode idle = new ActionNode(() => _fsm.Transition(enemyStates.Idle));
        INode shoot = new ActionNode(() => _fsm.Transition(enemyStates.Shoot));

        //INode QCanShoot = new QuestionNode(CheckChaseDistance,shoot, chase);
        //INode QCanSeek = new QuestionNode(CheckShootDistance, seek, QCanShoot);

        //INode QOnSight = new QuestionNode(DetectCommand, QCanSeek, patrol);
        //INode QCanShoot = new QuestionNode();
        INode QCanSeek = new QuestionNode(CheckForShooting, shoot, seek);
        INode QOnSight = new QuestionNode(DetectCommand, QCanSeek, patrol);
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
    private void PatrolCommand()
    {
        onMove?.Invoke(transform.forward, _enemyModel.Stats.PatrolSpeed);
        Vector3 dir = currWayPoint.position - _enemyModel.transform.position;
        dir = dir.normalized;
        onRotate?.Invoke(_obstacleAvoidance.GetFixedDir(dir));
        CheckWayPoint();
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
    private void SeekCommand()
    {
        onMove.Invoke(transform.forward, _enemyModel.Stats.SeekSpeed);
        onRotate?.Invoke(_obstacleAvoidance.GetFixedDir()); 
    }
    private void ChaseCommand()
    {
        print("Entre En Chase");
        onMove?.Invoke(transform.forward,_enemyModel.Stats.ChaseSpeed);
        onRotate?.Invoke(_obstacleAvoidance.GetFixedDir());
    }
    private void Shoot()
    {
        _enemyModel.Shoot();
        onMove.Invoke(transform.forward, _enemyModel.Stats.SeekSpeed);
        onRotate?.Invoke(_obstacleAvoidance.GetFixedDir());
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
    private void CheckWayPoint()
    {
        var posWay = currWayPoint.position;
        posWay.y = transform.position.y;
        var distance = Vector3.Distance(transform.position, posWay);
        if (distance <= 0.25f)
        {
            for (int i = 0; i < wayPoints.Length; i++)
            {
                if (i == wayPoints.Length - 1)
                {
                    currWayPoint = wayPoints[0];
                    break;
                }
                var actualWayPoint = wayPoints[i];
                if (currWayPoint == actualWayPoint)
                {
                    currWayPoint = wayPoints[i + 1];
                    break;
                }
            }
        }
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
