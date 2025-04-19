using MyGame.Utils;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UIElements;

public interface IShooter
{
    void Initialize(IPlayerContext context, bool isOfflineMode = false);
    void ActivateRange(bool isActive);
    void SpawnProjectile(Vector3 targetPoint);
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
    void Initialize(IPlayerContext context, float damage, Vector3 spawnPos);
    void Launch(Vector3 direction);
    void OnHit(Collider other);
}

public class AxeShooter : MonoBehaviourPun, IShooter
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
            Vector3? mousePosition = context.GetMousePosition();
            if (mousePosition.HasValue)
                rangeIndicator.UpdatePosition(mousePosition.Value, 10.0f);
        }
    }

    public void Initialize(IPlayerContext context, bool isOfflineMode=false)
    {
        this.context = context;
        this.isOfflineMode = isOfflineMode;
    }

    public void SpawnProjectile(Vector3 targetPoint)
    {
        if (!CanShoot) return;

        Vector3 direction = (targetPoint - context.Pos).normalized * 10.0f;
        direction.y = 0.0f;
        if (isOfflineMode)
        {
            // 오프라인 모드: 직접 Instantiate
            GameObject axeObj = Instantiate(axePrefab, transform.position, Quaternion.identity);

            IProjectile axe = axeObj.GetComponent<IProjectile>();
            axe.Initialize(context, attackPower, transform.position);
            axe.Launch(direction);

            // 쿨타임 시작
            currentCooldown = cooldownTime;
        }
        else if (context.IsLocalPlayer())
        {
            photonView.RPC("RPC_SpawnProjectile", RpcTarget.All, transform.position, direction);
        }
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

    [PunRPC]
    private void RPC_SpawnProjectile(Vector3 position, Vector3 direction)
    {
        // 온라인 모드: PhotonNetwork.Instantiate
        GameObject axeObj = PhotonNetwork.Instantiate(axePrefab.name, position, Quaternion.identity);

        IProjectile axe = axeObj.GetComponent<IProjectile>();
        axe.Initialize(context, attackPower, transform.position);
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
