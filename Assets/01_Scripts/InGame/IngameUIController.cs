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
    private void Start()
    {
        topPanelUI.StartTimer(2, 0);
    }
    private void Update()
    {
        // 초기화를 해줬을 때만 체력바 활성화
        if (isInitialized)
        {
            foreach (HpBar hpBar in HpBars)
            {
                hpBar.UpdateLocate();
                hpBar.UpdateDisplay();
            }
        }
    }

    /// <summary> UI 매니저 초기화. </summary>
    public void Init((int masterScore, int otherScore)score)
    {
        // 플레이어 수 가져오기 from IngameController
        int playerCount = IngameController.Instance.PlayerCharacters.Count;

        //if (playerCount > 0)
        //{
        //    // HpBar 배열 생성
        //    HpBars = new HpBar[playerCount];

        //    // HpBar 생성 및 할당
        //    // HpBar에 플레이어를 지정하여 초기화
        //    for (int i = 0; i < playerCount; ++i)
        //    {
        //        HpBars[i] = Instantiate(hpBarPrefab, transform).GetComponent<HpBar>();
        //        HpBars[i].Init(IngameController.Instance.Players[i]);
        //    }

        //    // 초기화 된 상태로 설정
        //    isInitialized = true;
        //}
        Debug.Log($"스코어 연동 호스트 :  {score.masterScore} 클라이언트 : {score.otherScore}");
        topPanelUI.InitScore(score.masterScore, score.otherScore);
    }
    public void addScore(int idx)
    {
        topPanelUI.AddScoreToPlayer(idx);
    }

    public HpBar[] HpBars { get; private set; }
    public GameObject hpBarPrefab;

    public bool isInitialized = false;

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
