// RoomManager.RoomOperations.cs
using System;
using System.Text;
using System.Threading.Tasks;
using Fusion;
using UnityEngine;
using UnityEngine.SceneManagement;

// Fusino 콜백 처리
public partial class RoomManager
{
    public void Awake()
    {
        //runnerPrefab = this.gameObject.AddComponent<NetworkRunner>();
        Debug.Log("add : runnerPrefab");
    }
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

    // 모두 레디 누가 했는지 받기
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_Ready(PlayerRef player)
    {
        if (!PlayerReadyDict.ContainsKey(player))
            PlayerReadyDict.Add(player, true);
        else
            PlayerReadyDict.Set(player, true);
        Debug.Log($"{player} → 레디 완료");

        if (AllPlayersReady())
        {
            Debug.Log("모든 플레이어 레디 완료 → 게임 시작");
            LoadGameScene();
        }
    }
    // 모든 플레이어가 레디 했는가
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
