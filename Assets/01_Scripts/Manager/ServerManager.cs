using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using Fusion.Photon.Realtime;
using Fusion.Sockets;
using UnityEngine;
using UnityEngine.SceneManagement;

public partial class ServerManager : MonoBehaviour
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
    public RoomController roomController;
    public MatchManager matchManager;
    [SerializeField] private string gameVersion = "1.0";

    private const float checkConstTime = 5f;
    private float checkInterval = 5f;
    private float nextCheckTime = 0f;

    private bool isStartGame = false;

    //private NetworkRunner runnerInstance;
    //public NetworkRunner Runner => runnerInstance;

    void Start()
    {
        string savedRegion = PlayerPrefs.GetString("LocalKey");
        if (string.IsNullOrEmpty(savedRegion))
        {
            //PopManager.instance.localSelectPop.Open();
            return;
        }
        //ApplyRegionSetting(savedRegion);
    }

    //void Update()
    //{
    //    if (Time.time >= nextCheckTime && isStartGame && runnerInstance != null)
    //    {
    //        nextCheckTime = Time.time + checkInterval;
    //        CheckConnectionStatus();
    //    }
    //}

    //#region 지역 연결코드
    //public async void ApplyRegionSetting(string regionCode)
    //{
    //    Debug.Log("Setting the Region in ServerManager");
    //    runnerInstance = Instantiate(roomManager.runnerPrefab);
    //    runnerInstance.ProvideInput = false;
    //    DontDestroyOnLoad(runnerInstance.gameObject);

    //    var sceneManager = runnerInstance.GetComponent<NetworkSceneManagerDefault>();
    //    if (sceneManager == null)
    //        sceneManager = runnerInstance.gameObject.AddComponent<NetworkSceneManagerDefault>();

    //    // 🔧 지역 설정 포함된 AppSettings 준비
    //    var appSettings = PhotonAppSettings.Global.AppSettings.GetCopy();
    //    appSettings.FixedRegion = regionCode.ToLower();

    //    // 🔧 StartGame 먼저 호출
    //    var startArgs = new StartGameArgs
    //    {
    //        GameMode = GameMode.Single,
    //        SessionName = "", // 임의 이름
    //        SceneManager = sceneManager,
    //        Scene = default,
    //        CustomPhotonAppSettings = appSettings
    //    };

    //    var startResult = await runnerInstance.StartGame(startArgs);
    //    if (!startResult.Ok)
    //    {
    //        Debug.LogError($"[Fusion] StartGame 실패 ❌: {startResult.ShutdownReason}");
    //        return;
    //    }

    //    // ✅ 로비 연결
    //    var lobbyResult = await runnerInstance.JoinSessionLobby(SessionLobby.ClientServer);
    //    if (lobbyResult.Ok)
    //        Debug.Log($"[Fusion] 로비 입장 성공 ✅ (지역: {regionCode})");
    //    else
    //        Debug.LogError($"[Fusion] 로비 입장 실패 ❌: {lobbyResult.ShutdownReason}");
    //}
    //#endregion


    //void CheckConnectionStatus()
    //{
    //    if (runnerInstance != null && runnerInstance.IsRunning)
    //    {
    //        checkInterval = checkConstTime;
    //        UIManager.instance.SetLoadingUI(false);
    //    }
    //    else
    //    {
    //        checkInterval = 0.2f;
    //        UIManager.instance.SetLoadingUI(true);
    //        Debug.LogWarning("[Fusion] 연결 끊김. 재시도 중...");
    //        ApplyRegionSetting(PlayerPrefs.GetString("LocalKey"));
    //    }
    //}
}
