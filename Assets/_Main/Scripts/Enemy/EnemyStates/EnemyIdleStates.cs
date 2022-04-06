using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class EnemyIdleStates<T> : State<T>
{
    private EnemyController _enemyController;
    private Action _onIdle;
    private INode _root;
    public EnemyIdleStates(Action onIdle, INode root)
    {
        _onIdle = onIdle;
        _root = root;

    }
    public override void Execute()
    {
        _onIdle?.Invoke();
        _root.Execute();
    }
}
