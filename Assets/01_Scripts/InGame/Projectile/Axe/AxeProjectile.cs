using Fusion;
using System.Runtime.InteropServices;
using UnityEngine;
[StructLayout(LayoutKind.Sequential)]
public struct AxeProjectileData : INetworkStruct
{
    public float DistanceTraveled;
    public NetworkBool IsFinished;
    public Vector3 StartPosition;
    public Vector3 Direction;
}

public class AxeProjectile : ProjectileBase
{
    [Header("References")]
    public NetworkMecanimAnimator animator;

    [Networked] TickTimer DroppingTimer { get; set; }

    private bool _isDropping = false;

    public override void Init(Vector3 startPos, Vector3 direction, PlayerRef owner)
    {
        base.Init(startPos, direction, owner);
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
            base.FixedUpdateNetwork();
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
