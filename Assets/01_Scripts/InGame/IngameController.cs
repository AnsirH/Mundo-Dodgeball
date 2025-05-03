using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using MyGame.Utils;
using UnityEngine.SceneManagement;

public class IngameController : MonoBehaviourPun
{
    #region 싱글톤 패턴
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

        
        if (FindFirstObjectByType<ObjectPooler>() == null)
        {
            Debug.LogError("There is no ObjectPooler in scene");
            return;
        }    

        UIManager.instance.ChangeGame(false);

        StartCoroutine(StartGameProcess());

    }
    #endregion
    public int playerAmount = 2;
    private List<int> PunPlayerScores = new List<int>();
    IEnumerator StartGameProcess()
    {
        // 모든 플레이어가 접속했는지 확인
        yield return StartCoroutine(CheckPlayerConnected());

        if (PhotonNetwork.IsMasterClient)
        {
            // 게임이 준비 되었음을 모든 클라이언트에게 알림
            photonView.RPC(nameof(CreatePlayerCharacter_RPC), RpcTarget.All);
        }

        // 캐릭터 생성이 완료되었는지 확인 후 게임 시작
        yield return StartCoroutine(CheckPlayersCharacterSpawned());

        // 게임 시작
        StartGame();
    }

    /// <summary>
    /// 플레이어가 모두 접속하였는지 확인
    /// </summary>
    /// <returns></returns>
    IEnumerator CheckPlayerConnected()
    {
        WaitForSeconds delay = new(1.0f);

        while (PhotonNetwork.PlayerList.Length < playerSpawnPoints.Length)
        {
            yield return delay;
        }
    }

    [PunRPC]
    private void CreatePlayerCharacter_RPC()
    {
        // 플레이어 캐릭터 생성
        CreatePlayerCharacter();
    }

    private void CreatePlayerCharacter()
    {
        // 플레이어의 ActorNumber로 위치 설정
        int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
        int spawnIndex = actorNumber - 1;

        PhotonNetwork.Instantiate("Player", playerSpawnPoints[spawnIndex].position, playerSpawnPoints[spawnIndex].rotation);
    }

    /// <summary>
    /// 모든 플레이어의 캐릭터가 생성되었는지 확인
    /// </summary>
    /// <returns></returns>
    IEnumerator CheckPlayersCharacterSpawned()
    {
        WaitForSeconds delay = new(0.5f);

        // 모든 캐릭터가 생성될 때까지 대기
        while (true)
        {
            var foundControllers = FindObjectsByType<PlayerController>(FindObjectsSortMode.None);

            // 필요한 수 만큼 다 찾았으면 정렬 및 저장
            if (foundControllers.Length >= playerSpawnPoints.Length)
            {
                playerControllers = new List<PlayerController>(new PlayerController[playerSpawnPoints.Length]);

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
                            (controller as IMousePositionGetter).SetClickableGroundLayer($"Ground_{index+1}");
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
        ingameUIController.OnRoundPanel(ServerManager.Instance.roomManager.GetCurrentRound());
    }
    [PunRPC]
    void ReloadSceneRPC()
    {
        // Photon 연결 유지한 채 현재 씬 리로드
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    /// <summary>플레이어 캐릭터 배열</summary>
    public List<PlayerController> playerControllers = new List<PlayerController>(2);
    /// <summary>인게임 UI 컨트롤러</summary>
    public IngameUIController ingameUIController;

    public Transform[] playerSpawnPoints = new Transform[2];

    public Ground ground;
}