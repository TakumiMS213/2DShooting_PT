using UnityEngine;
using UnityEngine.InputSystem;

namespace TwoDShooting.Simple
{
    /// <summary>プレイヤーの移動とショット。数値を変えてすぐ試せます。</summary>
    public sealed class Player : MonoBehaviour
    {
        [Header("移動")]
        [Tooltip("大きいほど速く動きます。")]
        [Min(0f)] public float moveSpeed = 6f;
        [Tooltip("移動できる左下です。Sceneビューの水色枠と同じ範囲です。")]
        public Vector2 moveAreaMin = new(-7.5f, -3.7f);
        [Tooltip("移動できる右上です。")]
        public Vector2 moveAreaMax = new(7.5f, 3.7f);

        [Header("ショット")]
        public Bullet bulletPrefab;
        [Tooltip("小さいほど連射できます。")]
        [Min(0.03f)] public float shotInterval = 0.22f;
        [Tooltip("機体の中心から見た発射位置です。黄色い印で確認できます。")]
        public Vector2 muzzleOffset = new(0.85f, 0f);

        private float nextShotTime;

        private void Update()
        {
            var keyboard = Keyboard.current;
            if (keyboard == null) return;

            var move = new Vector2(
                ReadAxis(keyboard.leftArrowKey.isPressed || keyboard.aKey.isPressed,
                    keyboard.rightArrowKey.isPressed || keyboard.dKey.isPressed),
                ReadAxis(keyboard.downArrowKey.isPressed || keyboard.sKey.isPressed,
                    keyboard.upArrowKey.isPressed || keyboard.wKey.isPressed)).normalized;

            var next = (Vector2)transform.position + move * (moveSpeed * Time.deltaTime);
            next.x = Mathf.Clamp(next.x, moveAreaMin.x, moveAreaMax.x);
            next.y = Mathf.Clamp(next.y, moveAreaMin.y, moveAreaMax.y);
            // 物理演算を使わず、入力している間だけ一定速度で動かします。
            // キーを離すと、その場ですぐに止まります。
            transform.position = next;

            var wantsShot = keyboard.spaceKey.isPressed || keyboard.zKey.isPressed;
            if (wantsShot && Time.time >= nextShotTime) Shoot();
        }

        private void Shoot()
        {
            nextShotTime = Time.time + shotInterval;
            if (bulletPrefab != null) Instantiate(bulletPrefab, (Vector2)transform.position + muzzleOffset, Quaternion.identity);
        }

        private static float ReadAxis(bool negative, bool positive) => negative == positive ? 0f : positive ? 1f : -1f;

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(0.49f, 0.83f, 0.99f, 0.95f);
            var center = (moveAreaMin + moveAreaMax) * 0.5f;
            var size = moveAreaMax - moveAreaMin;
            Gizmos.DrawWireCube(center, size);
            Gizmos.color = new Color(1f, 0.82f, 0.2f, 1f);
            Gizmos.DrawWireSphere((Vector2)transform.position + muzzleOffset, 0.16f);
        }
    }
}
