using Mundo_dodgeball.Player.StateMachine;
using System.Collections.Generic;
using UnityEngine;
using MyGame.Utils;
using System.Collections;
using System.Linq;
using Fusion;

public class PlayerController : NetworkBehaviour, IPlayerContext
{
    // 플레이어 상태 머신
    private PlayerStateMachine stateMachine;
    // 플레이어 스크립트 리스트
    //private List<IPlayerComponent> components = new List<IPlayerComponent>();
    //private List<IUpdatedPlayerComponent> updatedComponents = new List<IUpdatedPlayerComponent>();

    [SerializeField] private PlayerStats stats;

    [SerializeField] private Animator anim;

    [SerializeField] private AudioSource audioSource;

    // 이동
    [SerializeField] private PlayerMovement movement;
    // 공격
    [SerializeField] private PlayerAttack attack;
    // 체력
    [SerializeField] private PlayerHealth health;
    // 스펠
    [SerializeField] private PlayerSpellActuator playerSpell;

    public PlayerStateBase CurrentState => stateMachine.CurrentState;
    public PlayerStateMachine StateMachine => stateMachine;
    public PlayerStats Stats => stats;

    // NetworkBehaviours
    public PlayerMovement Movement => movement;
    public PlayerAttack Attack => attack;
    public PlayerHealth Health => health;

    // Unity Components
    public Animator Anim => anim;
    public AudioSource Audio => audioSource;

    public PlayerStatData CurrentStatData => _currentStatData;

    [Networked] private PlayerStatData _currentStatData { get; set; }

    public void ChangeState(EPlayerState state, StateTransitionInputData inputData = new())
    {
        if (Object.HasStateAuthority)
        {
            RPC_ChangeState(state, inputData);
        }
        else
        {
            stateMachine.ChangeState(state, inputData);
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_ChangeState(EPlayerState state, StateTransitionInputData inputData = new())
    {
        stateMachine.ChangeState(state, inputData);
    }

    public override void Spawned()
    {
        if (IngameController.Instance != null)
            movement.SetGround(IngameController.Instance.ground);
        else
            movement.SetGround(FindAnyObjectByType<Ground>());

        // 상태 머신 초기화
        stateMachine = new(this);

        movement.Initialize(this);
        attack.Initialize(this);
        health.Initialize(this);
        stats = new PlayerStats();
        _currentStatData = stats.CurrentStatData;
    }

    public override void Render()
    {
        stateMachine.Updated();

        _currentStatData = stats.CurrentStatData;
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData data))
        {
            if (data.buttons.IsSet(NetworkInputData.MOUSEBUTTON0)) // 좌클릭
            {
                if (CurrentState is PlayerAttackState || attack.CoolTiming) return;
                if (data.targetPoint == Vector3.zero) return;
                ChangeState(EPlayerState.Attack, new(data.targetPoint));
            }
            if (data.buttons.IsSet(NetworkInputData.MOUSEBUTTON1)) // 우클릭
            {
                if (CurrentState is PlayerAttackState) return;
                if (data.movePoint == Vector3.zero) return;
                ChangeState(EPlayerState.Move, new(data.movePoint));
            }
            if (data.buttons.IsSet(NetworkInputData.BUTTONF))
            {
                // 마우스 위치 저장
                if (data.targetPoint == Vector3.zero) return;

                ChangeState(EPlayerState.Idle);
                // 플레쉬 실행 추가
            }
        }

        stateMachine.NetworkUpdated(Runner.DeltaTime);
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(0, 10, 300, 200), "Current Health" + _currentStatData.Health);

        if (GUI.Button(new Rect(0, 200, 300, 200), "Set HP 0"))
        {
            stats.SetCurrentHealth(0.0f);
        }
    }
}