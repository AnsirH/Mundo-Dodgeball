using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fusion;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.SceneManagement;

public struct PlayerScoreEntry : INetworkStruct
{
    public PlayerRef Player;
    public int Score;
}

public class MatchManager : NetworkBehaviour
{
    // Fusion에서 Dictionary는 직접 Networked로 사용할 수 없으므로 구조체 배열로 대체
    [Networked, Capacity(2)]
    public NetworkLinkedList<PlayerScoreEntry> PlayerScores 
    {
        get;
    }
    public static event System.Action<MatchManager> OnSpawned;
    [Networked] public int Round { get; set; }
    public override void Spawned()
    {
        ServerManager.Instance.matchManager = this;
        Debug.Log("[MatchManager] 서버에서 matchManager 등록 완료");
        OnSpawned?.Invoke(this);
    }
    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        Debug.Log("[MatchManager] Despawned! 씬 이동 중 객체가 사라졌습니다.");
        ServerManager.Instance.roomController.PlayerScores.Clear();
        foreach (PlayerScoreEntry entry in PlayerScores) 
        {
            ServerManager.Instance.roomController.PlayerScores.Add(entry);
        }
        ServerManager.Instance.roomController.Round = Round;
    }
    public void Init(List<PlayerRef> players)
    {
        if (!HasStateAuthority) return;
        if (players.Count == 0) return;
        PlayerScores.Clear();
        if (ServerManager.Instance.roomController.Round > 0)
        {
            if(ServerManager.Instance.roomController.runner.IsServer)
                RPC_RequestInitScore();
        }
        else
        {
            Debug.Log("[MatchManager] 기존데이터가 없어서 생성합니다.");
            foreach (var player in players)
            {
                PlayerScores.Add(new PlayerScoreEntry
                {
                    Player = player,
                    Score = 0
                });
            }
        }
        Debug.Log("[MatchManager] 초기화 완료: 플레이어 점수 설정");
    }
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_RequestInitScore()
    {
        Debug.Log("[MatchManager] 기존데이터가 있어서 받아왔습니다." + ServerManager.Instance.roomController.PlayerScores.Count);
        foreach (var player in ServerManager.Instance.roomController.PlayerScores)
        {
            Debug.Log(player.Score);
            PlayerScores.Add(player);
        }
        Round = ServerManager.Instance.roomController.Round;
        (int host, int client) = (PlayerScores[0].Score, PlayerScores[1].Score);
        Debug.Log("daaaaahost :" + host + "dafafd client: " + client);
        RPC_UpdateScoreUI(host, client);
    }
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_RequestAddScore(PlayerRef player, int score = 1)
    {
        AddScore(player, score); // ✅ 서버에서만 실행되므로 안전
    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_UpdateScoreUI(int host, int client)
    {
        IngameController.Instance.ingameUIController.Init((host, client));
    }
    public void AddScore(PlayerRef player, int score = 1)
    {
        if (player.IsNone)
        {
            player = ServerManager.Instance.roomController.runner.ActivePlayers.ToList().First();
        }
        if (!HasStateAuthority) return; // ✅ 서버에서만 실행됨

        for (int i = 0; i < PlayerScores.Count; i++)
        {
            var entry = PlayerScores[i];
            if (entry.Player == player)
            {
                entry.Score += score;
                PlayerScores.Set(i, entry);
                Debug.Log($"[MatchManager] {player} 점수: {entry.Score}");

                var players = ServerManager.Instance.roomController.runner.ActivePlayers.OrderBy(p => p.PlayerId).ToList();
                int idx = players[0] == player ? 0 : 1;
                Debug.Log($"GameModePlayerCount : {players.Count}");
                Round++;
                RPC_UpdateScoreUI(idx);
                break;
            }
        }
    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_UpdateScoreUI(int idx)
    {
        IngameController.Instance.ingameUIController.addScore(idx);
        Invoke(nameof(NextRound), 0.7f);
    }
    public void NextRound()
    {
        if(ServerManager.Instance.roomController.runner.IsServer)
        {
            ServerManager.Instance.roomController.runner.LoadScene(SceneRef.FromIndex(1), LoadSceneMode.Single);
        }
    }
    public PlayerRef GetWinner()
    {
        if (PlayerScores.Count < 2) return PlayerRef.None;

        var first = PlayerScores[0];
        var second = PlayerScores[1];

        return first.Score > second.Score ? first.Player : second.Player;
    }
}
