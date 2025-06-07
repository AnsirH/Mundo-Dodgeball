// RoomManager.RoomOperations.cs
using System;
using System.Text;
using System.Threading.Tasks;
using Fusion;
using UnityEngine;
using UnityEngine.SceneManagement;

// Fusino �ݹ� ó��
public partial class RoomManager
{
    public void Awake()
    {
        //runnerPrefab = this.gameObject.AddComponent<NetworkRunner>();
        Debug.Log("add : runnerPrefab");
    }
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

    // �� ������
    public void LeaveRoom()
    {
        if (runnerInstance != null)
        {
            runnerInstance.Shutdown();
            SceneManager.LoadScene("MainScene");
        }
    }

    // ȣ��Ʈ�� �� �������� �ε��ϱ�
    public void LoadGameScene()
    {
        if (runnerInstance != null && runnerInstance.IsServer)
        {
            Debug.Log("[Fusion] ��� �÷��̾� �غ� �Ϸ� - �� �ε� ����");
            SceneRef sceneRef = SceneRef.Parse("GeneralModeScene");
            NetworkLoadSceneParameters parameters = new NetworkLoadSceneParameters();
            runnerInstance.SceneManager.LoadScene(sceneRef, parameters);
        }
    }

    // ��� ���� ���� �ߴ��� �ޱ�
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_Ready(PlayerRef player)
    {
        if (!PlayerReadyDict.ContainsKey(player))
            PlayerReadyDict.Add(player, true);
        else
            PlayerReadyDict.Set(player, true);
        Debug.Log($"{player} �� ���� �Ϸ�");

        if (AllPlayersReady())
        {
            Debug.Log("��� �÷��̾� ���� �Ϸ� �� ���� ����");
            LoadGameScene();
        }
    }
    // ��� �÷��̾ ���� �ߴ°�
    private bool AllPlayersReady()
    {
        foreach (var player in Runner.ActivePlayers)
        {
            if (!PlayerReadyDict.TryGet(player, out bool isReady) || !isReady)
                return false;
        }
        return true;
    }
}
