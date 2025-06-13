using Fusion;
using Mundo_dodgeball.Projectile;
using UnityEngine;

public class PlayerAttack : NetworkBehaviour
{
    private IPlayerContext context;
    [Networked] public TickTimer coolTimer { get; set; }
    [Networked] public TickTimer attackTimer { get; set; }
    public bool CoolTiming { get {  return !coolTimer.ExpiredOrNotRunning(Runner); } }
    public bool Attacking { get { return !attackTimer.ExpiredOrNotRunning(Runner); } }


    [Header("References")]
    // 도끼 발사체
    [SerializeField] private AxeShooter axeShooter;

    [SerializeField] private NetworkPrefabRef axePrefab;

    // 캐릭터 도끼 모델링
    [SerializeField] private GameObject axeObj;

    [SerializeField] private float rotationSpeed = 8.0f;

    [SerializeField] private float attackDuration = 0.25f;
    public float RotationSpeed => rotationSpeed;
    public float AttackDuration => attackDuration;

    private Vector3 targetPoint;


    public void Initialize(IPlayerContext context)
    {
        this.context = context;
    }


    public void StartAttack(Vector3 point)
    {
        SetTargetPoint(point);

        attackTimer = TickTimer.CreateFromSeconds(Runner, 0.25f);
    }

    private void SetTargetPoint(Vector3 point)
    {
        targetPoint = point;
    }

    public void StartCoolDown(float coolTime)
    {
        coolTimer = TickTimer.CreateFromSeconds(Runner, coolTime);
    }

    public void Fire(Vector3 direction)
    {
        if (HasStateAuthority)
        {
            SpawnProjectile(transform.position, direction);
        }
        else if (HasInputAuthority)
        {
            SpawnAxe_RPC(transform.position, direction);
        }
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    private void SpawnAxe_RPC(Vector3 startPos, Vector3 direction)
    {
        SpawnProjectile(startPos, direction);
    }

    private void SpawnProjectile(Vector3 startPos, Vector3 direction)
    {
        //AxeProjectileManager.instance.SpawnProjectile(transform.position, direction, Object);
        ProjectileManager.Instance.SpawnProjectile("TestProjectile", startPos, direction, Object.InputAuthority);

        //ProjectileBase projectile = Runner.Spawn(axePrefab, startPos, Quaternion.LookRotation(direction)).GetComponent<ProjectileBase>();
        //projectile.Init(transform.position, direction, Object.InputAuthority);
    }

    // 도끼 궤적 활성화 메서드
    public void ActivateRange(bool active)
    {
        //if (!HasStateAuthority) { return; }
        if (active == true)
        {
            if (axeShooter.CanShoot)
                axeShooter.ActivateRange(true);
        }
        else
            axeShooter.ActivateRange(false);
    }
}