using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class IngameController : MonoBehaviourPun
{
    public static IngameController Instance { get; private set; }

    private void Awake()
    {
        // �̹� �����ϴ� �ν��Ͻ��� �ְ�, �װ��� �ڽ�(this)�� �ƴ϶��
        // �ߺ� �ν��Ͻ��� ����
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            // �ڽ��� �ν��Ͻ��� ���
            Instance = this;
        }


        UIManager.instance.ChangeGame(false);


        StartCoroutine(WaitForGameReady());
    }

    IEnumerator WaitForGameReady()
    {
        WaitForSeconds delay = new(1.0f);

        while (PhotonNetwork.PlayerList.Length < playerSpawnPoints.Length)
        {
            yield return delay;
        }

        if (PhotonNetwork.IsMasterClient)
        {
            // ������ �غ� �Ǿ����� ��� Ŭ���̾�Ʈ���� �˸�
            photonView.RPC("ReadyGame_RPC", RpcTarget.All);
        }
    }

    [PunRPC]
    public void ReadyGame_RPC()
    {
        CreatePlayerCharacter();

        // ĳ���� ������ �Ϸ�Ǿ����� Ȯ�� �� ���� ����
        StartCoroutine(WaitToStartGame());
    }

    private void CreatePlayerCharacter()
    {
        int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
        int spawnIndex = actorNumber - 1;

        PhotonNetwork.Instantiate("Player", playerSpawnPoints[spawnIndex].position, playerSpawnPoints[spawnIndex].rotation);
    }

    IEnumerator WaitToStartGame()
    {
        WaitForSeconds delay = new(0.5f);

        // ��� ĳ���Ͱ� ������ ������ ���
        while (true)
        {
            var foundControllers = FindObjectsByType<NetworkPlayerController>(FindObjectsSortMode.None);

            // �ʿ��� �� ��ŭ �� ã������ ���� �� ����
            if (foundControllers.Length >= playerSpawnPoints.Length)
            {
                playerControllers = new List<NetworkPlayerController>(new NetworkPlayerController[playerSpawnPoints.Length]);

                foreach (var controller in foundControllers)
                {
                    PhotonView view = controller.GetComponent<PhotonView>();
                    if (view != null)
                    {
                        int actorNum = view.Owner.ActorNumber;
                        int index = actorNum - 1;

                        if (index >= 0 && index < playerControllers.Count)
                        {
                            playerControllers[index] = controller;
                        }
                        else
                        {
                            Debug.LogWarning($"ActorNumber {actorNum}�� ���� �ε��� ������ ���.");
                        }
                    }
                    else
                    {
                        Debug.LogWarning("PhotonView�� ���� ��Ʈ�ѷ� �߰�.");
                    }
                }

                // ���� ���������� �Ҵ�ƴ��� Ȯ��
                if (playerControllers.All(c => c != null))
                    break;
            }

            yield return delay;
        }

        StartGame();
    }

    /// <summary>
    /// ���� ����
    /// </summary>
    public void StartGame()
    {
        foreach (NetworkPlayerController player in playerControllers)
        {
            player.enabled = true;
        }
        ingameUIController.Init();
    }

    /// <summary>�÷��̾� ĳ���� �迭</summary>
    public List<NetworkPlayerController> playerControllers;
    /// <summary>�ΰ��� UI ��Ʈ�ѷ�</summary>
    public IngameUIController ingameUIController;

    public GameObject playerCharacterPrefab;

    public Transform[] playerSpawnPoints = new Transform[2];
}