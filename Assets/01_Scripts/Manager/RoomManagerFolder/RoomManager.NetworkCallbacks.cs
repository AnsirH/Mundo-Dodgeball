using Fusion;
using UnityEngine;
using System.Collections.Generic;
using Fusion.Sockets;
using System;
using UnityEditor.EditorTools;
// �� ����, ����, ����, �� ��ȯ ����
public partial class RoomManager : INetworkRunnerCallbacks
{

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log($"[Fusion] �÷��̾� ����: {player}");

        // Host ������ �ϴ� �ν��Ͻ������� ���� ó��
        if (runner.IsServer)
        {
            Vector3 spawnPos = Vector3.zero;

            runner.Spawn(playerPrefab, spawnPos, Quaternion.identity, player, (r, obj) =>
            {
                // �� ������ ������Ʈ�� NetworkPlayer ������Ʈ�� �پ� �ִ��� Ȯ��
                var netPlayer = obj.GetComponent<NetworkPlayer>();
                if (netPlayer == null)
                {
                    Debug.LogError("[Spawn] NetworkPlayer ������Ʈ�� �����ϴ�!");
                    return;
                }

                // �� �г��� ����
                int randomValue = UnityEngine.Random.Range(1, 101);
                //netPlayer.NickName = PlayerPrefs.GetString("NickName", "Player_" + randomValue);
                //Debug.Log($"[Spawn] NickName set to {netPlayer.NickName.Value}");

                // �� ������ ������ ���� �� �ٷ� UI ����
                UpdateLobbyUI();
            });
        }
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log($"[Fusion] �÷��̾� ����: {player}");
        _nicknames.Remove(player);
        UpdateLobbyUI();
    }

    public void OnSceneLoadStart(NetworkRunner runner) => Debug.Log("[Fusion] �� �ε� ����");
    
    public void OnSceneLoadDone(NetworkRunner runner) // �� �ε尡 �Ϸ�Ǹ� ȣ��
    {
        IngameController[] controllers = FindObjectsOfType<IngameController>();
        foreach (var controller in controllers)
        {
            runner.AddCallbacks(controller);
        }
        Debug.Log($"@user message@ Completed AddCallbacks for {controllers.Length} InGameControllers");
    }
    public void OnConnectedToServer(NetworkRunner runner) => Debug.Log("RoomManager : [Fusion] ���� ���� ����!!!!!!!!!!!!!!!!!!!!");
    public void OnDisconnectedFromServer(NetworkRunner runner) => Debug.Log("RoomManager : [Fusion] ���� ���� ����!!!!!!!!!!!!!!!!!!!!");

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
        // UI ���� ��� �÷��̾� ȣ��Ǵ� �κ���.
    }
}