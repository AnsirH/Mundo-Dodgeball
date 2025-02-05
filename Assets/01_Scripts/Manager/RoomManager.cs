using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class RoomManager : MonoBehaviourPunCallbacks
{
    #region ����� ����
    public void CreateRoom(string roomName, bool isVisible, string password)
    {
        // �� �̸��� �� ���ڿ��̸� ���� �̸�����
        if (string.IsNullOrEmpty(roomName))
        {
            roomName = "DefaultRoom_" + Random.Range(1000, 9999);
        }

        // �� �ɼ� ���� (����: �ִ� 4��)
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 2;
        roomOptions.IsVisible = isVisible;
        var customProps = new Hashtable();
        if(!string.IsNullOrEmpty(password))
        {
            customProps["Password"] = password;
        }
        // �� ���� �õ�
        PhotonNetwork.CreateRoom(roomName, roomOptions, TypedLobby.Default);
    }

    // �� ���� ���� ��
    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();
        UIManager.instance.ChangeRoomUI();
        Debug.Log($"Room Created: {PhotonNetwork.CurrentRoom.Name}");
    }

    // �� ���� ���� ��(���� �̸��� ���� �̹� �ִ� ��)
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.LogWarning($"Create Room Failed: {message}");
    }
    #endregion

    #region �� ���� ����
    public void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }

    // �� ���� ���� ��
    public override void OnJoinedRoom()
    {
        Debug.Log($"Joined Room: {PhotonNetwork.CurrentRoom.Name}");
        // ���⼭���ʹ� �� ���� ����(�÷��̾� ���, ä�� ��)�� ���� ����
    }

    // �� ���� ���� ��(���� �� �̸�, �� �� �� ��)
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogWarning($"Join Room Failed: {message}");
    }
    #endregion
    #region �泪����
    public void LeaveRoom()
    {
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom(); // ���� �� ������
        }
    }

    public override void OnLeftRoom()
    {
        Debug.Log("�濡�� ����");
        PhotonNetwork.LoadLevel("MainScene"); // �κ� ������ �̵�
        UIManager.instance.ChangeLobbyUI();
    }
    #endregion
    // �� ����� ���ŵ� ������ Photon�� �� �ݹ��� ȣ������
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        PopManager.instance.gameSelectPop.regularGamePop.SetRoomListSlot(roomList);
        // ���⼭ roomList�� ��ȸ�ϸ� RoomInfo�� ���� �� ����
        foreach (RoomInfo info in roomList)
        {
            Debug.Log($"Room Name: {info.Name}, PlayerCount: {info.PlayerCount}/{info.MaxPlayers}");
            // ���ϴ� UI ǥ�ó� �߰� ���� �ۼ�
        }
    }
}
