using System;
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
    private FSM<enemyStates> _fsm;
    public event Action onIdle;
    public event Action onPatrol;
    public event Action<Vector3> onRotate;
    public event Action<Vector3> onSeek;
    public event Action<Vector3> onChase;
    private LineOfSight _lineOfSight;
    private INode _root;
    [SerializeField]private PlayerModel _actualTarget = null;
    private ObstacleAvoidance _obstacleAvoidance;
    [SerializeField] private ObstacleAvoidanceSO obstacleAvoidanceSO;
    private void Awake()
    {
        _enemyModel = GetComponent<EnemyModel>();
        _lineOfSight = GetComponent<LineOfSight>();
        _obstacleAvoidance = new ObstacleAvoidance
                (obstacleAvoidanceSO.ObstacleLayers,obstacleAvoidanceSO.Radius, transform, 
                _actualTarget,obstacleAvoidanceSO.Angle,obstacleAvoidanceSO.PredictionTime,obstacleAvoidanceSO.AvoidanceMult, obstacleAvoidanceSO.BehaviourMult);
    }
    private void Start()
    {
        //_enemyModel.SuscribeEvents(this);
        InitDesitionTree();
        InitFSM();
    }
    private void InitFSM()
    {
        var idle = new EnemyIdleStates<enemyStates>(onIdle,_root);
        var patrol = new EnemyPatrolState<enemyStates>(onPatrol, _root);
        var seek = new EnemySeekState<enemyStates>(Seek, _root, _obstacleAvoidance);
        var chase = new EnemyChaseState<enemyStates>(Chase, _root, _obstacleAvoidance,ObstacleAvoidance.Behaviours.Persuit);

        idle.AddTransition(enemyStates.Patrol, patrol);
        idle.AddTransition(enemyStates.Seek,seek);
        idle.AddTransition(enemyStates.Chase, chase);

        seek.AddTransition(enemyStates.Idle, idle);

        chase.AddTransition(enemyStates.Idle, idle);

        patrol.AddTransition(enemyStates.Idle, idle);
        patrol.AddTransition(enemyStates.Seek, seek);



        _fsm = new FSM<enemyStates>(idle);
    }
    private void InitDesitionTree()
    {

        INode seek = new ActionNode(() => _fsm.Transition(enemyStates.Seek));
        INode chase = new ActionNode(() => _fsm.Transition(enemyStates.Chase));
        INode patrol = new ActionNode(() => _fsm.Transition(enemyStates.Patrol));
        INode idle = new ActionNode(() => _fsm.Transition(enemyStates.Idle));

        INode QOnSight = new QuestionNode(OnSight,chase, idle);
        _root = QOnSight;
    }

    private bool OnSight()
    {
        if (!_lineOfSight.CheckForOneTarget())
        {
            return false;
        }

        _obstacleAvoidance.SetTarget(_actualTarget);
        return true;

    }
    private void OnIdle()
    {
        onIdle?.Invoke();
    }
    private void Seek()
    {
        var direction = _obstacleAvoidance.GetFixedDir();
        onSeek?.Invoke(direction);
        onRotate?.Invoke(direction);
    }
    private void Chase()
    {
        _enemyModel.Chase(transform.forward);
        _enemyModel.LookDir(_obstacleAvoidance.GetFixedDir());
    }
    private void Update()
    {
 
        _fsm.UpdateState();
    }
    public void OnDrawGizmosSelected()
    {
 
        Gizmos.color = Color.red;
        if(_obstacleAvoidance != null)
        {
            var dir = _obstacleAvoidance.ActualBehaviour.GetDir();
            Gizmos.DrawRay(transform.position, dir * 2);
        }
        Gizmos.DrawWireSphere(transform.position, obstacleAvoidanceSO.Radius);
        Gizmos.DrawRay(transform.position, Quaternion.Euler(0, obstacleAvoidanceSO.Angle / 2, 0) * transform.forward * obstacleAvoidanceSO.Radius);
        Gizmos.DrawRay(transform.position, Quaternion.Euler(0, -obstacleAvoidanceSO.Angle / 2, 0) * transform.forward * obstacleAvoidanceSO.Radius);
    }
}
