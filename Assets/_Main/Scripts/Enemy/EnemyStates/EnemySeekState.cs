using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
public class EnemySeekState<T> : State<T>
{
    private Action _onSeek;
    private INode _root;
    private Steerings _obsEnum;
    private ObstacleAvoidance _obs;
    public EnemySeekState(Action onSeek, INode root, ObstacleAvoidance obs, Steerings obsEnum)
    {
        _onSeek = onSeek;
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
        _onSeek?.Invoke();
    }

}
