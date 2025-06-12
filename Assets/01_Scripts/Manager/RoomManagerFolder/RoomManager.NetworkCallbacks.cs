using Fusion;
using UnityEngine;
using System.Collections.Generic;
using Fusion.Sockets;
using System;
using UnityEditor.EditorTools;
// 방 생성, 참가, 퇴장, 씬 전환 로직
public partial class RoomManager : INetworkRunnerCallbacks
{

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log($"[Fusion] 플레이어 입장: {player}");

        // Host 역할을 하는 인스턴스에서만 스폰 처리
        if (runner.IsServer)
        {
            Vector3 spawnPos = Vector3.zero;

            runner.Spawn(playerPrefab, spawnPos, Quaternion.identity, player, (r, obj) =>
            {
                // ① 스폰된 오브젝트에 NetworkPlayer 컴포넌트가 붙어 있는지 확인
                var netPlayer = obj.GetComponent<NetworkPlayer>();
                if (netPlayer == null)
                {
                    Debug.LogError("[Spawn] NetworkPlayer 컴포넌트가 없습니다!");
                    return;
                }

                // ② 닉네임 세팅
                int randomValue = UnityEngine.Random.Range(1, 101);
                //netPlayer.NickName = PlayerPrefs.GetString("NickName", "Player_" + randomValue);
                //Debug.Log($"[Spawn] NickName set to {netPlayer.NickName.Value}");

                // ③ 스폰이 완전히 끝난 뒤 바로 UI 갱신
                UpdateLobbyUI();
            });
        }
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log($"[Fusion] 플레이어 퇴장: {player}");
        _nicknames.Remove(player);
        UpdateLobbyUI();
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
    public void OnConnectedToServer(NetworkRunner runner) => Debug.Log("RoomManager : [Fusion] 서버 연결 성공!!!!!!!!!!!!!!!!!!!!");
    public void OnDisconnectedFromServer(NetworkRunner runner) => Debug.Log("RoomManager : [Fusion] 서버 연결 끊김!!!!!!!!!!!!!!!!!!!!");

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
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) 
    {
        Debug.Log("update Session!");
        PopManager.instance.gameSelectPop.regularGamePop.SetRoomListSlot(sessionList);
    }
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