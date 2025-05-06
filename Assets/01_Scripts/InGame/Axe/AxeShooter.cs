using MyGame.Utils;
using Photon.Pun;
using System;
using UnityEngine;
using UnityEngine.UIElements;

public interface IShooter
{
    void Initialize(IPlayerContext context, bool isOfflineMode = false);
    void ActivateRange(bool isActive);
    void SpawnProjectile(Vector3 startPos, Vector3 direction, float execTime);
    bool IsRangeActive { get; }
    bool CanShoot { get; }
}

// IRangeIndicator.cs
public interface IRangeIndicator
{
    void Show();
    void Hide();
    void UpdatePosition(Vector3 position, float distance);
    bool IsActive { get; }
}

// IProjectile.cs
public interface IProjectile
{
    void Initialize(IPlayerContext context, float damage, Vector3 spawnPos, Vector3 direction, float execTime);
    void OnHit(Collider other);
}

public class AxeShooter : MonoBehaviour, IShooter
{
    private bool isOfflineMode = false;
    [Header("임시 변수")]
    [SerializeField] private float attackPower = 80.0f;
    [SerializeField] private float cooldownTime = 2.0f;

    [Header("References")]
    [SerializeField] private GameObject axePrefab;
    [SerializeField] private GameObject rangeIndicatorObj;

    private IPlayerContext context;
    private IRangeIndicator rangeIndicator;
    private bool isRangeActive;
    private float currentCooldown = 0;

    public bool IsRangeActive => isRangeActive;
    public bool CanShoot => !IsOnCooldown;  // 쿨타임 중이 아닐 때만 공격 가능
    private bool IsOnCooldown => currentCooldown > 0f;

    private void Awake()
    {
        rangeIndicator = rangeIndicatorObj.GetComponent<IRangeIndicator>();
    }

    private void Update()
    {
        if (currentCooldown > 0f)
        {
            currentCooldown -= Time.deltaTime;
        }

        if (IsRangeActive)
        {
            Vector3? mousePoint = context.MousePositionGetter.GetMousePosition();
            if (mousePoint.HasValue)
                rangeIndicator.UpdatePosition(mousePoint.Value, 10.0f);
        }
    }

    public void Initialize(IPlayerContext context, bool isOfflineMode=false)
    {
        this.context = context;
        this.isOfflineMode = isOfflineMode;
    }

    // 거리 표시 활성화
    public void ActivateRange(bool isActive)
    {
        isRangeActive = isActive;
        if (isActive)
            rangeIndicator.Show();
        else
            rangeIndicator.Hide();
    }

    // 쿨타임 관련 추가 기능들 (AxeShooter 고유 기능)
    public void ReduceCooldown(float amount)
    {
        currentCooldown = Mathf.Max(0f, currentCooldown - amount);
    }

    public float GetCooldownProgress()
    {
        return currentCooldown / cooldownTime;
    }

    public void SpawnProjectile(Vector3 startPos, Vector3 direction, float execTime)
    {
        if (!CanShoot) return;

        direction.y = 0.0f;

        GameObject axeObj = ObjectPooler.Get("Axe");

        IProjectile axe = axeObj.GetComponent<IProjectile>();
        axe.Initialize(context, attackPower, transform.position, direction, execTime);

        // 쿨타임 시작
        currentCooldown = cooldownTime;
    }
}
