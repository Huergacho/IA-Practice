using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
public class MonitorPredictState<T> : State<T>
{
    private Action<Vector3> _onPredict;
    private Func<bool> _isOnSight;
    private INode _root;
    private ObstacleAvoidance _obs;
    private Steerings _obsEnum;
    public MonitorPredictState(Action<Vector3> onPredict, INode root,ObstacleAvoidance obs,Steerings obsEnum, Func<bool> isOnSight)
    {
        _onPredict = onPredict;
        _root = root;
        _obsEnum = obsEnum;
        _isOnSight = isOnSight;
        _obs = obs;
    }

    public override void Awake()
    {
        _obs.SetActualBehaviour(_obsEnum);

    }
    public override void Execute()
    {
        if (!_isOnSight())
        {
            _root.Execute();
            return;
        }

        _onPredict?.Invoke(_obs.GetFixedDir());
    }


}
