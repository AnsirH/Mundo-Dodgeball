using Fusion;
using Mundo_dodgeball.Projectile;
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

public class AxeProjectile : NetworkBehaviour
{
    [Header("References")]
    public GameObject model;
    public GameObject droppingModel;

    public float Speed = 10.0f;
    public float MaxDistance = 20.0f;
    public LayerMask CollisionMask;

    [Networked] private AxeProjectileData _data { get; set; }
    private PlayerRef _owner { get; set; }

    private float _traveledDistance;
    private Vector3 _lastPosition;

    public void Init(AxeProjectileData data, PlayerRef owner)
    {
        if (Object.HasStateAuthority)
        {
            _data = data;
            _owner = owner;
            transform.position = data.StartPosition;
            _traveledDistance = 0f;
            _lastPosition = data.StartPosition;
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (_data.IsFinished || !gameObject.activeSelf)
            return;

        float moveDistance = Speed * Runner.DeltaTime;
        Vector3 nextPosition = transform.position + _data.Direction * moveDistance;

        if (Runner.LagCompensation.Raycast(
            origin: transform.position,
            direction: _data.Direction,
            length: moveDistance,
            player: _owner,
            out var hit,
            layerMask: CollisionMask
            ))
        {
            nextPosition = hit.Point;
            var newData = _data;
            newData.IsFinished = true;
            _data = newData;
        }
        else
        {
            _traveledDistance += moveDistance;
            if (_traveledDistance >= MaxDistance)
            {
                nextPosition = _data.StartPosition + _data.Direction * MaxDistance;
                var newData = _data;
                newData.IsFinished = true;
                _data = newData;
            }
        }

        transform.position = nextPosition;

        if (_data.IsFinished)
        {
            OnFinished_RPC();
        }
    }

    //[Rpc(sources:RpcSources.All, targets:RpcTargets.All)]
    void OnFinished_RPC()
    {
        if (AxeProjectileManager.instance != null)
        {
            AxeProjectileManager.instance.OnProjectileFinished(this);
        }
    }
}
