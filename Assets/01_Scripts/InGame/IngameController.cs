using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngameController : MonoBehaviour
{
    public static IngameController Instance { get; private set; }

    private void Awake()
    {
        // 이미 존재하는 인스턴스가 있고, 그것이 자신(this)이 아니라면
        // 중복 인스턴스를 제거
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            // 자신의 인스턴스를 기록
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

        // 플레이어 컨트롤러 모두 찾을 때까지 검색
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