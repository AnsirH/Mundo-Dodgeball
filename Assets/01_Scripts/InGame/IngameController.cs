using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
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

    /// <summary>
    /// ���� ����
    /// </summary>
    public void StartGame()
    {
        foreach (PlayerController player in playerControllers)
        {
            player.enabled = true;
        }
        ingameUIController.Init();
    }

    /// <summary>
    /// �ΰ��� ȯ�� �غ�. �÷��̾� ����, �ΰ��� UI �ʱ�ȭ, 
    /// </summary>
    public void ReadyGame()
    {
        photonView.RPC("CreatePlayerCharacter_RPC", RpcTarget.All);
        

    }

    /// <summary>
    /// �÷��̾� ����
    /// </summary>
    [PunRPC]
    private void CreatePlayerCharacter_RPC()
    {
        if (!photonView.IsMine)
            return;

        if (currentPlayerCharacterCount < maxPlayerCharacterCount)
        {
            try
            {
                // �÷��̾� ����
                // ���� �÷��̾� ���� �´� ��ġ�� �����Ѵ�.
                Instantiate(playerCharacterPrefab, playerSpawnPoints[currentPlayerCharacterCount].position, playerSpawnPoints[currentPlayerCharacterCount].rotation);

                // ���� �÷��̾� �� +1
                currentPlayerCharacterCount++;
            }

            catch (Exception ex)
            {
                Debug.Log($"���� �߻�: {ex.Message}");
                Debug.Log($"���� Ÿ��: {ex.GetType()}");
                Debug.Log($"���� Ʈ���̽�: {ex.StackTrace}");
            }
        }
    }

    IEnumerator WaitForGameReady()
    {
        WaitForSeconds secondDelay = new(1.0f);
        playerControllers = null;

        // �÷��̾� ��Ʈ�ѷ� ��� ã�� ������ �˻�
        while(playerControllers == null || playerControllers.Length < 2)
        {
            playerControllers = FindObjectsOfType<PlayerController>();
            yield return secondDelay;
        }

        StartGame();
    }

    /// <summary>�÷��̾� ĳ���� �迭</summary>
    public PlayerController[] playerControllers;
    /// <summary>�ΰ��� UI ��Ʈ�ѷ�</summary>
    public IngameUIController ingameUIController;

    public GameObject playerCharacterPrefab;

    public Transform[] playerSpawnPoints = new Transform[2];

    public const int maxPlayerCharacterCount = 2;
    private int currentPlayerCharacterCount = 0;
}