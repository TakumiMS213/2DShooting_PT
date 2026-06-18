using System;
using TwoDShooting.Proto.Domain;
using UnityEngine;

namespace TwoDShooting.Proto.Presentation
{
    public sealed class EnemyView : MonoBehaviour
    {
        public event Action<EnemyView> Defeated;

        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Collider2D cachedCollider;

        private EnemyConfig _config;
        private DamageableModel _damageableModel;
        private Vector2 _origin;
        private bool _isDefeatNotified;

        private void Awake()
        {
            if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
            if (cachedCollider == null) cachedCollider = GetComponent<Collider2D>();
        }

        public void Configure(EnemyConfig config, Vector2 spawnPosition)
        {
            _config = config;
            _origin = spawnPosition;
            _isDefeatNotified = false;
            transform.position = spawnPosition;
            if (_config == null) return;

            _damageableModel = new DamageableModel(_config.MaxHealth);
            _damageableModel.Defeated += HandleDefeated;

            if (spriteRenderer != null)
            {
                if (_config.Sprite != null) spriteRenderer.sprite = _config.Sprite;
                spriteRenderer.color = _config.Tint;
            }

            transform.localScale = _config.LocalScale;
        }

        public void ApplyDamage(int damage)
        {
            _damageableModel?.ApplyDamage(damage);
        }

        private void Update()
        {
            if (_config == null) return;
            var offsetX = Mathf.Sin(Time.time * _config.MoveSpeed) * _config.PatrolDistance;
            transform.position = new Vector2(_origin.x + offsetX, _origin.y);
        }

        private void HandleDefeated()
        {
            if (_isDefeatNotified) return;
            _isDefeatNotified = true;
            if (cachedCollider != null) cachedCollider.enabled = false;
            Defeated?.Invoke(this);
            gameObject.SetActive(false);
        }
    }
}
