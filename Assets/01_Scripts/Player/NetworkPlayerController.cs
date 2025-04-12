using Photon.Pun;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class NetworkPlayerController : MonoBehaviourPunCallbacks
{
    [SerializeField] private bool isOfflineMode = false;  // �������� �׽�Ʈ ���
    private PlayerController playerController;
    private PlayerInputEventSystem inputSystem;

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

    private void OnPlayerInput(InputAction.CallbackContext context)
    {
        if (isOfflineMode)
        {
            // �������� ���: ���� �Է� ó��
            playerController.HandleInput(context);
        }
        else if (photonView.IsMine)
        {
            // �¶��� ���: RPC�� �Է� ����
            photonView.RPC("RPC_HandleInput", RpcTarget.All,
                context.action.name);
        }
    }

    [PunRPC]
    private void RPC_HandleInput(string actionName)
    {
        // �Է� ó��
        switch (actionName)
        {
            case "Attack":
                playerController.HandleAttackInput();
                break;
            case "Click":
                playerController.HandleClickInput();
                break;
            case "Move":
                playerController.HandleMoveInput();
                break;
        }
    }
}
