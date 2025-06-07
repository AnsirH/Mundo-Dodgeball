using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using Fusion.Photon.Realtime;
using Fusion.Sockets;
using UnityEngine;
using UnityEngine.SceneManagement;

public partial class ServerManager : MonoBehaviour, INetworkRunnerCallbacks
{
    #region 싱글톤
    private static ServerManager instance;

    public static ServerManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<ServerManager>();
                if (instance == null)
                {
                    GameObject obj = new GameObject(typeof(ServerManager).Name);
                    instance = obj.AddComponent<ServerManager>();
                }
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }
    #endregion

    public RoomManager roomManager;
    [SerializeField] private string gameVersion = "1.0";

    private const float checkConstTime = 5f;
    private float checkInterval = 5f;
    private float nextCheckTime = 0f;

    private bool isStartGame = false;

    private NetworkRunner runnerInstance;
    public NetworkRunner Runner => runnerInstance;

    void Start()
    {
        string savedRegion = PlayerPrefs.GetString("LocalKey");
        if (string.IsNullOrEmpty(savedRegion))
        {
            PopManager.instance.localSelectPop.Open();
            return;
        }
        ApplyRegionSetting(savedRegion);
    }

    void Update()
    {
        if (Time.time >= nextCheckTime && isStartGame && runnerInstance != null)
        {
            nextCheckTime = Time.time + checkInterval;
            CheckConnectionStatus();
        }
    }

    #region 지역 연결코드
    public async void ApplyRegionSetting(string regionCode)
    {
        runnerInstance = Instantiate(roomManager.runnerPrefab);
        runnerInstance.ProvideInput = false;
        runnerInstance.AddCallbacks(this);
        DontDestroyOnLoad(runnerInstance.gameObject);

        var sceneManager = runnerInstance.GetComponent<NetworkSceneManagerDefault>();
        if (sceneManager == null)
            sceneManager = runnerInstance.gameObject.AddComponent<NetworkSceneManagerDefault>();

        // 🔧 지역 설정 포함된 AppSettings 준비
        var appSettings = PhotonAppSettings.Global.AppSettings.GetCopy();
        appSettings.FixedRegion = regionCode.ToLower();

        // 🔧 StartGame 먼저 호출
        var startArgs = new StartGameArgs
        {
            GameMode = GameMode.Single,
            SessionName = "", // 임의 이름
            SceneManager = sceneManager,
            Scene = default,
            CustomPhotonAppSettings = appSettings
        };

        var startResult = await runnerInstance.StartGame(startArgs);
        if (!startResult.Ok)
        {
            Debug.LogError($"[Fusion] StartGame 실패 ❌: {startResult.ShutdownReason}");
            return;
        }

        // ✅ 로비 연결
        var lobbyResult = await runnerInstance.JoinSessionLobby(SessionLobby.ClientServer);
        if (lobbyResult.Ok)
            Debug.Log($"[Fusion] 로비 입장 성공 ✅ (지역: {regionCode})");
        else
            Debug.LogError($"[Fusion] 로비 입장 실패 ❌: {lobbyResult.ShutdownReason}");
    }
    #endregion


    void CheckConnectionStatus()
    {
        if (runnerInstance != null && runnerInstance.IsRunning)
        {
            checkInterval = checkConstTime;
            UIManager.instance.SetLoadingUI(false);
        }
        else
        {
            checkInterval = 0.2f;
            UIManager.instance.SetLoadingUI(true);
            Debug.LogWarning("[Fusion] 연결 끊김. 재시도 중...");
            ApplyRegionSetting(PlayerPrefs.GetString("LocalKey"));
        }
    }

    public void OnConnectedToServer(NetworkRunner runner) => Debug.Log("ServerManager : [Fusion] 서버 연결 성공!!!!!!!!!!!!!!!!!!!!");
    public void OnDisconnectedFromServer(NetworkRunner runner) => Debug.Log("ServerManager : [Fusion] 서버 연결 끊김!!!!!!!!!!!!!!!!!!!!");
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player) 
    {
        Debug.Log($"[Fusion] 플레이어 입장: {player}");
        // 로컬 플레이어가 아니라면 Host에서만 스폰
        // (AutoHostOrClient 모드의 Host 혹은 Shared 모드)
        if (runner.GameMode != GameMode.Single)
        {
            // 스폰 위치를 정해둡니다 (예: Vector3.zero)
            Vector3 spawnPos = Vector3.zero;

            // Spawn overload 의 onBeforeSpawned 콜백을 이용해 NickName 세팅
            runner.Spawn(roomManager.playerPrefab, spawnPos, Quaternion.identity, player, (r, obj) =>
            {
                var netPlayer = obj.GetComponent<NetworkPlayer>();
                // PlayerPrefs에 저장해둔 닉네임을 할당
                int randomValue = UnityEngine.Random.Range(1, 101);
                netPlayer.NickName = PlayerPrefs.GetString("NickName", "Player_"+ randomValue);
            });
            // 스폰이 끝나면 UI 갱신
            roomManager.UpdatePlayerUI();
        }
    } 
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) => Debug.Log($"[Fusion] 플레이어 퇴장: {player}");

    public void OnInput(NetworkRunner runner, NetworkInput input) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        Debug.Log("ServerManager : update Session!");
        PopManager.instance.gameSelectPop.regularGamePop.SetRoomListSlot(sessionList);
    }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
        throw new NotImplementedException();
    }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
        throw new NotImplementedException();
    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
        throw new NotImplementedException();
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
        Debug.Log($"ServerManager : 타인 연결 시도");
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
        throw new NotImplementedException();
    }

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
    {
        throw new NotImplementedException();
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
        throw new NotImplementedException();
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
        throw new NotImplementedException();
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
        throw new NotImplementedException();
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
        throw new NotImplementedException();
    }
}
