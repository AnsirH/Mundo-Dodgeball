using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class RoomManager : MonoBehaviourPunCallbacks
{
    #region 방생성 로직
    public void CreateRoom(string roomName, bool isVisible, string password)
    {
        // 방 이름이 빈 문자열이면 임의 이름으로
        if (string.IsNullOrEmpty(roomName))
        {
            roomName = "DefaultRoom_" + Random.Range(1000, 9999);
        }

        // 방 옵션 지정 (예시: 최대 4명)
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 2;
        roomOptions.IsVisible = isVisible;
        var customProps = new Hashtable();
        if(!string.IsNullOrEmpty(password))
        {
            customProps["Password"] = password;
        }
        // 방 생성 시도
        PhotonNetwork.CreateRoom(roomName, roomOptions, TypedLobby.Default);
    }

    // 방 생성 성공 시
    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();
        UIManager.instance.ChangeRoomUI();
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
    #region 방나가기
    public void LeaveRoom()
    {
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom(); // 현재 방 나가기
        }
    }

    public override void OnLeftRoom()
    {
        Debug.Log("방에서 나감");
        PhotonNetwork.LoadLevel("MainScene"); // 로비 씬으로 이동
        UIManager.instance.ChangeLobbyUI();
    }
    #endregion
    // 방 목록이 갱신될 때마다 Photon이 이 콜백을 호출해줌
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        PopManager.instance.gameSelectPop.regularGamePop.SetRoomListSlot(roomList);
        // 여기서 roomList를 순회하며 RoomInfo를 얻을 수 있음
        foreach (RoomInfo info in roomList)
        {
            Debug.Log($"Room Name: {info.Name}, PlayerCount: {info.PlayerCount}/{info.MaxPlayers}");
            // 원하는 UI 표시나 추가 로직 작성
        }
    }
}
