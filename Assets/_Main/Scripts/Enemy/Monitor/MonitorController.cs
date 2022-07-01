using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
public class MonitorController : BaseEnemyController
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
    [SerializeField] private Transform returnPoint;
    private MonitorModel _enemyModel;
    private MonitorView _enemyView;
    private FSM<enemyStates> _fsm;
    public event Action<Vector3, float> onMove;
    public event Action<Vector3> onRotate;
    public event Action<bool> onDetect;
    public event Action onDie;
    public event Action onShoot;
    protected override void Awake()
    {
        base.Awake();
        _enemyModel = GetComponent<MonitorModel>();
        _enemyView = GetComponent<MonitorView>();
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
        var seek = new Seek(_actualTarget.transform, transform);
        behaviours.Add(Steerings.Seek, seek);
        //var persuit = new Chase(_actualTarget.transform, transform, _actualTarget, obstacleAvoidanceSO.PredictionTime);
        //behaviours.Add(Steerings.Chase, persuit);
    }
    protected override void InitFSM()
    {
        var shoot = new EnemyShootState<enemyStates>(Shoot, MovementCommand, RotateCommand, _root, _obstacleAvoidance, Steerings.Seek, DetectCommand, CheckForShooting, transform, _enemyModel.Stats.SeekSpeed);
        var idle = new EnemyIdleStates<enemyStates>(IdleCommand, _root, DetectCommand);

        idle.AddTransition(enemyStates.Shoot, shoot);

        shoot.AddTransition(enemyStates.Idle, idle);

        _fsm = new FSM<enemyStates>(idle);
    }
    protected override void InitDesitionTree()
    {
        INode shoot = new ActionNode(() => _fsm.Transition(enemyStates.Shoot));
        INode idle = new ActionNode(() => _fsm.Transition(enemyStates.Idle));

        INode QCanShoot = new QuestionNode(CheckForShooting, shoot, idle);
        INode QOnSight = new QuestionNode(DetectCommand, QCanShoot, idle);
        INode QPlayerAlive = new QuestionNode(CheckForPlayer, QOnSight, idle);
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
        onRotate?.Invoke(returnPoint.position - transform.position);
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

    private void Shoot()
    {
        onShoot?.Invoke();
    }
    private void MovementCommand(Vector3 moveDir, float desiredSpeed)
    {
    }
    private void RotateCommand(Vector3 rotationDir)
    {
        onRotate?.Invoke(_obstacleAvoidance.GetFixedDir(rotationDir));

    }
    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, obstacleAvoidanceSO.ShootDistance);
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
