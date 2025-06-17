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
        // �ʱ�ȭ�� ������ ���� ü�¹� Ȱ��ȭ
        if (isInitialized)
        {
            foreach (HpBar hpBar in HpBars)
            {
                hpBar.UpdateLocate();
                hpBar.UpdateDisplay();
            }
        }
    }

    /// <summary> UI �Ŵ��� �ʱ�ȭ. </summary>
    public void Init((int masterScore, int otherScore)score)
    {
        // �÷��̾� �� �������� from IngameController
        int playerCount = IngameController.Instance.PlayerCharacters.Count;

        //if (playerCount > 0)
        //{
        //    // HpBar �迭 ����
        //    HpBars = new HpBar[playerCount];

        //    // HpBar ���� �� �Ҵ�
        //    // HpBar�� �÷��̾ �����Ͽ� �ʱ�ȭ
        //    for (int i = 0; i < playerCount; ++i)
        //    {
        //        HpBars[i] = Instantiate(hpBarPrefab, transform).GetComponent<HpBar>();
        //        HpBars[i].Init(IngameController.Instance.Players[i]);
        //    }

        //    // �ʱ�ȭ �� ���·� ����
        //    isInitialized = true;
        //}
        Debug.Log($"���ھ� ���� ȣ��Ʈ :  {score.masterScore} Ŭ���̾�Ʈ : {score.otherScore}");
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
