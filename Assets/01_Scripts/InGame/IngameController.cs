using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Fusion;
using Fusion.Sockets;
using System;
using Cysharp.Threading.Tasks;
using System.Threading.Tasks;
using MyGame.Utils;


public class IngameController : NetworkBehaviour, INetworkRunnerCallbacks
{
    private PlayerInputHandler inputHandler;

    [SerializeField] private NetworkPrefabRef characterPrefab;

    private Dictionary<PlayerRef, NetworkObject> spawnedCharacters = new();
    private HashSet<PlayerRef> playersCompletedSpawn = new();

    private List<IPlayerContext> playerControllers = new();

    public int ExpectedPlayerCount = 2;

    public static IngameController Instance { get; private set; }

    public List<IPlayerContext> Players => playerControllers;
    
    public int GetPlayerIndex(IPlayerContext playerContext)
    {
        return playerControllers.IndexOf(playerContext);
    }

    private void Awake()
    {
        // �̹� �����ϴ� �ν��Ͻ��� �ְ�, �װ��� �ڽ�(this)�� �ƴ϶��
        // �ߺ� �ν��Ͻ��� ����
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            // �ڽ��� �ν��Ͻ��� ���
            Instance = this;
        }


        //if (FindFirstObjectByType<ObjectPooler>() == null)
        //{
        //    Debug.LogError("There is no ObjectPooler in scene");
        //    return;
        //}

        //UIManager.instance.ChangeGame(false);


        //if (Object.HasStateAuthority)
        //{
        //    _ = StartGameProcessAsync();
        //}
    }

    public override void Spawned()
    {
    }

    private async Task StartGameProcessAsync()
    {
        // 1. ��� �÷��̾� ���� ���
        await WaitForAllPlayersConnected();


        // 2. ĳ���� ���� ��û
        foreach (var player in Runner.ActivePlayers)
        {
            RPC_CreatePlayerCharacter(player);
        }

        await WaitForAllPlayersCharacterSpawned();

        SoundManager.instance.SetPlayerAudioGroup(playerControllers);

        StartGame();
    }

    private async Task WaitForAllPlayersConnected()
    {
        while (Runner.ActivePlayers.Count() < ExpectedPlayerCount)
        {
            await Task.Yield();
        }
    }

    private async Task WaitForAllPlayersCharacterSpawned()
    {
        while (playersCompletedSpawn.Count < ExpectedPlayerCount)
        {
            await Task.Yield();
        }
        playerControllers = spawnedCharacters.Values
        .Select(no => no.GetComponent<IPlayerContext>())
        .Where(pc => pc != null)
        .ToList();
    }

    // ĳ���� ���� RPC (StateAuthority �� InputAuthority)
    [Rpc(sources: RpcSources.StateAuthority, targets: RpcTargets.InputAuthority)]
    private void RPC_CreatePlayerCharacter([RpcTarget] PlayerRef targetPlayer, RpcInfo info = default)
    {
        if (Runner.LocalPlayer != targetPlayer) return;

        Transform spawnPoint = GetSpawnPoint(Runner.LocalPlayer);

        var character = Runner.Spawn(characterPrefab, spawnPoint.position, spawnPoint.rotation, Runner.LocalPlayer);

        // ���� �Ϸ� �˸�
        RPC_NotifyCharacterSpawned(Runner.LocalPlayer, character);
    }

    // ���� �Ϸ� �˸� RPC (InputAuthority �� StateAuthority)
    [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.StateAuthority)]
    private void RPC_NotifyCharacterSpawned(PlayerRef player, NetworkObject character)
    {
        if (!playersCompletedSpawn.Contains(player))
        {
            playersCompletedSpawn.Add(player);
            spawnedCharacters[player] = character;
        }
    }

    private Transform GetSpawnPoint(PlayerRef playerRef)
    {
        int index = playerRef.PlayerId;
        return playerSpawnPoints[index];
    }

    /// <summary>
    /// ���� ����
    /// </summary>
    public void StartGame()
    {
        foreach (var player in spawnedCharacters.Values)
        {
            player.GetComponent<PlayerController>().enabled = true;
        }
        //ingameUIController.Init(ServerManager.Instance.roomManager.GetScore());
        //ingameUIController.OnRoundPanel(ServerManager.Instance.roomManager.GetCurrentRound());
    }
    void ReloadSceneRPC()
    {
        // Photon ���� ������ ä ���� �� ���ε�
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
    }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
    {
    }

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
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

    public void OnConnectedToServer(NetworkRunner runner)
    {
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
    }

    /// <summary>�ΰ��� UI ��Ʈ�ѷ�</summary>
    public IngameUIController ingameUIController;

    public Transform[] playerSpawnPoints = new Transform[2];

    public Ground ground;
}