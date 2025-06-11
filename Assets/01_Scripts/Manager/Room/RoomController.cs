// ✅ FusionRoomController.cs
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

public class RoomController : MonoBehaviour, INetworkRunnerCallbacks
{
    [SerializeField] private RoomModel model;
    public NetworkRunner runnerPrefab;
    private NetworkRunner runner;
    private const string lobbyName = "default_lobby";
    private Dictionary<string, int> GameModePlayerCount = new Dictionary<string, int>{
        { "GeneralGameMode", 2 }
    };
    private const int REQUIRED_PLAYERS = 2;

    private void Awake()
    {
        PopManager.instance.gameSelectPop.OnCreateRoomRequested += CreateOrJoinRoom;
        UIManager.instance.roomUI.OnLeaveRoomRequested += LeaveRoom;
    }

    private async void Start()
    {
        await InitRunner();
    }

    private async Task InitRunner()
    {
        // 기존 Runner가 있다면 정리
        await DestroyRunner();

        // 새 Runner 인스턴스 생성
        runner = Instantiate(runnerPrefab);
        runner.ProvideInput = false;

        await runner.StartGame(new StartGameArgs
        {
            GameMode = GameMode.Shared,
            SessionName = "LOBBY",
            Scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex),
            SceneManager = runner.GetComponent<NetworkSceneManagerDefault>()
        });
    }

    private async void CreateOrJoinRoom(string roomName, string password = "", string playerCount = "GeneralGameMode")
    {
        // 기존 Runner 정리 (Shutdown 및 Destroy 포함된 함수여야 함)
        await DestroyRunner();

        // 새 Runner 생성
        runner = Instantiate(runnerPrefab);
        runner.ProvideInput = false;
        runner.AddCallbacks(this);

        // 세션 프로퍼티 설정
        var properties = new Dictionary<string, SessionProperty>
    {
        { "pwd", password }
    };

        // 씬 매니저 확보
        var sceneManager = runner.GetComponent<NetworkSceneManagerDefault>()
                          ?? runner.gameObject.AddComponent<NetworkSceneManagerDefault>();

        // StartGame 실행
        var result = await runner.StartGame(new StartGameArgs
        {
            GameMode = GameMode.AutoHostOrClient,
            SessionName = roomName,
            PlayerCount = GameModePlayerCount[playerCount],
            SceneManager = sceneManager,
            SessionProperties = properties,
            IsVisible = true,
            IsOpen = true
        });

        if (!result.Ok)
        {
            Debug.LogError($"StartGame Failed: {result.ShutdownReason}");
            return;
        }

        Debug.Log("RoomManager : 방 연결 시도 완료");
    }



    private async void LeaveRoom()
    {
        if (runner != null)
        {
            await runner.Shutdown();
            model.ClearSessionInfo();
            await InitRunner();
        }
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log($"플레이어 {player.PlayerId} 입장");
        int currentCount = runner.ActivePlayers.Count();
        UIManager.instance.ChangeRoomUI();

        if (runner.IsServer)
        {
            TryStartGameIfReady(currentCount);
        }
    }

    private async void TryStartGameIfReady(int playerCount)
    {
        if (playerCount >= REQUIRED_PLAYERS)
        {
            Debug.Log("인원 충족! 인게임 씬으로 전환");
            //await runner.StartGame(new StartGameArgs
            //{
            //    GameMode = GameMode.AutoHostOrClient,
            //    SessionName = model.GetRoomName(),
            //    Scene = SceneRef.FromIndex(1),
            //    SceneManager = GetComponent<NetworkSceneManagerDefault>()
            //});
            if (runner.IsSceneAuthority)
            {
                await runner.LoadScene(SceneRef.FromIndex(1), LoadSceneMode.Additive);
            }
        }
    }

    private async Task DestroyRunner()
    {
        Debug.Log("DestoroyRunner에서 Shutdown하겠음. ㅅㄱ링");
        if (runner != null)
        {
            if (runner.IsRunning)
                await runner.Shutdown();
            Destroy(runner.gameObject);
        }
    }
    // ✅ 주요 콜백 요약
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { Debug.Log($"런너 종료됨: {shutdownReason}"); }
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) { Debug.Log($"플레이어 {player.PlayerId} 퇴장"); }
    public void OnSceneLoadDone(NetworkRunner runner) { Debug.Log("씬 로딩 완료"); }
    public void OnSceneLoadStart(NetworkRunner runner) { Debug.Log("씬 로딩 시작"); }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) 
    {
        Debug.Log("세션 리스트 갱신"); 
        if (model.CurrentSession == null)
        {
            foreach (var session in sessionList)
            {
                if (session.Name == model.GetRoomName()) // 이름 매칭
                {
                    model.SetSessionInfo(session);
                    break;
                }
            }
        }
    }

    // 사용하지 않음 (빈 정의)
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnInput(NetworkRunner runner, NetworkInput input) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, System.ArraySegment<byte> data) { }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
    }

    public void OnConnectedToServer(NetworkRunner runner)
    {
    }
}
