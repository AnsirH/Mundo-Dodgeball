using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        RoomOptions roomOptions = new RoomOptions()
        {
            MaxPlayers = 2,
            IsVisible = isVisible,
            EmptyRoomTtl = 0,
            PublishUserId = true,
            PlayerTtl = 5000
        };
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
            if (player.CustomProperties.ContainsKey("Ready"))
            {
                UIManager.instance.roomUI.SetImReady(player.IsMasterClient, (bool)player.CustomProperties["Ready"]);
            }
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
        if (newPlayer.CustomProperties.ContainsKey("Ready"))
        {
            UIManager.instance.roomUI.SetImReady(newPlayer.IsMasterClient, (bool)newPlayer.CustomProperties["Ready"]);
        }
    }

    // �÷��̾ ������ �� ����
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdatePlayerUI();
        if (otherPlayer.CustomProperties.ContainsKey("Ready"))
        {
            UIManager.instance.roomUI.SetImReady(otherPlayer.IsMasterClient, (bool)otherPlayer.CustomProperties["Ready"]);
        }
    }
    #endregion
    // Ready ��ư�� ������ �� ȣ��Ǵ� �Լ� (��ư OnClick�� ����)
    public void OnClickReady(bool isReady)
    {
        if (!PhotonNetwork.InRoom) return;

        // Player Custom Properties�� "Ready" = true ����
        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable();
        props["Ready"] = isReady;
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        UIManager.instance.roomUI.SetImReady(PhotonNetwork.IsMasterClient, isReady);
    }

    // � �÷��̾��� CustomProperties�� ����� ������ ȣ��Ǵ� �ݹ�
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (targetPlayer.CustomProperties.ContainsKey("Ready"))
        {
            UIManager.instance.roomUI.SetImReady(targetPlayer.IsMasterClient, (bool)targetPlayer.CustomProperties["Ready"]);
        }
        // ��� �÷��̾ �������� ������ Ŭ���̾�Ʈ�� Ȯ��
        CheckAllPlayersReady();
    }

    private void CheckAllPlayersReady()
    {
        // ������ Ŭ���̾�Ʈ�� �ƴϸ� ó������ ����
        if (!PhotonNetwork.IsMasterClient || PhotonNetwork.PlayerList.Length < 2) return;

        // ��� �÷��̾��� "Ready" ���°� true���� Ȯ��
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            // Ready ������Ƽ�� ���ų� false��� ���� �غ� �� �� ������ ����
            if (!p.CustomProperties.ContainsKey("Ready") || !(bool)p.CustomProperties["Ready"])
            {
                return;
            }
        }

        // ������� �Դٸ�, ��� �ο��� Ready ����
        Debug.Log("��� �÷��̾ Ready! GameScene���� �̵��մϴ�.");

        // ������ Ŭ���̾�Ʈ�� �� �ε带 Ʈ�����ϸ�
        // AutomaticallySyncScene�� true�� �ٸ� Ŭ���̾�Ʈ�� �ڵ� �̵�
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.LoadLevel("PlayerCharacterTest");
    }
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
