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
    Action<Vector2> _onWalk;
    Action _onShoot;
    PlayerInputs _playerInputs;

    public PlayerWalkState(T idleInput, T runInput, Action<Vector2> onWalk, Action onShoot, PlayerInputs playerInputs)
    {
        _idleInput = idleInput;
        _runInput = runInput;
        _onWalk = onWalk;
        _onShoot = onShoot;
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
        if (_playerInputs.isShooting())
        {
            _onShoot?.Invoke();
        }
        _onWalk?.Invoke(new Vector2(_playerInputs.GetH,_playerInputs.GetV));
    }
}

