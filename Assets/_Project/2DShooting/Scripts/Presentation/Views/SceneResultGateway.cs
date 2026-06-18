using System.Collections;
using TwoDShooting.Proto.Application;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TwoDShooting.Proto.Presentation
{
    public sealed class SceneResultGateway : MonoBehaviour, IResultSceneGateway
    {
        [SerializeField] private StageConfig stageConfig;

        public void Configure(StageConfig config)
        {
            stageConfig = config;
        }

        public void LoadResult(float delaySeconds)
        {
            if (stageConfig != null) StartCoroutine(LoadResultRoutine(delaySeconds));
        }

        private IEnumerator LoadResultRoutine(float delaySeconds)
        {
            if (delaySeconds > 0f) yield return new WaitForSeconds(delaySeconds);
            if (stageConfig != null && !string.IsNullOrEmpty(stageConfig.ResultSceneName))
            {
                SceneManager.LoadScene(stageConfig.ResultSceneName);
            }
        }
    }
}
