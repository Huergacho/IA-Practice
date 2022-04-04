using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class EnemyIdleStates<T> : State<T>
{
    private EnemyController _enemyController;
    private Action _onIdle;
    public EnemyIdleStates(Action onIdle)
    {
        _onIdle = onIdle;

    }
    public override void Execute()
    {
        _onIdle?.Invoke();
    }
}
