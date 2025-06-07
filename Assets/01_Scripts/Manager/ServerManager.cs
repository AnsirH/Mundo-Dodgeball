using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
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

    public async void ApplyRegionSetting(string regionCode)
    {
        PlayerPrefs.SetString("LocalKey", regionCode);
        PlayerPrefs.Save();
        runnerInstance = Instantiate(roomManager.runnerPrefab);

        runnerInstance.ProvideInput = true;
        runnerInstance.AddCallbacks(this);
        DontDestroyOnLoad(runnerInstance.gameObject);

        var sceneManager = runnerInstance.GetComponent<NetworkSceneManagerDefault>();
        if (sceneManager == null)
            sceneManager = runnerInstance.gameObject.AddComponent<NetworkSceneManagerDefault>();
        else
            Debug.Log("SceneManager already exists");

        var startArgs = new StartGameArgs
        {
            GameMode = GameMode.AutoHostOrClient,
            SessionName = "DefaultRoom",
            Scene = default,
            SceneManager = sceneManager
        };

        var result = await runnerInstance.StartGame(startArgs);

        if (result.Ok)
        {
            isStartGame = true;
            Debug.Log("[Fusion] 연결 성공");
        }
        else
        {
            Debug.LogError($"[Fusion] 연결 실패: {result.ShutdownReason}");
        }
    }



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

    public void OnConnectedToServer(NetworkRunner runner) => Debug.Log("[Fusion] 서버 연결 성공");
    public void OnDisconnectedFromServer(NetworkRunner runner) => Debug.Log("[Fusion] 서버 연결 끊김");
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player) => Debug.Log($"[Fusion] 플레이어 입장: {player}");
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) => Debug.Log($"[Fusion] 플레이어 퇴장: {player}");

    public void OnInput(NetworkRunner runner, NetworkInput input) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
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
        throw new NotImplementedException();
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
