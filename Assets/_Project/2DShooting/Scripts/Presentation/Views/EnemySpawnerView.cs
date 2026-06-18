using System;
using UnityEngine;

namespace TwoDShooting.Proto.Presentation
{
    public sealed class EnemySpawnerView : MonoBehaviour
    {
        public event Action EnemyDefeated;

        [SerializeField] private EnemyConfig enemyConfig;
        [Tooltip("使用する敵Prefabです。複数登録すると、出現順に使われます。")]
        [SerializeField] private GameObject[] enemyPrefabs;
        [SerializeField] private Transform enemyRoot;

        public void Configure(EnemyConfig config, GameObject[] prefabs)
        {
            enemyConfig = config;
            enemyPrefabs = prefabs;
        }

        public void SpawnAll()
        {
            if (enemyConfig == null || enemyPrefabs == null || enemyPrefabs.Length == 0) return;
            if (enemyRoot == null) enemyRoot = transform;

            for (var i = 0; i < enemyConfig.EnemyCount; i++)
            {
                var prefabObject = enemyPrefabs[i % enemyPrefabs.Length];
                if (prefabObject == null || !prefabObject.TryGetComponent(out EnemyView prefab)) continue;
                var enemy = Instantiate(prefab, enemyRoot);
                enemy.Configure(enemyConfig, enemyConfig.SpawnPosition + enemyConfig.Spacing * i);
                enemy.Defeated += HandleEnemyDefeated;
            }
        }

        private void HandleEnemyDefeated(EnemyView enemyView)
        {
            if (enemyView != null) enemyView.Defeated -= HandleEnemyDefeated;
            EnemyDefeated?.Invoke();
        }
    }
}
