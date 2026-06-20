using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace TwoDShooting.Simple
{
    /// <summary>ステージ全体のルール。敵の数や並びをここで調整できます。</summary>
    public sealed class GameRules : MonoBehaviour
    {
        [Header("クリア条件")]
        [Tooltip("この数の敵を倒すとクリアです。")]
        [Min(1)] public int enemiesToClear = 4;
        [Tooltip("最後の敵を倒してから結果画面へ移るまでの秒数です。")]
        [Min(0f)] public float clearDelay = 0.8f;

        [Header("敵の出し方")]
        [Tooltip("ここへ敵Prefabを複数入れると、上から順番に使います。")]
        public Enemy[] enemyPrefabs;
        [Tooltip("最初の敵が現れる位置です。")]
        public Vector2 firstSpawnPosition = new(5.5f, 2.4f);
        [Tooltip("2体目以降をずらす量です。")]
        public Vector2 spawnStep = new(0.7f, -1.6f);
        [Tooltip("敵を1体ずつ出す間隔（秒）です。0なら全員が同時に出現します。")]
        [Min(0f)] public float spawnInterval = 0f;

        [Header("画面表示（通常は変更不要）")]
        [SerializeField] private Text progressText;
        [SerializeField] private Text statusText;

        private int defeatedCount;
        private bool isCleared;

        private void Start()
        {
            UpdateHud();
            StartCoroutine(SpawnEnemies());
        }

        private IEnumerator SpawnEnemies()
        {
            if (enemyPrefabs == null || enemyPrefabs.Length == 0)
            {
                SetStatus("GameRules の Enemy Prefabs に敵を設定してください");
                yield break;
            }

            for (var i = 0; i < enemiesToClear; i++)
            {
                var prefab = enemyPrefabs[i % enemyPrefabs.Length];
                if (prefab == null) continue;
                var enemy = Instantiate(prefab, firstSpawnPosition + spawnStep * i, Quaternion.identity);
                enemy.name = $"Enemy_{i + 1:00}";
                enemy.SetDefeatedCallback(OnEnemyDefeated);

                if (spawnInterval > 0f && i < enemiesToClear - 1)
                {
                    yield return new WaitForSeconds(spawnInterval);
                }
            }
        }

        private void OnEnemyDefeated()
        {
            if (isCleared) return;
            defeatedCount++;
            UpdateHud();
            if (defeatedCount < enemiesToClear) return;

            isCleared = true;
            SetStatus("クリア！ おつかれさまでした");
            StartCoroutine(LoadResult());
        }

        private IEnumerator LoadResult()
        {
            yield return new WaitForSeconds(clearDelay);
            SceneManager.LoadScene("ResultScene");
        }

        private void UpdateHud()
        {
            if (progressText != null) progressText.text = $"たおした敵  {defeatedCount} / {enemiesToClear}";
        }

        private void SetStatus(string message)
        {
            if (statusText != null) statusText.text = message;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(1f, 0.54f, 0.24f, 0.9f);
            for (var i = 0; i < Mathf.Max(1, enemiesToClear); i++)
            {
                var position = firstSpawnPosition + spawnStep * i;
                Gizmos.DrawWireSphere(position, 0.38f);
                Gizmos.DrawLine(position + Vector2.left * 0.55f, position + Vector2.right * 0.55f);
                Gizmos.DrawLine(position + Vector2.down * 0.55f, position + Vector2.up * 0.55f);
            }
        }
    }
}
