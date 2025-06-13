using Fusion;
using UnityEngine;
using System.Collections.Generic;

namespace Mundo_dodgeball.Projectile
{    
    public class AxeProjectileManager : NetworkBehaviour
    {
        private List<AxeProjectile> _activeProjectiles = new();
        public static AxeProjectileManager instance;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
        }

        public void SpawnProjectile(Vector3 startPosition, Vector3 direction, PlayerRef owner)
        {
            if (!Object.HasStateAuthority) return;

            var data = new AxeProjectileData
            {
                StartPosition = startPosition,
                Direction = direction,
                DistanceTraveled = 0f,
                IsFinished = false
            };

            var instance = ObjectPooler.Get("AxeProjectile");
            if (instance != null)
            {
                var axeProjectile = instance.GetComponent<AxeProjectile>();
                if (axeProjectile != null)
                {
                    axeProjectile.Init(data, owner);
                    _activeProjectiles.Add(axeProjectile);
                }
            }
        }

        public void OnProjectileFinished(AxeProjectile proj)
        {
            // if (!Object.HasStateAuthority) return;

            _activeProjectiles.Remove(proj);
            ObjectPooler.Release("AxeProjectile", proj.gameObject);
        }
    }
}