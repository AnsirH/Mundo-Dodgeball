using Photon.Pun;
using PlayerCharacterControl.State;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using MyGame.Utils;
using System.Collections;


public class PlayerController : MonoBehaviourPunCallbacks, IPlayerContext, IMousePositionGetter
{
    // 플레이어 상태 머신
    private PlayerStateMachine stateMachine;
    // 플레이어 스크립트 리스트
    private List<IPlayerComponent> components = new List<IPlayerComponent>();

    [SerializeField] private PlayerStats stats;

    [SerializeField] private Animator playerAnim;

    [SerializeField] private PlayerInputEventSystem inputSystem;

    [SerializeField] private PhotonTransformViewClassic ptv;
    private Vector3 previousPosition;

    // IngameController가 할당
    // 인게임 필드
    public Ground PlayGround { get; private set; }
    public int GroundSectionNum { get; private set; }
    public void InitGround(Ground ground, int sectionNum)
    {
        PlayGround = ground;
        GroundSectionNum = sectionNum;
    }


    #region IPlayerComponents
    // 이동
    [SerializeField] private PlayerMovement movement;
    // 공격
    [SerializeField] private PlayerAttack attack;
    // 체력
    [SerializeField] private PlayerHealth playerHealth;

    public bool isOfflineMode;

    // 스킬

    #endregion

    #region properties
    public PlayerStateMachine StateMachine => stateMachine;

    public PlayerMovement PM => movement;
    public PlayerAttack Attack => attack;
    public PlayerHealth Health => playerHealth;

    public PlayerStateBase PlayerState => stateMachine.CurrentState;
    #endregion

    #region IPlayerContext Implementation

    public Animator Anim => playerAnim;

    public Transform Trf => transform;

    public Vector3 Pos => transform.position;

    public Quaternion Rot => transform.rotation;

    public PlayerStats Stats => stats;

    public PhotonView p_PhotonView => photonView;

    public void OnPlayerDeath()
    {
        // 플레이어 사망 처리
        QuitAllAction();
        stateMachine.ChangeState(EPlayerState.Die);
    }

    public IMousePositionGetter MousePositionGetter => this;

    #endregion

    #region IMousePositionGetter Implementation

    public Vector3? ClickPoint { get; private set; }

    public Vector3? GetMousePosition() 
    {
        return Utility.GetMousePosition(Camera.main, LayerMask.GetMask("Ground"));
    }
    #endregion

    void Awake()
    {
        inputSystem = GetComponent<PlayerInputEventSystem>();
        ptv = GetComponent<PhotonTransformViewClassic>();

        // 플레이어 컴포넌트들 초기화
        InitializeComponents();

        // 상태 머신 초기화
        stateMachine = new(this, attack, movement);

        previousPosition = transform.position;
    }

    void Update()
    {
        stateMachine.UpdateCurrentState();

        // 모든 IPlayable 컴포넌트 업데이트
        foreach (var component in components)
        {
            component.Updated();
        }


        if (photonView.IsMine)
        {
            Vector3 velocity = (transform.position - previousPosition) / Time.deltaTime;

            // 보간/예측을 위한 속도 전달
            ptv.SetSynchronizedValues(velocity, 0f);  // 회전속도는 안 쓰면 0

            previousPosition = transform.position;
        }
    }
    public override void OnEnable()
    {
        base.OnEnable();
        stats = new PlayerStats();
        inputSystem.PlayerInputEvent.AddListener(HandleInput);
        foreach (var component in components)
        {
            component.OnEnabled();
        }
    }

    public override void OnDisable()
    {
        base.OnDisable();
        inputSystem.PlayerInputEvent.RemoveListener(HandleInput);
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
            //playerSpell      // 스킬은 마지막
        };

        components = initializationOrder;


        foreach (var component in components)
        {
            component.Initialize(this, isOfflineMode);
        }
    }

    private void QuitAllAction()
    {
        movement.StopMove();
        attack.CancelAttack();
    }

    public void HandleInput(InputAction.CallbackContext context)
    {
        if (!photonView.IsMine || Stats.IsDead()) return;

        if (context.performed)
        {
            switch (context.action.name)
            {
                case "Move":
                    HandleMoveInput();
                    break;

                case "Attack":
                    HandleAttackInput();
                    break;

                case "Click":
                    HandleClickInput();
                    break;

                default:
                    break;
            }
        }        
    }

    public void HandleMoveInput()
    {
        // 마우스 위치 저장
        ClickPoint = GetMousePosition();
        if (!ClickPoint.HasValue || !PlayGround.GetAdjustedPoint(GroundSectionNum, Pos, ClickPoint.Value, out Vector3 adjustedPoint)) return;

        ClickPoint = adjustedPoint;

        StartCoroutine(ActiveClickPointer(ClickPoint.Value));
        if (!attack.IsActionInProgress)
        {
            stateMachine.ChangeState(EPlayerState.Move);
            attack.ActivateRange(false);
        }
    }

    public void HandleAttackInput()
    {
        if (!attack.IsActionInProgress)
            attack.ActivateRange(true);
    }

    public void HandleClickInput()
    {
        // 마우스 위치 저장
        ClickPoint = GetMousePosition();

        if (!attack.IsActionInProgress && attack.CanExecuteAction)
        {
            stateMachine.ChangeState(EPlayerState.Attack);
            movement.StopMove();
        }
    }


    private IEnumerator ActiveClickPointer(Vector3 targetPoint)
    {
        GameObject clickPointer = ObjectPooler.Get("ClickPointer");

        targetPoint.y = PlayGround.transform.position.y;
        clickPointer.transform.position = targetPoint;

        yield return new WaitForSeconds(1.0f);

        ObjectPooler.Release("ClickPointer", clickPointer);
    }
}