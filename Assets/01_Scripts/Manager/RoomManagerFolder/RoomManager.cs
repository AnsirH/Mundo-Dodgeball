// RoomManager.cs
using Fusion;
using Fusion.Sockets;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

// 전역 변수 및 공용 접근자
public partial class RoomManager : NetworkBehaviour
{
    public NetworkRunner runnerPrefab; // Runner 프리팹
    private NetworkRunner runnerInstance; // 현재 사용 중인 Runner
    public SessionInfo joinRoom;
    public string gameSceneName = "GeneralModeScene"; // 게임 씬 이름
    public int maxScore = 3; // 최대 점수

    private string roomPassword; // 서버가 기억할 비번

    public NetworkRunner RunnerInstance => runnerInstance;
    public string RoomPassword => roomPassword;

    // PlayerRef → 닉네임 매핑
    Dictionary<PlayerRef, string> _nicknames = new Dictionary<PlayerRef, string>();
    [SerializeField] public NetworkPrefabRef playerPrefab;
    // 이전 프레임 상태를 기억할 변수
    private int _lastReadyCount = -1;
}
// --------------------------------------------------------------------------------------
// Photon Fusion 2.x 기준 RoomManager (정확한 씬 전환 포함)
// Photon Fusion 기반 RoomManager

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
//    public NetworkRunner runnerPrefab; // Runner 프리팹
//    private NetworkRunner runnerInstance; // 현재 사용 중인 Runner
//    public SessionInfo joinRoom;
//    public string gameSceneName = "GeneralModeScene"; // 게임 씬 이름
//    public int maxScore = 3; // 최대 점수

//    private string roomPassword; // 서버가 기억할 비번

//    #region 방 만들기 생성하기 참여하기
//    // 방 생성
//    public async void CreateRoom(string password, string roomNmae)
//    {
//        string roomId = roomNmae+ "[03%14]" + Guid.NewGuid().ToString();
//        byte[] token = Encoding.UTF8.GetBytes(password);
//        if (password.Length > 0) 
//        {
//            roomId += "[01%01]" + "password";
//        }
//        roomPassword = password; // 서버가 저장

//        await StartFusionSession(GameMode.Host, roomId, token);
//    }
//    public async void JoinRoom(string password, string roomName)
//    {
//        byte[] token = Encoding.UTF8.GetBytes(password); // 비밀번호를 바이트 배열로 인코딩
//        await StartFusionSession(GameMode.Client, roomName, token);
//    }
//    // 공통 로직
//    private async Task StartFusionSession(GameMode mode, string roomId, byte[] token)
//    {
//        runnerInstance = Instantiate(runnerPrefab);
//        runnerInstance.ProvideInput = true;

//        var sceneManager = runnerInstance.gameObject.AddComponent<NetworkSceneManagerDefault>();

//        StartGameArgs args = new StartGameArgs
//        {
//            GameMode = mode,
//            SessionName = roomId,
//            Scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex), // 방 생성 시 씬 전환은 하지 않음
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

//    // 방 나가기 및 메인 씬으로 복귀
//    public void LeaveRoom()
//    {
//        if (runnerInstance != null)
//        {
//            runnerInstance.Shutdown();
//            SceneManager.LoadScene("MainScene");
//        }
//    }

//    // 모든 플레이어가 준비되었을 때 씬 전환
//    // 모든 플레이어가 준비되었을 때 씬 전환 (SceneRef 사용)
//    public void LoadGameScene()
//    {
//        if (runnerInstance != null && runnerInstance.IsServer)
//        {
//            Debug.Log("[Fusion] 모든 플레이어 준비 완료 - 씬 로딩 시작");
//            SceneRef sceneRef = SceneRef.Parse(gameSceneName);
//            NetworkLoadSceneParameters parameters = new NetworkLoadSceneParameters();
//            runnerInstance.SceneManager.LoadScene(sceneRef, parameters);
//        }
//    }

//    // 플레이어가 입장했을 때 호출됨
//    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
//    {
//        Debug.Log($"[Fusion] 플레이어 입장: {player}");
//    }

//    // 플레이어가 퇴장했을 때 호출됨
//    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
//    {
//        Debug.Log($"[Fusion] 플레이어 퇴장: {player}");
//    }
//    #endregion
//    public void OnSceneLoadStart(NetworkRunner runner) => Debug.Log("[Fusion] 씬 로딩 시작");
//    public void OnSceneLoadDone(NetworkRunner runner) => Debug.Log("[Fusion] 씬 로드 완료됨");
//    public void OnConnectedToServer(NetworkRunner runner) => Debug.Log("[Fusion] 서버에 연결됨");
//    public void OnDisconnectedFromServer(NetworkRunner runner) => Debug.Log("[Fusion] 서버 연결 해제됨");

//    // 아래 콜백은 현재 사용하지 않음 - 필요 시 구현 예정
//    public void OnInput(NetworkRunner runner, NetworkInput input) { } // 입력 처리 (미사용)
//    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { } // 입력 누락 (미사용)
//    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { } // 연결 실패 (미사용)
//    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { } // Runner 종료 (미사용)
//    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { } // 사용자 메시지 (미사용)
//    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { } // 인증 응답 (미사용)
//    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { } // 호스트 마이그레이션 (미사용)
//    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, System.ArraySegment<byte> data) { } // 신뢰성 데이터 수신 (미사용)
//    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { } // 세션 리스트 갱신 (미사용)
//    public void OnPlayerRefAssigned(NetworkRunner runner, PlayerRef playerRef) { } // PlayerRef 할당 (미사용)
//    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { } // AOI 퇴장 (미사용)
//    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { } // AOI 진입 (미사용)
//    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { } // 연결 해제 이유 포함 (미사용)
//    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { } // 연결 요청 처리 (미사용)
//    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { } // 데이터 전송 진행률 (미사용)
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
//public static class NetworkEventCodes // 이벤트 코드
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
//            Debug.Log($"[Photon 상태 체크]");
//            Debug.Log($"- 현재 상태: {PhotonNetwork.NetworkClientState}");
//            Debug.Log($"- InRoom: {PhotonNetwork.InRoom}");
//            Debug.Log($"- InLobby: {PhotonNetwork.InLobby}");
//            Debug.Log($"- IsConnected: {PhotonNetwork.IsConnected}");
//            Debug.Log($"- IsConnectedAndReady: {PhotonNetwork.IsConnectedAndReady}");
//        }
//    }
//    #region 방생성 로직
//    public void CreateRoom(string roomName, bool isVisible, string password)
//    {
//        // 방 이름이 빈 문자열이면 임의 이름으로
//        if (string.IsNullOrEmpty(roomName))
//        {
//            roomName = "DefaultRoom_" + Random.Range(1000, 9999);
//        }

//        // 방 옵션 지정 (예시: 최대 4명)
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
//        // 방 생성 시도
//        PhotonNetwork.CreateRoom(roomName, roomOptions, TypedLobby.Default);
//    }

//    // 방 생성 성공 시
//    public override void OnCreatedRoom()
//    {
//        base.OnCreatedRoom();
//        OnConnectedToServer();
//        UIManager.instance.ChangeRoomUI();
//        UpdatePlayerUI();
//        Debug.Log($"Room Created: {PhotonNetwork.CurrentRoom.Name}");
//    }

//    // 방 생성 실패 시(동일 이름의 방이 이미 있다 등)
//    public override void OnCreateRoomFailed(short returnCode, string message)
//    {
//        Debug.LogWarning($"Create Room Failed: {message}");
//    }
//    #endregion
//    #region 방 참가 로직
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
//    // 방 참가 성공 시
//    public override void OnJoinedRoom()
//    {
//        // 방 입장 시 Ready 초기화
//        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable { { "Ready", false } };
//        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
//        UIManager.instance.ChangeRoomUI();
//        UpdatePlayerUI();
//        OnConnectedToServer();
//        Debug.Log($"Joined Room: {PhotonNetwork.CurrentRoom.Name}");
//        // 여기서부터는 룸 내부 상태(플레이어 목록, 채팅 등)를 관리 가능
//    }

//    // 방 참가 실패 시(없는 방 이름, 꽉 찬 방 등)
//    public override void OnJoinRoomFailed(short returnCode, string message)
//    {
//        Debug.LogWarning($"Join Room Failed: {message}");
//    }
//    #endregion
//    #region 방나가기
//    public void LeaveRoom()
//    {
//        if (PhotonNetwork.InRoom)
//        {
//            PhotonNetwork.LeaveRoom(); // 현재 방 나가기
//        }
//    }
//    public override void OnLeftRoom()
//    {
//        Debug.Log("방에서 나감");
//        PhotonNetwork.LoadLevel("MainScene"); // 로비 씬으로 이동
//        UIManager.instance.ChangeGame(true);
//        UIManager.instance.ChangeLobbyUI();
//    }
//    #endregion
//    #region 방들어오거나 나감
//    void UpdatePlayerUI()
//    {
//        Player master = PhotonNetwork.MasterClient;
//        Player otherPlayer = null;

//        // 모든 플레이어의 Ready 상태를 UI에 반영
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

//        // 닉네임 UI 업데이트
//        UIManager.instance.roomUI.leftPlayerText.text = master.NickName;
//        UIManager.instance.roomUI.rightPlayerText.text = otherPlayer != null ? otherPlayer.NickName : "대기 중...";
//    }

//    // 새로운 플레이어가 들어왔을 때 실행
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

//            CheckAllPlayersReady(); // 마스터 클라이언트만 이 함수 내부에서 체크
//        }
//    }
//    // 플레이어가 나갔을 때 실행
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
//        Debug.Log($" 마스터 클라이언트 변경: {newMasterClient.NickName}");

//        // 새 마스터의 Ready 상태 강제 초기화
//        if (newMasterClient.IsLocal)
//        {
//            ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable { { "Ready", false } };
//            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
//        }

//        // UI 갱신
//        UpdatePlayerUI();
//    }
//    #endregion
//    #region 인게임 전환
//    private void CheckAllPlayersReady()
//    {
//        // 마스터 클라이언트가 아니면 처리하지 않음
//        if (!PhotonNetwork.IsMasterClient || PhotonNetwork.PlayerList.Length < 2) return;

//        // 모든 플레이어의 "Ready" 상태가 true인지 확인
//        foreach (Player p in PhotonNetwork.PlayerList)
//        {
//            // Ready 프로퍼티가 없거나 false라면 아직 준비 안 된 것으로 간주
//            if (!p.CustomProperties.ContainsKey("Ready") || !(bool)p.CustomProperties["Ready"])
//            {
//                return;
//            }
//        }

//        // 여기까지 왔다면, 모든 인원이 Ready 상태
//        Debug.Log("모든 플레이어가 Ready! GameScene으로 이동합니다.");

//        // 마스터 클라이언트가 씬 로드를 트리거하면
//        // AutomaticallySyncScene이 true면 다른 클라이언트도 자동 이동
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
//    #region 점수 관련

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
//    #region 다음 라운드
//    public int GetCurrentRound()
//    {
//        if (PhotonNetwork.CurrentRoom == null) return 0;
//        object roundValue;
//        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("Round", out roundValue))
//        {
//            return (int)roundValue;
//        }

//        return 0; // 기본값
//    }
//    private void NextRound()
//    {
//        RaiseEventOptions options = new RaiseEventOptions
//        {
//            Receivers = ReceiverGroup.All // 모든 클라이언트에게
//        };
//        PhotonNetwork.RaiseEvent(NetworkEventCodes.NextRound, null, options, SendOptions.SendReliable);
//    }
//    private void EndRound()
//    {
//        RaiseEventOptions options = new RaiseEventOptions
//        {
//            Receivers = ReceiverGroup.All // 모든 클라이언트에게
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
//                Debug.Log("인덱스 : "+i);
//                Debug.Log("스코어 : " + (int)scores[i]);
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

//    // Ready 버튼이 눌렸을 때 호출되는 함수 (버튼 OnClick에 연결)
//    public void OnClickReady(bool isReady)
//    {
//        if (!PhotonNetwork.InRoom) return;

//        // Player Custom Properties에 "Ready" = true 설정
//        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable();
//        props["Ready"] = isReady;
//        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
//        UIManager.instance.roomUI.SetImReady(PhotonNetwork.IsMasterClient, isReady);
//    }

//    // 방 목록이 갱신될 때마다 Photon이 이 콜백을 호출해줌
//    public override void OnRoomListUpdate(List<RoomInfo> roomList)
//    {
//        PopManager.instance.gameSelectPop.regularGamePop.SetRoomListSlot(roomList);
//        // 여기서 roomList를 순회하며 RoomInfo를 얻을 수 있음
//        foreach (RoomInfo info in roomList)
//        {
//            Debug.Log($"Room Name: {info.Name}, PlayerCount: {info.PlayerCount}/{info.MaxPlayers}");
//            // 원하는 UI 표시나 추가 로직 작성
//        }
//    }

//    // 스팀에서 유저 아이콘 URL받아오는 코드
//    //25.03.04 스팀 출시 안할거 같아서 뺌.슈발
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
//        // 스코어 추가
//        if (photonEvent.Code == NetworkEventCodes.AddScoreEvent)
//        {
//            object[] data = (object[])photonEvent.CustomData;
//            int playerKey = (int)data[0];
//            int amount = (int)data[1];

//            if (PhotonNetwork.IsMasterClient)
//            {
//                // 점수 계산은 마스터만
//                var roomProps = PhotonNetwork.CurrentRoom.CustomProperties;

//                // 기존 점수 계산
//                int Playeridx = IngameController.Instance.PlayerIdx(playerKey);
//                object[] score = (object[])roomProps["PlayerScore"];
//                int newScore = (int)score[Playeridx] + 1;
//                score[Playeridx] = newScore;
//                roomProps["PlayerScore"] = score;

//                // 기존 Round 가져오기
//                int currentRound = roomProps.ContainsKey("Round") ? (int)roomProps["Round"] : 0;
//                roomProps["Round"] = currentRound + 1;

//                // 커스텀 프로퍼티 업데이트
//                PhotonNetwork.CurrentRoom.SetCustomProperties(roomProps);
//                // 점수 UI 업데이트 브로드캐스트
//                object[] result = new object[] { Playeridx, newScore };
//                RaiseEventOptions broadcastOpts = new RaiseEventOptions { Receivers = ReceiverGroup.All };
//                SendOptions sendOptions = new SendOptions { Reliability = true };

//                PhotonNetwork.RaiseEvent(NetworkEventCodes.ScoreUpdated, result, broadcastOpts, sendOptions);


//                // 결과 처리
//                if(newScore < maxScore)
//                {
//                    // 다음 라운드 함수 예약
//                    Invoke(nameof(NextRound), 3f);
//                }
//                else
//                {
//                    // 게임 종료
//                    Invoke(nameof(EndRound), 0.5f);
//                }
//            }
//        }

//        // 모두 UI대응
//        else if (photonEvent.Code == NetworkEventCodes.ScoreUpdated)
//        {
//            object[] result = (object[])photonEvent.CustomData;
//            int playerKey = (int)result[0];
//            showScore(playerKey);
//        }

//        // 재시작(다음라운드)
//        else if (photonEvent.Code == NetworkEventCodes.NextRound)
//        {
//            string currentScene = SceneManager.GetActiveScene().name;
//            SceneManager.LoadScene(currentScene);
//        }

//        // 게임 종료
//        else if (photonEvent.Code == NetworkEventCodes.EndGame)
//        {
//            IngameController.Instance.ingameUIController.OnEndGameResult(IsWin());
//            Invoke(nameof(LeaveRoom), 3f);
//        }
//    }
//}
