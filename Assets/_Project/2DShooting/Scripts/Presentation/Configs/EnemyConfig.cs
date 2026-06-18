using UnityEngine;

namespace TwoDShooting.Proto.Presentation
{
    [CreateAssetMenu(menuName = "2DShooting/Enemy Config")]
    public sealed class EnemyConfig : ScriptableObject
    {
        [Tooltip("敵に表示する画像です。未設定の場合はPrefab側の画像を使います。")]
        [SerializeField] private Sprite sprite;
        [Tooltip("敵の色です。")]
        [SerializeField] private Color tint = new(1f, 0.25f, 0.2f, 1f);
        [Tooltip("敵の大きさです。Xが横、Yが縦の倍率です。")]
        [SerializeField] private Vector3 localScale = Vector3.one;
        [Tooltip("敵のHPです。この値ぶんダメージを受けると倒れます。")]
        [SerializeField] private int maxHealth = 3;
        [Tooltip("敵が左右に動く速さです。")]
        [SerializeField] private float moveSpeed = 2f;
        [Tooltip("出現させる敵の数です。")]
        [SerializeField] private int enemyCount = 1;
        [Tooltip("最初の敵が出る位置です。")]
        [SerializeField] private Vector2 spawnPosition = new(7f, 0f);
        [Tooltip("複数の敵を出す時の間隔です。")]
        [SerializeField] private Vector2 spacing = new(2f, 1.25f);
        [Tooltip("敵が左右に動く幅です。")]
        [SerializeField] private float patrolDistance = 1.5f;

        public Sprite Sprite => sprite;
        public Color Tint => tint;
        public Vector3 LocalScale => localScale;
        public int MaxHealth => maxHealth;
        public float MoveSpeed => moveSpeed;
        public int EnemyCount => Mathf.Max(1, enemyCount);
        public Vector2 SpawnPosition => spawnPosition;
        public Vector2 Spacing => spacing;
        public float PatrolDistance => patrolDistance;
    }
}
