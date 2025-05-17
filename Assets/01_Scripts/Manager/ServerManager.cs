using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class ServerManager : MonoBehaviourPunCallbacks
{
    #region �̱���
    private static ServerManager instance;

    // �ܺο��� instance�� ������ ���� �� ������Ƽ�� ���
    public static ServerManager Instance
    {
        get
        {
            if (instance == null)
            {
                // �� ������ ServerManager�� ã�ų�, 
                // �ʿ��ϴٸ� ��Ÿ�ӿ� �� ������Ʈ�� ������ ���� ����
                instance = FindObjectOfType<ServerManager>();

                if (instance == null)
                {
                    // ���� ���� ServerManager�� ���ٸ� ���� ���� (���� ����)
                    GameObject obj = new GameObject(typeof(ServerManager).Name);
                    instance = obj.AddComponent<ServerManager>();
                }
            }
            return instance;
        }
    }

    private void Awake()
    {
        // �̹� �ν��Ͻ��� ������, �ڱ� �ڽ��� �ı��� �ߺ��� ����
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        // ������ �� �ν��Ͻ��� ���
        instance = this;

        // �� ��ȯ �� �ı����� �ʵ��� ����
        DontDestroyOnLoad(this.gameObject);
    }
    #endregion
    public RoomManager roomManager;
    [SerializeField] private string gameVersion = "1.0";

    
    private const float checkConstTime = 5f;
    private float checkInterval = 5f; // ���� Ȯ�� �ֱ� (��)
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
        //  �ڵ� ����ȭ
        PhotonNetwork.AutomaticallySyncScene = true;
        // ���� ����
        PhotonNetwork.PhotonServerSettings.AppSettings.UseNameServer = true;
        PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = regionCode;
        PlayerPrefs.SetString("LocalKey", "kr");
        PlayerPrefs.Save();
        // ���� ���� ����
        PhotonNetwork.GameVersion = gameVersion;
        PhotonNetwork.ConnectUsingSettings();
        isStartGame = true;
        Debug.Log("Connecting to Photon...");
    }
    void CheckConnectionStatus()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            // ���� �����!
            checkInterval = checkConstTime;
            UIManager.instance.SetLoadingUI(false);
        }
        else if(PhotonNetwork.NetworkClientState == ClientState.Disconnected)
        {
            checkInterval = 0.2f;
            UIManager.instance.SetLoadingUI(true);
            PhotonNetwork.Reconnect(); // �ڵ� �翬�� �õ�
        }
    }
    // ���� ������ ���� ����Ϸ�
    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        Debug.Log("Connected to Master Server!");
        PhotonNetwork.JoinLobby();
    }

    // �κ� ���� �Ϸ�
    public override void OnJoinedLobby()
    {
        PhotonNetwork.NickName = SteamManager.GetSteamName();
        Debug.Log("Joined Lobby!");
        Debug.Log($"[�� ���� ��û] ���� ����: {PhotonNetwork.NetworkClientState}");
    }
}
