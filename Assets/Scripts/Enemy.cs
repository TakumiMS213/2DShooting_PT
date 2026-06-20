using System;
using UnityEngine;

namespace TwoDShooting.Simple
{
    /// <summary>敵1体の体力と動き。Enemy Prefabごとに違う個性を作れます。</summary>
    public sealed class Enemy : MonoBehaviour
    {
        [Header("強さ")]
        [Tooltip("0になると倒れます。")][Min(1)] public int health = 3;

        [Header("動き")]
        [Tooltip("上下移動の速さです。0なら停止します。")][Min(0f)] public float moveSpeed = 1.2f;
        [Tooltip("最初の位置から動く幅です。")][Min(0f)] public float moveDistance = 1.1f;

        private Vector2 startPosition;
        private Action onDefeated;

        private void Start() => startPosition = transform.position;

        private void Update()
        {
            var y = Mathf.Sin(Time.time * moveSpeed) * moveDistance;
            transform.position = startPosition + Vector2.up * y;
        }

        public void SetDefeatedCallback(Action callback) => onDefeated = callback;

        public void TakeDamage(int damage)
        {
            health -= Mathf.Max(0, damage);
            if (health > 0) return;
            onDefeated?.Invoke();
            Destroy(gameObject);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(1f, 0.54f, 0.24f, 0.9f);
            Gizmos.DrawLine(transform.position + Vector3.down * moveDistance, transform.position + Vector3.up * moveDistance);
        }
    }
}
