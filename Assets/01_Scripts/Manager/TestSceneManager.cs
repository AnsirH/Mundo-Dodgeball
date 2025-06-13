using UnityEngine;
using MyGame.Utils;
using Fusion;
using Fusion.Sockets;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;

public class TestSceneManager : MonoBehaviour, INetworkRunnerCallbacks
{
    [SerializeField] private Ground ground;
    [SerializeField] private NetworkPrefabRef playerPrefab;
    [SerializeField] private int testPlayerCount = 2;  // 테스트할 플레이어 수

    private PlayerInputHandler inputHandler;

    public void OnConnectedToServer(NetworkRunner runner)
    {
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
        var data = new NetworkInputData();

        data.buttons.Set(NetworkInputData.MOUSEBUTTON0, inputHandler.LeftClick);
        data.buttons.Set(NetworkInputData.MOUSEBUTTON1, inputHandler.RightClick);
        data.buttons.Set(NetworkInputData.BUTTONQ, inputHandler.ButtonQ);
        data.buttons.Set(NetworkInputData.BUTTOND, inputHandler.ButtonD);
        data.buttons.Set(NetworkInputData.BUTTONF, inputHandler.ButtonF);

        if (inputHandler.RightClick)
        {

            data.movePoint = GroundClick.GetMousePosition(Camera.main, LayerMask.GetMask("Ground"));
        }
        if (inputHandler.LeftClick)
            data.targetPoint = GroundClick.GetMousePosition(Camera.main, LayerMask.GetMask("Ground"));
        input.Set(data);
        inputHandler.ResetInputValue();
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
        if (_runner.IsServer)
        {
            SpawnTestPlayers(player);
        }
        Debug.Log("success player join");
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
        inputHandler = GetComponent<PlayerInputHandler>();
    }

    async private void StartGame(GameMode mode)
    {
        // Create the Fusion runner and let it know that we will be providing user input
        _runner = gameObject.AddComponent<NetworkRunner>();
        _runner.ProvideInput = true;

        // Create the NetworkSceneInfo from the current scene
        var scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex);
        var sceneInfo = new NetworkSceneInfo();
        if (scene.IsValid)
        {
            sceneInfo.AddSceneRef(scene, LoadSceneMode.Additive);
        }
        // Start or join (depends on gamemode) a session with a specific name
        await _runner.StartGame(new StartGameArgs()
        {
            GameMode = mode,
            SessionName = "TestRoom",
            Scene = scene,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
        });
    }

    private NetworkRunner _runner;

    private void SpawnTestPlayers(PlayerRef player)
    {
        for (int i = 0; i < testPlayerCount; i++)
        {
            NetworkObject spawned_player = _runner.Spawn(playerPrefab, position: ground.sections[i].position, rotation: Quaternion.identity, player);


            // 각 플레이어에 고유한 이름 할당
            spawned_player.name = $"TestPlayer_{i}";
        }
    }

    private void OnGUI()
    {
        if (_runner == null)
        {
            if (GUI.Button(new Rect(0, 0, 200, 40), "Host"))
            {
                StartGame(GameMode.Host);
            }
            if (GUI.Button(new Rect(0, 40, 200, 40), "Join"))
            {
                StartGame(GameMode.Client);
            }
        }
    }
}