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
    private PlayerStateMachine playerStateMachine;
    // 플레이어 스크립트 리스트
    private List<IPlayerComponent> components = new List<IPlayerComponent>();

    #region IPlayerComponents
    // 이동
    [SerializeField] private PlayableMovement playableMovement;
    // 공격
    [SerializeField] private PlayerAttack playerAttack;
    // 체력
    [SerializeField] private PlayerHealth playerHealth;
    // 스킬

    #endregion

    [SerializeField] private PlayerAnimEventHandler playerAnimEventHandler;

    [SerializeField] private Animator playerAnim;


    #region properties
    public PlayerStateMachine StateMachine => playerStateMachine;

    public PlayableMovement PM => playableMovement;
    public PlayerAttack Attack => playerAttack;
    public PlayerHealth Health => playerHealth;

    public PlayerStateBase PlayerState => playerStateMachine.CurrentState;
    #endregion

    #region IPlayerContext Implementation

    public Animator Anim => playerAnim;

    public Transform Trf => transform;

    public Vector3 Pos => transform.position;

    public Quaternion Rot => transform.rotation;

    public bool IsLocalPlayer() { return photonView.IsMine; }

    public void OnPlayerDeath()
    {
        // 플레이어 사망 처리
        QuitAllAction();
        gameObject.SetActive(false);
    }

    public PlayerStateBase GetCurrentState() { return playerStateMachine.CurrentState; }

    public Vector3? GetMousePosition(string layer = "Ground") { return Utility.GetMousePosition(Camera.main, layer); }
    #endregion

    void Awake()
    {
        // 컴포넌트 초기화
        InitializeComponents();
        
        // 애니메이션 이벤트 설정
        playerAnimEventHandler.OnAnimationEventActions.AddListener(GetAnimationEvent);

        // 상태 머신 초기화
        playerStateMachine = new(this, playerAttack, playableMovement);
    }

    // IPlayerComponent 컴포넌트들 초기화
    private void InitializeComponents()
    {
        // 초기화 순서가 중요한 경우 순서 지정
        var initializationOrder = new List<IPlayerComponent>
        {
            playerHealth,      // 체력은 가장 먼저 초기화
            playableMovement,  // 이동은 그 다음
            playerAttack,      // 공격은 이동 이후
            //playerSpell      // 스킬은 마지막
        };

        components = initializationOrder;


        foreach (var component in components)
        {
            component.Initialize(this);
        }
    }

    void Update()
    {
        playerStateMachine.UpdateCurrentState();

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

    private void QuitAllAction()
    {
        playableMovement.StopMove();
        playerAttack.CancelAttack();
    }

    private void GetAnimationEvent(string tag)
    {
        switch (tag)
        {
            case "Throw Axe":
                playerAttack.SpawnAxe();
                break;
        }
    }

    public void HandleInput(InputAction.CallbackContext context)
    {
        //if (!photonView.IsMine) return;
        if (context.performed)
        {
            switch (context.action.name)
            {
                case "Attack":
                    playerAttack.ActivateRange(true);
                    Debug.Log("Q");
                    break;
                default:
                    playerStateMachine.HandleInput(context.action.name);
                    Debug.Log(context.action.name);
                    break;

            }
        }
    }
}