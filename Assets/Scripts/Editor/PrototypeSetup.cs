using TwoDShooting.Simple;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace TwoDShooting.Editor
{
    /// <summary>教材を壊しても、Toolsメニューから最初の状態へ戻せます。</summary>
    public static class PrototypeSetup
    {
        private const string Root = "Assets";
        private const string Prefabs = Root + "/Prefabs";
        private const string Scenes = Root + "/Scenes";
        private const string SquarePath = Root + "/Images/PrototypeSquare.png";
        private const float UiScale = 1.5f; // 1280x720で作った配置を1920x1080へ合わせる

        private static readonly Color Ink = Hex("26364D");
        private static readonly Color Blue = Hex("5479B8");
        private static readonly Color Sky = Hex("76A7D4");
        private static readonly Color Orange = Hex("E8A64A");
        private static readonly Color Paper = Hex("F5F7FA");
        private static readonly Color Muted = Hex("6E7C90");

        [MenuItem("Tools/2D Shooting/教材を最初の状態に戻す")]
        public static void BuildPrototype()
        {
            var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(SquarePath);
            if (sprite == null)
            {
                Debug.LogError("PrototypeSquare.png が見つかりません。");
                return;
            }

            var bullet = BuildBullet(sprite);
            BuildPlayer(sprite, bullet);
            BuildEnemy(sprite, "Enemy_Scout", Orange, 3, 1.3f, 0.85f);
            BuildEnemy(sprite, "Enemy_Heavy", Blue, 5, 0.75f, 1.15f);

            var title = BuildTitleScene();
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            var game = BuildGameScene(sprite);
            var result = BuildResultScene();

            EditorBuildSettings.scenes = new[]
            {
                new EditorBuildSettingsScene(title, true),
                new EditorBuildSettingsScene(game, true),
                new EditorBuildSettingsScene(result, true)
            };
            AssetDatabase.SaveAssets();
            EditorSceneManager.OpenScene(title);
            Debug.Log("P班特訓の教材を再構築しました。GameScene の GameRules から試してみてください。");
        }

        private static Bullet BuildBullet(Sprite sprite)
        {
            var root = new GameObject("Bullet");
            var renderer = root.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.color = Sky;
            renderer.sortingOrder = 5;
            root.transform.localScale = new Vector3(0.55f, 0.16f, 1f);
            var body = root.AddComponent<Rigidbody2D>();
            body.bodyType = RigidbodyType2D.Kinematic;
            body.gravityScale = 0f;
            var collider = root.AddComponent<BoxCollider2D>();
            collider.isTrigger = true;
            var bullet = root.AddComponent<Bullet>();
            bullet.speed = 12f;
            bullet.damage = 1;
            bullet.lifeSeconds = 2f;
            var saved = SavePrefab(root, Prefabs + "/Bullet.prefab");
            return saved.GetComponent<Bullet>();
        }

        private static Player BuildPlayer(Sprite sprite, Bullet bulletPrefab)
        {
            var root = new GameObject("Player");
            root.transform.localScale = new Vector3(1.3f, 0.55f, 1f);
            var renderer = root.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.color = Sky;
            renderer.sortingOrder = 2;
            var player = root.AddComponent<Player>();
            player.bulletPrefab = bulletPrefab;
            player.moveSpeed = 6f;
            player.shotInterval = 0.22f;

            var saved = SavePrefab(root, Prefabs + "/Player.prefab");
            return saved.GetComponent<Player>();
        }

        private static Enemy BuildEnemy(Sprite sprite, string name, Color color, int health, float speed, float size)
        {
            var root = new GameObject(name);
            root.transform.localScale = new Vector3(size * 1.15f, size * 0.8f, 1f);
            var renderer = root.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.color = color;
            renderer.sortingOrder = 2;
            var enemy = root.AddComponent<Enemy>();
            enemy.health = health;
            enemy.moveSpeed = speed;
            enemy.moveDistance = 0.75f;
            var collider = root.AddComponent<BoxCollider2D>();
            collider.isTrigger = true;
            var saved = SavePrefab(root, Prefabs + "/" + name + ".prefab");
            return saved.GetComponent<Enemy>();
        }

        private static string BuildTitleScene()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            var camera = CreateCamera();
            var canvas = CreateCanvas(camera, "UI_Title — タイトル画面");

            Label(canvas.transform, "SeriesLabel", "P班特訓  /  2Dシューティング編", 18, FontStyle.Bold, Blue,
                new Vector2(72f, -72f), new Vector2(520f, 32f), new Vector2(0f, 1f), TextAnchor.MiddleLeft);
            Label(canvas.transform, "Title", "まずは、遊んでみよう", 48, FontStyle.Bold, Ink,
                new Vector2(72f, -142f), new Vector2(850f, 90f), new Vector2(0f, 1f), TextAnchor.MiddleLeft);
            Label(canvas.transform, "Subtitle", "ゲームを動かして、気になったところをひとつ変えてみます。", 22, FontStyle.Normal, Muted,
                new Vector2(76f, -224f), new Vector2(860f, 44f), new Vector2(0f, 1f), TextAnchor.MiddleLeft);

            Label(canvas.transform, "Controls", "操作\n移動　　　 WASD / 矢印キー\nショット　 Space / Z", 19, FontStyle.Bold, Ink,
                new Vector2(72f, 86f), new Vector2(530f, 110f), new Vector2(0f, 0f), TextAnchor.UpperLeft);

            var start = Button(canvas.transform, "StartButton", "ゲームを始める", new Vector2(-72f, 92f), new Vector2(320f, 72f), new Vector2(1f, 0f), Blue);
            var flow = new GameObject("ScreenFlow — 画面を切り替える").AddComponent<ScreenFlow>();
            Set(flow, "screenKind", ScreenFlow.ScreenKind.Title);
            Set(flow, "primaryButton", start);
            return SaveScene(scene, "TitleScene");
        }

        private static string BuildGameScene(Sprite sprite)
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            var camera = CreateCamera();
            camera.orthographicSize = 5f;
            BuildCenterLine(sprite);

            var playerAsset = AssetDatabase.LoadAssetAtPath<GameObject>(Prefabs + "/Player.prefab");
            var player = (GameObject)PrefabUtility.InstantiatePrefab(playerAsset);
            player.name = "Player — 速さと連射を変える";
            player.transform.position = new Vector3(-5.7f, 0f, 0f);

            var canvas = CreateCanvas(camera, "UI_Game — カメラ内に見えるHUD");
            Label(canvas.transform, "StageLabel", "P班特訓  /  2Dシューティング", 16, FontStyle.Bold, Blue,
                new Vector2(34f, -18f), new Vector2(440f, 28f), new Vector2(0f, 1f), TextAnchor.MiddleLeft);
            var progress = Label(canvas.transform, "Progress", "たおした敵  0 / 4", 24, FontStyle.Bold, Ink,
                new Vector2(-34f, -18f), new Vector2(360f, 38f), new Vector2(1f, 1f), TextAnchor.MiddleRight);
            var status = Label(canvas.transform, "Status", "移動：WASD・矢印キー　　ショット：Space・Z", 15, FontStyle.Bold, Muted,
                new Vector2(34f, 26f), new Vector2(680f, 32f), new Vector2(0f, 0f), TextAnchor.MiddleLeft);

            var rules = new GameObject("★ GameRules — 敵の数や並びを変える").AddComponent<GameRules>();
            rules.enemiesToClear = 4;
            rules.enemyPrefabs = new[]
            {
                AssetDatabase.LoadAssetAtPath<GameObject>(Prefabs + "/Enemy_Scout.prefab").GetComponent<Enemy>(),
                AssetDatabase.LoadAssetAtPath<GameObject>(Prefabs + "/Enemy_Heavy.prefab").GetComponent<Enemy>()
            };
            rules.firstSpawnPosition = new Vector2(5.1f, 2.4f);
            rules.spawnStep = new Vector2(0.55f, -1.55f);
            rules.spawnInterval = 0f;
            Set(rules, "progressText", progress);
            Set(rules, "statusText", status);
            return SaveScene(scene, "GameScene");
        }

        private static string BuildResultScene()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            var camera = CreateCamera();
            var canvas = CreateCanvas(camera, "UI_Result — クリア画面");
            Label(canvas.transform, "Title", "クリア！\nおつかれさまでした", 40, FontStyle.Bold, Ink,
                new Vector2(0f, 120f), new Vector2(800f, 130f), new Vector2(0.5f, 0.5f), TextAnchor.MiddleCenter);
            Label(canvas.transform, "Copy", "少し数字を変えるだけでも、遊び心地は変わります。\n次は気になるところをひとつ試してみましょう。", 20, FontStyle.Normal, Muted,
                new Vector2(0f, 20f), new Vector2(760f, 80f), new Vector2(0.5f, 0.5f), TextAnchor.MiddleCenter);
            var retry = Button(canvas.transform, "Retry", "もう一度試す", new Vector2(0f, -100f), new Vector2(360f, 68f), new Vector2(0.5f, 0.5f), Blue);
            var title = Button(canvas.transform, "BackToTitle", "タイトルへ戻る", new Vector2(0f, -188f), new Vector2(360f, 54f), new Vector2(0.5f, 0.5f), Ink);
            var flow = new GameObject("ScreenFlow — 画面を切り替える").AddComponent<ScreenFlow>();
            Set(flow, "screenKind", ScreenFlow.ScreenKind.Result);
            Set(flow, "primaryButton", retry);
            Set(flow, "secondaryButton", title);
            return SaveScene(scene, "ResultScene");
        }

        private static void BuildCenterLine(Sprite sprite)
        {
            var line = new GameObject("CenterLine — 移動の目安");
            line.transform.localScale = new Vector3(20f, 0.025f, 1f);
            var renderer = line.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.color = new Color(0.33f, 0.48f, 0.72f, 0.25f);
            renderer.sortingOrder = -20;
        }

        private static Camera CreateCamera()
        {
            var go = new GameObject("Main Camera");
            go.tag = "MainCamera";
            var camera = go.AddComponent<Camera>();
            camera.orthographic = true;
            camera.orthographicSize = 5f;
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = Paper;
            camera.transform.position = new Vector3(0f, 0f, -10f);
            return camera;
        }

        private static Canvas CreateCanvas(Camera camera, string name)
        {
            var go = new GameObject(name);
            var canvas = go.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = camera;
            canvas.planeDistance = 5f;
            var scaler = go.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.matchWidthOrHeight = 0.5f;
            go.AddComponent<GraphicRaycaster>();
            var events = new GameObject("EventSystem");
            events.AddComponent<EventSystem>();
            events.AddComponent<InputSystemUIInputModule>();
            return canvas;
        }

        private static GameObject CreateBlock(Transform parent, string name, Color color, Vector2 position, Vector2 size, Vector2 anchor)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var rect = go.AddComponent<RectTransform>();
            rect.anchorMin = rect.anchorMax = anchor;
            rect.pivot = anchor;
            rect.anchoredPosition = position * UiScale;
            rect.sizeDelta = size * UiScale;
            go.AddComponent<Image>().color = color;
            return go;
        }

        private static Text Label(Transform parent, string name, string value, int size, FontStyle style, Color color,
            Vector2 position, Vector2 dimensions, Vector2 anchor, TextAnchor alignment)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var rect = go.AddComponent<RectTransform>();
            rect.anchorMin = rect.anchorMax = anchor;
            rect.pivot = anchor;
            rect.anchoredPosition = position * UiScale;
            rect.sizeDelta = dimensions * UiScale;
            var text = go.AddComponent<Text>();
            text.text = value;
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = Mathf.RoundToInt(size * UiScale);
            text.fontStyle = style;
            text.color = color;
            text.alignment = alignment;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Truncate;
            return text;
        }

        private static Button Button(Transform parent, string name, string text, Vector2 position, Vector2 size, Vector2 anchor, Color color)
        {
            var go = CreateBlock(parent, name, color, position, size, anchor);
            var image = go.GetComponent<Image>();
            var button = go.AddComponent<Button>();
            button.targetGraphic = image;
            var colors = button.colors;
            colors.highlightedColor = Color.Lerp(color, Color.white, 0.16f);
            colors.pressedColor = Color.Lerp(color, Color.black, 0.18f);
            button.colors = colors;
            Label(go.transform, "Label", text, 19, FontStyle.Bold, Paper, Vector2.zero, size, new Vector2(0.5f, 0.5f), TextAnchor.MiddleCenter);
            return button;
        }

        private static GameObject SavePrefab(GameObject root, string path)
        {
            PrefabUtility.SaveAsPrefabAsset(root, path);
            Object.DestroyImmediate(root);
            AssetDatabase.SaveAssets();
            return AssetDatabase.LoadAssetAtPath<GameObject>(path);
        }

        private static string SaveScene(UnityEngine.SceneManagement.Scene scene, string name)
        {
            var path = Scenes + "/" + name + ".unity";
            EditorSceneManager.SaveScene(scene, path);
            return path;
        }

        private static void Set(Object target, string propertyName, Object value)
        {
            var serialized = new SerializedObject(target);
            serialized.FindProperty(propertyName).objectReferenceValue = value;
            serialized.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void Set(Object target, string propertyName, int enumValue)
        {
            var serialized = new SerializedObject(target);
            serialized.FindProperty(propertyName).enumValueIndex = enumValue;
            serialized.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void Set(Object target, string propertyName, ScreenFlow.ScreenKind value) => Set(target, propertyName, (int)value);

        private static Color Hex(string hex)
        {
            ColorUtility.TryParseHtmlString("#" + hex, out var color);
            return color;
        }
    }
}
