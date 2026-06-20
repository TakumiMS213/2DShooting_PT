using UnityEngine;

namespace TwoDShooting.Simple
{
    public enum BulletPath { Straight, Wave }

    /// <summary>弾1発の動き。Prefabを開いて色・速さ・軌道を調整できます。</summary>
    public sealed class Bullet : MonoBehaviour
    {
        [Tooltip("弾の速さです。")][Min(0f)] public float speed = 12f;
        [Tooltip("敵へ与えるダメージです。")][Min(1)] public int damage = 1;
        [Tooltip("Straightは直進、Waveは波打ちます。")]
        public BulletPath path = BulletPath.Straight;
        [Tooltip("Waveの揺れ幅です。")][Min(0f)] public float waveSize = 0.35f;
        [Tooltip("Waveの細かさです。")][Min(0f)] public float waveSpeed = 8f;
        [Tooltip("この秒数が過ぎると消えます。")][Min(0.1f)] public float lifeSeconds = 2f;

        private Vector2 basePosition;
        private float age;

        private void Start() => basePosition = transform.position;

        private void Update()
        {
            age += Time.deltaTime;
            basePosition += Vector2.right * (speed * Time.deltaTime);
            var wave = path == BulletPath.Wave ? Mathf.Sin(age * waveSpeed) * waveSize : 0f;
            transform.position = basePosition + Vector2.up * wave;
            if (age >= lifeSeconds) Destroy(gameObject);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.TryGetComponent(out Enemy enemy)) return;
            enemy.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
