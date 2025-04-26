using Photon.Pun;
using UnityEngine;

public class TestSceneManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private int testPlayerCount = 2;  // 테스트할 플레이어 수

    private void Start()
    {
        if (PhotonNetwork.IsConnected)
        {
            SpawnTestPlayers();
        }
        else
        {
            // Photon 연결
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnConnectedToMaster()
    {
        // 테스트용 룸 생성
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

            // 각 플레이어에 고유한 이름 할당
            player.GetComponent<PhotonView>().Owner.NickName = $"TestPlayer_{i}";
            IMousePositionGetter mouse = player.GetComponent<PlayerController>();
            mouse.SetClickableGroundLayer("Ground_1");
            print(mouse.GroundLayer);
        }
    }
}