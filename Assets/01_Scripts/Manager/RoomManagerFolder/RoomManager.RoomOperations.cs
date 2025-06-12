// RoomManager.RoomOperations.cs
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fusion;
using Fusion.Sockets;
using UnityEngine;
using UnityEngine.SceneManagement;

// Fusino 콜백 처리
public partial class RoomManager
{
    // 방 생성
    public async void CreateRoom(string password, string roomNmae)
    {
        string roomId = roomNmae + "[03%14]" + Guid.NewGuid();
        byte[] token = Encoding.UTF8.GetBytes(password);

        if (!string.IsNullOrEmpty(password))
        {
            roomId += "[01%01]password";
        }

        roomPassword = password;
        //await StartSession(GameMode.Host, roomId, token);
    }

    // 방 참가
    public async void JoinRoom(string password, string roomName)
    {
        byte[] token = Encoding.UTF8.GetBytes(password);
        //await StartSession(GameMode.Client, roomName, token);
    }

    // 공통 세션 시작 로직
    async void StartSession(string roomName)
    {
        runnerInstance = Instantiate(runnerPrefab);
        runnerInstance.AddCallbacks(this);
        DontDestroyOnLoad(runnerInstance.gameObject);

        var result = await runnerInstance.StartGame(new StartGameArgs
        {
            GameMode = GameMode.AutoHostOrClient,
            SessionName = roomName,
            Scene = default,
            SceneManager = runnerInstance.gameObject.AddComponent<NetworkSceneManagerDefault>()
        });

        if (!result.Ok)
        {
            Debug.LogError($"StartGame 실패: {result.ShutdownReason}");
            return;
        }

        // 자신의 닉네임을 모두에게 전송
        string myNick = PlayerPrefs.GetString("NickName", "Player");
        //RPC_AnnounceNickname(
        //    RpcTargets.All, 
        //    runnerInstance.LocalPlayer, 
        //    myNick
        //    );
    }

    // 모든 클라이언트가 이 RPC를 통해 닉네임을 공유
    //[Rpc(RpcSources.All, RpcTargets.All)]
    //void RPC_AnnounceNickname(NetworkRunner _, PlayerRef who, string nick)
    //{
    //    _nicknames[who] = nick;
    //    UpdateLobbyUI();
    //}

    public void UpdateLobbyUI()
    {
        var players = runnerInstance.ActivePlayers;
        // 왼쪽: Host, 오른쪽: other
        var arr = new List<PlayerRef>(players);
        PlayerRef host = arr.Count > 0 ? arr[0] : PlayerRef.None;
        PlayerRef other = arr.Count > 1 ? arr[1] : PlayerRef.None;

        UIManager.instance.ChangeRoomUI();

        if (host != PlayerRef.None && _nicknames.TryGetValue(host, out var hn))
            UIManager.instance.roomUI.leftPlayerText.text = hn;
        else
            UIManager.instance.roomUI.leftPlayerText.text = "대기 중...";

        if (other != PlayerRef.None && _nicknames.TryGetValue(other, out var on))
            UIManager.instance.roomUI.rightPlayerText.text = on;
        else
            UIManager.instance.roomUI.rightPlayerText.text = "대기 중...";
    }

    // 2) 실제로 오브젝트가 준비될 때까지 기다렸다가 닉네임을 뿌려주는 코루틴
    private IEnumerator WaitAndSetPlayerUI(PlayerRef playerRef, bool isHost)
    {
        NetworkObject netObj = null;

        // TryGetPlayerObject 가 true 를 반환하고, obj 가 null 아니면 탈출
        while (!runnerInstance.TryGetPlayerObject(playerRef, out netObj) || netObj == null)
            yield return null;

        var np = netObj.GetComponent<NetworkPlayer>();
        if (np == null)
        {
            Debug.LogError("[RoomManager] NetworkPlayer 컴포넌트가 없습니다!");
            yield break;
        }

        // 닉네임 가져오기
        //string nick = np.NickName.Value;

        //// UI에 반영
        //if (isHost)
        //    UIManager.instance.roomUI.leftPlayerText.text = nick;
        //else
        //    UIManager.instance.roomUI.rightPlayerText.text = nick;
    }
    // 방 떠나기
    public void LeaveRoom()
    {
        if (runnerInstance != null)
        {
            runnerInstance.Shutdown();
            SceneManager.LoadScene("MainScene");
        }
    }

    // 호스트가 씬 수동으로 로드하기
    public void LoadGameScene()
    {
        if (runnerInstance != null && runnerInstance.IsServer)
        {
            Debug.Log("[Fusion] 모든 플레이어 준비 완료 - 씬 로딩 시작");
            SceneRef sceneRef = SceneRef.Parse("GeneralModeScene");
            NetworkLoadSceneParameters parameters = new NetworkLoadSceneParameters();
            runnerInstance.SceneManager.LoadScene(sceneRef, parameters);
        }
    }
}
