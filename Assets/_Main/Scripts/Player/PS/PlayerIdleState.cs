using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
public class PlayerIdleState<T> : State<T>
{
    T _walkInput;
    private Action _onIdle;
    private PlayerInputs _playerInputs;

    public PlayerIdleState(T walkInput, Action onIdle, PlayerInputs playerInputs)
    {
        _walkInput = walkInput;
        _onIdle = onIdle;
        _playerInputs = playerInputs;
    }
    public override void Execute()
    {
        _playerInputs.UpdateInputs();

        if (_playerInputs.IsMoving())
        {
            _parentFSM.Transition(_walkInput);
            return;
        }
        _onIdle?.Invoke();
    }


}
