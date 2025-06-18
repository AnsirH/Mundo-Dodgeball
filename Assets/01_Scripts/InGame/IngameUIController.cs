using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Fusion;
using UnityEngine;

public class IngameUIController : MonoBehaviour
{
    [SerializeField] TopPanelUI topPanelUI;
    [SerializeField] InGameResultUI resultUI;
    public InGameSettingPop InGameSettingPop;
    public UserHUD hud;
    public HpBarHUD hpBarHud;
    private void Start()
    {
        topPanelUI.StartTimer(2, 0);
    }

    private void Update()
    {
        hud.UpdateHud();
    }

    private void LateUpdate()
    {
        foreach (IPlayerContext player in IngameController.Instance.PlayerCharacters.Values)
        {
            hpBarHud.UpdateHpBar(player);
        }
    }

    public void Init_new(IPlayerContext playerContext)
    {
        hud.Init(playerContext);
    }

    /// <summary> UI 매니저 초기화. </summary>
    public void Init((int masterScore, int otherScore)score)
    {
        topPanelUI.InitScore(score.masterScore, score.otherScore);
    }
    public void addScore(int idx)
    {
        topPanelUI.AddScoreToPlayer(idx);
    }

    public void OnRoundPanel(int idx) 
    {
        resultUI.RoundAnimPlay(idx);
    }
    public void OnEndGameResult(bool iswin)
    {
        resultUI.ShowResultAnimPlay(iswin);
    }
    public void TestAddScore()
    {
        var playerRef = ServerManager.Instance.roomController.runner.LocalPlayer;
        ServerManager.Instance.matchManager.RPC_RequestAddScore(playerRef, 1);
        //if (ServerManager.Instance.roomController.runner.IsServer)
        //{
        //    ServerManager.Instance.matchManager.RPC_RequestAddScore(PlayerRef.None, 1);
        //}
    }
}
