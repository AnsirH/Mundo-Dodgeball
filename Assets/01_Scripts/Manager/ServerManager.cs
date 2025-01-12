using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class ServerManager : MonoBehaviourPunCallbacks
{
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
        Debug.Log("Joined Lobby!");
        // ���� �� ����/�˻��� ��������
    }
}
