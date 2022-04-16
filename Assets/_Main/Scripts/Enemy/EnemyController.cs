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


    public event Action onIdle;
    public event Action onPatrol;
    public event Action<Vector3> onRotate;
    public event Action<Vector3> onSeek;
    public event Action<Vector3> onChase;
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
        var patrol = new EnemyPatrolState<enemyStates>(onPatrol, _root);
        var seek = new EnemySeekState<enemyStates>(Seek, _root, _obstacleAvoidance,Steerings.Seek);
        var chase = new EnemyChaseState<enemyStates>(Chase, _root, _obstacleAvoidance,Steerings.Persuit);

        idle.AddTransition(enemyStates.Patrol, patrol);
        idle.AddTransition(enemyStates.Seek,seek);
        idle.AddTransition(enemyStates.Chase, chase);

        seek.AddTransition(enemyStates.Idle, idle);
        seek.AddTransition(enemyStates.Chase, chase);

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

        INode QCanChase = new QuestionNode(CheckDistance, chase, seek);
        INode QOnSight = new QuestionNode(DetectCommand, QCanChase, idle);
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
        onIdle?.Invoke();
    }
    private bool CheckDistance()
    {
        var distance = Vector3.Distance(transform.position, _actualTarget.transform.position);
        if(distance > obstacleAvoidanceSO.SeekDistance)
        {
            return false;
        }
        return true;
    }
    private void Seek()
    {
        onSeek?.Invoke(transform.forward);
        onRotate?.Invoke(_obstacleAvoidance.GetFixedDir());
    }
    private void Chase()
    {
        onChase?.Invoke(transform.forward);
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
}
