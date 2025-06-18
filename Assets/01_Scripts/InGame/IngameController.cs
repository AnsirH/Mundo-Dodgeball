using Fusion;
using Fusion.Sockets;
using Mundo_dodgeball.Player.StateMachine;
using MyGame.Utils;
using PlayerCharacterControl.Camera;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;


public class IngameController : NetworkBehaviour, INetworkRunnerCallbacks
{
    public static IngameController Instance { get; private set; }
    [Header("References")]
    [SerializeField] private NetworkPrefabRef characterPrefab;
    [SerializeField] private PlayerCamMovement playerCam;

    private PlayerInputHandler inputHandler;

    public int ExpectedPlayerCount = 2;

    private Dictionary<PlayerRef, IPlayerContext> spawnedCharacters = new();
    private HashSet<PlayerRef> playersCompletedSpawn = new();

    public Dictionary<PlayerRef, IPlayerContext> PlayerCharacters => spawnedCharacters;

    public Ground Ground { get; private set; }

    /// <summary>인게임 UI 컨트롤러</summary>
    public IngameUIController ingameUIController;

    public Transform[] playerSpawnPoints = new Transform[2];

    private void Awake()
    {
        // 이미 존재하는 인스턴스가 있고, 그것이 자신(this)이 아니라면
        // 중복 인스턴스를 제거
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            // 자신의 인스턴스를 기록
            Instance = this;
        }

        inputHandler = GetComponent<PlayerInputHandler>();

        Ground = FindFirstObjectByType<Ground>();

        if (playerCam == null) playerCam = FindFirstObjectByType<PlayerCamMovement>();

        //UIManager.instance.ChangeGame(false);
    }

    public override void Spawned()
    {
        if (Object.HasStateAuthority)
        {
            _ = StartGameProcessAsync();
        }
    }

    private async Task StartGameProcessAsync()
    {
        // 1. 모든 플레이어 접속 대기
        await WaitForAllPlayersConnected();


        // 2. 캐릭터 생성 요청
        foreach (var player in Runner.ActivePlayers)
        {
            CreatePlayerCharacter(player);
        }

        await WaitForAllPlayersCharacterSpawned();

        //SoundManager.instance.SetPlayerAudioGroup(playerControllers);

        StartGame();
    }

    private async Task WaitForAllPlayersConnected()
    {
        while (Runner.ActivePlayers.Count() < ExpectedPlayerCount)
        {
            await Task.Yield();
        }
    }

    private void CreatePlayerCharacter(PlayerRef targetPlayer)
    {
        Transform spawnPoint = GetSpawnPoint(targetPlayer);

        var character = Runner.Spawn(characterPrefab, spawnPoint.position, spawnPoint.rotation, targetPlayer);

        // 생성 완료 알림
        RPC_NotifyCharacterSpawned(targetPlayer, character);
    }

    // 생성 완료 알림 RPC (StateAuthority → InputAuthority)
    [Rpc(sources: RpcSources.StateAuthority, targets: RpcTargets.All)]
    private void RPC_NotifyCharacterSpawned(PlayerRef player, NetworkObject character)
    {
        if (!playersCompletedSpawn.Contains(player))
        {
            playersCompletedSpawn.Add(player);
            spawnedCharacters[player] = character.GetComponent<IPlayerContext>();
        }
    }

    private async Task WaitForAllPlayersCharacterSpawned()
    {
        while (playersCompletedSpawn.Count < Runner.ActivePlayers.Count())
        {
            await Task.Yield();
        }

        while (true)
        {
            bool allPlayerIsSpawned = true;
            foreach (var player in playersCompletedSpawn)
            {
                if (player.IsUnityNull())
                {
                    allPlayerIsSpawned = false;
                    break;
                }                
            }

            if (allPlayerIsSpawned)
                break;

            await Task.Yield();
        }
    }

    private Transform GetSpawnPoint(PlayerRef playerRef)
    {
        int index = playerRef.PlayerId - 1;
        return Ground.sections[index];
    }

    /// <summary>
    /// 게임 시작
    /// </summary>
    public void StartGame()
    {
        StartGame_RPC();
        //ingameUIController.Init(ServerManager.Instance.roomManager.GetScore());
        //ingameUIController.OnRoundPanel(ServerManager.Instance.roomManager.GetCurrentRound());
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void StartGame_RPC()
    {
        Runner.AddCallbacks(this);

        ingameUIController.gameObject.SetActive(true);
        ingameUIController.Init_new(PlayerCharacters[Runner.LocalPlayer]);

        foreach (var player in spawnedCharacters.Values)
        {
            player.Movement.GetComponent<PlayerController>().enabled = true;
        }

        playerCam.SetCameraPosition(PlayerCharacters[Runner.LocalPlayer].Movement.transform.position);
    }

    void ReloadSceneRPC()
    {
        // Photon 연결 유지한 채 현재 씬 리로드
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public bool CheckPlayerDie(out PlayerRef playerRef)
    {
        playerRef = PlayerRef.None;
        foreach (var player in spawnedCharacters.Values)
        {
            if (player.CurrentState is PlayerDieState)
            {
                playerRef = player.Runner.LocalPlayer;
                return true;
            }
        }
        return false;
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
        data.buttons.Set(NetworkInputData.BUTTONSTOP, inputHandler.ButtonStopMove);

        if (inputHandler.RightClick)
        {
            data.movePoint = GroundClick.GetMousePosition(Camera.main, LayerMask.GetMask("Ground"));
        }
        if (inputHandler.LeftClick || inputHandler.ButtonD || inputHandler.ButtonF)
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

    //public void OnGUI()
    //{
    //    if (Runner == null) return;
    //    if (HasStateAuthority)
    //    {
    //        if (spawnedCharacters.Count > 0) return;
    //        if (GUI.Button(new Rect(500, 500, 200, 100), "Start Game"))
    //        {
    //            _ = StartGameProcessAsync();
    //        }
    //    }
    //    foreach (var player in Runner.ActivePlayers)
    //    {
    //        GUI.Label(new Rect(500, player.PlayerId * 100, 500, 100), player.PlayerId + " player is in");
    //    }
    //}
}