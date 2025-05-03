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

    #region 포톤 이벤트 바이트
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
    #region 방생성 로직
    public void CreateRoom(string roomName, bool isVisible, string password)
    {
        // 방 이름이 빈 문자열이면 임의 이름으로
        if (string.IsNullOrEmpty(roomName))
        {
            roomName = "DefaultRoom_" + Random.Range(1000, 9999);
        }

        // 방 옵션 지정 (예시: 최대 4명)
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
        // 방 생성 시도
        PhotonNetwork.CreateRoom(roomName, roomOptions, TypedLobby.Default);
    }

    // 방 생성 성공 시
    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();
        OnConnectedToServer();
        UIManager.instance.ChangeRoomUI();
        UpdatePlayerUI();
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
    // 방 참가 성공 시
    public override void OnJoinedRoom()
    {
        UIManager.instance.ChangeRoomUI();
        UpdatePlayerUI();
        OnConnectedToServer();
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
        UIManager.instance.ChangeGame(true);
        UIManager.instance.ChangeLobbyUI();
    }
    #endregion
    #region 방들어오거나 나감
    void UpdatePlayerUI()
    {
        Player master = PhotonNetwork.MasterClient; // 방장 정보 가져오기
        Player otherPlayer = null; // 참여자 정보

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

        // 방장 왼쪽, 참여자 오른쪽에 표시

        UIManager.instance.roomUI.leftPlayerText.text = master.NickName;
        UIManager.instance.roomUI.rightPlayerText.text = otherPlayer != null ? otherPlayer.NickName : "대기 중...";
    }
    // 새로운 플레이어가 들어왔을 때 실행
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        UpdatePlayerUI();
        if (newPlayer.CustomProperties.ContainsKey("Ready"))
        {
            UIManager.instance.roomUI.SetImReady(newPlayer.IsMasterClient, (bool)newPlayer.CustomProperties["Ready"]);
        }
    }

    // 플레이어가 나갔을 때 실행
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdatePlayerUI();
        if (otherPlayer.CustomProperties.ContainsKey("Ready"))
        {
            UIManager.instance.roomUI.SetImReady(otherPlayer.IsMasterClient, (bool)otherPlayer.CustomProperties["Ready"]);
        }
    }
    #endregion
    #region 인게임 전환
    private void CheckAllPlayersReady()
    {
        // 마스터 클라이언트가 아니면 처리하지 않음
        if (!PhotonNetwork.IsMasterClient || PhotonNetwork.PlayerList.Length < 2) return;

        // 모든 플레이어의 "Ready" 상태가 true인지 확인
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            // Ready 프로퍼티가 없거나 false라면 아직 준비 안 된 것으로 간주
            if (!p.CustomProperties.ContainsKey("Ready") || !(bool)p.CustomProperties["Ready"])
            {
                return;
            }
        }

        // 여기까지 왔다면, 모든 인원이 Ready 상태
        Debug.Log("모든 플레이어가 Ready! GameScene으로 이동합니다.");

        // 마스터 클라이언트가 씬 로드를 트리거하면
        // AutomaticallySyncScene이 true면 다른 클라이언트도 자동 이동
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
    #region 점수 추가
    // 이벤트 보내기
    //public void AddScoreEvent(string playerName)
    [PunRPC]
    public void AddScore(string playerKey, int amount)
    {
        Debug.Log("check LOG : AddScore!");
        // 현재 CustomProperties 가져오기
        var roomProps = PhotonNetwork.CurrentRoom.CustomProperties;

        int currentScore = 0;

        if (roomProps.ContainsKey(playerKey))
        {
            // 키가 있으면 기존 점수 가져오기
            currentScore = (int)roomProps[playerKey];
        }
        // 키가 없으면 currentScore = 0 (기본값)

        int newScore = currentScore + amount;

        // 점수 업데이트
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


    // Ready 버튼이 눌렸을 때 호출되는 함수 (버튼 OnClick에 연결)
    public void OnClickReady(bool isReady)
    {
        if (!PhotonNetwork.InRoom) return;

        // Player Custom Properties에 "Ready" = true 설정
        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable();
        props["Ready"] = isReady;
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        UIManager.instance.roomUI.SetImReady(PhotonNetwork.IsMasterClient, isReady);
    }

    // 어떤 플레이어의 CustomProperties가 변경될 때마다 호출되는 콜백
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
        // 모든 플레이어가 레디인지 마스터 클라이언트가 확인
        CheckAllPlayersReady();
    }

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

    // 스팀에서 유저 아이콘 URL받아오는 코드
    //25.03.04 스팀 출시 안할거 같아서 뺌.슈발
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
