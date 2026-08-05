using Platformer;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

namespace Platformer.Editor
{
    internal static class PlatformerScenePresentation
    {
        public static void CreateEnvironment(int groundLayer)
        {
            CreateGlobalLight();
            CreateBackground();
            CreatePlatforms(groundLayer);
        }

        public static void CreateHud(CoinWallet wallet)
        {
            GameObject canvasObject = new GameObject("HUD", typeof(RectTransform), typeof(Canvas),
                typeof(CanvasScaler), typeof(GraphicRaycaster));
            Canvas canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100;

            CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.matchWidthOrHeight = 0.5f;

            GameObject panelObject = new GameObject("Counter Panel", typeof(RectTransform), typeof(CanvasRenderer),
                typeof(Image));
            panelObject.transform.SetParent(canvasObject.transform, false);
            RectTransform panelRect = panelObject.GetComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0f, 1f);
            panelRect.anchorMax = new Vector2(0f, 1f);
            panelRect.pivot = new Vector2(0f, 1f);
            panelRect.anchoredPosition = new Vector2(28f, -28f);
            panelRect.sizeDelta = new Vector2(310f, 96f);

            Image panel = panelObject.GetComponent<Image>();
            panel.color = new Color(0.08f, 0.1f, 0.18f, 0.86f);
            panel.raycastTarget = false;

            Text counter = CreateText("Coin Counter", panelObject.transform, "COINS: 0", 36,
                TextAnchor.MiddleLeft, Color.white);
            RectTransform counterRect = counter.rectTransform;
            counterRect.anchorMin = Vector2.zero;
            counterRect.anchorMax = Vector2.one;
            counterRect.offsetMin = new Vector2(28f, 8f);
            counterRect.offsetMax = new Vector2(-18f, -8f);
            CoinCounterView counterView = counter.gameObject.AddComponent<CoinCounterView>();
            SetObjectReference(counterView, "_wallet", wallet);

            GameObject instructionsPanelObject = new GameObject("Controls Panel", typeof(RectTransform),
                typeof(CanvasRenderer), typeof(Image));
            instructionsPanelObject.transform.SetParent(canvasObject.transform, false);
            RectTransform instructionsPanelRect = instructionsPanelObject.GetComponent<RectTransform>();
            instructionsPanelRect.anchorMin = new Vector2(0.5f, 0f);
            instructionsPanelRect.anchorMax = new Vector2(0.5f, 0f);
            instructionsPanelRect.pivot = new Vector2(0.5f, 0f);
            instructionsPanelRect.anchoredPosition = new Vector2(0f, 28f);
            instructionsPanelRect.sizeDelta = new Vector2(1000f, 74f);

            Image instructionsPanel = instructionsPanelObject.GetComponent<Image>();
            instructionsPanel.color = new Color(0.08f, 0.1f, 0.18f, 0.78f);
            instructionsPanel.raycastTarget = false;

            Text instructions = CreateText("Controls", instructionsPanelObject.transform,
                "MOVE: A/D or ARROWS     JUMP: SPACE / W / UP", 24, TextAnchor.MiddleCenter,
                new Color(0.95f, 0.98f, 1f));
            RectTransform instructionsRect = instructions.rectTransform;
            instructionsRect.anchorMin = Vector2.zero;
            instructionsRect.anchorMax = Vector2.one;
            instructionsRect.offsetMin = new Vector2(18f, 4f);
            instructionsRect.offsetMax = new Vector2(-18f, -4f);
        }

        public static void CreateCamera(Transform target)
        {
            GameObject cameraObject = new GameObject("Main Camera");
            cameraObject.tag = "MainCamera";
            cameraObject.transform.position = new Vector3(-8f, 0f, -10f);

            Camera camera = cameraObject.AddComponent<Camera>();
            camera.orthographic = true;
            camera.orthographicSize = 5.4f;
            camera.backgroundColor = new Color(0.35f, 0.71f, 0.88f);
            camera.clearFlags = CameraClearFlags.SolidColor;

            AudioListener listener = cameraObject.AddComponent<AudioListener>();
            listener.enabled = true;

            CameraFollower follower = cameraObject.AddComponent<CameraFollower>();
            SetObjectReference(follower, "_target", target);
        }

        private static void CreateGlobalLight()
        {
            GameObject lightObject = new GameObject("Global Light 2D");
            Light2D light = lightObject.AddComponent<Light2D>();
            light.lightType = Light2D.LightType.Global;
            light.intensity = 0.92f;
            light.color = new Color(1f, 0.95f, 0.86f);
        }

        private static void CreateBackground()
        {
            GameObject background = new GameObject("Background");
            Sprite hill = LoadSprite(PlatformerArtGenerator.HillPath);
            Sprite cloud = LoadSprite(PlatformerArtGenerator.CloudPath);

            CreateDecoration("Hill Left", background.transform, hill, new Vector3(-11f, -1.9f, 0f),
                new Vector3(8f, 5f, 1f), -20, new Color(0.65f, 0.9f, 0.77f));
            CreateDecoration("Hill Center", background.transform, hill, new Vector3(1f, -2.1f, 0f),
                new Vector3(9f, 5.5f, 1f), -20, new Color(0.55f, 0.83f, 0.69f));
            CreateDecoration("Hill Right", background.transform, hill, new Vector3(14f, -2f, 0f),
                new Vector3(8f, 5f, 1f), -20, new Color(0.62f, 0.88f, 0.74f));
            CreateDecoration("Cloud Left", background.transform, cloud, new Vector3(-8f, 3.7f, 0f),
                new Vector3(2.3f, 2.3f, 1f), -15, Color.white);
            CreateDecoration("Cloud Center", background.transform, cloud, new Vector3(3f, 4.3f, 0f),
                new Vector3(1.8f, 1.8f, 1f), -15, Color.white);
            CreateDecoration("Cloud Right", background.transform, cloud, new Vector3(13f, 3.4f, 0f),
                new Vector3(2.1f, 2.1f, 1f), -15, Color.white);
        }

        private static void CreateDecoration(
            string objectName,
            Transform parent,
            Sprite sprite,
            Vector3 position,
            Vector3 scale,
            int sortingOrder,
            Color color)
        {
            GameObject decoration = new GameObject(objectName);
            decoration.transform.SetParent(parent);
            decoration.transform.position = position;
            decoration.transform.localScale = scale;

            SpriteRenderer renderer = decoration.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.sortingOrder = sortingOrder;
            renderer.color = color;
        }

        private static void CreatePlatforms(int groundLayer)
        {
            GameObject level = new GameObject("Level");
            Sprite platformSprite = LoadSprite(PlatformerArtGenerator.PlatformPath);

            CreatePlatform("Ground", level.transform, platformSprite, new Vector2(0f, -3.65f),
                new Vector2(40f, 0.7f), groundLayer);
            CreatePlatform("Platform Left", level.transform, platformSprite, new Vector2(-6f, -1.5f),
                new Vector2(4.5f, 0.5f), groundLayer);
            CreatePlatform("Platform Center", level.transform, platformSprite, new Vector2(0f, 0f),
                new Vector2(4.5f, 0.5f), groundLayer);
            CreatePlatform("Platform High", level.transform, platformSprite, new Vector2(6f, 1.45f),
                new Vector2(5f, 0.5f), groundLayer);
            CreatePlatform("Platform Right", level.transform, platformSprite, new Vector2(11.5f, -0.5f),
                new Vector2(4f, 0.5f), groundLayer);
            CreateBoundary("Left Boundary", level.transform, new Vector2(-20.4f, 0f), groundLayer);
            CreateBoundary("Right Boundary", level.transform, new Vector2(20.4f, 0f), groundLayer);
        }

        private static void CreatePlatform(
            string objectName,
            Transform parent,
            Sprite sprite,
            Vector2 position,
            Vector2 size,
            int layer)
        {
            GameObject platform = new GameObject(objectName);
            platform.layer = layer;
            platform.transform.SetParent(parent);
            platform.transform.position = position;
            platform.transform.localScale = new Vector3(size.x, size.y, 1f);

            SpriteRenderer renderer = platform.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.sortingOrder = 0;

            BoxCollider2D collider = platform.AddComponent<BoxCollider2D>();
            collider.size = Vector2.one;
        }

        private static void CreateBoundary(string objectName, Transform parent, Vector2 position, int layer)
        {
            GameObject boundary = new GameObject(objectName);
            boundary.layer = layer;
            boundary.transform.SetParent(parent);
            boundary.transform.position = position;

            BoxCollider2D collider = boundary.AddComponent<BoxCollider2D>();
            collider.size = new Vector2(0.5f, 12f);
        }

        private static Text CreateText(
            string objectName,
            Transform parent,
            string content,
            int fontSize,
            TextAnchor alignment,
            Color color)
        {
            GameObject textObject = new GameObject(objectName, typeof(RectTransform), typeof(CanvasRenderer),
                typeof(Text));
            textObject.transform.SetParent(parent, false);

            Text text = textObject.GetComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.text = content;
            text.fontSize = fontSize;
            text.fontStyle = FontStyle.Bold;
            text.alignment = alignment;
            text.color = color;
            text.raycastTarget = false;
            return text;
        }

        private static Sprite LoadSprite(string assetPath)
        {
            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);

            if (sprite == null)
                throw new MissingReferenceException("Sprite is missing: " + assetPath);

            return sprite;
        }

        private static void SetObjectReference(UnityEngine.Object target, string propertyName, UnityEngine.Object value)
        {
            SerializedObject serializedObject = new SerializedObject(target);
            SerializedProperty property = serializedObject.FindProperty(propertyName);
            property.objectReferenceValue = value;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}
