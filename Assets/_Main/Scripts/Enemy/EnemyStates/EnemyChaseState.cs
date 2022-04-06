using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class EnemyChaseState<T> : State<T>
{
    private Action _onChase;
    private INode _root;
    private ObstacleAvoidance _obs;
    private ObstacleAvoidance.Behaviours _obsEnum;
    public EnemyChaseState(Action onChase, INode root, ObstacleAvoidance obs, ObstacleAvoidance.Behaviours obsEnum)
    {
        _onChase = onChase;
        _root = root;
        _obs = obs;
        _obsEnum = obsEnum;
    }
    public override void Awake()
    {
        _obs.SetActualBehaviour(_obsEnum);

    }
    public override void Execute()
    {
        _root.Execute();
        _onChase?.Invoke();
    }

}
