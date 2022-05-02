using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
public class PlayerIdleState<T> : State<T>
{
    T _walkInput;
    private Action<Vector2, float> _onIdle;
    private PlayerInputs _playerInputs;
    Action _onShoot;
    Action _animation;
    public PlayerIdleState(T walkInput, Action<Vector2,float> onIdle, Action onShoot, PlayerInputs playerInputs, Action animation)
    {
        _walkInput = walkInput;
        _onIdle = onIdle;
        _playerInputs = playerInputs;
        _onShoot = onShoot;
        _animation = animation;
    }
    public override void Execute()
    {
        _playerInputs.UpdateInputs();

        if (_playerInputs.IsMoving())
        {
            _parentFSM.Transition(_walkInput);
            return;
        }
        if (_playerInputs.isShooting())
        {
            _onShoot?.Invoke();
        }
        _onIdle?.Invoke(new Vector2(_playerInputs.GetH, _playerInputs.GetV), 0);
        _animation?.Invoke();
    }


}
