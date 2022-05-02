using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
class PlayerShootState<T> : State<T>
{
    T _walkInput;
    private Action _onShoot;
    private PlayerInputs _playerInputs;
    public PlayerShootState(T walkInput, Action oNShoot, PlayerInputs playerInputs)
    {
        _walkInput = walkInput;
        _onShoot = oNShoot;
        _playerInputs = playerInputs;
    }
    public override void Execute()
    {

       
    }
}
