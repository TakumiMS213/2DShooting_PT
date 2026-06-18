using System.Collections.Generic;
using UnityEngine;

namespace TwoDShooting.Proto.Presentation
{
    public sealed class BulletPoolView : MonoBehaviour
    {
        [Tooltip("使用する弾Prefabです。複数登録すると、順番に使われます。")]
        [SerializeField] private GameObject[] bulletPrefabs;
        [SerializeField] private BulletConfig bulletConfig;
        [SerializeField] private Transform bulletRoot;

        private readonly List<BulletView> _bullets = new();

        public void Configure(GameObject[] prefabs, BulletConfig config)
        {
            bulletPrefabs = prefabs;
            bulletConfig = config;
            Warmup();
        }

        private void Awake()
        {
            Warmup();
        }

        public void Fire(Vector2 position, Vector2 direction)
        {
            if (bulletConfig == null) return;
            var bullet = GetInactiveBullet();
            if (bullet != null) bullet.Launch(position, direction);
        }

        private void Warmup()
        {
            if (bulletPrefabs == null || bulletPrefabs.Length == 0 || bulletConfig == null) return;
            if (bulletRoot == null) bulletRoot = transform;

            while (_bullets.Count < bulletConfig.PoolSize)
            {
                var prefabObject = bulletPrefabs[_bullets.Count % bulletPrefabs.Length];
                if (prefabObject == null || !prefabObject.TryGetComponent(out BulletView prefab)) return;
                var bullet = Instantiate(prefab, bulletRoot);
                bullet.Configure(bulletConfig);
                bullet.gameObject.SetActive(false);
                _bullets.Add(bullet);
            }
        }

        private BulletView GetInactiveBullet()
        {
            for (var i = 0; i < _bullets.Count; i++)
            {
                if (_bullets[i] != null && !_bullets[i].IsActive) return _bullets[i];
            }

            return null;
        }
    }
}
