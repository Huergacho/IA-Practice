using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
public class PlayerWalkStates<T> : State<T>
{
    T _idleInput;
    Action<Vector2> _onWalk;
    PlayerInputs _playerInputs;

    public PlayerWalkStates(T idleInput, Action<Vector2> onWalk, PlayerInputs playerInputs)
    {
        _idleInput = idleInput;
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
        _onWalk?.Invoke(new Vector2(_playerInputs.GetH,_playerInputs.GetV));
    }
}

