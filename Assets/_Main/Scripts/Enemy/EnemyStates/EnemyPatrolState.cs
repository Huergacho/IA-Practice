using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class EnemyPatrolState<T> : State<T>
{
    private Action _patrol;
    private INode _root;
    public EnemyPatrolState(Action patrol, INode root)
    {
        _patrol = patrol;
        _root = root;
    }
    public override void Execute()
    {
        _root.Execute();
        _patrol?.Invoke();
        
    }

}
