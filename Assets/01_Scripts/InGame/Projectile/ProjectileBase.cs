using Fusion;
using UnityEngine;

public abstract class ProjectileBase : NetworkBehaviour
{
    [Header("Projectile Settings")]
    [SerializeField] protected float speed = 10f;
    [SerializeField] protected float maxDistance = 20f;
    [SerializeField] protected LayerMask collisionMask;
    [SerializeField] protected float damage = 10f;

    [Networked] protected Vector3 Direction { get; set; }
    [Networked] protected Vector3 StartPosition { get; set; }
    [Networked] protected float DistanceTraveled { get; set; }
    [Networked] protected PlayerRef Owner { get; set; }
    [Networked] protected NetworkBool IsFinished { get; set; }

    protected ChangeDetector _changeDetector;
    protected float _currentDistance;

    public bool IsMaxDistanceReached { get { return DistanceTraveled >= maxDistance; } }

    public override void Spawned()
    {
        _changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
    }

    public virtual void Init(Vector3 startPos, Vector3 direction, PlayerRef owner)
    {
        StartPosition = startPos;
        Direction = direction.normalized;
        Owner = owner;
        DistanceTraveled = 0f;
        IsFinished = false;

        _currentDistance = 0f;
        transform.position = startPos;
    }

    public override void FixedUpdateNetwork()
    {
        float moveDistance = speed * Runner.DeltaTime;
        Vector3 nextPosition = transform.position + Direction * moveDistance;

        if (CheckCollision(moveDistance, out LagCompensatedHit hit))
        {
            OnHit();
        }
        else
        {
            if (HasStateAuthority)
            {
                _currentDistance += moveDistance;
                DistanceTraveled = _currentDistance;
            }
            if (IsMaxDistanceReached)
            {
                nextPosition = StartPosition + Direction * maxDistance;
                OnMaxDistanceReached();
            }
        }

        transform.position = nextPosition;
    }

    protected virtual bool CheckCollision(float distance, out LagCompensatedHit lagCompensatedHit)
    {
        lagCompensatedHit = new();
        if (Runner.LagCompensation.Raycast(
            origin: transform.position,
            direction: Direction,
            length: distance,
            player: Owner,
            out var hit,
            layerMask: collisionMask))
        {
            lagCompensatedHit = hit;
            return true;
        }
        return false;
    }

    protected virtual void OnHit()
    {
        IsFinished = true;
        Runner.Despawn(Object);
    }

    protected virtual void OnMaxDistanceReached()
    {
        IsFinished = true;
        Runner.Despawn(Object);
    }

    public override void Render()
    {
    }
}
