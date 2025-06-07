using UnityEngine;
using MyGame.Utils;
using Fusion;
using Fusion.Sockets;
using System.Collections.Generic;
using System;

public class TestSceneManager : MonoBehaviour, INetworkRunnerCallbacks
{
    [SerializeField] private Ground ground;
    [SerializeField] private NetworkObject playerPrefab;
    [SerializeField] private int testPlayerCount = 2;  // 테스트할 플레이어 수

    new public void OnConnectedToServer(NetworkRunner runner)
    {
        SpawnTestPlayers();
        Debug.Log("connect success!");
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
    }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
    }

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
    }

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
    {
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
    {
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
    }

    private void Awake()
    {
        StartGame(GameMode.Host);
    }

    async private void StartGame(GameMode mode)
    {
        // Create the Fusion runner and let it know that we will be providing user input
        _runner = gameObject.AddComponent<NetworkRunner>();
        _runner.ProvideInput = true;

        _runner.AddCallbacks(this);
        // Start or join (depends on gamemode ) a session with a specific name
        await _runner.StartGame(new StartGameArgs()
        {
            GameMode = mode,
            SessionName = "TestRoom"
        });
    }

    private NetworkRunner _runner;

    private void SpawnTestPlayers()
    {
        Debug.Log("spawn function is called!");
        for (int i = 0; i < testPlayerCount; i++)
        {
            NetworkObject player = _runner.Spawn(playerPrefab, position: ground.sections[i].position, rotation: Quaternion.identity);
            

            // 각 플레이어에 고유한 이름 할당
            player.name = $"TestPlayer_{i}";
            player.GetComponent<IPlayerContext>().InitGround(i);
        }
    }
}