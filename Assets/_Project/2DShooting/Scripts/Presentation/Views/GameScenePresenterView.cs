using TwoDShooting.Proto.Application;
using UnityEngine;

namespace TwoDShooting.Proto.Presentation
{
    public sealed class GameScenePresenterView : MonoBehaviour
    {
        [SerializeField] private StageConfig stageConfig;
        [SerializeField] private GameHudView hudView;
        [SerializeField] private SceneResultGateway resultGateway;
        [SerializeField] private EnemySpawnerView enemySpawnerView;

        private GameProgressPresenter _presenter;

        public void Configure(StageConfig config, GameHudView hud, SceneResultGateway gateway, EnemySpawnerView spawner)
        {
            stageConfig = config;
            hudView = hud;
            resultGateway = gateway;
            enemySpawnerView = spawner;
        }

        private void Start()
        {
            if (stageConfig == null) return;
            resultGateway?.Configure(stageConfig);
            _presenter = new GameProgressPresenter(stageConfig.RequiredEnemyDefeats, stageConfig.ResultDelaySeconds, hudView, resultGateway);
            _presenter.Start();

            if (enemySpawnerView != null)
            {
                enemySpawnerView.EnemyDefeated += HandleEnemyDefeated;
                enemySpawnerView.SpawnAll();
            }
        }

        private void OnDestroy()
        {
            if (enemySpawnerView != null) enemySpawnerView.EnemyDefeated -= HandleEnemyDefeated;
        }

        private void HandleEnemyDefeated()
        {
            _presenter?.NotifyEnemyDefeated();
        }
    }
}
