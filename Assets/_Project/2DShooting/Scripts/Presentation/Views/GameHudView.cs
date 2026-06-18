using TwoDShooting.Proto.Application;
using UnityEngine;
using UnityEngine.UI;

namespace TwoDShooting.Proto.Presentation
{
    public sealed class GameHudView : MonoBehaviour, IGameHudView
    {
        [SerializeField] private Text progressText;

        public void SetProgress(int defeatedEnemies, int requiredDefeats)
        {
            if (progressText != null) progressText.text = $"Enemy {defeatedEnemies}/{requiredDefeats}";
        }
    }
}
