using UnityEngine;
using MyGame.Utils;
using Fusion;

public class TestSceneManager : NetworkBehaviour
{
    [SerializeField] private Ground ground;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private int testPlayerCount = 2;  // 테스트할 플레이어 수

    //private void Start()
    //{
    //    if (PhotonNetwork.IsConnected)
    //    {
    //        SpawnTestPlayers();
    //    }
    //    else
    //    {
    //        // Photon 연결
    //        PhotonNetwork.ConnectUsingSettings();
    //    }
    //}

    //public override void OnConnectedToMaster()
    //{
    //    // 테스트용 룸 생성
    //    PhotonNetwork.CreateRoom("TestRoom");
    //}

    //public override void OnJoinedRoom()
    //{
    //    SpawnTestPlayers();
    //}

    //private void SpawnTestPlayers()
    //{
    //    for (int i = 0; i < testPlayerCount; i++)
    //    {
    //        GameObject player = PhotonNetwork.Instantiate(playerPrefab.name, ground.sections[i].position, Quaternion.identity);

    //        // 각 플레이어에 고유한 이름 할당
    //        player.GetComponent<PhotonView>().Owner.NickName = $"TestPlayer_{i}";
    //        player.GetComponent<IPlayerContext>().InitGround(i);
    //    }
    //}
}