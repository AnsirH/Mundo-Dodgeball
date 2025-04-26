using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;

public class NetworkPlayerController : MonoBehaviourPunCallbacks
{
    [SerializeField] private bool isOfflineMode = false;  // �������� �׽�Ʈ ���
    [SerializeField] private bool isDebugMode = false;  // ����� ��� �߰�
    private PlayerController playerController;
    private PlayerInputEventSystem inputSystem;

    public PlayerController PlayerCtrl => playerController;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        inputSystem = GetComponent<PlayerInputEventSystem>();

        // �������� ����� ��� PhotonView�� ��Ȱ��ȭ
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
            // ����� �α� �߰�
            Debug.Log($"Input Received: {context.action.name} from {photonView.Owner.NickName}");
        }

        if (context.performed)
        {
            if (isOfflineMode)
            {
                // �������� ���: ���� �Է� ó��
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
