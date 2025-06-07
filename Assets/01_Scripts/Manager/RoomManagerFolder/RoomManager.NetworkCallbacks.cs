using Fusion;
using UnityEngine;
using System.Collections.Generic;
using Fusion.Sockets;
using System;
// 방 생성, 참가, 퇴장, 씬 전환 로직
public partial class RoomManager : INetworkRunnerCallbacks
{

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log($"[Fusion] 플레이어 입장: {player}");
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log($"[Fusion] 플레이어 퇴장: {player}");
    }

    public void OnSceneLoadStart(NetworkRunner runner) => Debug.Log("[Fusion] 씬 로딩 시작");
    
    public void OnSceneLoadDone(NetworkRunner runner) // 씬 로드가 완료되면 호출
    {
        IngameController[] controllers = FindObjectsOfType<IngameController>();
        foreach (var controller in controllers)
        {
            runner.AddCallbacks(controller);
        }
        Debug.Log($"@user message@ Completed AddCallbacks for {controllers.Length} InGameControllers");
    }
    public void OnConnectedToServer(NetworkRunner runner) => Debug.Log("[Fusion] 서버에 연결됨");
    public void OnDisconnectedFromServer(NetworkRunner runner) => Debug.Log("[Fusion] 서버 연결 해제됨");

    public void OnInput(NetworkRunner runner, NetworkInput input) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr messagePtr) 
    {
    }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, System.ArraySegment<byte> data) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    public void OnPlayerRefAssigned(NetworkRunner runner, PlayerRef playerRef) { }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
    public void OnChanged_PlayerReadyDict()
    {
        // UI 대응 모든 플레이어 호출되는 부분임.
    }
}