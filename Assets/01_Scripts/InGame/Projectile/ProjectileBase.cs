using Fusion;
using UnityEngine;

public abstract class ProjectileBase : NetworkBehaviour
{
    [Header("Projectile Settings")]
    [SerializeField] protected float speed = 10f;
    [SerializeField] protected float maxDistance = 20f;
    [SerializeField] protected LayerMask collisionMask;
    [SerializeField] protected float damage = 10f;

    [Networked] public bool IsActive { get; set; }
    [Networked] protected Vector3 Direction { get; set; }
    [Networked] protected Vector3 StartPosition { get; set; }
    [Networked] protected float DistanceTraveled { get; set; }
    [Networked] protected PlayerRef Owner { get; set; }

    protected ChangeDetector _changeDetector;
    protected float _currentDistance;

    public override void Spawned()
    {
        _changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
        SetActive(false);
    }

    public virtual void Init(Vector3 startPos, Vector3 direction, PlayerRef owner)
    {
        if (Object.HasStateAuthority)
        {
            StartPosition = startPos;
            Direction = direction.normalized;
            Owner = owner;
            DistanceTraveled = 0f;
            _currentDistance = 0f;
            transform.position = startPos;
            SetActive(true);
        }
    }

    public void SetActive(bool active)
    {
        gameObject.SetActive(active);
        IsActive = active;
    }

    public override void FixedUpdateNetwork()
    {
        if (!IsActive) return;

        float moveDistance = speed * Runner.DeltaTime;
        Vector3 nextPosition = transform.position + Direction * moveDistance;

        if (CheckCollision(moveDistance, out Vector3 hitPoint))
        {
            nextPosition = hitPoint;
            OnHit();
        }
        else
        {
            _currentDistance += moveDistance;
            if (_currentDistance >= maxDistance)
            {
                nextPosition = StartPosition + Direction * maxDistance;
                OnMaxDistanceReached();
            }
        }

        transform.position = nextPosition;
    }

    protected virtual bool CheckCollision(float distance, out Vector3 hitPoint)
    {
        hitPoint = Vector3.zero;
        if (Runner.LagCompensation.Raycast(
            origin: transform.position,
            direction: Direction,
            length: distance,
            player: Owner,
            out var hit,
            layerMask: collisionMask))
        {
            hitPoint = hit.Point;
            return true;
        }
        return false;
    }

    protected virtual void OnHit()
    {
        SetActive(false);
    }

    protected virtual void OnMaxDistanceReached()
    {
        SetActive(false);
    }

    public override void Render()
    {
        foreach (var change in _changeDetector.DetectChanges(this))
        {
            switch (change)
            {
                case nameof(IsActive):
                    SetActive(IsActive);
                    break;
            }
        }
    }
}
