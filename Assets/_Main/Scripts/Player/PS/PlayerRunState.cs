using System;
using UnityEngine;
using System.Collections.Generic;

class PlayerRunState<T> : State<T>
{
    T _walkInput;
    Action<Vector2> _onRun;
    PlayerInputs _playerInputs;

    public PlayerRunState(T walkInput, Action<Vector2> onRun, PlayerInputs playerInputs)
    {
        _walkInput = walkInput;
        _onRun = onRun;
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
        _onRun?.Invoke(new Vector2(_playerInputs.GetH, _playerInputs.GetV));
    }

}
