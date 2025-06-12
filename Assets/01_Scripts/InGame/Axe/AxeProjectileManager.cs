using Fusion;
using UnityEngine;
using System.Collections.Generic;

namespace Mundo_dodgeball.Projectile
{    
    public class AxeProjectileManager : MonoBehaviour
    {
        [SerializeField] private NetworkPrefabRef projectilePrefab;

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
            var data = new AxeProjectileData
            {
                StartPosition = startPosition,
                Direction = direction,
                DistanceTraveled = 0f,
                IsFinished = false
            };

            var instance = ObjectPooler.Get("AxeProjectile").GetComponent<AxeProjectile>();

            //AxeProjectile instance = Runner.Spawn(projectilePrefab, startPosition, Quaternion.identity).GetComponent<AxeProjectile>();
            instance.Init(data, owner);

            _activeProjectiles.Add(instance);
        }

        public void OnProjectileFinished(AxeProjectile proj)
        {
            _activeProjectiles.Remove(proj);
            ObjectPooler.Release("AxeProjectile", proj.gameObject);
        }
    }
}