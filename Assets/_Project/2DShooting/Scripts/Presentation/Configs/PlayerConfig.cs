using TwoDShooting.Proto.Domain;
using UnityEngine;

namespace TwoDShooting.Proto.Presentation
{
    [CreateAssetMenu(menuName = "2DShooting/Player Config")]
    public sealed class PlayerConfig : ScriptableObject
    {
        [Tooltip("自機に表示する画像です。未設定の場合はPrefab側の画像を使います。")]
        [SerializeField] private Sprite sprite;
        [Tooltip("自機の色です。")]
        [SerializeField] private Color tint = new(0.1f, 0.65f, 1f, 1f);
        [Tooltip("自機の大きさです。Xが横、Yが縦の倍率です。")]
        [SerializeField] private Vector3 localScale = new(1.2f, 0.8f, 1f);
        [Tooltip("自機のHPです。今のプロトタイプでは拡張用の値です。")]
        [SerializeField] private int maxHealth = 3;
        [Tooltip("自機の移動速度です。大きいほど速く動きます。")]
        [SerializeField] private float moveSpeed = 6f;
        [Tooltip("自機の移動方法です。Freeは上下左右、HorizontalOnlyは横のみ、VerticalOnlyは縦のみです。")]
        [SerializeField] private MovementMode movementMode = MovementMode.Free;
        [Tooltip("ゲーム開始時の自機の位置です。")]
        [SerializeField] private Vector2 spawnPosition = new(-6f, 0f);
        [Tooltip("自機が移動できる左下の限界位置です。")]
        [SerializeField] private Vector2 movementMin = new(-8f, -4f);
        [Tooltip("自機が移動できる右上の限界位置です。")]
        [SerializeField] private Vector2 movementMax = new(8f, 4f);
        [Tooltip("弾を撃てる間隔です。小さいほど連射できます。")]
        [SerializeField] private float fireInterval = 0.25f;
        [Tooltip("自機の中心から見た弾の発射位置です。")]
        [SerializeField] private Vector2 muzzleOffset = new(0.8f, 0f);

        public Sprite Sprite => sprite;
        public Color Tint => tint;
        public Vector3 LocalScale => localScale;
        public int MaxHealth => maxHealth;
        public float MoveSpeed => moveSpeed;
        public MovementMode MovementMode => movementMode;
        public Vector2 SpawnPosition => spawnPosition;
        public Vector2 MovementMin => movementMin;
        public Vector2 MovementMax => movementMax;
        public float FireInterval => fireInterval;
        public Vector2 MuzzleOffset => muzzleOffset;
    }
}
