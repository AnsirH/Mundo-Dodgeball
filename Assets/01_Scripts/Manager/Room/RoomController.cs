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
using Fusion.Photon.Realtime;

public class RoomController : NetworkBehaviour, INetworkRunnerCallbacks
{
    [SerializeField] public RoomModel model;
    public NetworkRunner runnerPrefab;
    public NetworkRunner runner;
    public NetworkPlayer playerPrefab;
    public MatchManager matchManagerPrefab;
    [SerializeField] public List<NetworkPlayer> allPlayerInfos = new();
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
        UIManager.instance.roomUI.OnLeaveRoomRequested += async () =>
        {
            await Task.Yield(); // 씬 로딩 충돌 방지용 한 틱 대기
            LeaveRoom();
        };
    }

    private async void Start()
    {
        ServerManager.Instance.roomController = this;
        if (runner == null)
        {
            await InitRunner();
        }
    }
    private async Task InitRunner()
    {
        UIManager.instance.SetLoadingUI(true);
        await DestroyRunner();

        // ✅ NetworkRunner 생성
        runner = Instantiate(runnerPrefab);
        runner.ProvideInput = false;
        runner.AddCallbacks(this);

        // ✅ SceneManager 연결
        var sceneManager = runner.GetComponent<NetworkSceneManagerDefault>();
        if (sceneManager == null)
            sceneManager = runner.gameObject.AddComponent<NetworkSceneManagerDefault>();

        // ✅ AppSettings 복사 (자동 지역 설정: FixedRegion 미지정)
        var appSettings = PhotonAppSettings.Global.AppSettings.GetCopy();
        appSettings.FixedRegion = null; // ❗ null 또는 비우면 자동 선택
        appSettings.UseNameServer = true; // ✅ NameServer 통해 지역 자동 측정

        // ✅ StartGame 실행 (Shared 모드)
        var result = await runner.StartGame(new StartGameArgs
        {
            GameMode = GameMode.Single,
            SessionName = "LOBBY",
            SceneManager = sceneManager,
            Scene = SceneRef.FromIndex(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex),
            CustomPhotonAppSettings = appSettings
        });

        if (!result.Ok)
        {
            Debug.LogError($"❌ StartGame 실패: {result.ShutdownReason}");
            return;
        }

        Debug.Log($"✅ Runner 시작됨. 자동 연결 지역: {appSettings.FixedRegion}");

        // ✅ 로비 입장
        var lobbyResult = await runner.JoinSessionLobby(SessionLobby.ClientServer);
        if (lobbyResult.Ok)
        {
            Debug.Log("✅ Shared 로비 입장 성공");
        }
        else
        {
            Debug.LogError($"❌ Shared 로비 입장 실패: {lobbyResult.ShutdownReason}");
        }
        UIManager.instance.SetLoadingUI(false);
    }

    private async void CreateOrJoinRoom(string roomName, string password = "", string playerCount = "GeneralGameMode")
    {
        if(runner == null)
        {
            Debug.Log("방만들거나 들어가려고 했는데 기존에 러너가 없어서 안만들게~");
            return;
        }
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
        else
        {
            Debug.LogError($"입장 성공");
        }
        Debug.Log("RoomManager : 방 연결 시도 완료");
    }

    public async void LeaveRoom()
    {
        if (runner != null)
        {
            Debug.Log("[LeaveRoom] 러너 종료 시작");
            if(runner.IsRunning)
                await runner.Shutdown(); // 러너가 실행 중이면 종료
            Debug.Log("[LeaveRoom] 러너 종료 완료");

            model.ClearSessionInfo();
            UIManager.instance.SetLoadingUI(true);
            await InitRunner(); // 새로 시작
            UIManager.instance.SetLoadingUI(false);
            if (runner.IsSceneAuthority)
                await runner.LoadScene(SceneRef.FromIndex(0), LoadSceneMode.Single);
            UIManager.instance.ChangeLobbyUI();
            UIManager.instance.ChangeGame(true); // 게임 UI로 변경
            Debug.Log("[LeaveRoom] InitRunner 완료");
        }
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log($"111111 플레이어 {player.PlayerId} 입장");
        if (runner.GameMode == GameMode.Host || runner.GameMode == GameMode.Client)
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
        }
    }
    public async void TryStartGameIfReady()
    {
        if (runner == null)
            return;
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
            await Task.Yield();
            await runner.LoadScene(SceneRef.FromIndex(1), LoadSceneMode.Single); // 보통 Additive는 필요 없을 경우 Single이 안정적
        }
    }

    private async Task DestroyRunner()
    {
        if (runner != null)
        {
            Debug.Log("DestoroyRunner에서 Shutdown하겠음. ㅅㄱ링");
            if (runner.IsRunning)
                await runner.Shutdown();
            Destroy(runner.gameObject);
        }
    }

    public void UpdateLobbyUI()
    {
        Debug.Log($"GameModePlayerCount Test");
        if (runner == null)
        {
            Debug.Log("러너가 없습니다. ");
            return;
        }
            
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
        if(runner == null)
        {
            Debug.Log("runner가 null이라서 플레이어 데이터 못 줌.");
        }
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
        Debug.Log(runner == null);
        foreach (var obj in runner.GetAllNetworkObjects())
        {
            var info = obj.GetComponent<NetworkPlayer>();
            if (info != null && info.HasInputAuthority)
                return info;
        }
        return null;
    }
  
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        Debug.Log($"런너 종료됨: {shutdownReason}");
        if(runner.GameMode == GameMode.Host || runner.GameMode == GameMode.Client)
            LeaveRoom();
    }
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log($"플레이어 {player.PlayerId} 퇴장");

        // ✅ 해당 PlayerRef를 가진 오브젝트 제거
        foreach (var obj in runner.GetAllNetworkObjects())
        {
            var info = obj.GetComponent<NetworkPlayer>();
            if (info != null && info.Object.InputAuthority == player)
            {
                runner.Despawn(obj); // ✅ Despawn
                allPlayerInfos.Remove(info); // 리스트에서도 제거
                Debug.Log($"퇴장한 플레이어 오브젝트 Despawn 완료: {player.PlayerId}");
                break;
            }
        }

        UpdateLobbyUI(); // UI 갱신
    }
    public void OnSceneLoadDone(NetworkRunner runner) 
    {
        if(runner.GameMode == GameMode.Host || runner.GameMode == GameMode.Client)
            SceneLoadDone(runner);
    }
    public async void SceneLoadDone(NetworkRunner runner)
    {
        if (!runner.IsServer) return;

        MatchManager result = null;
        var tcs = new TaskCompletionSource<MatchManager>();

        // 1. 스폰 콜백 등록
        void OnMatchSpawned(MatchManager mgr)
        {
            result = mgr;
            MatchManager.OnSpawned -= OnMatchSpawned;
            tcs.SetResult(mgr);
        }
        MatchManager.OnSpawned += OnMatchSpawned;

        // 2. 씬에서 찾기 → 없으면 스폰
        MatchManager existing = FindObjectOfType<MatchManager>();
        if (existing == null)
        {
            runner.Spawn(matchManagerPrefab, Vector3.zero, Quaternion.identity);
            result = await tcs.Task; // ✅ 여기서 Spawned까지 기다림
        }
        else
        {
            MatchManager.OnSpawned -= OnMatchSpawned;
            result = existing;
        }

        // 3. 안전하게 초기화
        result.Init(runner.ActivePlayers.ToList());
    }
    public void OnSceneLoadStart(NetworkRunner runner) 
    {
        Debug.Log("씬 로딩 시작"); 
    }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        Debug.Log("세션 리스트 갱신");
        PopManager.instance?.gameSelectPop?.regularGamePop?.SetRoomListSlot(sessionList);
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnInput(NetworkRunner runner, NetworkInput input) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
        Debug.LogWarning("호스트 마이그레이션 시작!");

        if (runner != null)
        {
            runner.Shutdown();
            Destroy(runner.gameObject);
        }

        runner = Instantiate(runnerPrefab);
        runner.ProvideInput = true;
        runner.AddCallbacks(this);

        runner.StartGame(new StartGameArgs()
        {
            HostMigrationToken = hostMigrationToken,
            SceneManager = runner.GetComponent<NetworkSceneManagerDefault>() ?? runner.gameObject.AddComponent<NetworkSceneManagerDefault>()
        });
    }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, System.ArraySegment<byte> data) { }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
    public void OnConnectedToServer(NetworkRunner runner) { }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
        Debug.Log($"RoomController : s서버 연결 끊김: {reason}");
    }
}
