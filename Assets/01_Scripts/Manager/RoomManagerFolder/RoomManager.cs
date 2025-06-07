// RoomManager.cs
using Fusion;
using Fusion.Sockets;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

// ���� ���� �� ���� ������
public partial class RoomManager : NetworkBehaviour
{
    public NetworkRunner runnerPrefab; // Runner ������
    private NetworkRunner runnerInstance; // ���� ��� ���� Runner
    public SessionInfo joinRoom;
    public string gameSceneName = "GeneralModeScene"; // ���� �� �̸�
    public int maxScore = 3; // �ִ� ����

    private string roomPassword; // ������ ����� ���

    public NetworkRunner RunnerInstance => runnerInstance;
    public string RoomPassword => roomPassword;

    // PlayerRef �� �г��� ����
    Dictionary<PlayerRef, string> _nicknames = new Dictionary<PlayerRef, string>();
    [SerializeField] public NetworkPrefabRef playerPrefab;
    // ���� ������ ���¸� ����� ����
    private int _lastReadyCount = -1;
}
// --------------------------------------------------------------------------------------
// Photon Fusion 2.x ���� RoomManager (��Ȯ�� �� ��ȯ ����)
// Photon Fusion ��� RoomManager

//using UnityEngine;
//using UnityEngine.SceneManagement;
//using Fusion;
//using Fusion.Sockets;
//using System;
//using System.Text;
//using System.Collections.Generic;
//using System.Threading.Tasks;

//public class RoomManager : MonoBehaviour, INetworkRunnerCallbacks
//{
//    public NetworkRunner runnerPrefab; // Runner ������
//    private NetworkRunner runnerInstance; // ���� ��� ���� Runner
//    public SessionInfo joinRoom;
//    public string gameSceneName = "GeneralModeScene"; // ���� �� �̸�
//    public int maxScore = 3; // �ִ� ����

//    private string roomPassword; // ������ ����� ���

//    #region �� ����� �����ϱ� �����ϱ�
//    // �� ����
//    public async void CreateRoom(string password, string roomNmae)
//    {
//        string roomId = roomNmae+ "[03%14]" + Guid.NewGuid().ToString();
//        byte[] token = Encoding.UTF8.GetBytes(password);
//        if (password.Length > 0) 
//        {
//            roomId += "[01%01]" + "password";
//        }
//        roomPassword = password; // ������ ����

//        await StartFusionSession(GameMode.Host, roomId, token);
//    }
//    public async void JoinRoom(string password, string roomName)
//    {
//        byte[] token = Encoding.UTF8.GetBytes(password); // ��й�ȣ�� ����Ʈ �迭�� ���ڵ�
//        await StartFusionSession(GameMode.Client, roomName, token);
//    }
//    // ���� ����
//    private async Task StartFusionSession(GameMode mode, string roomId, byte[] token)
//    {
//        runnerInstance = Instantiate(runnerPrefab);
//        runnerInstance.ProvideInput = true;

//        var sceneManager = runnerInstance.gameObject.AddComponent<NetworkSceneManagerDefault>();

//        StartGameArgs args = new StartGameArgs
//        {
//            GameMode = mode,
//            SessionName = roomId,
//            Scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex), // �� ���� �� �� ��ȯ�� ���� ����
//            SceneManager = sceneManager,
//            ConnectionToken = token
//        };

//        await runnerInstance.StartGame(args);
//    }
//    public async void CreateOrJoinRoom(bool isHost, string roomName, string password)
//    {
//        if (runnerInstance != null)
//        {
//            Debug.LogWarning("Runner already exists");
//            return;
//        }

//        runnerInstance = Instantiate(runnerPrefab);
//        runnerInstance.ProvideInput = true;

//        var sceneManager = runnerInstance.gameObject.AddComponent<NetworkSceneManagerDefault>();

//        StartGameArgs args = new StartGameArgs()
//        {
//            GameMode = isHost ? GameMode.Host : GameMode.Client,
//            SessionName = roomName + "[-&%&-]:" + System.Guid.NewGuid().ToString(),
//            SceneManager = sceneManager
//        };
//        await runnerInstance.StartGame(args);
//    }

//    // �� ������ �� ���� ������ ����
//    public void LeaveRoom()
//    {
//        if (runnerInstance != null)
//        {
//            runnerInstance.Shutdown();
//            SceneManager.LoadScene("MainScene");
//        }
//    }

//    // ��� �÷��̾ �غ�Ǿ��� �� �� ��ȯ
//    // ��� �÷��̾ �غ�Ǿ��� �� �� ��ȯ (SceneRef ���)
//    public void LoadGameScene()
//    {
//        if (runnerInstance != null && runnerInstance.IsServer)
//        {
//            Debug.Log("[Fusion] ��� �÷��̾� �غ� �Ϸ� - �� �ε� ����");
//            SceneRef sceneRef = SceneRef.Parse(gameSceneName);
//            NetworkLoadSceneParameters parameters = new NetworkLoadSceneParameters();
//            runnerInstance.SceneManager.LoadScene(sceneRef, parameters);
//        }
//    }

//    // �÷��̾ �������� �� ȣ���
//    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
//    {
//        Debug.Log($"[Fusion] �÷��̾� ����: {player}");
//    }

//    // �÷��̾ �������� �� ȣ���
//    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
//    {
//        Debug.Log($"[Fusion] �÷��̾� ����: {player}");
//    }
//    #endregion
//    public void OnSceneLoadStart(NetworkRunner runner) => Debug.Log("[Fusion] �� �ε� ����");
//    public void OnSceneLoadDone(NetworkRunner runner) => Debug.Log("[Fusion] �� �ε� �Ϸ��");
//    public void OnConnectedToServer(NetworkRunner runner) => Debug.Log("[Fusion] ������ �����");
//    public void OnDisconnectedFromServer(NetworkRunner runner) => Debug.Log("[Fusion] ���� ���� ������");

//    // �Ʒ� �ݹ��� ���� ������� ���� - �ʿ� �� ���� ����
//    public void OnInput(NetworkRunner runner, NetworkInput input) { } // �Է� ó�� (�̻��)
//    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { } // �Է� ���� (�̻��)
//    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { } // ���� ���� (�̻��)
//    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { } // Runner ���� (�̻��)
//    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { } // ����� �޽��� (�̻��)
//    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { } // ���� ���� (�̻��)
//    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { } // ȣ��Ʈ ���̱׷��̼� (�̻��)
//    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, System.ArraySegment<byte> data) { } // �ŷڼ� ������ ���� (�̻��)
//    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { } // ���� ����Ʈ ���� (�̻��)
//    public void OnPlayerRefAssigned(NetworkRunner runner, PlayerRef playerRef) { } // PlayerRef �Ҵ� (�̻��)
//    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { } // AOI ���� (�̻��)
//    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { } // AOI ���� (�̻��)
//    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { } // ���� ���� ���� ���� (�̻��)
//    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { } // ���� ��û ó�� (�̻��)
//    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { } // ������ ���� ����� (�̻��)
//}


//-------------------------------------------------
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using Photon.Pun;
//using Photon.Realtime;
//using UnityEngine;
//using MyGame.Utils;
//using UnityEngine.SceneManagement;
//using UnityEngine.Diagnostics;
//using UnityEngine.UI;
//using UnityEngine.Timeline;
//using System.Security.Cryptography;
//using ExitGames.Client.Photon;
//using NUnit.Framework;
//public static class NetworkEventCodes // �̺�Ʈ �ڵ�
//{
//    public const byte AddScoreEvent = 1; 
//    public const byte ScoreUpdated = 2;
//    public const byte NextRound = 3; 
//    public const byte EndGame = 4;
//}
//public class RoomManager : MonoBehaviourPunCallbacks, IOnEventCallback
//{
//    public RoomInfo joinRoom;

//    private int maxScore = 3;
//    public override void OnEnable()
//    {
//        base.OnEnable();
//        SceneManager.sceneLoaded += ChangeIngameMode;
//        PhotonNetwork.AddCallbackTarget(this);
//    }
//    public override void OnDisable()
//    {
//        base.OnDisable();
//        SceneManager.sceneLoaded -= ChangeIngameMode;
//        PhotonNetwork.RemoveCallbackTarget(this);
//    }
//    void Update()
//    {
//        if (Input.GetKeyDown(KeyCode.F2))
//        {
//            Debug.Log($"[Photon ���� üũ]");
//            Debug.Log($"- ���� ����: {PhotonNetwork.NetworkClientState}");
//            Debug.Log($"- InRoom: {PhotonNetwork.InRoom}");
//            Debug.Log($"- InLobby: {PhotonNetwork.InLobby}");
//            Debug.Log($"- IsConnected: {PhotonNetwork.IsConnected}");
//            Debug.Log($"- IsConnectedAndReady: {PhotonNetwork.IsConnectedAndReady}");
//        }
//    }
//    #region ����� ����
//    public void CreateRoom(string roomName, bool isVisible, string password)
//    {
//        // �� �̸��� �� ���ڿ��̸� ���� �̸�����
//        if (string.IsNullOrEmpty(roomName))
//        {
//            roomName = "DefaultRoom_" + Random.Range(1000, 9999);
//        }

//        // �� �ɼ� ���� (����: �ִ� 4��)
//        RoomOptions roomOptions = new RoomOptions()
//        {
//            MaxPlayers = 2,
//            IsVisible = isVisible,
//            EmptyRoomTtl = 0,
//            PublishUserId = true,
//            PlayerTtl = 5000
//        };
//        object[] scores = new object[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0};
//        roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable();
//        if (!string.IsNullOrEmpty(password))
//        {
//            roomOptions.CustomRoomProperties["Password"] = password;
//            roomOptions.CustomRoomProperties["Round"] = 0;
//            roomOptions.CustomRoomPropertiesForLobby = new string[]{"Password", "Round" };
//        }
//        roomOptions.CustomRoomProperties["PlayerScore"] = scores;
//        // �� ���� �õ�
//        PhotonNetwork.CreateRoom(roomName, roomOptions, TypedLobby.Default);
//    }

//    // �� ���� ���� ��
//    public override void OnCreatedRoom()
//    {
//        base.OnCreatedRoom();
//        OnConnectedToServer();
//        UIManager.instance.ChangeRoomUI();
//        UpdatePlayerUI();
//        Debug.Log($"Room Created: {PhotonNetwork.CurrentRoom.Name}");
//    }

//    // �� ���� ���� ��(���� �̸��� ���� �̹� �ִ� ��)
//    public override void OnCreateRoomFailed(short returnCode, string message)
//    {
//        Debug.LogWarning($"Create Room Failed: {message}");
//    }
//    #endregion
//    #region �� ���� ����
//    public void JoinRoom(string roomName)
//    {
//        if(joinRoom.CustomProperties.ContainsKey("Password"))
//        {
//            PopManager.instance.gameSelectPop.SetPasswordWindow();
//            return;
//        }
//        PhotonNetwork.JoinRoom(roomName);
//    }
//    public void PasswordJoinRoom(string password)
//    {
//        if((string)joinRoom.CustomProperties["Password"] == password)
//        {
//            PhotonNetwork.JoinRoom(joinRoom.Name);
//        }
//        else
//        {
//            Debug.Log("The password is incorrect.");
//        }
//    }
//    // �� ���� ���� ��
//    public override void OnJoinedRoom()
//    {
//        // �� ���� �� Ready �ʱ�ȭ
//        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable { { "Ready", false } };
//        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
//        UIManager.instance.ChangeRoomUI();
//        UpdatePlayerUI();
//        OnConnectedToServer();
//        Debug.Log($"Joined Room: {PhotonNetwork.CurrentRoom.Name}");
//        // ���⼭���ʹ� �� ���� ����(�÷��̾� ���, ä�� ��)�� ���� ����
//    }

//    // �� ���� ���� ��(���� �� �̸�, �� �� �� ��)
//    public override void OnJoinRoomFailed(short returnCode, string message)
//    {
//        Debug.LogWarning($"Join Room Failed: {message}");
//    }
//    #endregion
//    #region �泪����
//    public void LeaveRoom()
//    {
//        if (PhotonNetwork.InRoom)
//        {
//            PhotonNetwork.LeaveRoom(); // ���� �� ������
//        }
//    }
//    public override void OnLeftRoom()
//    {
//        Debug.Log("�濡�� ����");
//        PhotonNetwork.LoadLevel("MainScene"); // �κ� ������ �̵�
//        UIManager.instance.ChangeGame(true);
//        UIManager.instance.ChangeLobbyUI();
//    }
//    #endregion
//    #region ������ų� ����
//    void UpdatePlayerUI()
//    {
//        Player master = PhotonNetwork.MasterClient;
//        Player otherPlayer = null;

//        // ��� �÷��̾��� Ready ���¸� UI�� �ݿ�
//        foreach (Player player in PhotonNetwork.PlayerList)
//        {
//            if (player.CustomProperties.TryGetValue("Ready", out object readyObj) && readyObj is bool isReady)
//            {
//                UIManager.instance.roomUI.SetImReady(player.IsMasterClient, isReady);
//            }

//            if (!player.IsMasterClient)
//            {
//                otherPlayer = player;
//            }
//        }

//        // �г��� UI ������Ʈ
//        UIManager.instance.roomUI.leftPlayerText.text = master.NickName;
//        UIManager.instance.roomUI.rightPlayerText.text = otherPlayer != null ? otherPlayer.NickName : "��� ��...";
//    }

//    // ���ο� �÷��̾ ������ �� ����
//    public override void OnPlayerEnteredRoom(Player newPlayer)
//    {
//        UpdatePlayerUI();
//        //if (newPlayer.CustomProperties.ContainsKey("Ready"))
//        //{
//        //    UIManager.instance.roomUI.SetImReady(newPlayer.IsMasterClient, (bool)newPlayer.CustomProperties["Ready"]);
//        //}
//    }

//    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
//    {
//        if (changedProps.ContainsKey("Ready"))
//        {
//            bool isReady = (bool)changedProps["Ready"];
//            UIManager.instance.roomUI.SetImReady(targetPlayer.IsMasterClient, isReady);

//            CheckAllPlayersReady(); // ������ Ŭ���̾�Ʈ�� �� �Լ� ���ο��� üũ
//        }
//    }
//    // �÷��̾ ������ �� ����
//    public override void OnPlayerLeftRoom(Player otherPlayer)
//    {
//        UpdatePlayerUI();

//        if (otherPlayer.CustomProperties.ContainsKey("Ready"))
//        {
//            UIManager.instance.roomUI.SetImReady(otherPlayer.IsMasterClient, (bool)otherPlayer.CustomProperties["Ready"]);
//        }
//    }
//    public override void OnMasterClientSwitched(Player newMasterClient)
//    {
//        Debug.Log($" ������ Ŭ���̾�Ʈ ����: {newMasterClient.NickName}");

//        // �� �������� Ready ���� ���� �ʱ�ȭ
//        if (newMasterClient.IsLocal)
//        {
//            ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable { { "Ready", false } };
//            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
//        }

//        // UI ����
//        UpdatePlayerUI();
//    }
//    #endregion
//    #region �ΰ��� ��ȯ
//    private void CheckAllPlayersReady()
//    {
//        // ������ Ŭ���̾�Ʈ�� �ƴϸ� ó������ ����
//        if (!PhotonNetwork.IsMasterClient || PhotonNetwork.PlayerList.Length < 2) return;

//        // ��� �÷��̾��� "Ready" ���°� true���� Ȯ��
//        foreach (Player p in PhotonNetwork.PlayerList)
//        {
//            // Ready ������Ƽ�� ���ų� false��� ���� �غ� �� �� ������ ����
//            if (!p.CustomProperties.ContainsKey("Ready") || !(bool)p.CustomProperties["Ready"])
//            {
//                return;
//            }
//        }

//        // ������� �Դٸ�, ��� �ο��� Ready ����
//        Debug.Log("��� �÷��̾ Ready! GameScene���� �̵��մϴ�.");

//        // ������ Ŭ���̾�Ʈ�� �� �ε带 Ʈ�����ϸ�
//        // AutomaticallySyncScene�� true�� �ٸ� Ŭ���̾�Ʈ�� �ڵ� �̵�
//        PhotonNetwork.AutomaticallySyncScene = true;
//        PhotonNetwork.LoadLevel("GeneralModeScene");
//    }
//    public void ChangeIngameMode(Scene scene, LoadSceneMode mode)
//    {
//        if(scene.name != "MainScene")
//        {
//            UIManager.instance.ChangeGame(false);
//        }
//    }
//    #endregion
//    #region ���� ����

//    private void showScore(int idx)
//    {
//        IngameController.Instance.ingameUIController.addScore(idx);
//    }

//    public (int, int) GetScore()
//    {
//        var roomProps = PhotonNetwork.CurrentRoom.CustomProperties;
//        Debug.Log("-------------------------------------------------" + roomProps);
//        object[] scores = (object[])roomProps["PlayerScore"];
//        int masterScore = (int)scores[0];
//        int otherScore = (int)scores[1];
//        return (masterScore, otherScore);
//    }
//    #endregion
//    #region ���� ����
//    public int GetCurrentRound()
//    {
//        if (PhotonNetwork.CurrentRoom == null) return 0;
//        object roundValue;
//        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("Round", out roundValue))
//        {
//            return (int)roundValue;
//        }

//        return 0; // �⺻��
//    }
//    private void NextRound()
//    {
//        RaiseEventOptions options = new RaiseEventOptions
//        {
//            Receivers = ReceiverGroup.All // ��� Ŭ���̾�Ʈ����
//        };
//        PhotonNetwork.RaiseEvent(NetworkEventCodes.NextRound, null, options, SendOptions.SendReliable);
//    }
//    private void EndRound()
//    {
//        RaiseEventOptions options = new RaiseEventOptions
//        {
//            Receivers = ReceiverGroup.All // ��� Ŭ���̾�Ʈ����
//        };
//        PhotonNetwork.RaiseEvent(NetworkEventCodes.EndGame, null, options, SendOptions.SendReliable);
//    }
//    private bool IsWin()
//    {
//        var roomProps = PhotonNetwork.CurrentRoom.CustomProperties;
//        int winIdx = 0;
//        object[] scores = (object[])roomProps["PlayerScore"];
//        Debug.Log(scores);
//        for (int i = 0; i <= scores.Length; i++)
//        {
//            if ((int)scores[i] == maxScore)
//            {
//                Debug.Log("�ε��� : "+i);
//                Debug.Log("���ھ� : " + (int)scores[i]);
//                winIdx = i;
//                break;
//            }
//        }


//        if (IngameController.Instance.playerControllers[winIdx].p_PhotonView.IsMine)
//        {
//            return false;
//        }
//        else
//        {
//            return true;
//        }
//    }
//    #endregion

//    // Ready ��ư�� ������ �� ȣ��Ǵ� �Լ� (��ư OnClick�� ����)
//    public void OnClickReady(bool isReady)
//    {
//        if (!PhotonNetwork.InRoom) return;

//        // Player Custom Properties�� "Ready" = true ����
//        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable();
//        props["Ready"] = isReady;
//        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
//        UIManager.instance.roomUI.SetImReady(PhotonNetwork.IsMasterClient, isReady);
//    }

//    // �� ����� ���ŵ� ������ Photon�� �� �ݹ��� ȣ������
//    public override void OnRoomListUpdate(List<RoomInfo> roomList)
//    {
//        PopManager.instance.gameSelectPop.regularGamePop.SetRoomListSlot(roomList);
//        // ���⼭ roomList�� ��ȸ�ϸ� RoomInfo�� ���� �� ����
//        foreach (RoomInfo info in roomList)
//        {
//            Debug.Log($"Room Name: {info.Name}, PlayerCount: {info.PlayerCount}/{info.MaxPlayers}");
//            // ���ϴ� UI ǥ�ó� �߰� ���� �ۼ�
//        }
//    }

//    // �������� ���� ������ URL�޾ƿ��� �ڵ�
//    //25.03.04 ���� ��� ���Ұ� ���Ƽ� ��.����
//    public void OnConnectedToServer()
//    {
//        Debug.Log("Connected to Master Server!");
//        PhotonNetwork.JoinLobby();
//        //string iconURL = SteamManager.GetPlayerAvatarURL();
//        //Debug.Log(iconURL);
//        //ExitGames.Client.Photon.Hashtable properties = new ExitGames.Client.Photon.Hashtable()
//        //{
//        //    { "PlayerIconURL", iconURL}
//        //};

//        //PhotonNetwork.LocalPlayer.SetCustomProperties(properties);
//    }
//    public void OnEvent(EventData photonEvent)
//    {
//        // ���ھ� �߰�
//        if (photonEvent.Code == NetworkEventCodes.AddScoreEvent)
//        {
//            object[] data = (object[])photonEvent.CustomData;
//            int playerKey = (int)data[0];
//            int amount = (int)data[1];

//            if (PhotonNetwork.IsMasterClient)
//            {
//                // ���� ����� �����͸�
//                var roomProps = PhotonNetwork.CurrentRoom.CustomProperties;

//                // ���� ���� ���
//                int Playeridx = IngameController.Instance.PlayerIdx(playerKey);
//                object[] score = (object[])roomProps["PlayerScore"];
//                int newScore = (int)score[Playeridx] + 1;
//                score[Playeridx] = newScore;
//                roomProps["PlayerScore"] = score;

//                // ���� Round ��������
//                int currentRound = roomProps.ContainsKey("Round") ? (int)roomProps["Round"] : 0;
//                roomProps["Round"] = currentRound + 1;

//                // Ŀ���� ������Ƽ ������Ʈ
//                PhotonNetwork.CurrentRoom.SetCustomProperties(roomProps);
//                // ���� UI ������Ʈ ��ε�ĳ��Ʈ
//                object[] result = new object[] { Playeridx, newScore };
//                RaiseEventOptions broadcastOpts = new RaiseEventOptions { Receivers = ReceiverGroup.All };
//                SendOptions sendOptions = new SendOptions { Reliability = true };

//                PhotonNetwork.RaiseEvent(NetworkEventCodes.ScoreUpdated, result, broadcastOpts, sendOptions);


//                // ��� ó��
//                if(newScore < maxScore)
//                {
//                    // ���� ���� �Լ� ����
//                    Invoke(nameof(NextRound), 3f);
//                }
//                else
//                {
//                    // ���� ����
//                    Invoke(nameof(EndRound), 0.5f);
//                }
//            }
//        }

//        // ��� UI����
//        else if (photonEvent.Code == NetworkEventCodes.ScoreUpdated)
//        {
//            object[] result = (object[])photonEvent.CustomData;
//            int playerKey = (int)result[0];
//            showScore(playerKey);
//        }

//        // �����(��������)
//        else if (photonEvent.Code == NetworkEventCodes.NextRound)
//        {
//            string currentScene = SceneManager.GetActiveScene().name;
//            SceneManager.LoadScene(currentScene);
//        }

//        // ���� ����
//        else if (photonEvent.Code == NetworkEventCodes.EndGame)
//        {
//            IngameController.Instance.ingameUIController.OnEndGameResult(IsWin());
//            Invoke(nameof(LeaveRoom), 3f);
//        }
//    }
//}
