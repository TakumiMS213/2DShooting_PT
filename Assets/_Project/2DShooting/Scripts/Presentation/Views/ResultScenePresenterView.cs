using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace TwoDShooting.Proto.Presentation
{
    public sealed class ResultScenePresenterView : MonoBehaviour
    {
        [SerializeField] private StageConfig stageConfig;
        [SerializeField] private Button retryButton;
        [SerializeField] private Button titleButton;
        [SerializeField] private string fallbackGameSceneName = "GameScene";
        [SerializeField] private string fallbackTitleSceneName = "TitleScene";

        private void Awake()
        {
            if (retryButton != null) retryButton.onClick.AddListener(LoadGame);
            if (titleButton != null) titleButton.onClick.AddListener(LoadTitle);
        }

        private void OnDestroy()
        {
            if (retryButton != null) retryButton.onClick.RemoveListener(LoadGame);
            if (titleButton != null) titleButton.onClick.RemoveListener(LoadTitle);
        }

        private void Update()
        {
            var keyboard = Keyboard.current;
            if (keyboard == null) return;
            if (keyboard.enterKey.wasPressedThisFrame || keyboard.spaceKey.wasPressedThisFrame) LoadGame();
            if (keyboard.escapeKey.wasPressedThisFrame) LoadTitle();
        }

        private void LoadGame()
        {
            var sceneName = stageConfig != null ? stageConfig.GameSceneName : fallbackGameSceneName;
            if (!string.IsNullOrEmpty(sceneName)) SceneManager.LoadScene(sceneName);
        }

        private void LoadTitle()
        {
            var sceneName = stageConfig != null ? stageConfig.TitleSceneName : fallbackTitleSceneName;
            if (!string.IsNullOrEmpty(sceneName)) SceneManager.LoadScene(sceneName);
        }
    }
}
