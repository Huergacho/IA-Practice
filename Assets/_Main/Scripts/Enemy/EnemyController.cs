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
        Chase
    }
    private EnemyModel _enemyModel;
    private FSM<enemyStates> _fsm;
    public event Action onIdle;
    public event Action onPatrol;
    public event Action onChase;
    private LineOfSight _lineOfSight;
    private INode _root;
    private void Awake()
    {
        _enemyModel = GetComponent<EnemyModel>();
        _lineOfSight = GetComponent<LineOfSight>();
    }
    private void Start()
    {
        _enemyModel.SuscribeEvents(this);
        InitDesitionTree();
        InitFSM();
    }
    private void InitFSM()
    {
        var idle = new EnemyIdleStates<enemyStates>(onIdle);
        var patrol = new EnemyPatrolState<enemyStates>(onPatrol, _root);
        var chase = new EnemyChaseState<enemyStates>(onChase, _root);

        idle.AddTransition(enemyStates.Patrol, patrol);
        idle.AddTransition(enemyStates.Chase,chase);

        chase.AddTransition(enemyStates.Patrol, patrol);

        patrol.AddTransition(enemyStates.Idle, idle);
        patrol.AddTransition(enemyStates.Chase, chase);



        _fsm = new FSM<enemyStates>(patrol);
    }
    private void InitDesitionTree()
    {

        INode chase = new ActionNode(() => _fsm.Transition(enemyStates.Chase));
        INode patrol = new ActionNode(() => _fsm.Transition(enemyStates.Patrol));
        //INode idle = new ActionNode(() => _fsm.Transition(enemyStates.Patrol));

        INode QOnSight = new QuestionNode(OnSight, chase, patrol);
        _root = QOnSight;
    }

    private bool OnSight()
    {
        return _lineOfSight.CheckForOneTarget();
    }
    private void OnIdle()
    {
        onIdle?.Invoke();
    }
    private void Update()
    {
        _fsm.UpdateState();
    }

}
