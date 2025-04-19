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

    IEnumerator WaitForGameReady()
    {
        WaitForSeconds delay = new(1.0f);

        while (PhotonNetwork.PlayerList.Length < playerSpawnPoints.Length)
        {
            yield return delay;
        }

        if (PhotonNetwork.IsMasterClient)
        {
            // 게임이 준비 되었음을 모든 클라이언트에게 알림
            photonView.RPC("ReadyGame_RPC", RpcTarget.All);
        }
    }

    [PunRPC]
    public void ReadyGame_RPC()
    {
        CreatePlayerCharacter();

        // 캐릭터 생성이 완료되었는지 확인 후 게임 시작
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

        // 모든 캐릭터가 생성될 때까지 대기
        while (true)
        {
            var foundControllers = FindObjectsByType<NetworkPlayerController>(FindObjectsSortMode.None);

            // 필요한 수 만큼 다 찾았으면 정렬 및 저장
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
                            Debug.LogWarning($"ActorNumber {actorNum}가 스폰 인덱스 범위를 벗어남.");
                        }
                    }
                    else
                    {
                        Debug.LogWarning("PhotonView가 없는 컨트롤러 발견.");
                    }
                }

                // 전부 정상적으로 할당됐는지 확인
                if (playerControllers.All(c => c != null))
                    break;
            }

            yield return delay;
        }

        StartGame();
    }

    /// <summary>
    /// 게임 시작
    /// </summary>
    public void StartGame()
    {
        foreach (NetworkPlayerController player in playerControllers)
        {
            player.enabled = true;
        }
        ingameUIController.Init();
    }

    /// <summary>플레이어 캐릭터 배열</summary>
    public List<NetworkPlayerController> playerControllers;
    /// <summary>인게임 UI 컨트롤러</summary>
    public IngameUIController ingameUIController;

    public GameObject playerCharacterPrefab;

    public Transform[] playerSpawnPoints = new Transform[2];
}