using System;
using UnityEngine;
using System.Collections.Generic;

class PlayerRunState<T> : State<T>
{
    T _walkInput;
    Action _onShoot;
    Action<Vector2,float> _onRun;
    PlayerInputs _playerInputs;
    private float _desiredSpeed;
    Action _animation;
    public PlayerRunState(T walkInput, Action<Vector2, float> onRun, Action onShoot, PlayerInputs playerInputs, float desiredSpeed, Action animation)
    {
        _walkInput = walkInput;
        _onRun = onRun;
        _onShoot = onShoot;
        _playerInputs = playerInputs;
        _desiredSpeed = desiredSpeed;
        _animation = animation;
    }
    public override void Execute()
    {
        _playerInputs.UpdateInputs();
        if (!_playerInputs.IsRunning())
        {
            _parentFSM.Transition(_walkInput);
            return;
        }
        if (_playerInputs.isShooting())
        {
            _onShoot?.Invoke();
        }
        _onRun?.Invoke(new Vector2(_playerInputs.GetH, _playerInputs.GetV), _desiredSpeed);
        _animation?.Invoke();
    }

}
