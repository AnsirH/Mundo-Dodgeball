using Fusion;
using UnityEngine;
using System.Collections.Generic;

public class ProjectileManager : NetworkBehaviour
{
    public static ProjectileManager Instance { get; private set; }

    [System.Serializable]
    public class ProjectilePool
    {
        public string projectileType;
        public NetworkPrefabRef prefab;
        public int poolSize = 20;
    }

    [SerializeField] private List<ProjectilePool> projectilePools;
    private Dictionary<string, Queue<NetworkObject>> _projectilePools;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public override void Spawned()
    {
        base.Spawned();
        if (Object.HasStateAuthority)
        {
            InitializePools();
        }
    }

    private void InitializePools()
    {
        _projectilePools = new Dictionary<string, Queue<NetworkObject>>();

        foreach (var pool in projectilePools)
        {
            var queue = new Queue<NetworkObject>();
            for (int i = 0; i < pool.poolSize; i++)
            {
                var instance = Runner.Spawn(pool.prefab, Vector3.zero, Quaternion.identity);
                instance.transform.parent = transform;
                instance.gameObject.SetActive(false);
                queue.Enqueue(instance);
            }
            _projectilePools.Add(pool.projectileType, queue);
        }
    }

    public NetworkObject GetProjectile(string type, Vector3 position, Quaternion rotation, PlayerRef owner)
    {
        if (_projectilePools.TryGetValue(type, out var queue) && queue.Count > 0)
        {
            var projectile = queue.Dequeue();
            projectile.transform.SetPositionAndRotation(position, rotation);
            projectile.gameObject.SetActive(true);
            return projectile;
        }
        return null;
    }

    public void ReleaseProjectile(string type, NetworkObject projectile)
    {
        if (_projectilePools.TryGetValue(type, out var queue))
        {
            projectile.gameObject.SetActive(false);
            queue.Enqueue(projectile);
        }
        else
        {
            Runner.Despawn(projectile);
        }
    }

    public void SpawnProjectile(string type, Vector3 position, Vector3 direction, PlayerRef owner)
    {
        if (Object.HasStateAuthority)
        {
            _SpawnProjectile(type, position, direction, owner);
        }
        else if (Object.HasInputAuthority)
        {
            SpawnProjectile_RPC(type, position, direction, owner);
        }
    }

    private void _SpawnProjectile(string type, Vector3 position, Vector3 direction, PlayerRef owner)
    {
        if (!Object.HasStateAuthority)
        {
            Debug.LogWarning("SpawnProjectile should only be called on the host");
            return;
        }

        var projectile = GetProjectile(type, position, Quaternion.LookRotation(direction), owner);
        if (projectile != null)
        {
            var projectileBase = projectile.GetComponent<ProjectileBase>();
            if (projectileBase != null)
            {
                projectileBase.Init(position, direction, owner);
            }
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.InputAuthority)]
    private void SpawnProjectile_RPC(string type, Vector3 position, Vector3 direction, PlayerRef owner)
    {
        _SpawnProjectile(type, position, direction, owner);
    }
} 