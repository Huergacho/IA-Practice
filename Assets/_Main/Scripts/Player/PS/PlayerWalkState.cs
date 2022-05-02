using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
public class PlayerWalkState<T> : State<T>
{
    T _idleInput;
    T _runInput;
    Action<Vector2, float> _onWalk;
    Action _onShoot;
    Action _animation;
    PlayerInputs _playerInputs;
    private float _desiredSpeed;
    public PlayerWalkState(T idleInput, T runInput, Action<Vector2,float> onWalk, Action onShoot, PlayerInputs playerInputs, float desiredSpeed,Action animation)
    {
        _idleInput = idleInput;
        _runInput = runInput;
        _onWalk = onWalk;
        _onShoot = onShoot;
        _playerInputs = playerInputs;
        _desiredSpeed = desiredSpeed;
        _animation = animation;
    }
    public override void Execute()
    {
        _playerInputs.UpdateInputs();

        if (!_playerInputs.IsMoving())
        {
            _parentFSM.Transition(_idleInput);
            return;
        }
        if (_playerInputs.IsRunning())
        {
            _parentFSM.Transition(_runInput);
            return;
        }
        if (_playerInputs.isShooting())
        {
            _onShoot?.Invoke();
        }
        _onWalk?.Invoke(new Vector2(_playerInputs.GetH,_playerInputs.GetV), _desiredSpeed);
        _animation?.Invoke();
    }
}

