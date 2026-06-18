using TwoDShooting.Proto.Domain;
using UnityEngine;

namespace TwoDShooting.Proto.Application
{
    public interface IPlayerInputSource
    {
        Vector2 Move { get; }
        bool FirePressed { get; }
    }

    public interface IGameHudView
    {
        void SetProgress(int defeatedEnemies, int requiredDefeats);
    }

    public interface IResultSceneGateway
    {
        void LoadResult(float delaySeconds);
    }

    public sealed class PlayerFirePresenter
    {
        private readonly float _intervalSeconds;
        private float _lastFireTime = -999f;

        public PlayerFirePresenter(float intervalSeconds)
        {
            _intervalSeconds = Mathf.Max(0.01f, intervalSeconds);
        }

        public bool TryConsumeFire(bool firePressed, float timeSeconds)
        {
            if (!firePressed || timeSeconds - _lastFireTime < _intervalSeconds)
            {
                return false;
            }

            _lastFireTime = timeSeconds;
            return true;
        }
    }

    public sealed class GameProgressPresenter
    {
        private readonly GameProgressModel _model;
        private readonly IGameHudView _hudView;
        private readonly IResultSceneGateway _resultGateway;
        private readonly float _resultDelaySeconds;
        private bool _resultRequested;

        public GameProgressPresenter(
            int requiredDefeats,
            float resultDelaySeconds,
            IGameHudView hudView,
            IResultSceneGateway resultGateway)
        {
            _model = new GameProgressModel(requiredDefeats);
            _resultDelaySeconds = Mathf.Max(0f, resultDelaySeconds);
            _hudView = hudView;
            _resultGateway = resultGateway;
        }

        public void Start()
        {
            _hudView?.SetProgress(_model.DefeatedEnemies, _model.RequiredDefeats);
        }

        public void NotifyEnemyDefeated()
        {
            if (_resultRequested)
            {
                return;
            }

            _model.AddDefeat();
            _hudView?.SetProgress(_model.DefeatedEnemies, _model.RequiredDefeats);

            if (_model.IsComplete)
            {
                _resultRequested = true;
                _resultGateway?.LoadResult(_resultDelaySeconds);
            }
        }
    }
}
