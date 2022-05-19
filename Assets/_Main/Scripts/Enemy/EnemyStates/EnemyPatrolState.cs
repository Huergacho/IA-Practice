using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
public class EnemyPatrolState<T> : State<T>
{
    private Action<Vector3, float> _patrol;
    Action<Vector3> _onRotate;
    private INode _root;
    private Steerings _obsEnum;
    private ObstacleAvoidance _obs;
    private Func<bool> _isOnSight;
    private Transform[] _wayPoints;
    private Transform currWayPoint;
    private Transform _self;
    private int currentWayPointIndex;
    private float _desiredSpeed;
    public EnemyPatrolState(Action<Vector3, float> patrol,Action<Vector3>onRotate,Transform self, INode root, Transform[] wayPoints, Steerings obsEnum, 
        ObstacleAvoidance obs, Func<bool> isOnSight, float desiredSpeed)
    {
        _patrol = patrol;
        _self = self;
        _root = root;
        _obs = obs;
        _wayPoints = wayPoints;
        _obsEnum = obsEnum;
        _isOnSight = isOnSight;
        _desiredSpeed = desiredSpeed;
        currentWayPointIndex = 0;
        _onRotate = onRotate;
    }
    public override void Awake()
    {
        _obs.SetActualBehaviour(_obsEnum);

    }
    public override void Execute()
    {
        if (_isOnSight())
        {
            _root.Execute();
            return;
        }
        CheckWayPoint();
        _patrol?.Invoke((currWayPoint.position - _self.position).normalized, _desiredSpeed);
        _onRotate?.Invoke((currWayPoint.position - _self.position).normalized);
        
    }
    private void CheckWayPoint()
    {

        if (currWayPoint != _wayPoints[currentWayPointIndex])
        {
            currWayPoint = _wayPoints[currentWayPointIndex];
        }
        var posWay = currWayPoint.position;
        posWay.y = _self.position.y;
        var distance = Vector3.Distance(_self.position, posWay);

        if (distance <= 0.25f)
        {
            if (currWayPoint == _wayPoints[_wayPoints.Length - 1])
            {
                currentWayPointIndex = 0;
                currWayPoint = _wayPoints[currentWayPointIndex];
            }
            else
            {
                currentWayPointIndex++;
                currWayPoint = _wayPoints[currentWayPointIndex];
            }
        }
    }
}
