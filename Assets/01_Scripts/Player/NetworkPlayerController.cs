using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;

public class NetworkPlayerController : MonoBehaviourPunCallbacks
{
    [SerializeField] private bool isOfflineMode = false;  // 오프라인 테스트 모드
    [SerializeField] private bool isDebugMode = false;  // 디버그 모드 추가
    private PlayerController playerController;
    private PlayerInputEventSystem inputSystem;

    public PlayerController PlayerCtrl => playerController;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        inputSystem = GetComponent<PlayerInputEventSystem>();

        // 오프라인 모드일 경우 PhotonView를 비활성화
        if (isOfflineMode)
        {
            playerController.isOfflineMode = true;
            photonView.enabled = false;
        }
    }

    public override void OnEnable()
    {
        inputSystem.PlayerInputEvent.AddListener(OnPlayerInput);
    }

    public override void OnDisable()
    {
        inputSystem.PlayerInputEvent.RemoveListener(OnPlayerInput);
    }

    public void OnPlayerInput(InputAction.CallbackContext context)
    {
        if (isDebugMode)
        {
            // 디버그 로그 추가
            Debug.Log($"Input Received: {context.action.name} from {photonView.Owner.NickName}");
        }

        if (context.performed)
        {
            if (isOfflineMode)
            {
                // 오프라인 모드: 직접 입력 처리
                playerController.HandleInput(context.action.name);
            }

            else if (playerController.p_PhotonView.IsMine)
            {
                playerController.p_PhotonView.RPC("HandleInput_RPC", RpcTarget.All, context.action.name);
            }
        }
    }

    [PunRPC]
    private void HandleInput_RPC(string actionName)
    {
        if (isDebugMode)
        {
            Debug.Log($"RPC Received: {actionName} on {playerController.p_PhotonView.ViewID}");
        }


        playerController.HandleInput(actionName);
    }
}
