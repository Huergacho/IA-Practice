using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
public class SecurityEnemyController : BaseEnemyController
{
    enum enemyStates
    {
        Idle,
        Patrol,
        Seek,
        Chase,
        Shoot
    }

    [SerializeField] private Transform[] wayPoints;
    private SecurityEnemyModel _enemyModel;
    private SecurityEnemyView _enemyView;
    private FSM<enemyStates> _fsm;

    public event Action<Vector3> onRotate;
    public event Action<Vector3, float> onMove;
    public event Action<bool> onDetect;
    public event Action onDie;
    public event Action onShoot;
    protected override void Awake()
    {
        base.Awake();
        _enemyModel = GetComponent<SecurityEnemyModel>();
        _enemyView = GetComponent<SecurityEnemyView>();
        lifeController = GetComponent<LifeController>();
        lifeController.actionToDo += DieActions;
    }
    protected override void Start()
    {
        _enemyModel.SuscribeEvents(this);
        _enemyView.SuscribeEvents(this);
        base.Start();
    }
    protected override void InitBehaviours()
    {
        var seek =  new Seek(_actualTarget.transform, transform);
        behaviours.Add(Steerings.Seek,seek);
        var persuit = new Chase(_actualTarget.transform, transform, _actualTarget, obstacleAvoidanceSO.PredictionTime);
        behaviours.Add(Steerings.Chase, persuit);
    }
    protected override void InitFSM()
    {
        var patrol = new EnemyPatrolState<enemyStates>(MovementCommand, RotateCommand, transform,_root,wayPoints,Steerings.Seek,_obstacleAvoidance,DetectCommand,_enemyModel.Stats.PatrolSpeed);
        var chase = new EnemyChaseState<enemyStates>(MovementCommand,RotateCommand,CheckForShooting, _root, _obstacleAvoidance,Steerings.Chase, DetectCommand,transform,_enemyModel.Stats.ChaseSpeed);
        var shoot = new EnemyShootState<enemyStates>(Shoot, MovementCommand, RotateCommand, _root,_obstacleAvoidance, Steerings.Seek,DetectCommand,CheckForShooting,transform,_enemyModel.Stats.SeekSpeed);

        //idle.AddTransition(enemyStates.Patrol, patrol);
        //idle.AddTransition(enemyStates.Chase, chase);


        //chase.AddTransition(enemyStates.Idle, idle);
        chase.AddTransition(enemyStates.Shoot, shoot);
        chase.AddTransition(enemyStates.Patrol, patrol);

        //patrol.AddTransition(enemyStates.Idle, idle);
        patrol.AddTransition(enemyStates.Chase, chase);
        patrol.AddTransition(enemyStates.Shoot, shoot);

        shoot.AddTransition(enemyStates.Chase, chase);
        shoot.AddTransition(enemyStates.Patrol, patrol);



        _fsm = new FSM<enemyStates>(patrol);
    }
    protected override void InitDesitionTree() 
    { 

        INode chase = new ActionNode(() => _fsm.Transition(enemyStates.Chase));
        INode patrol = new ActionNode(() => _fsm.Transition(enemyStates.Patrol));
        //INode idle = new ActionNode(() => _fsm.Transition(enemyStates.Idle));
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
        if(_obstacleAvoidance != null && _obstacleAvoidance.ActualBehaviour != null)
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
