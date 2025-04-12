using MyGame.Utils;
using Photon.Pun;
using UnityEngine;

public interface IShooter
{
    void Initialize(IPlayerContext context);
    void ActivateRange(bool isActive);
    void SpawnProjectile();
    bool IsRangeActive { get; }
    bool CanShoot { get; }
}

// IRangeIndicator.cs
public interface IRangeIndicator
{
    void Show();
    void Hide();
    void UpdatePosition(Vector3 position);
    bool IsActive { get; }
}

// IProjectile.cs
public interface IProjectile
{
    void Initialize(IPlayerContext context, float damage);
    void Launch(Vector3 direction);
    void OnHit(Collider other);
}

public class AxeShooter : MonoBehaviourPun, IShooter
{
    [SerializeField] private float attackPower = 80.0f;
    [SerializeField] private float cooldownTime = 2.0f;
    [SerializeField] private GameObject axePrefab;

    private IPlayerContext context;
    private IRangeIndicator rangeIndicator;
    private bool isRangeActive;
    private float currentCooldown;

    public bool IsRangeActive => isRangeActive;
    public bool CanShoot => !IsOnCooldown;  // 쿨타임 중이 아닐 때만 공격 가능
    private bool IsOnCooldown => currentCooldown > 0f;

    private void Awake()
    {
        rangeIndicator = GetComponent<IRangeIndicator>();
    }

    private void Update()
    {
        if (currentCooldown > 0f)
        {
            currentCooldown -= Time.deltaTime;
        }
    }

    public void Initialize(IPlayerContext context)
    {
        this.context = context;
    }

    public void ActivateRange(bool isActive)
    {
        isRangeActive = isActive;
        if (isActive)
            rangeIndicator.Show();
        else
            rangeIndicator.Hide();
    }

    public void SpawnProjectile()
    {
        if (!photonView.IsMine) return;

        Vector3? targetPoint = context.GetMousePosition();
        if (!targetPoint.HasValue) return;

        Vector3 direction = (targetPoint.Value - context.Pos).normalized;
        direction.y = 0.0f;

        // 도끼 생성 및 발사
        GameObject axeObj = PhotonNetwork.Instantiate(axePrefab.name, transform.position, Quaternion.identity);
        IProjectile axe = axeObj.GetComponent<IProjectile>();
        axe.Initialize(context, attackPower);
        axe.Launch(direction);

        // 쿨타임 시작
        currentCooldown = cooldownTime;
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
}
