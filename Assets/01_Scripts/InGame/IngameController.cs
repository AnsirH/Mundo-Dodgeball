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


        StartCoroutine(WaitForGameReady());
    }

    /// <summary>
    /// 게임 시작
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
    /// 인게임 환경 준비. 플레이어 생성, 인게임 UI 초기화, 
    /// </summary>
    public void ReadyGame()
    {
        photonView.RPC("CreatePlayerCharacter_RPC", RpcTarget.All);
        

    }

    /// <summary>
    /// 플레이어 생성
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
                // 플레이어 생성
                // 현재 플레이어 수에 맞는 위치에 생성한다.
                Instantiate(playerCharacterPrefab, playerSpawnPoints[currentPlayerCharacterCount].position, playerSpawnPoints[currentPlayerCharacterCount].rotation);

                // 현재 플레이어 수 +1
                currentPlayerCharacterCount++;
            }

            catch (Exception ex)
            {
                Debug.Log($"에러 발생: {ex.Message}");
                Debug.Log($"에러 타입: {ex.GetType()}");
                Debug.Log($"스택 트레이스: {ex.StackTrace}");
            }
        }
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

    /// <summary>플레이어 캐릭터 배열</summary>
    public PlayerController[] playerControllers;
    /// <summary>인게임 UI 컨트롤러</summary>
    public IngameUIController ingameUIController;

    public GameObject playerCharacterPrefab;

    public Transform[] playerSpawnPoints = new Transform[2];

    public const int maxPlayerCharacterCount = 2;
    private int currentPlayerCharacterCount = 0;
}