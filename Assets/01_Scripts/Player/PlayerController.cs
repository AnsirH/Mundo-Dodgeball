using Photon.Pun;
using PlayerCharacterControl;
using PlayerCharacterControl.State;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using MyGame.Utils;


public class PlayerController : MonoBehaviourPunCallbacks, IPlayerContext
{
    // 플레이어 상태 머신
    private PlayerStateMachine stateMachine;
    // 플레이어 스크립트 리스트
    private List<IPlayerComponent> components = new List<IPlayerComponent>();

    //[SerializeField] private PlayerAnimEventHandler playerAnimEventHandler;
    [SerializeField] private PlayerStats stats;

    [SerializeField] private Animator playerAnim;

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

    public bool IsLocalPlayer() { return photonView.IsMine; }

    public void OnPlayerDeath()
    {
        // 플레이어 사망 처리
        QuitAllAction();
        gameObject.SetActive(false);
    }

    public PlayerStateBase GetCurrentState() { return stateMachine.CurrentState; }

    public Vector3? GetMousePosition(string layer = "Ground") { return Utility.GetMousePosition(Camera.main, layer); }
    #endregion

    void Awake()
    {
        // 플레이어 컴포넌트들 초기화
        InitializeComponents();

        // 상태 머신 초기화
        stateMachine = new(this, attack, movement);
    }

    void Update()
    {
        stateMachine.UpdateCurrentState();

        // 모든 IPlayable 컴포넌트 업데이트
        foreach (var component in components)
        {
            component.Updated();
        }
    }

    public override void OnEnable()
    {
        base.OnEnable();
        foreach (var component in components)
        {
            component.OnEnabled();
        }
    }

    public override void OnDisable()
    {
        base.OnDisable();
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
        if (!photonView.IsMine) return;

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
        if (!attack.IsActionInProgress && attack.CanExecuteAction)
        {
            stateMachine.ChangeState(EPlayerState.Attack);
            movement.StopMove();
        }
    }
}