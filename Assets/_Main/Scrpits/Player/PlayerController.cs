using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class PlayerController : MonoBehaviour
{
    enum PlayerHelper
    {
        Idle,
        Walk
    }
    PlayerModel _playerModel;
    PlayerInputs _playerInputs;
    FSM<PlayerHelper> _fsm;
    public event Action _onIdle;
    public event Action<Vector2> _onWalk;

    private void Awake()
    {
        _playerInputs = GetComponent<PlayerInputs>();
        _playerModel = GetComponent<PlayerModel>();
        InitFSM();
    }
    private void Start()
    {
        _playerModel.SuscribeEvents(this);

    }
    private void InitFSM()
    {
        var idle = new PlayerIdleState<PlayerHelper>(PlayerHelper.Walk, OnIdleCommand, _playerInputs);
        var walk = new PlayerWalkStates<PlayerHelper>(PlayerHelper.Idle, OnMoveCommand, _playerInputs);

        idle.AddTransition(PlayerHelper.Walk, walk);

        walk.AddTransition(PlayerHelper.Idle, idle);
        _fsm = new FSM<PlayerHelper>(idle);
    }
    private void Update()
    {
        _fsm.UpdateState();

    }
    private void OnMoveCommand(Vector2 dir)
    {
        print(dir);
        _onWalk?.Invoke(dir);
    }
    private void OnIdleCommand()
    {
        _onIdle?.Invoke();
    }

}
