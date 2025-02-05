using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public RoomInfo joinRoom;
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
        roomOptions.EmptyRoomTtl = 0;
        roomOptions.PublishUserId = true;
        roomOptions.PlayerTtl = 5000;
        if(!string.IsNullOrEmpty(password))
        {
            roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable() { { "Password", password } };
            roomOptions.CustomRoomPropertiesForLobby = new string[]{"Password"};
        }
        // �� ���� �õ�
        PhotonNetwork.CreateRoom(roomName, roomOptions, TypedLobby.Default);
    }

    // �� ���� ���� ��
    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();
        UIManager.instance.ChangeRoomUI();
        UpdatePlayerUI();
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
        if(joinRoom.CustomProperties.ContainsKey("Password"))
        {
            PopManager.instance.gameSelectPop.SetPasswordWindow();
            return;
        }
        PhotonNetwork.JoinRoom(roomName);
    }
    public void PasswordJoinRoom(string password)
    {
        if((string)joinRoom.CustomProperties["Password"] == password)
        {
            PhotonNetwork.JoinRoom(joinRoom.Name);
        }
        else
        {
            Debug.Log("The password is incorrect.");
        }
    }
    // �� ���� ���� ��
    public override void OnJoinedRoom()
    {
        UIManager.instance.ChangeRoomUI();
        UpdatePlayerUI();
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
    #region ������ų� ����
    void UpdatePlayerUI()
    {
        Player master = PhotonNetwork.MasterClient; // ���� ���� ��������
        Player otherPlayer = null; // ������ ����

        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (player != master)
            {
                otherPlayer = player;
                break;
            }
        }

        // ���� ����, ������ �����ʿ� ǥ��
        UIManager.instance.roomUI.leftPlayerText.text = master.NickName;
        UIManager.instance.roomUI.rightPlayerText.text = otherPlayer != null ? otherPlayer.NickName : "��� ��...";
    }
    // ���ο� �÷��̾ ������ �� ����
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        UpdatePlayerUI();
    }

    // �÷��̾ ������ �� ����
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdatePlayerUI();
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
