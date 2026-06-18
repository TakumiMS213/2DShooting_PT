using TwoDShooting.Proto.Application;
using TwoDShooting.Proto.Domain;
using UnityEngine;

namespace TwoDShooting.Proto.Presentation
{
    public sealed class PlayerView : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Rigidbody2D cachedRigidbody;
        [SerializeField] private BulletPoolView bulletPoolView;

        private PlayerConfig _config;
        private IPlayerInputSource _inputSource;
        private PlayerFirePresenter _firePresenter;
        private Vector2 _move;

        private void Awake()
        {
            if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
            if (cachedRigidbody == null) cachedRigidbody = GetComponent<Rigidbody2D>();
        }

        public void Configure(PlayerConfig config, IPlayerInputSource inputSource, BulletPoolView bulletPool)
        {
            _config = config;
            _inputSource = inputSource;
            bulletPoolView = bulletPool;
            _firePresenter = _config != null ? new PlayerFirePresenter(_config.FireInterval) : null;
            if (_config == null) return;

            if (spriteRenderer != null)
            {
                if (_config.Sprite != null) spriteRenderer.sprite = _config.Sprite;
                spriteRenderer.color = _config.Tint;
            }

            transform.position = _config.SpawnPosition;
            transform.localScale = _config.LocalScale;
        }

        private void Update()
        {
            if (_config == null || _inputSource == null)
            {
                _move = Vector2.zero;
                return;
            }

            _move = FilterMove(_inputSource.Move);
            if (_firePresenter != null && _firePresenter.TryConsumeFire(_inputSource.FirePressed, Time.time)) Fire();
        }

        private void FixedUpdate()
        {
            if (_config == null || cachedRigidbody == null) return;
            var next = cachedRigidbody.position + _move * (_config.MoveSpeed * Time.fixedDeltaTime);
            next.x = Mathf.Clamp(next.x, _config.MovementMin.x, _config.MovementMax.x);
            next.y = Mathf.Clamp(next.y, _config.MovementMin.y, _config.MovementMax.y);
            cachedRigidbody.MovePosition(next);
        }

        private Vector2 FilterMove(Vector2 move)
        {
            return _config.MovementMode switch
            {
                MovementMode.HorizontalOnly => new Vector2(move.x, 0f),
                MovementMode.VerticalOnly => new Vector2(0f, move.y),
                _ => move
            };
        }

        private void Fire()
        {
            if (_config == null || bulletPoolView == null) return;
            bulletPoolView.Fire((Vector2)transform.position + _config.MuzzleOffset, Vector2.right);
        }
    }
}
