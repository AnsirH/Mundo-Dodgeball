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

public class RoomController : MonoBehaviour, INetworkRunnerCallbacks
{
    [SerializeField] private RoomModel model;
    public NetworkRunner runnerPrefab;
    private NetworkRunner runner;
    public NetworkPlayer playerPrefab;
    private List<NetworkPlayer> allPlayerInfos = new();
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
    public void OnSceneLoadDone(NetworkRunner runner) { Debug.Log("씬 로딩 완료"); }
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

// ✅ FusionRoomController.cs
//using UnityEngine;
//using Fusion;
//using Fusion.Sockets;
//using UnityEngine.SceneManagement;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using System;

//public class RoomController : NetworkBehaviour, INetworkRunnerCallbacks
//{
//    [SerializeField] private RoomModel model;
//    public NetworkRunner runnerPrefab;
//    private NetworkRunner runner;
//    private Dictionary<PlayerRef, string> nicknames = new();
//    private Dictionary<PlayerRef, bool> readyStates = new();
//    private const string lobbyName = "default_lobby";
//    private Dictionary<string, int> GameModePlayerCount = new Dictionary<string, int>{
//        { "GeneralGameMode", 2 }
//    };
//    private const int REQUIRED_PLAYERS = 2;

//    private void Awake()
//    {
//        PopManager.instance.gameSelectPop.OnCreateRoomRequested += CreateOrJoinRoom;
//        UIManager.instance.roomUI.OnLeaveRoomRequested += LeaveRoom;
//    }

//    private async void Start()
//    {
//        await InitRunner();
//    }

//    private async Task InitRunner()
//    {
//        // 기존 Runner가 있다면 정리
//        await DestroyRunner();

//        // 새 Runner 인스턴스 생성
//        runner = Instantiate(runnerPrefab);
//        runner.ProvideInput = false;

//        await runner.StartGame(new StartGameArgs
//        {
//            GameMode = GameMode.Shared,
//            SessionName = "LOBBY",
//            Scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex),
//            SceneManager = runner.GetComponent<NetworkSceneManagerDefault>()
//        });
//    }

//    private async void CreateOrJoinRoom(string roomName, string password = "", string playerCount = "GeneralGameMode")
//    {
//        // 기존 Runner 정리 (Shutdown 및 Destroy 포함된 함수여야 함)
//        await DestroyRunner();

//        // 새 Runner 생성
//        runner = Instantiate(runnerPrefab);
//        runner.ProvideInput = false;
//        runner.AddCallbacks(this);

//        // 세션 프로퍼티 설정
//        var properties = new Dictionary<string, SessionProperty>
//    {
//        { "pwd", password }
//    };

//        // 씬 매니저 확보
//        var sceneManager = runner.GetComponent<NetworkSceneManagerDefault>()
//                          ?? runner.gameObject.AddComponent<NetworkSceneManagerDefault>();

//        // StartGame 실행
//        var result = await runner.StartGame(new StartGameArgs
//        {
//            GameMode = GameMode.AutoHostOrClient,
//            SessionName = roomName,
//            PlayerCount = GameModePlayerCount[playerCount],
//            SceneManager = sceneManager,
//            SessionProperties = properties,
//            IsVisible = true,
//            IsOpen = true
//        });

//        if (!result.Ok)
//        {
//            Debug.LogError($"StartGame Failed: {result.ShutdownReason}");
//            return;
//        }

//        Debug.Log("RoomManager : 방 연결 시도 완료");
//    }



//    private async void LeaveRoom()
//    {
//        if (runner != null)
//        {
//            await runner.Shutdown();
//            model.ClearSessionInfo();
//            await InitRunner();
//        }
//    }

//    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
//    {
//        Debug.Log($"플레이어 {player.PlayerId} 입장");
//        int currentCount = runner.ActivePlayers.Count();
//        UIManager.instance.ChangeRoomUI();
//        foreach (var kvp in nicknames)
//        {
//            RPC_SyncNickname(player, kvp.Key, kvp.Value);
//        }
//        if (player == runner.LocalPlayer)
//        {
//            // 로컬 닉네임 보내기
//            string myNick = PlayerPrefs.GetString("NickName", $"플레이어_{UnityEngine.Random.Range(1000, 9999)}");
//            RPC_SendNickname(player, myNick);
//        }
//        if (runner.IsServer)
//        {
//            TryStartGameIfReady(currentCount);
//        }
//    }

//    private async void TryStartGameIfReady(int playerCount)
//    {
//        if (playerCount >= REQUIRED_PLAYERS)
//        {
//            Debug.Log("인원 충족! 인게임 씬으로 전환");
//            //await runner.StartGame(new StartGameArgs
//            //{
//            //    GameMode = GameMode.AutoHostOrClient,
//            //    SessionName = model.GetRoomName(),
//            //    Scene = SceneRef.FromIndex(1),
//            //    SceneManager = GetComponent<NetworkSceneManagerDefault>()
//            //});
//            if (runner.IsSceneAuthority)
//            {
//               // await runner.LoadScene(SceneRef.FromIndex(1), LoadSceneMode.Additive);
//            }
//        }
//    }

//    private async Task DestroyRunner()
//    {
//        Debug.Log("DestoroyRunner에서 Shutdown하겠음. ㅅㄱ링");
//        if (runner != null)
//        {
//            if (runner.IsRunning)
//                await runner.Shutdown();
//            Destroy(runner.gameObject);
//        }
//    }
//    // ✅ 닉네임 RPC
//    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
//    public void RPC_SendNickname(PlayerRef from, string nickname, RpcInfo info = default)
//    {
//        nicknames[from] = nickname;
//        readyStates[from] = false;
//        UpdateLobbyUI();
//    }
//    public void UpdateLobbyUI()
//    {
//        var players = runner.ActivePlayers;

//        foreach (var p in players)
//        {
//            Debug.Log($"PlayerRef: {p}");
//        }
//        // 왼쪽: Host, 오른쪽: other
//        var arr = new List<PlayerRef>(players);
//        PlayerRef host = arr.Count > 0 ? arr[0] : PlayerRef.None;
//        PlayerRef other = arr.Count > 1 ? arr[1] : PlayerRef.None;

//        UIManager.instance.ChangeRoomUI();
//        Debug.Log($"host Have test : {host == PlayerRef.None} : {nicknames.TryGetValue(host, out var aa)}");
//        if (host != PlayerRef.None && nicknames.TryGetValue(host, out var hn))
//            UIManager.instance.roomUI.leftPlayerText.text = hn;
//        else
//            UIManager.instance.roomUI.leftPlayerText.text = "대기 중...";
//        Debug.Log($"other Have test : {other == PlayerRef.None} : {nicknames.TryGetValue(other, out var aa2)}");

//        if (other != PlayerRef.None && nicknames.TryGetValue(other, out var on))
//            UIManager.instance.roomUI.rightPlayerText.text = on;
//        else
//            UIManager.instance.roomUI.rightPlayerText.text = "대기 중...";
//    }
//    // ✅ 주요 콜백 요약
//    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { Debug.Log($"런너 종료됨: {shutdownReason}"); }
//    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) { Debug.Log($"플레이어 {player.PlayerId} 퇴장"); }
//    public void OnSceneLoadDone(NetworkRunner runner) { Debug.Log("씬 로딩 완료"); }
//    public void OnSceneLoadStart(NetworkRunner runner) { Debug.Log("씬 로딩 시작"); }
//    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) 
//    {
//        Debug.Log("세션 리스트 갱신"); 
//        if (model.CurrentSession == null)
//        {
//            foreach (var session in sessionList)
//            {
//                if (session.Name == model.GetRoomName()) // 이름 매칭
//                {
//                    model.SetSessionInfo(session);
//                    break;
//                }
//            }
//        }
//    }

//    // 사용하지 않음 (빈 정의)
//    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
//    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
//    public void OnInput(NetworkRunner runner, NetworkInput input) { }
//    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
//    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
//    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
//    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
//    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, System.ArraySegment<byte> data) { }
//    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
//    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
//    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
//    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }

//    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
//    {
//    }

//    public void OnConnectedToServer(NetworkRunner runner)
//    {
//    }
//}
