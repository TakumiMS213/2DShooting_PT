using TwoDShooting.Proto.Domain;
using UnityEngine;

namespace TwoDShooting.Proto.Presentation
{
    public sealed class BulletView : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Rigidbody2D cachedRigidbody;

        private BulletConfig _config;
        private Vector2 _direction;
        private Vector2 _basePosition;
        private float _elapsed;

        public bool IsActive => gameObject.activeSelf;

        private void Awake()
        {
            if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
            if (cachedRigidbody == null) cachedRigidbody = GetComponent<Rigidbody2D>();
        }

        public void Configure(BulletConfig config)
        {
            _config = config;
            if (_config == null || spriteRenderer == null) return;
            if (_config.Sprite != null) spriteRenderer.sprite = _config.Sprite;
            spriteRenderer.color = _config.Tint;
            transform.localScale = _config.LocalScale;
        }

        public void Launch(Vector2 position, Vector2 direction)
        {
            if (_config == null) return;
            _direction = direction.normalized;
            _basePosition = position;
            _elapsed = 0f;
            transform.position = position;
            gameObject.SetActive(true);
        }

        private void Update()
        {
            if (_config == null)
            {
                gameObject.SetActive(false);
                return;
            }

            _elapsed += Time.deltaTime;
            _basePosition += _direction * (_config.Speed * Time.deltaTime);
            var offset = Vector2.zero;

            if (_config.Trajectory == BulletTrajectory.SineWave)
            {
                var side = new Vector2(-_direction.y, _direction.x);
                offset = side * (Mathf.Sin(_elapsed * _config.SineFrequency) * _config.SineAmplitude);
            }

            transform.position = _basePosition + offset;

            if (_elapsed >= _config.LifeTime) gameObject.SetActive(false);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_config == null || other == null) return;
            if (other.TryGetComponent(out EnemyView enemyView))
            {
                enemyView.ApplyDamage(_config.Damage);
                gameObject.SetActive(false);
            }
        }
    }
}
