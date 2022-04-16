using System;
using UnityEngine;
using System.Collections.Generic;

class PlayerRunState<T> : State<T>
{
    T _walkInput;
    Action _onShoot;
    Action<Vector2> _onRun;
    PlayerInputs _playerInputs;

    public PlayerRunState(T walkInput, Action<Vector2> onRun, Action onShoot, PlayerInputs playerInputs)
    {
        _walkInput = walkInput;
        _onRun = onRun;
        _onShoot = onShoot;
        _playerInputs = playerInputs;
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
        _onRun?.Invoke(new Vector2(_playerInputs.GetH, _playerInputs.GetV));
    }

}
