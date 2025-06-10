using PlayerCharacterControl.State;
using System.Collections.Generic;
using UnityEngine;
using MyGame.Utils;
using System.Collections;
using System.Linq;
using Fusion;

public class PlayerController : NetworkBehaviour, IPlayerContext, IMousePositionGetter
{
    // 플레이어 상태 머신
    private PlayerStateMachine stateMachine;
    // 플레이어 스크립트 리스트
    private List<IPlayerComponent> components = new List<IPlayerComponent>();
    private List<IUpdatedPlayerComponent> updatedComponents = new List<IUpdatedPlayerComponent>();

    [SerializeField] private PlayerStats stats;

    [SerializeField] private Animator anim;

    [SerializeField] private AudioSource audioSource;

    private NetworkCharacterController cc;

    #region IPlayerContext Implementation

    public Animator Anim => anim;
    public AudioSource Audio => audioSource;
    public NetworkCharacterController NCC => cc;
    public PlayerStats Stats => stats;


    public void OnPlayerDeath()
    {
        // 플레이어 사망 처리
        QuitAllAction();
        stateMachine.ChangeState(EPlayerState.Die);
    }

    public void InitGround(int sectionNum)
    {
        GroundSectionNum = sectionNum;
    }
    public int GroundSectionNum { get; private set; }

    public IMousePositionGetter MousePositionGetter => this;

    #endregion

    #region IPlayerComponents
    // 이동
    [SerializeField] private PlayerMovement movement;
    // 공격
    [SerializeField] private PlayerAttack attack;
    // 체력
    [SerializeField] private PlayerHealth playerHealth;
    // 스펠
    [SerializeField] private PlayerSpellActuator playerSpell;

    public bool isOfflineMode;

    // 스킬

    #endregion

    public PlayerStateMachine StateMachine => stateMachine;

    public PlayerMovement PM => movement;
    public PlayerAttack Attack => attack;
    public PlayerHealth Health => playerHealth;

    public PlayerStateBase PlayerState => stateMachine.CurrentState;

    #region IMousePositionGetter Implementation

    public Vector3? ClickPoint { get; private set; }

    public Vector3? GetMousePosition() 
    {
        return GroundClick.GetMousePosition(Camera.main, LayerMask.GetMask("Ground"));
    }
    #endregion


    public override void Spawned()
    {
        // 플레이어 컴포넌트들 초기화
        InitializeComponents();

        // 상태 머신 초기화
        stateMachine = new(this, attack, movement);
    }

    public override void FixedUpdateNetwork()
    {
        stateMachine.UpdateCurrentState();

        // 모든 IPlayable 컴포넌트 업데이트
        foreach (var component in updatedComponents)
        {
            component.NetworkUpdated(Runner.DeltaTime);
        }

        if (GetInput(out NetworkInputData data))
        {
            if (data.buttons.IsSet(NetworkInputData.MOUSEBUTTON1))
            {
                Debug.Log("right click!");
                // 마우스 위치 저장
                ClickPoint = GetMousePosition();
                if (!ClickPoint.HasValue || !IngameController.Instance.ground.GetAdjustedPoint(GroundSectionNum, transform.position, ClickPoint.Value, out Vector3 adjustedPoint)) return;

                ClickPoint = adjustedPoint;
                StartCoroutine(SpawnEffect("ClickPointer", ClickPoint.Value));

                if (!attack.IsActionInProgress)
                    stateMachine.ChangeState(EPlayerState.Move);
            }
            if (data.buttons.IsSet(NetworkInputData.MOUSEBUTTON0))
            {
                Debug.Log("left click!");
                ClickPoint = GetMousePosition();
                if (!attack.IsActionInProgress && attack.CanExecuteAction)
                {
                    stateMachine.ChangeState(EPlayerState.Attack);
                }
            }
            if (data.buttons.IsSet(NetworkInputData.BUTTONF))
            {
                Debug.Log("F Button pressed!");
                // 마우스 위치 저장
                ClickPoint = GetMousePosition();
                if (!ClickPoint.HasValue || !IngameController.Instance.ground.GetAdjustedPoint(GroundSectionNum, transform.position, ClickPoint.Value, out Vector3 adjustedPoint)) return;

                ClickPoint = adjustedPoint;
            }
        }
    }
    public void OnEnable()
    {
        stats = new PlayerStats();
        foreach (var component in components)
        {
            component.OnEnabled();
        }
    }

    public void OnDisable()
    {
        foreach (var component in components)
        {
            component.OnDisabled();
        }
    }

    // IPlayerComponent 컴포넌트들 초기화
    private void InitializeComponents()
    {
        // 초기화 순서가 중요한 경우 순서 지정
        var initializationOrder = new List<IPlayerComponent>
        {
            playerHealth,      // 체력은 가장 먼저 초기화
            movement,  // 이동은 그 다음
            attack,      // 공격은 이동 이후
            playerSpell      // 스킬은 마지막
        };

        components = initializationOrder;


        foreach (var component in components)
        {
            component.Initialize(this, isOfflineMode);
            if (component is IUpdatedPlayerComponent)
                updatedComponents.Add(component as IUpdatedPlayerComponent);
        }
    }

    private void QuitAllAction()
    {
        components.Where(comp => comp as IPlayerAction != null)
            .ToList()
            .ForEach(comp => (comp as IPlayerAction).StopAction());
    }

    private IEnumerator SpawnEffect(string effectTag, Vector3 targetPoint)
    {
        GameObject effect = ObjectPooler.Get(effectTag);

        if (effect == null)
        {
            Debug.LogError($"Effect with tag {effectTag} not found in ObjectPooler.");
            yield break;
        }

        targetPoint.y = IngameController.Instance.ground.transform.position.y;
        effect.transform.position = targetPoint;

        yield return new WaitForSeconds(1.0f);

        ObjectPooler.Release(effectTag, effect);
    }
}