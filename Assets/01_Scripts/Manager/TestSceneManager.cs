using UnityEngine;
using MyGame.Utils;
using Fusion;

public class TestSceneManager : NetworkBehaviour
{
    [SerializeField] private Ground ground;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private int testPlayerCount = 2;  // �׽�Ʈ�� �÷��̾� ��

    //private void Start()
    //{
    //    if (PhotonNetwork.IsConnected)
    //    {
    //        SpawnTestPlayers();
    //    }
    //    else
    //    {
    //        // Photon ����
    //        PhotonNetwork.ConnectUsingSettings();
    //    }
    //}

    //public override void OnConnectedToMaster()
    //{
    //    // �׽�Ʈ�� �� ����
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

    //        // �� �÷��̾ ������ �̸� �Ҵ�
    //        player.GetComponent<PhotonView>().Owner.NickName = $"TestPlayer_{i}";
    //        player.GetComponent<IPlayerContext>().InitGround(i);
    //    }
    //}
}