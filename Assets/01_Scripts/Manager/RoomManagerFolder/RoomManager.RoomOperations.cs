// RoomManager.RoomOperations.cs
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Fusion;
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
        await StartFusionSession(GameMode.Host, roomId, token);
    }

    // 방 참가
    public async void JoinRoom(string password, string roomName)
    {
        byte[] token = Encoding.UTF8.GetBytes(password);
        await StartFusionSession(GameMode.Client, roomName, token);
    }

    // 공통 세션 시작 로직
    private async Task StartFusionSession(GameMode mode, string roomId, byte[] token)
    {
        // ① 이전 세션이 있으면 깔끔히 종료
        if (runnerInstance != null)
        {
            Debug.Log("시뮬레이션 종료 시작");
            await runnerInstance.Shutdown();                     // 시뮬레이션 종료 요청
            Debug.Log("시뮬레이션 종료 완료");
            Destroy(runnerInstance.gameObject);            // 오브젝트 제거
            runnerInstance = null;                         // 참조 해제
            await Task.Yield();                            // 한 프레임만 기다려 주세요
        }
        runnerInstance = Instantiate(runnerPrefab);
        runnerInstance.ProvideInput = true;

        var sceneManager = runnerInstance.gameObject.AddComponent<NetworkSceneManagerDefault>();

        StartGameArgs args = new StartGameArgs
        {
            GameMode = mode,
            SessionName = roomId,
            Scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex),
            SceneManager = sceneManager,
            ConnectionToken = token
        };
        runnerInstance.AddCallbacks(this);
        await runnerInstance.StartGame(args);
    }
    public void UpdatePlayerUI()
    {
        UIManager.instance.ChangeRoomUI();
        var players = runnerInstance.ActivePlayers.ToList();
        var hostRef = players.FirstOrDefault();
        var otherRef = players.Skip(1).FirstOrDefault();

        // 호스트 닉네임
        if (hostRef != PlayerRef.None)
        {
            var go = runnerInstance.GetPlayerObject(hostRef);
            var np = go.GetComponent<NetworkPlayer>();
            UIManager.instance.roomUI.leftPlayerText.text = np.NickName.Value;
        }

        // 상대 닉네임
        if (otherRef != PlayerRef.None)
        {
            var go = runnerInstance.GetPlayerObject(otherRef);
            var np = go.GetComponent<NetworkPlayer>();
            UIManager.instance.roomUI.rightPlayerText.text = np.NickName.Value;
        }
        else
        {
            UIManager.instance.roomUI.rightPlayerText.text = "대기 중...";
        }
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
