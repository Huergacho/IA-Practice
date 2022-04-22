using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
[RequireComponent (typeof (PlayerModel), typeof (PlayerInputs))]
public class PlayerController : MonoBehaviour
{
    enum PlayerHelper
    {
        Idle,
        Walk,
        Run
    }
    PlayerModel _playerModel;
    PlayerInputs _playerInputs;
    PlayerView _playerView;
    FSM<PlayerHelper> _fsm;
    public event Action _onIdle;
    public event Action<Vector2> _onWalk;
    public event Action<Vector2> _onRun;
    public event Action _onShoot;
    public event Action _onDie;
    private LifeController lifeController;
    private void Awake()
    {
        _playerInputs = GetComponent<PlayerInputs>();
        _playerModel = GetComponent<PlayerModel>();
        _playerView = GetComponent<PlayerView>();
        InitFSM();
    }
    private void Start()
    {
        GameManager.Instance.AssignPlayer(this);
        lifeController = GetComponent<LifeController>();
        lifeController.actionToDo = DieActions;
        _playerModel.SuscribeEvents(this);
        _playerView.SuscribeEvents(this);

    }
    private void InitFSM()
    {
        var idle = new PlayerIdleState<PlayerHelper>(PlayerHelper.Walk, OnIdleCommand,ShootCommand, _playerInputs);
        var walk = new PlayerWalkState<PlayerHelper>(PlayerHelper.Idle, PlayerHelper.Run, OnWalkCommand,ShootCommand, _playerInputs);
        var run = new PlayerRunState<PlayerHelper>(PlayerHelper.Walk, OnRunCommand,ShootCommand, _playerInputs);

        idle.AddTransition(PlayerHelper.Walk, walk);

        walk.AddTransition(PlayerHelper.Idle, idle);
        walk.AddTransition(PlayerHelper.Run, run);

        run.AddTransition(PlayerHelper.Walk, walk);

        _fsm = new FSM<PlayerHelper>(idle);
    }
    private void Update()
    {
        _fsm.UpdateState();

    }
    private void OnWalkCommand(Vector2 dir)
    {
        _onWalk?.Invoke(dir);
      
    }
    private void OnRunCommand(Vector2 dir)
    {
        _onRun?.Invoke(dir);
    }
    private void OnIdleCommand()
    {
        _onIdle?.Invoke();
    }
    private void ShootCommand()
    {
        _onShoot?.Invoke();
    }
    private void ModelShoot()
    {
        _playerModel.Shoot();
    }
    private void DieActions()
    {
        _onDie?.Invoke();
    }

}
