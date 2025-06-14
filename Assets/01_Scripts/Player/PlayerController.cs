using Mundo_dodgeball.Player.StateMachine;
using System.Collections.Generic;
using UnityEngine;
using MyGame.Utils;
using System.Collections;
using System.Linq;
using Fusion;
using Unity.VisualScripting;

public class PlayerController : NetworkBehaviour, IPlayerContext, IMousePositionGetter
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
    [SerializeField] private PlayerHealth playerHealth;
    // 스펠
    [SerializeField] private PlayerSpellActuator playerSpell;

    public PlayerStateMachine StateMachine => stateMachine;
    public PlayerMovement Movement => movement;
    public PlayerAttack Attack => attack;
    public PlayerHealth Health => playerHealth;

    #region IPlayerContext Implementation

    public Animator Anim => anim;
    public AudioSource Audio => audioSource;
    public PlayerStats Stats => stats;

    public PlayerStateBase CurrentState => stateMachine.CurrentState;


    public void InitGround(int sectionNum)
    {
        GroundSectionNum = sectionNum;
    }
    public int GroundSectionNum { get; private set; }

    public IMousePositionGetter MousePositionGetter => this;

    #endregion

    #region IMousePositionGetter Implementation

    public Vector3? ClickPoint { get; private set; }

    public Vector3? GetMousePosition() 
    {
        return GroundClick.GetMousePosition(Camera.main, LayerMask.GetMask("Ground"));
    }
    #endregion


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
        stats = new PlayerStats();
    }

    public override void Render()
    {
        stateMachine.Updated();
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData data))
        {
            if (data.buttons.IsSet(NetworkInputData.MOUSEBUTTON0)) // 좌클릭
            {
                if (CurrentState is PlayerAttackState || attack.CoolTiming) return;
                ClickPoint = data.targetPoint;
                ChangeState(EPlayerState.Attack, new(ClickPoint.Value));
            }
            if (data.buttons.IsSet(NetworkInputData.MOUSEBUTTON1)) // 우클릭
            {
                if (CurrentState is PlayerAttackState) return;
                ClickPoint = data.movePoint;
                if (!ClickPoint.HasValue) return;
                ChangeState(EPlayerState.Move, new(ClickPoint.Value));
            }
            if (data.buttons.IsSet(NetworkInputData.BUTTONF))
            {
                // 마우스 위치 저장
                ClickPoint = data.targetPoint;
                if (!ClickPoint.HasValue) return;

                ChangeState(EPlayerState.Idle);
                // 플레쉬 실행 추가
            }
        }

        stateMachine.NetworkUpdated(Runner.DeltaTime);

    }

    private IEnumerator SpawnEffect(string effectTag, Vector3 targetPoint)
    {
        GameObject effect = ObjectPooler.GetLocal(effectTag);

        if (effect == null)
        {
            Debug.LogError($"Effect with tag {effectTag} not found in ObjectPooler.");
            yield break;
        }

        targetPoint.y = IngameController.Instance.ground.transform.position.y;
        effect.transform.position = targetPoint;

        yield return new WaitForSeconds(1.0f);

        ObjectPooler.ReleaseLocal(effectTag, effect);
    }
}