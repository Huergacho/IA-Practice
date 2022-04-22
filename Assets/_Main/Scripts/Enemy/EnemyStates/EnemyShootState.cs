using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class EnemyShootState<T> : State<T>
{
    private Action _onShoot;
    private INode _root;
    private Steerings _obsEnum;
    private ObstacleAvoidance _obstacleAvoidance;
    public EnemyShootState(Action onShoot, INode root,ObstacleAvoidance obstacleAvoidance ,Steerings steering)
    {
        _onShoot = onShoot;
        _root = root;
        _obsEnum = steering;
        _obstacleAvoidance = obstacleAvoidance;
    }
    public override void Awake()
    {
        _obstacleAvoidance.SetActualBehaviour(_obsEnum);
    }
    public override void Execute()
    {
        _root.Execute();
        _onShoot?.Invoke();
    }

}

