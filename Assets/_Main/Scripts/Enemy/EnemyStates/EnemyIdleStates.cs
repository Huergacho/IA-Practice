using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
class EnemyIdleStates<T> : State<T>
{
    private Func<bool> _outOfIdleCommand;
    private Action _onIdle;
    private INode _root;
    public EnemyIdleStates(Action onIdle, INode root,Func<bool> outOfIdleCommand)
    {
        _onIdle = onIdle;
        _root = root;
        _outOfIdleCommand = outOfIdleCommand;
    }
    public override void Execute()
    {
        if (_outOfIdleCommand())
        {
            _root.Execute();
        }
        _onIdle?.Invoke();
    }
}
