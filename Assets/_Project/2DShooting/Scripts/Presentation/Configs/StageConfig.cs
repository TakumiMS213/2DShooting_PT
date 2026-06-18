using UnityEngine;

namespace TwoDShooting.Proto.Presentation
{
    [CreateAssetMenu(menuName = "2DShooting/Stage Config")]
    public sealed class StageConfig : ScriptableObject
    {
        [Tooltip("タイトル画面のScene名です。")]
        [SerializeField] private string titleSceneName = "TitleScene";
        [Tooltip("ゲーム画面のScene名です。")]
        [SerializeField] private string gameSceneName = "GameScene";
        [Tooltip("リザルト画面のScene名です。")]
        [SerializeField] private string resultSceneName = "ResultScene";
        [Tooltip("クリアに必要な敵撃破数です。")]
        [SerializeField] private int requiredEnemyDefeats = 1;
        [Tooltip("クリア後、リザルト画面へ移動するまでの秒数です。")]
        [SerializeField] private float resultDelaySeconds = 0.7f;
        [Tooltip("カメラが追従できる左下の限界位置です。")]
        [SerializeField] private Vector2 cameraMin = new(-8f, -4.5f);
        [Tooltip("カメラが追従できる右上の限界位置です。")]
        [SerializeField] private Vector2 cameraMax = new(8f, 4.5f);
        [Tooltip("背景が左へ流れる速さです。")]
        [SerializeField] private float backgroundScrollSpeed = 1.5f;

        public string TitleSceneName => titleSceneName;
        public string GameSceneName => gameSceneName;
        public string ResultSceneName => resultSceneName;
        public int RequiredEnemyDefeats => Mathf.Max(1, requiredEnemyDefeats);
        public float ResultDelaySeconds => resultDelaySeconds;
        public Vector2 CameraMin => cameraMin;
        public Vector2 CameraMax => cameraMax;
        public float BackgroundScrollSpeed => backgroundScrollSpeed;
    }
}
