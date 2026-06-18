using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace TwoDShooting.Proto.Presentation
{
    public sealed class TitleScenePresenterView : MonoBehaviour
    {
        [SerializeField] private StageConfig stageConfig;
        [SerializeField] private Button startButton;
        [SerializeField] private string fallbackGameSceneName = "GameScene";

        private void Awake()
        {
            if (startButton != null) startButton.onClick.AddListener(LoadGame);
        }

        private void OnDestroy()
        {
            if (startButton != null) startButton.onClick.RemoveListener(LoadGame);
        }

        private void Update()
        {
            var keyboard = Keyboard.current;
            if (keyboard != null && (keyboard.enterKey.wasPressedThisFrame || keyboard.spaceKey.wasPressedThisFrame))
            {
                LoadGame();
            }
        }

        private void LoadGame()
        {
            var sceneName = stageConfig != null ? stageConfig.GameSceneName : fallbackGameSceneName;
            if (!string.IsNullOrEmpty(sceneName)) SceneManager.LoadScene(sceneName);
        }
    }
}
