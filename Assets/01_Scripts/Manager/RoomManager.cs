using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class RoomManager : MonoBehaviourPunCallbacks
{
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }
    // 마스터 서버랑 연결되고 콜백으로 로비입장.
    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to MasterServer.");
        PhotonNetwork.JoinLobby();
    }

    // 로비 입장 성공
    public override void OnJoinedLobby()
    {
        Debug.Log("Joined Lobby. (Now you can create or join rooms)");
    }

    #region 방생성 로직
    public void CreateRoom(string roomName)
    {
        // 방 이름이 빈 문자열이면 임의 이름으로
        if (string.IsNullOrEmpty(roomName))
        {
            roomName = "DefaultRoom_" + Random.Range(1000, 9999);
        }

        // 방 옵션 지정 (예시: 최대 4명)
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 4;

        // 방 생성 시도
        PhotonNetwork.CreateRoom(roomName, roomOptions, TypedLobby.Default);
    }

    // 방 생성 성공 시
    public override void OnCreatedRoom()
    {
        Debug.Log($"Room Created: {PhotonNetwork.CurrentRoom.Name}");
    }

    // 방 생성 실패 시(동일 이름의 방이 이미 있다 등)
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.LogWarning($"Create Room Failed: {message}");
    }
    #endregion

    #region 방 참가 로직
    public void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }

    // 방 참가 성공 시
    public override void OnJoinedRoom()
    {
        Debug.Log($"Joined Room: {PhotonNetwork.CurrentRoom.Name}");
        // 여기서부터는 룸 내부 상태(플레이어 목록, 채팅 등)를 관리 가능
    }

    // 방 참가 실패 시(없는 방 이름, 꽉 찬 방 등)
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogWarning($"Join Room Failed: {message}");
    }
    #endregion
}
