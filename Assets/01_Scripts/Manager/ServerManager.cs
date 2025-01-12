using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class ServerManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private string gameVersion = "1.0";
    void Start()
    {
        //  자동 동기화
        PhotonNetwork.AutomaticallySyncScene = true;

        // 게임 버전 셋팅
        PhotonNetwork.GameVersion = gameVersion;
        PhotonNetwork.ConnectUsingSettings(); 
        Debug.Log("Connecting to Photon...");
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
        Debug.Log("Joined Lobby!");
        // 이제 방 생성/검색이 가능해짐
    }
}
