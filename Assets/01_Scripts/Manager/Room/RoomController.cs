// ✅ FusionRoomController.cs
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

public class RoomController : MonoBehaviour, INetworkRunnerCallbacks
{
    [Header("References")]
    [SerializeField] private RoomView view;

    [SerializeField] private RoomModel model;
    private NetworkRunner runner;
    private const string lobbyName = "default_lobby";
    private Dictionary<string, int> GameModePlayerCount = new Dictionary<string, int>{
        { "GeneralGameMode", 2 }
    };
    private const int REQUIRED_PLAYERS = 2;

    private void Awake()
    {
        view.OnCreateRoomRequested += CreateOrJoinRoom;
        view.OnLeaveRoomRequested += LeaveRoom;
    }

    private async void Start()
    {
        await InitRunner();
    }

    private async Task InitRunner()
    {
        runner = gameObject.AddComponent<NetworkRunner>();
        runner.ProvideInput = false;
        runner.AddCallbacks(this);

        await runner.StartGame(new StartGameArgs
        {
            GameMode = GameMode.Shared,
            SessionName = "LOBBY",
            Scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex),
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
        });
    }

    private async void CreateOrJoinRoom(string roomName, string playerConut = "GeneralGameMode", string password = "")
    {
        if (runner == null) return;

        var properties = new Dictionary<string, SessionProperty>
    {
        { "pwd", password }
    };

        await runner.StartGame(new StartGameArgs
        {
            GameMode = GameMode.AutoHostOrClient,
            SessionName = roomName,
            PlayerCount = GameModePlayerCount[playerConut],
            SceneManager = GetComponent<NetworkSceneManagerDefault>(),
            SessionProperties = properties,
            IsVisible = true,
            IsOpen = true
        });
    }


    private async void LeaveRoom()
    {
        if (runner != null)
        {
            await runner.Shutdown();
            model.ClearSessionInfo();
            view.UpdateCurrentRoomText("");
            view.UpdatePlayerCount(0);
            await InitRunner();
        }
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log($"플레이어 {player.PlayerId} 입장");
        int currentCount = runner.ActivePlayers.Count();


        view.UpdatePlayerCount(currentCount);
        TryStartGameIfReady(currentCount);
    }

    private async void TryStartGameIfReady(int playerCount)
    {
        if (playerCount >= REQUIRED_PLAYERS)
        {
            Debug.Log("인원 충족! 인게임 씬으로 전환");
            await runner.StartGame(new StartGameArgs
            {
                GameMode = GameMode.AutoHostOrClient,
                SessionName = model.GetRoomName(),
                Scene = SceneRef.FromIndex(1),
                SceneManager = GetComponent<NetworkSceneManagerDefault>()
            });
        }
    }

    // ✅ 주요 콜백 요약
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { Debug.Log($"런너 종료됨: {shutdownReason}"); }
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) { Debug.Log($"플레이어 {player.PlayerId} 퇴장"); }
    public void OnSceneLoadDone(NetworkRunner runner) { Debug.Log("씬 로딩 완료"); }
    public void OnSceneLoadStart(NetworkRunner runner) { Debug.Log("씬 로딩 시작"); }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) 
    {
        Debug.Log("세션 리스트 갱신"); 
        if (model.CurrentSession == null)
        {
            foreach (var session in sessionList)
            {
                if (session.Name == model.GetRoomName()) // 이름 매칭
                {
                    model.SetSessionInfo(session);
                    view.UpdatePlayerCount(session.PlayerCount);
                    break;
                }
            }
            view.UpdateCurrentRoomText(model.GetRoomName());
        }
    }

    // 사용하지 않음 (빈 정의)
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnInput(NetworkRunner runner, NetworkInput input) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, System.ArraySegment<byte> data) { }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
    }

    public void OnConnectedToServer(NetworkRunner runner)
    {
    }
}
