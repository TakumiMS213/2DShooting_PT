using UnityEngine;

namespace TwoDShooting.Proto.Presentation
{
    public sealed class BackgroundScrollView : MonoBehaviour
    {
        [SerializeField] private StageConfig stageConfig;
        [SerializeField] private float wrapWidth = 18f;

        private void Update()
        {
            if (stageConfig == null) return;
            transform.Translate(Vector3.left * (stageConfig.BackgroundScrollSpeed * Time.deltaTime), Space.World);
            if (transform.position.x <= -wrapWidth) transform.position += Vector3.right * wrapWidth;
        }
    }
}
