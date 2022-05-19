using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
public class MonitorSeekState<T> : State<T>
{
    private event Action<Transform> _onSeek;
    private LineOfSight _lineOfSight;
    private T _idleInput;
    public MonitorSeekState(T idleInput,Action<Transform> onSeek, LineOfSight lineOfSight)
    {
        _idleInput = idleInput;
        _onSeek = onSeek;
        _lineOfSight = lineOfSight;
    }
    public override void Execute()
    {

        //Quiero un checkeo Multiple y que no siga a nadie
        //if (!_lineOfSight.CanSeeManyTargets())
        //{
        //    _parentFSM.Transition(_idleInput);
        //    return;
        //}
        //_onSeek.Invoke(null);



        //Quiero que sea un Checkeo Individual y que siga al primero que vio
        //if (!_lineOfSight.CheckForOneTarget())
        //{
        //    _parentFSM.Transition(_idleInput);
        //    return;
        //}
        //_onSeek?.Invoke(_lineOfSight.GetTheFirstTarget());

        //Quiero un Checkeo Individual y que siga al ultimo que vio
        if (!_lineOfSight.CheckForOneTarget())
        {
            _parentFSM.Transition(_idleInput);
            return;
        }
        _onSeek?.Invoke(_lineOfSight.GetTheLastTarget());
    }

}
