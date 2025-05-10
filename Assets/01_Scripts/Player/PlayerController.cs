using Photon.Pun;
using PlayerCharacterControl.State;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using MyGame.Utils;
using System.Collections;
using System.Linq;


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

    #region IPlayerComponents
    // 이동
    [SerializeField] private PlayerMovement movement;
    // 공격
    [SerializeField] private PlayerAttack attack;
    // 체력
    [SerializeField] private PlayerHealth playerHealth;
    // 스펠
    [SerializeField] private PlayerSpell playerSpell;

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
            playerSpell      // 스킬은 마지막
        };

        components = initializationOrder;


        foreach (var component in components)
        {
            component.Initialize(this, isOfflineMode);
        }
    }

    private void QuitAllAction()
    {
        components.Where(comp => comp as IPlayerAction != null)
            .ToList()
            .ForEach(comp => (comp as IPlayerAction).StopAction());
    }

    public void HandleInput(InputAction.CallbackContext context)
    {
        if (!photonView.IsMine || Stats.IsDead()) return;

        if (context.performed)
        {
            switch (context.action.name)
            {
                case "Move":
                    // 마우스 위치 저장
                    ClickPoint = GetMousePosition();
                    if (!ClickPoint.HasValue || !IngameController.Instance.ground.GetAdjustedPoint(GroundSectionNum, Pos, ClickPoint.Value, out Vector3 adjustedPoint)) return;

                    ClickPoint = adjustedPoint;
                    StartCoroutine(SpawnEffect("ClickPointer", ClickPoint.Value));

                    if (!attack.IsActionInProgress)
                        stateMachine.ChangeState(EPlayerState.Move);
                    break;

                case "Attack":
                    break;

                case "Click":
                    // 마우스 위치 저장
                    ClickPoint = GetMousePosition();
                    if (!attack.IsActionInProgress && attack.CanExecuteAction)
                    {
                        stateMachine.ChangeState(EPlayerState.Attack);
                    }
                    else
                        return;
                    break;

                case "D":
                    break;
                case "F":
                    // 마우스 위치 저장
                    ClickPoint = GetMousePosition();
                    if (!ClickPoint.HasValue || !IngameController.Instance.ground.GetAdjustedPoint(GroundSectionNum, Pos, ClickPoint.Value, out adjustedPoint)) return;

                    ClickPoint = adjustedPoint;
                    break;

                default:
                    break;
            }

            foreach(var component in components)
            {
                component.HandleInput(context);
            }
        }        
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