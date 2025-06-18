using System.Collections.Generic;
using UnityEngine;

public class HpBarHUD : MonoBehaviour
{
    [Header("References")]
    [SerializeField] GameObject _hpBarPrefab;

    private Dictionary<IPlayerContext, HpBar> spawnedHpBars = new();

    private float screenRatio = 1.0f;
    [SerializeField] private RectTransform hpBarRootRect;
    private void Awake()
    {
        // �θ� ���� ��� �ڵ����� �θ��� RectTransform�� ������
        if (hpBarRootRect == null)
        {
            hpBarRootRect = transform.parent as RectTransform;

            if (hpBarRootRect == null)
            {
                Debug.LogError("[HpBarHUD] �θ� RectTransform�� �����ϴ�.");
            }
        }
        screenRatio = 1080.0f / Screen.height;
    }

    public void UpdateHpBar(IPlayerContext playerContext)
    {
        // �Ӹ� �� ��ġ ������
        float headOffset = 1.5f;

        // ���� ��ǥ ���
        Vector3 worldPosition = playerContext.Movement.transform.position + Vector3.up * headOffset;

        // ȭ�� ��ǥ (�ȼ� ����)
        Vector2 screenPos = Camera.main.WorldToScreenPoint(worldPosition);

        // Canvas ���� ���� ��ǥ�� ��ȯ
        Vector2 anchoredPos;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            hpBarRootRect,    // ü�¹� �θ� UI ���� (Canvas�� �ڽ�)
            screenPos,
            Camera.main,
            out anchoredPos))
        {
            // ü�¹� ��� or ����
            if (spawnedHpBars.TryGetValue(playerContext, out var hpBar))
            {
                hpBar.RectTransform.anchoredPosition = anchoredPos;
                hpBar.UpdateHpBar(playerContext.Health.CurrentHealth / playerContext.Stats.GetMaxHealth());
            }
            else
            {
                HpBar newHpBar = CreateHpBar(playerContext);
                newHpBar.RectTransform.anchoredPosition = anchoredPos;
                newHpBar.UpdateHpBar(playerContext.Health.CurrentHealth / playerContext.Stats.GetMaxHealth());
            }
        }
    }

    private HpBar CreateHpBar(IPlayerContext playerContext)
    {
        HpBar newHpBar = Instantiate(_hpBarPrefab, transform).GetComponent<HpBar>();
        spawnedHpBars.Add(playerContext, newHpBar);
        newHpBar.Init();
        return newHpBar;
    }
}
