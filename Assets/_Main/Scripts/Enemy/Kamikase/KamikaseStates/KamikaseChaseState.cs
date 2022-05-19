using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
public class KamikaseChaseState<T> : State<T>
{
    private Action<Vector3, float> _onChase;
    private Action<Vector3> _onRotate;
    private Func<bool> _canExplode;
    private Func<bool> _isOnSight;
    private INode _root;
    private ObstacleAvoidance _obs;
    private Steerings _obsEnum;
    private Transform _self;
    private float _desiredSpeed;
    public KamikaseChaseState(Action<Vector3, float> onChase,Action<Vector3> onRotate,Func<bool> canExplode, INode root, ObstacleAvoidance obs, Steerings obsEnum, Func<bool> isOnSight, Transform self, float desiredSpeed)
    {
        _onChase = onChase;
        _onRotate = onRotate;
        _root = root;
        _obs = obs;
        _obsEnum = obsEnum;
        _isOnSight = isOnSight;
        _self = self;
        _desiredSpeed = desiredSpeed;
        _canExplode = canExplode;
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
        if (_canExplode())
        {
            _root.Execute();
            return;
        }
        _onChase?.Invoke(_self.forward, _desiredSpeed);
        _onRotate?.Invoke(_obs.GetFixedDir());
    }

}

