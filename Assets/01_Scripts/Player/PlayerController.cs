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

    [SerializeField] private PlayerSound sound;

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
    public PlayerSpellActuator Spell => playerSpell;
    public PlayerSound Sound => sound;

    // Unity Components
    public Animator Anim => anim;

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
        // 상태 머신 초기화
        stateMachine = new(this);

        movement.Initialize(this);
        attack.Initialize(this);
        health.Initialize(this);
        playerSpell.Initialize(this);
        sound.Init();
        stats = new PlayerStats();
    }

    public override void Render()
    {
        stateMachine.Updated();
    }

    public override void FixedUpdateNetwork()
    {
        if (CurrentState is PlayerDieState) return;

        if (GetInput(out NetworkInputData data))
        {
            if (data.buttons.IsSet(NetworkInputData.BUTTONSTOP)) // 스탑 버튼
            {
                if (CurrentState is PlayerAttackState) return;
                ChangeState(EPlayerState.Idle);
            }
            if (data.buttons.IsSet(NetworkInputData.MOUSEBUTTON0)) // 좌클릭
            {
                if (CurrentState is PlayerAttackState || attack.CoolTime > 0.0f) return;
                if (data.targetPoint == Vector3.zero) return;
                ChangeState(EPlayerState.Attack, new(data.targetPoint));
            }
            if (data.buttons.IsSet(NetworkInputData.MOUSEBUTTON1)) // 우클릭
            {
                if (ObjectPooler.Instance != null)
                {
                    GameObject clickEffect = ObjectPooler.GetLocal("MoveClickEffect");
                    clickEffect.transform.position = data.movePoint;
                }
                if (CurrentState is PlayerAttackState) return;
                if (data.movePoint == Vector3.zero) return;
                ChangeState(EPlayerState.Move, new(data.movePoint));
            }
            if (data.buttons.IsSet(NetworkInputData.BUTTOND)) // D 스펠
            {
                if (data.targetPoint == Vector3.zero) return;

                playerSpell.ExecuteD(data.targetPoint);
            }

            if (data.buttons.IsSet(NetworkInputData.BUTTONF)) // F 스펠
            {
                if (data.targetPoint == Vector3.zero) return;

                playerSpell.ExecuteF(data.targetPoint);
            }
        }

        stateMachine.NetworkUpdated(Runner.DeltaTime);
    }
}