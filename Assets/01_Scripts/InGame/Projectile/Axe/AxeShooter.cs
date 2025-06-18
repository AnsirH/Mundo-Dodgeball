using MyGame.Utils;
using UnityEngine;

public interface IShooter
{
    void Initialize(IPlayerContext context, bool isOfflineMode = false);
    void ActivateRange(bool isActive);
    void SpawnProjectile(Vector3 startPos, Vector3 direction);
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
    void Initialize(IPlayerContext context, Vector3 spawnPos, Vector3 direction);
    void OnHit(Collider other);
}

public class AxeShooter : MonoBehaviour, IShooter
{
    private bool isOfflineMode = false;
    [Header("�ӽ� ����")]
    [SerializeField] private float cooldownTime = 2.0f;

    [Header("References")]
    [SerializeField] private GameObject axePrefab;
    [SerializeField] private GameObject rangeIndicatorObj;
    [SerializeField] private AudioClip[] sounds;

    private IPlayerContext context;
    private IRangeIndicator rangeIndicator;
    private bool isRangeActive;
    private float currentCooldown = 0;

    public bool IsRangeActive => isRangeActive;
    public bool CanShoot => !IsOnCooldown;  // ��Ÿ�� ���� �ƴ� ���� ���� ����
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
            Vector3? mousePoint = GroundClick.GetMousePosition(Camera.main, LayerMask.GetMask("Ground"));
            if (mousePoint.HasValue)
                rangeIndicator.UpdatePosition(mousePoint.Value, 10.0f);
        }
    }

    public void Initialize(IPlayerContext context, bool isOfflineMode=false)
    {
        this.context = context;
        this.isOfflineMode = isOfflineMode;
    }

    // �Ÿ� ǥ�� Ȱ��ȭ
    public void ActivateRange(bool isActive)
    {
        isRangeActive = isActive;
        if (isActive)
            rangeIndicator.Show();
        else
            rangeIndicator.Hide();
    }

    // ��Ÿ�� ���� �߰� ��ɵ� (AxeShooter ���� ���)
    public void ReduceCooldown(float amount)
    {
        currentCooldown = Mathf.Max(0f, currentCooldown - amount);
    }

    public float GetCooldownProgress()
    {
        return currentCooldown / cooldownTime;
    }

    public void SpawnProjectile(Vector3 startPos, Vector3 direction)
    {
        if (!CanShoot) return;

        direction.y = 0.0f;

        GameObject axeObj = ObjectPooler.GetLocal("Axe");

        IProjectile axe = axeObj.GetComponent<IProjectile>();
        axe.Initialize(context, transform.position, direction);

        // �Ҹ� ���
        //SoundManager.instance.PlayOneShot(context.Sound, sounds[Random.Range(0, sounds.Length)]);

        // ��Ÿ�� ����
        currentCooldown = cooldownTime;
    }
}
