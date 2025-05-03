using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using MyGame.Utils;
using UnityEngine.SceneManagement;
using UnityEngine.Diagnostics;
using UnityEngine.UI;
using UnityEngine.Timeline;
using System.Security.Cryptography;

public class RoomManager : MonoBehaviourPunCallbacks
{

    #region ���� �̺�Ʈ ����Ʈ
    [HideInInspector] public const byte AddScoreEvent = 0x01;
    #endregion

    public RoomInfo joinRoom;

    public override void OnEnable()
    {
        base.OnEnable();
        SceneManager.sceneLoaded += ChangeIngameMode;
    }
    public override void OnDisable()
    {
        base.OnDisable();
        SceneManager.sceneLoaded -= ChangeIngameMode;
    }
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
        OnConnectedToServer();
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
        OnConnectedToServer();
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
        UIManager.instance.ChangeGame(true);
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
    #region �ΰ��� ��ȯ
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
        PhotonNetwork.LoadLevel("GeneralModeScene");
    }
    public void ChangeIngameMode(Scene scene, LoadSceneMode mode)
    {
        if(scene.name != "MainScene")
        {
            UIManager.instance.ChangeGame(false);
        }
    }

    #endregion
    #region ���� �߰�
    // �̺�Ʈ ������
    //public void AddScoreEvent(string playerName)
    [PunRPC]
    public void AddScore(string playerKey, int amount)
    {
        Debug.Log("check LOG : AddScore!");
        // ���� CustomProperties ��������
        var roomProps = PhotonNetwork.CurrentRoom.CustomProperties;

        int currentScore = 0;

        if (roomProps.ContainsKey(playerKey))
        {
            // Ű�� ������ ���� ���� ��������
            currentScore = (int)roomProps[playerKey];
        }
        // Ű�� ������ currentScore = 0 (�⺻��)

        int newScore = currentScore + amount;

        // ���� ������Ʈ
        roomProps[playerKey] = newScore;

        PhotonNetwork.CurrentRoom.SetCustomProperties(roomProps);
        Debug.Log($"log : --------------{playerKey}");
        if ($"Score_{IngameController.Instance.playerControllers[0].photonView.ViewID}" == playerKey)
        {
            photonView.RPC(nameof(showScore), RpcTarget.All, 0);
        }
        else
        {
            photonView.RPC(nameof(showScore), RpcTarget.All, 1);
        }

        

    }
    [PunRPC]
    private void showScore(int idx)
    {
        IngameController.Instance.ingameUIController.addScore(idx);
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
        string playerIconURL = (string)targetPlayer.CustomProperties["PlayerIconURL"];
        
        //if (!targetPlayer.IsMasterClient)
        //{
        //    StartCoroutine(Utility.DownloadImage(playerIconURL, UIManager.instance.roomUI.rightPlayerImage));
        //}
        //else
        //{
        //    StartCoroutine(Utility.DownloadImage(playerIconURL, UIManager.instance.roomUI.leftPlayerImage));
        //}
        // ��� �÷��̾ �������� ������ Ŭ���̾�Ʈ�� Ȯ��
        CheckAllPlayersReady();
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

    // �������� ���� ������ URL�޾ƿ��� �ڵ�
    //25.03.04 ���� ��� ���Ұ� ���Ƽ� ��.����
    public void OnConnectedToServer()
    {
        Debug.Log("Connected to Master Server!");
        //string iconURL = SteamManager.GetPlayerAvatarURL();
        //Debug.Log(iconURL);
        //ExitGames.Client.Photon.Hashtable properties = new ExitGames.Client.Photon.Hashtable()
        //{
        //    { "PlayerIconURL", iconURL}
        //};

        //PhotonNetwork.LocalPlayer.SetCustomProperties(properties);
    }
}
