// RoomManager.RoomOperations.cs
using System;
using System.Text;
using System.Threading.Tasks;
using Fusion;
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

        await runnerInstance.StartGame(args);
    }

    public void LeaveRoom()
    {
        if (runnerInstance != null)
        {
            runnerInstance.Shutdown();
            SceneManager.LoadScene("MainScene");
        }
    }

    public void LoadGameScene()
    {
        if (runnerInstance != null && runnerInstance.IsServer)
        {
            Debug.Log("[Fusion] 모든 플레이어 준비 완료 - 씬 로딩 시작");
            SceneRef sceneRef = SceneRef.Parse(gameSceneName);
            NetworkLoadSceneParameters parameters = new NetworkLoadSceneParameters();
            runnerInstance.SceneManager.LoadScene(sceneRef, parameters);
        }
    }
}
