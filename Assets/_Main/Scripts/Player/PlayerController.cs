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
    public event Action<Vector2,float> _onMove;
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
        lifeController = GetComponent<LifeController>();
        lifeController.actionToDo = DieActions;
        _playerModel.SuscribeEvents(this);
        _playerView.SuscribeEvents(this);
    }
    private void InitFSM()
    {
        var idle = new PlayerIdleState<PlayerHelper>(PlayerHelper.Walk, MovementCommand, ShootCommand, _playerInputs,_playerView.Idle);
        var walk = new PlayerWalkState<PlayerHelper>(PlayerHelper.Idle, PlayerHelper.Run, MovementCommand, ShootCommand, _playerInputs, _playerModel.Stats.WalkSpeed,_playerView.Walk);
        var run = new PlayerRunState<PlayerHelper>(PlayerHelper.Walk, MovementCommand, ShootCommand, _playerInputs, _playerModel.Stats.RunSpeed,_playerView.Run);

        idle.AddTransition(PlayerHelper.Walk, walk);
        idle.AddTransition(PlayerHelper.Run, run);

        walk.AddTransition(PlayerHelper.Idle, idle);
        walk.AddTransition(PlayerHelper.Run, run);

        run.AddTransition(PlayerHelper.Walk, walk);

        _fsm = new FSM<PlayerHelper>(idle);
    }
    private void Update()
    {
        _fsm.UpdateState();

    }

    private void MovementCommand(Vector2 dir, float desiredSpeed)
    {
        _onMove?.Invoke(dir, desiredSpeed);
    }
    private void ShootCommand()
    {
        _onShoot?.Invoke();
    }
    private void DieActions()
    {
        _onDie?.Invoke();
    }

}
