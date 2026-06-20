using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace TwoDShooting.Simple
{
    /// <summary>タイトルと結果画面のボタンだけを担当します。</summary>
    public sealed class ScreenFlow : MonoBehaviour
    {
        public enum ScreenKind { Title, Result }

        [SerializeField] private ScreenKind screenKind;
        [SerializeField] private Button primaryButton;
        [SerializeField] private Button secondaryButton;

        private void Awake()
        {
            if (primaryButton != null) primaryButton.onClick.AddListener(LoadGame);
            if (secondaryButton != null) secondaryButton.onClick.AddListener(LoadTitle);
        }

        private void OnDestroy()
        {
            if (primaryButton != null) primaryButton.onClick.RemoveListener(LoadGame);
            if (secondaryButton != null) secondaryButton.onClick.RemoveListener(LoadTitle);
        }

        private void Update()
        {
            var keyboard = Keyboard.current;
            if (keyboard == null) return;
            if (keyboard.enterKey.wasPressedThisFrame || keyboard.spaceKey.wasPressedThisFrame) LoadGame();
            if (screenKind == ScreenKind.Result && keyboard.escapeKey.wasPressedThisFrame) LoadTitle();
        }

        public void LoadGame() => SceneManager.LoadScene("GameScene");
        public void LoadTitle() => SceneManager.LoadScene("TitleScene");
    }
}
