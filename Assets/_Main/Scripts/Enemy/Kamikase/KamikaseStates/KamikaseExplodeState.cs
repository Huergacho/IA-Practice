using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
public class KamikaseExplodeState<T> : State<T>
{
    private Action _onExplode;
    private Func<bool> _canExplode;
    private INode _root;
    public KamikaseExplodeState(Func<bool> canExplode,Action onExplode, INode root)
    {
        _onExplode = onExplode;
        _root = root;
        _canExplode = canExplode;
    }
    public override void Execute()
    {
        if (!_canExplode())
        {
            _root.Execute();
            return;
        }
        _onExplode?.Invoke();
    }

}

