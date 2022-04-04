using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
public class PlayerWalkStates<T> : State<T>
{
    T _idleInput;
    T _runInput;
    Action<Vector2> _onWalk;
    PlayerInputs _playerInputs;

    public PlayerWalkStates(T idleInput, T runInput, Action<Vector2> onWalk, PlayerInputs playerInputs)
    {
        _idleInput = idleInput;
        _runInput = runInput;
        _onWalk = onWalk;
        _playerInputs = playerInputs;
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
        _onWalk?.Invoke(new Vector2(_playerInputs.GetH,_playerInputs.GetV));
    }
}

