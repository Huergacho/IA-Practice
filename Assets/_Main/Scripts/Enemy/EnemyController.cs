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
        Chase
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
    private void Awake()
    {
        _enemyModel = GetComponent<EnemyModel>();
        _enemyView = GetComponent<EnemyView>();
        _lineOfSight = GetComponent<LineOfSight>();
        InitBehaviours();
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
        var chase = new EnemyChaseState<enemyStates>(ChaseCommand, _root, _obstacleAvoidance,Steerings.Persuit);

        idle.AddTransition(enemyStates.Patrol, patrol);
        idle.AddTransition(enemyStates.Seek,seek);
        idle.AddTransition(enemyStates.Chase, chase);

        seek.AddTransition(enemyStates.Idle, idle);
        seek.AddTransition(enemyStates.Chase, chase);
        seek.AddTransition(enemyStates.Patrol, patrol);

        chase.AddTransition(enemyStates.Idle, idle);

        patrol.AddTransition(enemyStates.Idle, idle);
        patrol.AddTransition(enemyStates.Patrol, patrol);
        patrol.AddTransition(enemyStates.Seek, seek);



        _fsm = new FSM<enemyStates>(idle);
    }
    private void InitDesitionTree() 
    { 

        INode seek = new ActionNode(() => _fsm.Transition(enemyStates.Seek));
        INode chase = new ActionNode(() => _fsm.Transition(enemyStates.Chase));
        INode patrol = new ActionNode(() => _fsm.Transition(enemyStates.Patrol));
        INode idle = new ActionNode(() => _fsm.Transition(enemyStates.Idle));

        INode QCanChase = new QuestionNode(CheckChaseDistance, chase, _root);
        INode QCanSeek = new QuestionNode(CheckSeekDistance, seek, patrol);
        INode QOnSight = new QuestionNode(DetectCommand, QCanSeek, patrol);
        _root = QOnSight;
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
    private void PatrolCommand()
    {
        onMove?.Invoke(transform.forward, _enemyModel.Stats.PatrolSpeed);
        Vector3 dir = currWayPoint.position - _enemyModel.transform.position;
        dir = dir.normalized;
        onRotate?.Invoke(_obstacleAvoidance.GetFixedDir(dir));
        CheckWayPoint();
    }
    private bool CheckSeekDistance()
    {
        var distance = Vector3.Distance(transform.position, _actualTarget.transform.position);

        if (distance > obstacleAvoidanceSO.SeekDistance)
        {
            return false;
        }
        return true;
    }
    private bool CheckChaseDistance()
    {
        var distance = Vector3.Distance(transform.position, _actualTarget.transform.position);
        if (distance > obstacleAvoidanceSO.ChaseDistance)
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
        onMove?.Invoke(transform.forward,_enemyModel.Stats.ChaseSpeed);
        onRotate?.Invoke(_obstacleAvoidance.GetFixedDir());
    }
    private void Update()
    {
        _fsm.UpdateState();
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
   
}
