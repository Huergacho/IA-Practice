using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class MonitorIdleState<T> : State<T>
{
    T _seekInput;
    private event Action _onIdle;
    private LineOfSight _lineOfSight;
    public MonitorIdleState(T seekInput, Action onIdle,LineOfSight lineOfSight)
    {
        _onIdle = onIdle;
        _seekInput = seekInput;
        _lineOfSight = lineOfSight;
    }
    public override void Execute()
    {
        if (_lineOfSight.CheckForOneTarget())
        {
            _parentFSM.Transition(_seekInput);
            return;
        }
        _onIdle?.Invoke();
    }
}

