using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class ServerManager : MonoBehaviourPunCallbacks
{
    #region 싱글톤
    private static ServerManager instance;

    // 외부에서 instance를 가져올 때는 이 프로퍼티를 사용
    public static ServerManager Instance
    {
        get
        {
            if (instance == null)
            {
                // 씬 내에서 ServerManager를 찾거나, 
                // 필요하다면 런타임에 새 오브젝트를 생성할 수도 있음
                instance = FindObjectOfType<ServerManager>();

                if (instance == null)
                {
                    // 만약 씬에 ServerManager가 없다면 새로 생성 (선택 사항)
                    GameObject obj = new GameObject(typeof(ServerManager).Name);
                    instance = obj.AddComponent<ServerManager>();
                }
            }
            return instance;
        }
    }

    private void Awake()
    {
        // 이미 인스턴스가 있으면, 자기 자신을 파괴해 중복을 막음
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        // 없으면 이 인스턴스를 사용
        instance = this;

        // 씬 전환 시 파괴되지 않도록 설정
        DontDestroyOnLoad(this.gameObject);
    }
    #endregion
    public RoomManager roomManager;
    [SerializeField] private string gameVersion = "1.0";

    
    private const float checkConstTime = 5f;
    private float checkInterval = 5f; // 상태 확인 주기 (초)
    private float nextCheckTime = 0f;

    private bool isStartGame = false;
    void Start()
    {
        string savedRegion = PlayerPrefs.GetString("LocalKey");
        if (savedRegion == null || savedRegion == "")
        {
            PopManager.instance.localSelectPop.Open();
            return;
        }
        else
        {
            ApplyRegionSetting(savedRegion);
        }
    }

    void Update()
    {
        if (Time.time >= nextCheckTime && isStartGame)
        {
            nextCheckTime = Time.time + checkInterval;
            CheckConnectionStatus();
        }
    }
    public void ApplyRegionSetting(string regionCode)
    {
        //  자동 동기화
        PhotonNetwork.AutomaticallySyncScene = true;
        // 지역 설정
        PhotonNetwork.PhotonServerSettings.AppSettings.UseNameServer = true;
        PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = regionCode;
        PlayerPrefs.SetString("LocalKey", "kr");
        PlayerPrefs.Save();
        // 게임 버전 셋팅
        PhotonNetwork.GameVersion = gameVersion;
        PhotonNetwork.ConnectUsingSettings();
        isStartGame = true;
        Debug.Log("Connecting to Photon...");
    }
    void CheckConnectionStatus()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            // 정상 연결됨!
            checkInterval = checkConstTime;
            UIManager.instance.SetLoadingUI(false);
        }
        else if(PhotonNetwork.NetworkClientState == ClientState.Disconnected)
        {
            checkInterval = 0.2f;
            UIManager.instance.SetLoadingUI(true);
            PhotonNetwork.Reconnect(); // 자동 재연결 시도
        }
    }
    // 포톤 마스터 서버 연결완료
    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        Debug.Log("Connected to Master Server!");
        PhotonNetwork.JoinLobby();
    }

    // 로비 입장 완료
    public override void OnJoinedLobby()
    {
        PhotonNetwork.NickName = SteamManager.GetSteamName();
        Debug.Log("Joined Lobby!");
        Debug.Log($"[방 생성 요청] 현재 상태: {PhotonNetwork.NetworkClientState}");
    }
}
