using TwoDShooting.Proto.Domain;
using UnityEngine;

namespace TwoDShooting.Proto.Presentation
{
    [CreateAssetMenu(menuName = "2DShooting/Bullet Config")]
    public sealed class BulletConfig : ScriptableObject
    {
        [Tooltip("弾に表示する画像です。未設定の場合はPrefab側の画像を使います。")]
        [SerializeField] private Sprite sprite;
        [Tooltip("弾の色です。")]
        [SerializeField] private Color tint = new(1f, 0.9f, 0.15f, 1f);
        [Tooltip("弾の大きさです。Xが横、Yが縦の倍率です。")]
        [SerializeField] private Vector3 localScale = new(0.35f, 0.18f, 1f);
        [Tooltip("敵に当たった時に与えるダメージです。")]
        [SerializeField] private int damage = 1;
        [Tooltip("同時に用意しておく弾の数です。多いほど連射に強くなります。")]
        [SerializeField] private int poolSize = 8;
        [Tooltip("弾の移動速度です。大きいほど速く飛びます。")]
        [SerializeField] private float speed = 12f;
        [Tooltip("弾が自動で消えるまでの秒数です。")]
        [SerializeField] private float lifeTime = 2f;
        [Tooltip("弾の飛び方です。Straightは直線、SineWaveは上下に揺れます。")]
        [SerializeField] private BulletTrajectory trajectory = BulletTrajectory.Straight;
        [Tooltip("SineWave時の揺れ幅です。")]
        [SerializeField] private float sineAmplitude = 0.4f;
        [Tooltip("SineWave時の揺れの速さです。")]
        [SerializeField] private float sineFrequency = 7f;

        public Sprite Sprite => sprite;
        public Color Tint => tint;
        public Vector3 LocalScale => localScale;
        public int Damage => damage;
        public int PoolSize => Mathf.Max(1, poolSize);
        public float Speed => speed;
        public float LifeTime => lifeTime;
        public BulletTrajectory Trajectory => trajectory;
        public float SineAmplitude => sineAmplitude;
        public float SineFrequency => sineFrequency;
    }
}
