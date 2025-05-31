// RoomManager.RoomOperations.cs
using System;
using System.Text;
using System.Threading.Tasks;
using Fusion;
using UnityEngine.SceneManagement;

// Fusino �ݹ� ó��
public partial class RoomManager
{
    // �� ����
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

    // �� ����
    public async void JoinRoom(string password, string roomName)
    {
        byte[] token = Encoding.UTF8.GetBytes(password);
        await StartFusionSession(GameMode.Client, roomName, token);
    }

    // ���� ���� ���� ����
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
            Debug.Log("[Fusion] ��� �÷��̾� �غ� �Ϸ� - �� �ε� ����");
            SceneRef sceneRef = SceneRef.Parse(gameSceneName);
            NetworkLoadSceneParameters parameters = new NetworkLoadSceneParameters();
            runnerInstance.SceneManager.LoadScene(sceneRef, parameters);
        }
    }
}
