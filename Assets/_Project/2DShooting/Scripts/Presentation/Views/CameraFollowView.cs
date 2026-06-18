using UnityEngine;

namespace TwoDShooting.Proto.Presentation
{
    public sealed class CameraFollowView : MonoBehaviour
    {
        [SerializeField] private StageConfig stageConfig;
        [SerializeField] private Transform target;
        [SerializeField] private float followSpeed = 8f;

        public void Configure(StageConfig config, Transform followTarget)
        {
            stageConfig = config;
            target = followTarget;
        }

        private void LateUpdate()
        {
            if (stageConfig == null || target == null) return;
            var current = transform.position;
            var targetPosition = new Vector3(
                Mathf.Clamp(target.position.x, stageConfig.CameraMin.x, stageConfig.CameraMax.x),
                Mathf.Clamp(target.position.y, stageConfig.CameraMin.y, stageConfig.CameraMax.y),
                current.z);
            transform.position = Vector3.Lerp(current, targetPosition, followSpeed * Time.deltaTime);
        }
    }
}
