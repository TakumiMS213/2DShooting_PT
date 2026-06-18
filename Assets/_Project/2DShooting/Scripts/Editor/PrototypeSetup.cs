using System.IO;
using TwoDShooting.Proto.Infrastructure;
using TwoDShooting.Proto.Presentation;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace TwoDShooting.Proto.Editor
{
    public static class PrototypeSetup
    {
        private const string Root = "Assets/_Project/2DShooting";
        private const string ConfigRoot = Root + "/Configs";
        private const string PrefabRoot = Root + "/Prefabs";
        private const string SceneRoot = Root + "/Scenes";
        private const string ArtRoot = Root + "/Imagea";

        [InitializeOnLoadMethod]
        private static void AutoBuildPrototypeOnce()
        {
            if (AssetDatabase.LoadAssetAtPath<SceneAsset>(SceneRoot + "/GameScene.unity") != null)
            {
                return;
            }

            if (SessionState.GetBool("2DShooting.Prototype.AutoBuildQueued", false))
            {
                return;
            }

            SessionState.SetBool("2DShooting.Prototype.AutoBuildQueued", true);
            EditorApplication.delayCall += BuildPrototype;
        }

        [MenuItem("Tools/2DShooting/Build Prototype")]
        public static void BuildPrototype()
        {
            CreateFolders();
            DeleteCoreGeneratedAssets();

            var squareSprite = CreateSquareSprite();
            var playerConfig = CreateAsset<PlayerConfig>(ConfigRoot + "/PlayerConfig.asset");
            var enemyConfig = CreateAsset<EnemyConfig>(ConfigRoot + "/EnemyConfig.asset");
            var bulletConfig = CreateAsset<BulletConfig>(ConfigRoot + "/BulletConfig.asset");
            var stageConfig = CreateAsset<StageConfig>(ConfigRoot + "/StageConfig.asset");

            SetObject(playerConfig, "sprite", squareSprite);
            SetObject(enemyConfig, "sprite", squareSprite);
            SetObject(bulletConfig, "sprite", squareSprite);

            var bulletPrefab = CreateBulletPrefab(squareSprite, "Bullet", new Color(1f, 0.9f, 0.15f, 1f));
            var bulletAltPrefab = CreateBulletPrefab(squareSprite, "Bullet_Alt", new Color(0.35f, 1f, 0.85f, 1f));
            var playerPrefab = CreatePlayerPrefab(squareSprite);
            var enemyPrefab = CreateEnemyPrefab(squareSprite, "Enemy", new Color(1f, 0.25f, 0.2f, 1f), Vector3.one);
            var enemyAltPrefab = CreateEnemyPrefab(squareSprite, "Enemy_Alt", new Color(1f, 0.55f, 0.15f, 1f), new Vector3(1.15f, 1.15f, 1f));

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            playerConfig = LoadAsset<PlayerConfig>(ConfigRoot + "/PlayerConfig.asset");
            enemyConfig = LoadAsset<EnemyConfig>(ConfigRoot + "/EnemyConfig.asset");
            bulletConfig = LoadAsset<BulletConfig>(ConfigRoot + "/BulletConfig.asset");
            stageConfig = LoadAsset<StageConfig>(ConfigRoot + "/StageConfig.asset");
            var titleScene = BuildTitleScene(stageConfig);
            var gameScene = BuildGameScene(
                stageConfig,
                playerConfig,
                enemyConfig,
                bulletConfig,
                playerPrefab,
                new[] { enemyPrefab, enemyAltPrefab },
                new[] { bulletPrefab, bulletAltPrefab });
            var resultScene = BuildResultScene(stageConfig);

            EditorBuildSettings.scenes = new[]
            {
                new EditorBuildSettingsScene(titleScene, true),
                new EditorBuildSettingsScene(gameScene, true),
                new EditorBuildSettingsScene(resultScene, true)
            };

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorSceneManager.OpenScene(titleScene);
            Debug.Log("2DShooting prototype generated.");
        }

        private static void CreateFolders()
        {
            Directory.CreateDirectory(ConfigRoot);
            Directory.CreateDirectory(PrefabRoot);
            Directory.CreateDirectory(SceneRoot);
            Directory.CreateDirectory(ArtRoot);
        }

        private static void DeleteCoreGeneratedAssets()
        {
            DeleteAsset(ConfigRoot + "/PlayerConfig.asset");
            DeleteAsset(ConfigRoot + "/EnemyConfig.asset");
            DeleteAsset(ConfigRoot + "/BulletConfig.asset");
            DeleteAsset(ConfigRoot + "/StageConfig.asset");
            DeleteAsset(PrefabRoot + "/Player.prefab");
            DeleteAsset(PrefabRoot + "/Enemy.prefab");
            DeleteAsset(PrefabRoot + "/Enemy_Alt.prefab");
            DeleteAsset(PrefabRoot + "/Bullet.prefab");
            DeleteAsset(PrefabRoot + "/Bullet_Alt.prefab");
        }

        private static void DeleteAsset(string path)
        {
            if (AssetDatabase.LoadAssetAtPath<Object>(path) != null)
            {
                AssetDatabase.DeleteAsset(path);
            }
        }

        private static Sprite CreateSquareSprite()
        {
            var path = ArtRoot + "/PrototypeSquare.png";
            if (!File.Exists(path))
            {
                var texture = new Texture2D(16, 16, TextureFormat.RGBA32, false);
                var pixels = new Color[16 * 16];

                for (var i = 0; i < pixels.Length; i++)
                {
                    pixels[i] = Color.white;
                }

                texture.SetPixels(pixels);
                texture.Apply();
                File.WriteAllBytes(path, texture.EncodeToPNG());
                Object.DestroyImmediate(texture);
                AssetDatabase.ImportAsset(path);
            }

            var importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer != null)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.spritePixelsPerUnit = 16f;
                importer.filterMode = FilterMode.Point;
                importer.SaveAndReimport();
            }

            return AssetDatabase.LoadAssetAtPath<Sprite>(path);
        }

        private static T CreateAsset<T>(string path) where T : ScriptableObject
        {
            var asset = AssetDatabase.LoadAssetAtPath<T>(path);
            if (asset != null)
            {
                return asset;
            }

            asset = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(asset, path);
            return asset;
        }

        private static T LoadAsset<T>(string path) where T : Object
        {
            return AssetDatabase.LoadAssetAtPath<T>(path);
        }

        private static GameObject CreateBulletPrefab(Sprite sprite, string prefabName, Color color)
        {
            var path = PrefabRoot + "/" + prefabName + ".prefab";
            var go = new GameObject(prefabName);
            var renderer = go.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.color = color;

            var rigidbody = go.AddComponent<Rigidbody2D>();
            rigidbody.gravityScale = 0f;
            rigidbody.bodyType = RigidbodyType2D.Kinematic;

            var collider = go.AddComponent<BoxCollider2D>();
            collider.isTrigger = true;

            var view = go.AddComponent<BulletView>();
            SetObject(view, "spriteRenderer", renderer);
            SetObject(view, "cachedRigidbody", rigidbody);

            var saved = PrefabUtility.SaveAsPrefabAsset(go, path);
            Object.DestroyImmediate(go);
            return saved;
        }

        private static PlayerView CreatePlayerPrefab(Sprite sprite)
        {
            var path = PrefabRoot + "/Player.prefab";
            var go = new GameObject("Player");
            var renderer = go.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.color = new Color(0.1f, 0.65f, 1f, 1f);

            var rigidbody = go.AddComponent<Rigidbody2D>();
            rigidbody.gravityScale = 0f;
            rigidbody.bodyType = RigidbodyType2D.Kinematic;

            var view = go.AddComponent<PlayerView>();
            SetObject(view, "spriteRenderer", renderer);
            SetObject(view, "cachedRigidbody", rigidbody);

            var saved = PrefabUtility.SaveAsPrefabAsset(go, path).GetComponent<PlayerView>();
            Object.DestroyImmediate(go);
            return saved;
        }

        private static GameObject CreateEnemyPrefab(Sprite sprite, string prefabName, Color color, Vector3 scale)
        {
            var path = PrefabRoot + "/" + prefabName + ".prefab";
            var go = new GameObject(prefabName);
            go.transform.localScale = scale;
            var renderer = go.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.color = color;

            var collider = go.AddComponent<BoxCollider2D>();
            collider.isTrigger = true;

            var view = go.AddComponent<EnemyView>();
            SetObject(view, "spriteRenderer", renderer);
            SetObject(view, "cachedCollider", collider);

            var saved = PrefabUtility.SaveAsPrefabAsset(go, path);
            Object.DestroyImmediate(go);
            return saved;
        }

        private static string BuildTitleScene(StageConfig stageConfig)
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            scene.name = "TitleScene";
            CreateCamera();
            var canvas = CreateCanvas();
            var startButton = CreateButton(canvas.transform, "StartButton", "START", new Vector2(0f, -50f), new Vector2(220f, 64f));
            CreateText(canvas.transform, "TitleText", "2D SHOOTING", new Vector2(0f, 90f), new Vector2(520f, 80f), 44);
            CreateText(canvas.transform, "GuideText", "WASD / Arrows: Move   Space / Z: Shot", new Vector2(0f, 15f), new Vector2(620f, 40f), 22);

            var presenter = new GameObject("TitlePresenter").AddComponent<TitleScenePresenterView>();
            SetObject(presenter, "stageConfig", stageConfig);
            SetObject(presenter, "startButton", startButton);

            var path = SceneRoot + "/TitleScene.unity";
            EditorSceneManager.SaveScene(scene, path);
            return path;
        }

        private static string BuildGameScene(
            StageConfig stageConfig,
            PlayerConfig playerConfig,
            EnemyConfig enemyConfig,
            BulletConfig bulletConfig,
            PlayerView playerPrefab,
            GameObject[] enemyPrefabs,
            GameObject[] bulletPrefabs)
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            scene.name = "GameScene";

            var camera = CreateCamera();
            camera.orthographicSize = 5f;

            var playerObject = PrefabUtility.InstantiatePrefab(playerPrefab) as GameObject;
            var player = playerObject != null ? playerObject.GetComponent<PlayerView>() : null;
            var bulletPool = new GameObject("BulletPool").AddComponent<BulletPoolView>();
            var enemySpawner = new GameObject("EnemySpawner").AddComponent<EnemySpawnerView>();
            var inputSource = new GameObject("KeyboardInput").AddComponent<KeyboardPlayerInputSource>();
            var gateway = new GameObject("SceneResultGateway").AddComponent<SceneResultGateway>();
            var presenter = new GameObject("GameScenePresenter").AddComponent<GameScenePresenterView>();
            var installer = new GameObject("GameInstaller").AddComponent<GameInstaller>();

            var canvas = CreateCanvas();
            var hudText = CreateText(canvas.transform, "ProgressText", "Enemy 0/1", new Vector2(0f, 210f), new Vector2(260f, 40f), 24);
            var hud = canvas.gameObject.AddComponent<GameHudView>();
            SetObject(hud, "progressText", hudText);

            if (player != null)
            {
                player.name = "Player";
                var follow = camera.gameObject.AddComponent<CameraFollowView>();
                follow.Configure(stageConfig, player.transform);
            }

            CreateBackground(stageConfig);

            SetObject(installer, "playerConfig", playerConfig);
            SetObject(installer, "enemyConfig", enemyConfig);
            SetObject(installer, "bulletConfig", bulletConfig);
            SetObject(installer, "stageConfig", stageConfig);
            SetObject(installer, "playerView", player);
            SetObjectArray(installer, "bulletPrefabs", bulletPrefabs);
            SetObject(installer, "bulletPoolView", bulletPool);
            SetObjectArray(installer, "enemyPrefabs", enemyPrefabs);
            SetObject(installer, "enemySpawnerView", enemySpawner);
            SetObject(installer, "presenterView", presenter);
            SetObject(installer, "hudView", hud);
            SetObject(installer, "resultGateway", gateway);
            SetObject(installer, "inputSourceBehaviour", inputSource);

            var path = SceneRoot + "/GameScene.unity";
            EditorSceneManager.SaveScene(scene, path);
            return path;
        }

        private static string BuildResultScene(StageConfig stageConfig)
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            scene.name = "ResultScene";
            CreateCamera();
            var canvas = CreateCanvas();
            var retryButton = CreateButton(canvas.transform, "RetryButton", "RETRY", new Vector2(-125f, -70f), new Vector2(200f, 58f));
            var titleButton = CreateButton(canvas.transform, "TitleButton", "TITLE", new Vector2(125f, -70f), new Vector2(200f, 58f));
            CreateText(canvas.transform, "ResultText", "MISSION CLEAR", new Vector2(0f, 80f), new Vector2(520f, 80f), 42);

            var presenter = new GameObject("ResultPresenter").AddComponent<ResultScenePresenterView>();
            SetObject(presenter, "stageConfig", stageConfig);
            SetObject(presenter, "retryButton", retryButton);
            SetObject(presenter, "titleButton", titleButton);

            var path = SceneRoot + "/ResultScene.unity";
            EditorSceneManager.SaveScene(scene, path);
            return path;
        }

        private static Camera CreateCamera()
        {
            var cameraGo = new GameObject("Main Camera");
            cameraGo.tag = "MainCamera";
            var camera = cameraGo.AddComponent<Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.04f, 0.05f, 0.08f, 1f);
            camera.orthographic = true;
            camera.transform.position = new Vector3(0f, 0f, -10f);
            return camera;
        }

        private static Canvas CreateCanvas()
        {
            var canvasGo = new GameObject("Canvas");
            var canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvasGo.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1280f, 720f);
            canvasGo.AddComponent<GraphicRaycaster>();

            var eventSystemGo = new GameObject("EventSystem");
            eventSystemGo.AddComponent<EventSystem>();
            eventSystemGo.AddComponent<InputSystemUIInputModule>();
            return canvas;
        }

        private static Button CreateButton(Transform parent, string name, string label, Vector2 position, Vector2 size)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var rect = go.AddComponent<RectTransform>();
            rect.anchoredPosition = position;
            rect.sizeDelta = size;

            var image = go.AddComponent<Image>();
            image.color = new Color(0.18f, 0.33f, 0.5f, 1f);

            var button = go.AddComponent<Button>();
            button.targetGraphic = image;

            var text = CreateText(go.transform, "Label", label, Vector2.zero, size, 26);
            text.color = Color.white;
            return button;
        }

        private static Text CreateText(Transform parent, string name, string value, Vector2 position, Vector2 size, int fontSize)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var rect = go.AddComponent<RectTransform>();
            rect.anchoredPosition = position;
            rect.sizeDelta = size;

            var text = go.AddComponent<Text>();
            text.text = value;
            text.alignment = TextAnchor.MiddleCenter;
            text.fontSize = fontSize;
            text.color = Color.white;
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            if (text.font == null)
            {
                text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            }

            return text;
        }

        private static void CreateBackground(StageConfig stageConfig)
        {
            var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(ArtRoot + "/PrototypeSquare.png");

            for (var i = 0; i < 4; i++)
            {
                var go = new GameObject("BackgroundPanel_" + i);
                go.transform.position = new Vector3(i * 6f - 9f, 0f, 5f);
                go.transform.localScale = new Vector3(5.8f, 9f, 1f);

                var renderer = go.AddComponent<SpriteRenderer>();
                renderer.sprite = sprite;
                renderer.color = i % 2 == 0
                    ? new Color(0.08f, 0.12f, 0.18f, 1f)
                    : new Color(0.12f, 0.17f, 0.24f, 1f);
                renderer.sortingOrder = -10;

                var scroll = go.AddComponent<BackgroundScrollView>();
                SetObject(scroll, "stageConfig", stageConfig);
                SetFloat(scroll, "wrapWidth", 24f);
            }
        }

        private static void SetObject(Object target, string fieldName, Object value)
        {
            if (target == null)
            {
                return;
            }

            var serializedObject = new SerializedObject(target);
            var property = serializedObject.FindProperty(fieldName);
            if (property != null)
            {
                property.objectReferenceValue = value;
                serializedObject.ApplyModifiedPropertiesWithoutUndo();
            }
        }

        private static void SetFloat(Object target, string fieldName, float value)
        {
            if (target == null)
            {
                return;
            }

            var serializedObject = new SerializedObject(target);
            var property = serializedObject.FindProperty(fieldName);
            if (property != null)
            {
                property.floatValue = value;
                serializedObject.ApplyModifiedPropertiesWithoutUndo();
            }
        }

        private static void SetObjectArray(Object target, string fieldName, Object[] values)
        {
            if (target == null)
            {
                return;
            }

            var serializedObject = new SerializedObject(target);
            var property = serializedObject.FindProperty(fieldName);
            if (property == null || !property.isArray)
            {
                return;
            }

            property.arraySize = values != null ? values.Length : 0;
            for (var i = 0; i < property.arraySize; i++)
            {
                property.GetArrayElementAtIndex(i).objectReferenceValue = values[i];
            }

            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

    }
}
