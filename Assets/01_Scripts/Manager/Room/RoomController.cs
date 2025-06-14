// ✅ FusionRoomController.cs - MonoBehaviour 버전 (RPC 제거)
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using ExitGames.Client.Photon.StructWrapping;
using Unity.VisualScripting.Antlr3.Runtime.Misc;

public class RoomController : NetworkBehaviour, INetworkRunnerCallbacks
{
    [SerializeField] private RoomModel model;
    public NetworkRunner runnerPrefab;
    public NetworkRunner runner;
    public NetworkPlayer playerPrefab;
    public MatchManager matchManagerPrefab;
    private List<NetworkPlayer> allPlayerInfos = new();
    private const string lobbyName = "default_lobby";
    private Dictionary<string, int> GameModePlayerCount = new Dictionary<string, int>{
        { "GeneralGameMode", 2 }
    };
    private const int REQUIRED_PLAYERS = 2;


    // MatchManager Data Save
    public List<PlayerScoreEntry> PlayerScores = new List<PlayerScoreEntry>();
    public int Round = 0;
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
        await DestroyRunner();

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
        await DestroyRunner();

        runner = Instantiate(runnerPrefab);
        runner.ProvideInput = false;
        runner.AddCallbacks(this);
        var properties = new Dictionary<string, SessionProperty>
        {
            { "pwd", password }
        };

        var sceneManager = runner.GetComponent<NetworkSceneManagerDefault>()
                          ?? runner.gameObject.AddComponent<NetworkSceneManagerDefault>();

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
            var info = runner.Spawn(playerPrefab, Vector3.zero, Quaternion.identity, player);
            info.Nickname = PlayerPrefs.GetString("NickName", $"플레이어_{UnityEngine.Random.Range(1000, 9999)}");
            allPlayerInfos.Add(info);
        }
        UpdateLobbyUI();
    }

    public async void TryStartGameIfReady()
    {
        var players = runner.ActivePlayers.ToList();

        // 인원수 확인
        if (players.Count < REQUIRED_PLAYERS)
            return;

        // 모든 플레이어의 IsReady 상태 확인
        bool allReady = true;
        foreach (var playerRef in players)
        {
            var info = GetPlayerInfo(playerRef);
            if (info == null || !info.IsReady)
            {
                allReady = false;
                break;
            }
        }

        // 조건 만족 시 게임 시작
        if (allReady && runner.IsSceneAuthority)
        {
            Debug.Log("인게임 씬으로 전환");
            await runner.LoadScene(SceneRef.FromIndex(1), LoadSceneMode.Single); // 보통 Additive는 필요 없을 경우 Single이 안정적
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

    public void UpdateLobbyUI()
    {
        var players = runner.ActivePlayers.OrderBy(p => p.PlayerId).ToList();
        Debug.Log($"GameModePlayerCount : {players.Count}");

        var host = GetPlayerInfo(players.ElementAtOrDefault(0));
        var guest = GetPlayerInfo(players.ElementAtOrDefault(1));

        Debug.Log($"host : {(host != null)} name : {(host != null ? host.Nickname : "없음")}");
        Debug.Log($"guest : {(guest != null)} name : {(guest != null ? guest.Nickname : "없음")}");

        UIManager.instance.roomUI.leftPlayerText.text = host != null
            ? $"{host.Nickname}"
            : "대기 중...";

        UIManager.instance.roomUI.rightPlayerText.text = guest != null
            ? $"{guest.Nickname}"
            : "대기 중...";

        if (host != null)
            UIManager.instance.roomUI.SetImReady(true, host.IsReady);
        if (guest != null)
            UIManager.instance.roomUI.SetImReady(false, guest.IsReady);
    }
    private NetworkPlayer GetPlayerInfo(PlayerRef player)
    {
        foreach (var obj in runner.GetAllNetworkObjects())
        {
            var info = obj.GetComponent<NetworkPlayer>();
            if (info != null && info.Object.InputAuthority == player)
            {
                return info;
            }
        }
        return null;
    }
    public NetworkPlayer FindMyPlayerInfo()
    {
        foreach (var obj in runner.GetAllNetworkObjects())
        {
            var info = obj.GetComponent<NetworkPlayer>();
            if (info != null && info.HasInputAuthority)
                return info;
        }
        return null;
    }
  
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { Debug.Log($"런너 종료됨: {shutdownReason}"); }
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) { Debug.Log($"플레이어 {player.PlayerId} 퇴장"); }
    public void OnSceneLoadDone(NetworkRunner runner) 
    {
        SceneLoadDone(runner);
    }
    public async void SceneLoadDone(NetworkRunner runner)
    {
        if (!runner.IsServer) return;

        MatchManager existing = null;
        bool isCreated = false;
        // 최대 1000ms 동안 기다림 (10번 x 100ms)
        if (!isCreated)
        {
            isCreated = true;
            Debug.Log("[MatchManager] 못찾음, 스폰 시작");
            existing = runner.Spawn(matchManagerPrefab, Vector3.zero, Quaternion.identity);
        }
        for (int i = 0; i < 100; i++)
        {
            if (existing != null)
            {
                Debug.Log("[MatchManager] 씬에서 찾음");
                break;
            }
            await Task.Delay(100); // 0.1초 대기
        }
        existing.Init(runner.ActivePlayers.ToList());
    }
    public void OnSceneLoadStart(NetworkRunner runner) { Debug.Log("씬 로딩 시작"); }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        Debug.Log("세션 리스트 갱신");
        if (model.CurrentSession == null)
        {
            foreach (var session in sessionList)
            {
                if (session.Name == model.GetRoomName())
                {
                    model.SetSessionInfo(session);
                    break;
                }
            }
        }
    }

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
    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }
    public void OnConnectedToServer(NetworkRunner runner) { }
}
