using Photon.Pun;
using UnityEngine;

public class TestSceneManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private int testPlayerCount = 2;  // �׽�Ʈ�� �÷��̾� ��

    private void Start()
    {
        if (PhotonNetwork.IsConnected)
        {
            SpawnTestPlayers();
        }
        else
        {
            // Photon ����
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnConnectedToMaster()
    {
        // �׽�Ʈ�� �� ����
        PhotonNetwork.CreateRoom("TestRoom");
    }

    public override void OnJoinedRoom()
    {
        SpawnTestPlayers();
    }

    private void SpawnTestPlayers()
    {
        for (int i = 0; i < testPlayerCount; i++)
        {
            Vector3 spawnPos = new Vector3(i * 2f, 0f, 0f);
            GameObject player = PhotonNetwork.Instantiate(playerPrefab.name, spawnPos, Quaternion.identity);

            // �� �÷��̾ ������ �̸� �Ҵ�
            player.GetComponent<PhotonView>().Owner.NickName = $"TestPlayer_{i}";
            IMousePositionGetter mouse = player.GetComponent<PlayerController>();
            mouse.SetClickableGroundLayer("Ground_1");
            print(mouse.GroundLayer);
        }
    }
}