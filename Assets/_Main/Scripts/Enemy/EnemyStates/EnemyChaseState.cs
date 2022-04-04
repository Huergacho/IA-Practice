using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class EnemyChaseState<T> : State<T>
{
    private Action _onChase;
    private INode _root;
    public EnemyChaseState(Action onChase, INode root)
    {
        _onChase = onChase;
        _root = root;
    }
    public override void Execute()
    {
        _root.Execute();
        _onChase?.Invoke();
    }

}
