using TwoDShooting.Proto.Application;
using UnityEngine;

namespace TwoDShooting.Proto.Presentation
{
    public sealed class GameInstaller : MonoBehaviour
    {
        [SerializeField] private PlayerConfig playerConfig;
        [SerializeField] private EnemyConfig enemyConfig;
        [SerializeField] private BulletConfig bulletConfig;
        [SerializeField] private StageConfig stageConfig;
        [SerializeField] private PlayerView playerView;
        [Tooltip("使用する弾Prefabです。複数登録できます。")]
        [SerializeField] private GameObject[] bulletPrefabs;
        [SerializeField] private BulletPoolView bulletPoolView;
        [Tooltip("使用する敵Prefabです。複数登録できます。")]
        [SerializeField] private GameObject[] enemyPrefabs;
        [SerializeField] private EnemySpawnerView enemySpawnerView;
        [SerializeField] private GameScenePresenterView presenterView;
        [SerializeField] private GameHudView hudView;
        [SerializeField] private SceneResultGateway resultGateway;
        [SerializeField] private MonoBehaviour inputSourceBehaviour;

        private void Awake()
        {
            EnsureDefaults();

            var inputSource = inputSourceBehaviour as IPlayerInputSource;
            bulletPoolView?.Configure(bulletPrefabs, bulletConfig);
            playerView?.Configure(playerConfig, inputSource, bulletPoolView);
            enemySpawnerView?.Configure(enemyConfig, enemyPrefabs);
            presenterView?.Configure(stageConfig, hudView, resultGateway, enemySpawnerView);
        }

        private void EnsureDefaults()
        {
            playerConfig ??= ScriptableObject.CreateInstance<PlayerConfig>();
            enemyConfig ??= ScriptableObject.CreateInstance<EnemyConfig>();
            bulletConfig ??= ScriptableObject.CreateInstance<BulletConfig>();
            stageConfig ??= ScriptableObject.CreateInstance<StageConfig>();

            bulletPoolView ??= FindFirstObjectByType<BulletPoolView>();
            enemySpawnerView ??= FindFirstObjectByType<EnemySpawnerView>();
            presenterView ??= FindFirstObjectByType<GameScenePresenterView>();
            hudView ??= FindFirstObjectByType<GameHudView>();
            resultGateway ??= FindFirstObjectByType<SceneResultGateway>();
            playerView ??= FindFirstObjectByType<PlayerView>();

            bulletPoolView ??= new GameObject("BulletPool").AddComponent<BulletPoolView>();
            enemySpawnerView ??= new GameObject("EnemySpawner").AddComponent<EnemySpawnerView>();
            presenterView ??= new GameObject("GameScenePresenter").AddComponent<GameScenePresenterView>();
            resultGateway ??= new GameObject("SceneResultGateway").AddComponent<SceneResultGateway>();

            if (inputSourceBehaviour == null)
            {
                var inputType = System.Type.GetType("TwoDShooting.Proto.Infrastructure.KeyboardPlayerInputSource, Assembly-CSharp");
                if (inputType != null)
                {
                    inputSourceBehaviour = new GameObject("KeyboardInput").AddComponent(inputType) as MonoBehaviour;
                }
            }

            playerView ??= CreatePlayer();
            if (!HasAnyBulletPrefab())
            {
                bulletPrefabs = new[] { CreateBulletPrefab().gameObject };
            }

            if (!HasAnyEnemyPrefab())
            {
                enemyPrefabs = new[] { CreateEnemyPrefab().gameObject };
            }
        }

        private bool HasAnyBulletPrefab()
        {
            if (bulletPrefabs == null)
            {
                return false;
            }

            for (var i = 0; i < bulletPrefabs.Length; i++)
            {
                if (bulletPrefabs[i] != null)
                {
                    return true;
                }
            }

            return false;
        }

        private bool HasAnyEnemyPrefab()
        {
            if (enemyPrefabs == null)
            {
                return false;
            }

            for (var i = 0; i < enemyPrefabs.Length; i++)
            {
                if (enemyPrefabs[i] != null)
                {
                    return true;
                }
            }

            return false;
        }

        private static PlayerView CreatePlayer()
        {
            var go = new GameObject("Player");
            var spriteRenderer = go.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = CreateRuntimeSprite();
            var rigidbody = go.AddComponent<Rigidbody2D>();
            rigidbody.gravityScale = 0f;
            rigidbody.bodyType = RigidbodyType2D.Kinematic;
            return go.AddComponent<PlayerView>();
        }

        private static BulletView CreateBulletPrefab()
        {
            var go = new GameObject("BulletRuntimePrefab");
            go.SetActive(false);
            var spriteRenderer = go.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = CreateRuntimeSprite();
            var rigidbody = go.AddComponent<Rigidbody2D>();
            rigidbody.gravityScale = 0f;
            rigidbody.bodyType = RigidbodyType2D.Kinematic;
            var collider = go.AddComponent<BoxCollider2D>();
            collider.isTrigger = true;
            return go.AddComponent<BulletView>();
        }

        private static EnemyView CreateEnemyPrefab()
        {
            var go = new GameObject("EnemyRuntimePrefab");
            go.SetActive(false);
            var spriteRenderer = go.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = CreateRuntimeSprite();
            var collider = go.AddComponent<BoxCollider2D>();
            collider.isTrigger = true;
            return go.AddComponent<EnemyView>();
        }

        private static Sprite CreateRuntimeSprite()
        {
            var texture = new Texture2D(16, 16, TextureFormat.RGBA32, false);
            var pixels = new Color[16 * 16];
            for (var i = 0; i < pixels.Length; i++) pixels[i] = Color.white;
            texture.SetPixels(pixels);
            texture.Apply();
            return Sprite.Create(texture, new Rect(0f, 0f, 16f, 16f), new Vector2(0.5f, 0.5f), 16f);
        }
    }

}
