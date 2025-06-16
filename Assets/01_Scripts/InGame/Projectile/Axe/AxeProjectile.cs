using Fusion;
using UnityEngine;

public class AxeProjectile : ProjectileBase
{
    [Header("References")]
    public NetworkMecanimAnimator animator;

    [Networked] TickTimer DroppingTimer { get; set; }

    private bool _isDropping = false;

    public override void Init(Vector3 startPos, Vector3 direction, float damage, PlayerRef owner)
    {
        base.Init(startPos, direction, damage, owner);
    }

    protected override void OnMaxDistanceReached()
    {
        IsFinished = true;

        DroppingTimer = TickTimer.CreateFromSeconds(Runner, 1.0f);
    }

    public override void Render()
    {
        if (IsFinished && !_isDropping)
        {
            animator.Animator.SetTrigger("Drop");
            _isDropping = true;
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (!IsFinished)
        {
            float moveDistance = speed * Runner.DeltaTime;
            Vector3 nextPosition = transform.position + Direction * moveDistance;

            if (Object.HasStateAuthority && CheckCollision(moveDistance, out LagCompensatedHit hit))
            {
                if (hit.Hitbox.Root.gameObject.TryGetComponent(out IDamageable target))
                {
                    target.TakeDamage(damage);
                }                

                OnHit();
            }
            else
            {
                if (HasStateAuthority)
                {
                    _currentDistance += moveDistance;
                    DistanceTraveled = _currentDistance;
                }
                if (DistanceTraveled > maxDistance)
                {
                    nextPosition = StartPosition + Direction * maxDistance;
                    OnMaxDistanceReached();
                }
            }

            transform.position = nextPosition;
        }
        else
        {
            transform.position = new Vector3(transform.position.x, 0.0f, transform.position.z);
            if (HasStateAuthority && DroppingTimer.ExpiredOrNotRunning(Runner))
            {
                Runner.Despawn(Object);
            }
        }        
    }
}
