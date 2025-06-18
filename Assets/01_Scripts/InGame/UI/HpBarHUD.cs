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
        // 부모가 있을 경우 자동으로 부모의 RectTransform을 가져옴
        if (hpBarRootRect == null)
        {
            hpBarRootRect = transform.parent as RectTransform;

            if (hpBarRootRect == null)
            {
                Debug.LogError("[HpBarHUD] 부모에 RectTransform이 없습니다.");
            }
        }
        screenRatio = 1080.0f / Screen.height;
    }

    public void UpdateHpBar(IPlayerContext playerContext)
    {
        // 머리 위 위치 오프셋
        float headOffset = 1.5f;

        // 월드 좌표 계산
        Vector3 worldPosition = playerContext.Movement.transform.position + Vector3.up * headOffset;

        // 화면 좌표 (픽셀 단위)
        Vector2 screenPos = Camera.main.WorldToScreenPoint(worldPosition);

        // Canvas 상의 로컬 좌표로 변환
        Vector2 anchoredPos;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            hpBarRootRect,    // 체력바 부모 UI 영역 (Canvas의 자식)
            screenPos,
            Camera.main,
            out anchoredPos))
        {
            // 체력바 등록 or 갱신
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
