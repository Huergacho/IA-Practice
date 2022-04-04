using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class CameraController : MonoBehaviour
{

    CameraModel _model;
    CameraView _cameraView;
    public event Action onIdle;
    public event Action<Transform> onSeek;
    private FSM<CameraStates> _fsm;
    [SerializeField] private LineOfSight _lineOfSight;
    // Start is called before the first frame update
    private void Awake()
    {
        _model = GetComponent<CameraModel>();
        _lineOfSight = GetComponent<LineOfSight>();
        _cameraView = GetComponent<CameraView>();
        InitFsm();

    }
    void Start()
    {
        _model.SuscribeEvents(this);
        _cameraView.SuscribeEvents(this);
    }


    private void InitFsm()
    {
        var idle = new CameraIdleState<CameraStates>(CameraStates.Seek, OnIdle, _lineOfSight);
        var seek = new CameraSeekState<CameraStates>(CameraStates.Idle, OnDetect, _lineOfSight);

        idle.AddTransition(CameraStates.Seek, seek);

        seek.AddTransition(CameraStates.Idle, idle);


        _fsm = new FSM<CameraStates>(idle);
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
