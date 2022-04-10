using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class MonitorController : MonoBehaviour
{

    MonitorModel _model;
    MonitorView _cameraView;
    public event Action onIdle;
    public event Action<Transform> onSeek;
    private FSM<MonitorStates> _fsm;
    private LineOfSight _lineOfSight;
    // Start is called before the first frame update
    private void Awake()
    {
        _model = GetComponent<MonitorModel>();
        _lineOfSight = GetComponent<LineOfSight>();
        _cameraView = GetComponent<MonitorView>();
        InitFsm();

    }
    void Start()
    {
        _model.SuscribeEvents(this);
        _cameraView.SuscribeEvents(this);
    }


    private void InitFsm()
    {
        var idle = new MonitorIdleState<MonitorStates>(MonitorStates.Seek, OnIdle, _lineOfSight);
        var seek = new MonitorSeekState<MonitorStates>(MonitorStates.Idle, OnDetect, _lineOfSight);

        idle.AddTransition(MonitorStates.Seek, seek);

        seek.AddTransition(MonitorStates.Idle, idle);


        _fsm = new FSM<MonitorStates>(idle);
    }
    private void Update()
    {
        _fsm.UpdateState();
    }
    public void OnIdle()
    {
        onIdle?.Invoke();
    }
    public void OnDetect(Transform target)
    {
        onSeek?.Invoke(target);
    }
}
