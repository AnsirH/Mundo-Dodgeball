using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngameController : MonoBehaviour
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
        PhotonNetwork.Instantiate("Player", Vector3.zero, Quaternion.identity);

        StartCoroutine(WaitForGameReady());
    }

    public void StartGame()
    {
        foreach (PlayerController player in playerControllers)
        {
            player.enabled = true;
        }
        ingameUIController.Init();
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

    public PlayerController[] playerControllers;
    public IngameUIController ingameUIController;
}