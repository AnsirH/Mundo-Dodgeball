using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
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
    void Start()
    {
        //  �ڵ� ����ȭ
        PhotonNetwork.AutomaticallySyncScene = true;

        // ���� ���� ����
        PhotonNetwork.GameVersion = gameVersion;
        PhotonNetwork.ConnectUsingSettings(); 
        Debug.Log("Connecting to Photon...");
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
    }
}
