using Fusion;
using MyGame.Utils;
using UnityEngine;

public class PlayerAttack : NetworkBehaviour
{
    private IPlayerContext context;
    [Networked] private TickTimer CoolTimer { get; set; }
    [Networked] private TickTimer AttackTimer { get; set; }
    [Networked] public int AttackCount { get; set; } = 0;
    public float CoolTime => CoolTimer.RemainingTime(Runner).HasValue ? CoolTimer.RemainingTime(Runner).Value : 0.0f;
    public bool Attacking { get { return !AttackTimer.ExpiredOrNotRunning(Runner); } }

    [Networked] public bool IsActivating { get; set; }


    [Header("References")]
    [SerializeField] private NetworkPrefabRef axePrefab;
    [SerializeField] Transform shotPosition;

    // 캐릭터 도끼 모델링
    [SerializeField] private GameObject axeObj;

    [SerializeField] private GameObject indicator;

    [SerializeField] private float rotationSpeed = 8.0f;

    [SerializeField] private float attackDuration = 0.25f;
    public float RotationSpeed => rotationSpeed;

    private Vector3 targetPoint;
    private int _visibleAttackCount;

    public void Initialize(IPlayerContext context)
    {
        this.context = context;
        ActivateIndicator(false);
    }


    public void StartAttack(Vector3 point)
    {
        SetTargetPoint(point);
        if (Object.HasStateAuthority)
        {
            AttackCount++;
            StartCoolDown(context.Stats.GetAttackCooldown());
        }
        AttackTimer = TickTimer.CreateFromSeconds(Runner, attackDuration);
        axeObj.SetActive(false);
        ActivateIndicator(false);
    }

    private void SetTargetPoint(Vector3 point)
    {
        targetPoint = point;
    }

    private void StartCoolDown(float coolTime)
    {
        CoolTimer = TickTimer.CreateFromSeconds(Runner, coolTime);
    }

    /// <summary>
    /// 투사체 발사
    /// </summary>
    /// <param name="direction"></param>
    public void Fire(Vector3 direction)
    {
        if (HasStateAuthority)
        {
            SpawnProjectile(shotPosition.position, direction);
            context.Sound.PlayOneShot_Attack();
        }
    }
    private void SpawnProjectile(Vector3 startPos, Vector3 direction)
    {
        Runner.Spawn(axePrefab, 
            startPos, 
            Quaternion.LookRotation(direction), 
            Object.InputAuthority, 
            (runner, o) => 
            {
                o.GetComponent<ProjectileBase>().Init(startPos, direction, context.Stats.GetAttackPower(), Object.InputAuthority);
            });
    }

    public void ActivateIndicator(bool active)
    {
        IsActivating = active;
        if (HasInputAuthority)
            indicator.SetActive(active);
    }

    public override void Render()
    {
        if (context == null) return;

        if (CoolTime > 0 && axeObj.activeSelf) axeObj.SetActive(false);
        else if (CoolTime == 0 && !axeObj.activeSelf) axeObj.SetActive(true);


        if (_visibleAttackCount < AttackCount)
        {
            context.Anim.SetTrigger("Attack");

            _visibleAttackCount = AttackCount;
        }
    }

    public void ResetCoolTime()
    {
        if (CoolTimer.IsRunning)
        {
            CoolTimer = TickTimer.None;
        }
    }

    private void Update()
    {
        if (IsActivating)
        {
            Vector3 targetPoint = GroundClick.GetMousePosition(Camera.main, LayerMask.GetMask("Ground"));
            indicator.transform.rotation = Quaternion.LookRotation((targetPoint - transform.position).normalized);
        }
    }

}