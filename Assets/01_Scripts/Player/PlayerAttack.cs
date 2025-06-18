using Fusion;
using Mundo_dodgeball.Projectile;
using TMPro;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class PlayerAttack : NetworkBehaviour
{
    private IPlayerContext context;
    [Networked] private TickTimer CoolTimer { get; set; }
    [Networked] private TickTimer AttackTimer { get; set; }
    [Networked] public int AttackCount { get; set; } = 0;
    public float CoolTime => CoolTimer.RemainingTime(Runner).HasValue ? CoolTimer.RemainingTime(Runner).Value : 0.0f;
    public bool Attacking { get { return !AttackTimer.ExpiredOrNotRunning(Runner); } }

    public bool IsActivating => indicator.activeSelf;


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

    public void Initialize(IPlayerContext context)
    {
        this.context = context;
        indicator.SetActive(false);
    }


    public void StartAttack(Vector3 point)
    {
        SetTargetPoint(point);
        if(Object.HasStateAuthority)
            AttackCount++;
        AttackTimer = TickTimer.CreateFromSeconds(Runner, attackDuration);
        axeObj.SetActive(false);
        indicator.SetActive(false);
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
            StartCoolDown(context.Stats.GetAttackCooldown());
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

    public void ActiveIndicator(bool active)
    {
        indicator.SetActive(active);
    }

    public override void Render()
    {
        if (context == null) return;

        if (CoolTime > 0 && axeObj.activeSelf) axeObj.SetActive(false);
        else if (CoolTime == 0 && !axeObj.activeSelf) axeObj.SetActive(true);
    }
}