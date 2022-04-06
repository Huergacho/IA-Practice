using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
public class EnemySeekState<T> : State<T>
{
    private Action _onChase;
    private INode _root;
    public EnemySeekState(Action onChase, INode root, ObstacleAvoidance obs)
    {
        _onChase = onChase;
        _root = root;
        obs.SetActualBehaviour(ObstacleAvoidance.Behaviours.Seek);
    }
    public override void Execute()
    {
        _root.Execute();
        _onChase?.Invoke();
    }

}
