using Fusion;
using UnityEngine;
using System.Collections.Generic;

namespace Mundo_dodgeball.Projectile
{    
    public class AxeProjectileManager : NetworkBehaviour
    {
        public NetworkPrefabRef axePrefab;

        private List<AxeProjectile> _activeProjectiles = new();
        public static AxeProjectileManager instance;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
        }

        public void SpawnProjectile(Vector3 startPosition, Vector3 direction, NetworkObject sender)
        {
            var data = new AxeProjectileData
            {
                StartPosition = startPosition,
                Direction = direction,
                DistanceTraveled = 0f,
                IsFinished = false
            };

            //var instance = ObjectPooler.GetNetwork("AxeProjectile", startPosition, Quaternion.LookRotation(direction));
            var projectile = Runner.Spawn(axePrefab, position: startPosition, rotation: Quaternion.LookRotation(direction), sender.InputAuthority);
            if (projectile != null)
            {
                var axeProjectile = projectile.GetComponent<AxeProjectile>();
                if (axeProjectile != null)
                {
                    axeProjectile.Init(data, sender.InputAuthority);
                    _activeProjectiles.Add(axeProjectile);
                }
            }
        }

        public void OnProjectileFinished(AxeProjectile proj)
        {
            _activeProjectiles.Remove(proj);
            Runner.Despawn(proj.Object);
            //ObjectPooler.ReleaseNetwork("AxeProjectile", proj.Object);
        }
    }
}