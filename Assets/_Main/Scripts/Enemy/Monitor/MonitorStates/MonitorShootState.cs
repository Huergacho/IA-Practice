using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
public class MonitorShootState<T> : State<T>
{
    private Action _onShoot;
    private Action<Vector3> _onRotate;
    private INode _root;
    private Steerings _obsEnum;
    private ObstacleAvoidance _obstacleAvoidance;
    private Func<bool> _onDetect;
    public MonitorShootState(Action onShoot, Action<Vector3> onRotate, INode root, ObstacleAvoidance obstacleAvoidance, Steerings steering, Func<bool> onDetect)
    {
        _onShoot = onShoot;
        _root = root;
        _obsEnum = steering;
        _obstacleAvoidance = obstacleAvoidance;
        _onDetect = onDetect;
        _onRotate = onRotate;
    }
    public override void Awake()
    {
        _obstacleAvoidance.SetActualBehaviour(_obsEnum);
    }
    public override void Execute()
    {
        if (!_onDetect())
        {
            _root.Execute();
            return;
        }
        _onShoot?.Invoke();
        _onRotate?.Invoke(_obstacleAvoidance.GetFixedDir());
    }

}

