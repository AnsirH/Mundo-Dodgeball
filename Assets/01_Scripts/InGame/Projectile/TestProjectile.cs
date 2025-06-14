using Fusion;
using UnityEngine;

public class TestProjectile : ProjectileBase
{
    [Header("Test Projectile Settings")]
    [SerializeField] private GameObject hitEffectPrefab;
    [SerializeField] private float rotationSpeed = 360f; // 초당 회전 각도
    [SerializeField] private TrailRenderer trailRenderer;

    private void Start()
    {
        if (trailRenderer != null)
        {
            trailRenderer.enabled = false;
        }
    }

    public override void Init(Vector3 startPos, Vector3 direction, PlayerRef owner)
    {
        base.Init(startPos, direction, owner);
        
        if (trailRenderer != null)
        {
            trailRenderer.enabled = true;
            trailRenderer.Clear();
        }
    }

    protected override void OnHit()
    {
        // 히트 이펙트 생성
        if (hitEffectPrefab != null)
        {
            var hitEffect = Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
            Destroy(hitEffect, 2f); // 2초 후 이펙트 제거
        }

        // 트레일 비활성화
        if (trailRenderer != null)
        {
            trailRenderer.enabled = false;
        }

        base.OnHit();
    }

    protected override void OnMaxDistanceReached()
    {
        // 트레일 비활성화
        if (trailRenderer != null)
        {
            trailRenderer.enabled = false;
        }

        base.OnMaxDistanceReached();
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();

        //if (!IsMaxDistanceReached)
        //{
        //    transform.Rotate(Vector3.forward, rotationSpeed * Runner.DeltaTime);
        //}        
    }

    //protected override bool CheckCollision(float distance, out LagCompensatedHit lagCompensatedHit)
    //{
    //    if (base.CheckCollision(distance, out lagCompensatedHit))
    //    {
    //        // 충돌한 오브젝트가 플레이어인 경우 데미지 처리
    //        if (Runner.LagCompensation.Raycast(
    //            origin: transform.position,
    //            direction: Direction,
    //            length: distance,
    //            player: Owner,
    //            out var hit,
    //            layerMask: collisionMask))
    //        {
    //            Debug.Log("Hit");
    //        }
    //        return true;
    //    }
    //    return false;
    //}
} 