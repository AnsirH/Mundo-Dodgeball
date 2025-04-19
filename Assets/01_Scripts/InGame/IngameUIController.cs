using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class IngameUIController : MonoBehaviour
{
    [SerializeField] TopPanelUI topPanelUI;
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
    public void Init()
    {
        // �÷��̾� �� �������� from IngameController
        int playerCount = IngameController.Instance.playerControllers.Length;

        if (playerCount > 0)
        {
            // HpBar �迭 ����
            HpBars = new HpBar[playerCount];

            // HpBar ���� �� �Ҵ�
            // HpBar�� �÷��̾ �����Ͽ� �ʱ�ȭ
            for (int i = 0; i < playerCount; ++i)
            {
                HpBars[i] = Instantiate(hpBarPrefab, transform).GetComponent<HpBar>();
                HpBars[i].Init(IngameController.Instance.playerControllers[i].Health);
            }

            // �ʱ�ȭ �� ���·� ����
            isInitialized = true;
        }
    }

    public HpBar[] HpBars { get; private set; }
    public GameObject hpBarPrefab;

    public bool isInitialized = false;
}
