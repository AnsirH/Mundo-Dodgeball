using System.Collections.Generic;
using UnityEngine;

public class HpBarHUD : MonoBehaviour
{
    [Header("References")]
    [SerializeField] GameObject _hpBarPrefab;

    private Dictionary<IPlayerContext, HpBar> spawnedHpBars = new();

    private float screenRatio = 1.0f;

    private void Awake()
    {
        screenRatio = 1080.0f / Screen.height;
    }

    public void UpdateHpBar(IPlayerContext playerContext)
    {
        Vector2 screenPoint = Camera.main.WorldToScreenPoint(playerContext.Movement.transform.position);
        screenPoint -= new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
        screenPoint.y += 150 * screenRatio;
        if (spawnedHpBars.TryGetValue(playerContext, out var hpBar))
        {
            hpBar.RectTransform.anchoredPosition = screenPoint;
            hpBar.UpdateHpBar(playerContext.Health.CurrentHealth / playerContext.Stats.GetMaxHealth());
        }
        else
        {
            HpBar newHpBar = CreateHpBar(playerContext);
            newHpBar.RectTransform.anchoredPosition = screenPoint;
            newHpBar.UpdateHpBar(playerContext.Health.CurrentHealth / playerContext.Stats.GetMaxHealth());
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
