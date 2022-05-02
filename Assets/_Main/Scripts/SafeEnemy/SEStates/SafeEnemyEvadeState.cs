using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
class SafeEnemyEvadeState<T> : State<T>
{
    private Action<Vector3, float, Vector3> _onEvade;
    private Func<bool> _isOnSight;
    private INode _root;
    private ObstacleAvoidance _obs;
    private Steerings _obsEnum;
    private Transform _self;
    private float _desiredSpeed;
    private Func<bool> _canShoot;
    public SafeEnemyEvadeState(Action<Vector3, float, Vector3> onEvade, Func<bool> canShoot, INode root, ObstacleAvoidance obs, Steerings obsEnum, Func<bool> isOnSight, Transform self, float desiredSpeed)
    {
        _onEvade = onEvade;
        _root = root;
        _obs = obs;
        _obsEnum = obsEnum;
        _isOnSight = isOnSight;
        _self = self;
        _desiredSpeed = desiredSpeed;
        _canShoot = canShoot;
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
        if (_isOnSight() && _canShoot())
        {
            _root.Execute();
            return;
        }
        _onEvade?.Invoke(_self.forward, _desiredSpeed, _obs.GetFixedDir());
    }

}
