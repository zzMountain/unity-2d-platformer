using System;
using System.Collections.Generic;
using Platformer;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Platformer.Editor
{
    public static class PlatformerSceneBuilder
    {
        public const string ScenePath = "Assets/Platformer/Scenes/Platformer.unity";

        private const string AnimatorControllerPath = "Assets/Platformer/Animations/Player.controller";
        private const string CoinPrefabPath = "Assets/Platformer/Prefabs/Coin.prefab";
        private const string IdleAnimationPath = "Assets/Platformer/Animations/Player_Idle.anim";
        private const string PlayerPhysicsMaterialPath =
            "Assets/Platformer/Physics/PlayerNoFriction.physicsMaterial2D";
        private const string RunAnimationPath = "Assets/Platformer/Animations/Player_Run.anim";
        private const string GroundLayerName = "Ground";
        private const int ExpectedEnemyCount = 2;
        private const int ExpectedPlayerCount = 1;

        public static string Build()
        {
            EnsureAssetFolders();
            PlatformerArtGenerator.Generate();

            int groundLayer = EnsureLayer(GroundLayerName);
            AnimatorController playerController = CreatePlayerAnimatorController();
            PhysicsMaterial2D playerPhysicsMaterial = CreatePlayerPhysicsMaterial();
            CreateCoinPrefab();

            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            PlatformerScenePresentation.CreateEnvironment(groundLayer);

            CoinWallet wallet = CreatePlayer(playerController, playerPhysicsMaterial, groundLayer);
            CreateEnemies();
            CreateCoinSpawner();
            PlatformerScenePresentation.CreateHud(wallet);
            PlatformerScenePresentation.CreateCamera(wallet.transform);

            EditorSceneManager.SaveScene(scene, ScenePath);
            EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(ScenePath, true) };
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            return Validate();
        }

        public static string Validate()
        {
            ValidateCount<Player>(ExpectedPlayerCount);
            ValidateCount<EnemyPatrol>(ExpectedEnemyCount);
            ValidateCount<CoinSpawner>(1);
            ValidateCount<CoinCounterView>(1);
            ValidateCount<CameraFollower>(1);

            if (AssetDatabase.LoadAssetAtPath<Coin>(CoinPrefabPath) == null)
                throw new InvalidOperationException("Coin prefab was not created.");

            if (AssetDatabase.LoadAssetAtPath<AnimatorController>(AnimatorControllerPath) == null)
                throw new InvalidOperationException("Player Animator Controller was not created.");

            if (SceneManager.GetActiveScene().path != ScenePath)
                throw new InvalidOperationException("The active scene is not the generated platformer scene.");

            return "Platformer scene is ready: 1 player, 2 enemies, 6 active coin slots, run animation and HUD.";
        }

        [MenuItem("Tools/Platformer/Build Demo")]
        private static void BuildFromMenu()
        {
            Build();
        }

        private static void EnsureAssetFolders()
        {
            EnsureFolder("Assets/Platformer", "Animations");
            EnsureFolder("Assets/Platformer", "Art");
            EnsureFolder("Assets/Platformer", "Physics");
            EnsureFolder("Assets/Platformer", "Prefabs");
            EnsureFolder("Assets/Platformer", "Scenes");
        }

        private static void EnsureFolder(string parentPath, string folderName)
        {
            string folderPath = parentPath + "/" + folderName;

            if (AssetDatabase.IsValidFolder(folderPath) == false)
                AssetDatabase.CreateFolder(parentPath, folderName);
        }

        private static int EnsureLayer(string layerName)
        {
            SerializedObject tagManager = new SerializedObject(
                AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            SerializedProperty layers = tagManager.FindProperty("layers");

            for (int i = 8; i < layers.arraySize; i++)
            {
                SerializedProperty layer = layers.GetArrayElementAtIndex(i);

                if (layer.stringValue == layerName)
                    return i;
            }

            for (int i = 8; i < layers.arraySize; i++)
            {
                SerializedProperty layer = layers.GetArrayElementAtIndex(i);

                if (string.IsNullOrEmpty(layer.stringValue) == false)
                    continue;

                layer.stringValue = layerName;
                tagManager.ApplyModifiedProperties();
                return i;
            }

            throw new InvalidOperationException("No free Unity layer is available for " + layerName + ".");
        }

        private static AnimatorController CreatePlayerAnimatorController()
        {
            DeleteAsset(AnimatorControllerPath);
            DeleteAsset(IdleAnimationPath);
            DeleteAsset(RunAnimationPath);

            Sprite idleSprite = LoadSprite(PlatformerArtGenerator.PlayerIdlePath);
            Sprite[] runSprites =
            {
                LoadSprite(PlatformerArtGenerator.PlayerRunOnePath),
                LoadSprite(PlatformerArtGenerator.PlayerRunTwoPath),
                LoadSprite(PlatformerArtGenerator.PlayerRunThreePath),
                LoadSprite(PlatformerArtGenerator.PlayerRunFourPath)
            };

            AnimationClip idleClip = CreateSpriteAnimation(IdleAnimationPath, new[] { idleSprite }, 2f);
            AnimationClip runClip = CreateSpriteAnimation(RunAnimationPath, runSprites, 12f);
            AnimatorController controller = AnimatorController.CreateAnimatorControllerAtPath(AnimatorControllerPath);
            controller.AddParameter(PlayerAnimator.SpeedParameterName, AnimatorControllerParameterType.Float);

            AnimatorStateMachine stateMachine = controller.layers[0].stateMachine;
            AnimatorState idleState = stateMachine.AddState("Idle");
            AnimatorState runState = stateMachine.AddState("Run");
            idleState.motion = idleClip;
            runState.motion = runClip;
            stateMachine.defaultState = idleState;

            AnimatorStateTransition runTransition = idleState.AddTransition(runState);
            runTransition.hasExitTime = false;
            runTransition.duration = 0.06f;
            runTransition.AddCondition(
                AnimatorConditionMode.Greater,
                0.1f,
                PlayerAnimator.SpeedParameterName);

            AnimatorStateTransition idleTransition = runState.AddTransition(idleState);
            idleTransition.hasExitTime = false;
            idleTransition.duration = 0.06f;
            idleTransition.AddCondition(
                AnimatorConditionMode.Less,
                0.1f,
                PlayerAnimator.SpeedParameterName);

            EditorUtility.SetDirty(controller);
            AssetDatabase.SaveAssets();
            return controller;
        }

        private static PhysicsMaterial2D CreatePlayerPhysicsMaterial()
        {
            PhysicsMaterial2D material = AssetDatabase.LoadAssetAtPath<PhysicsMaterial2D>(
                PlayerPhysicsMaterialPath);

            if (material == null)
            {
                material = new PhysicsMaterial2D("Player No Friction");
                AssetDatabase.CreateAsset(material, PlayerPhysicsMaterialPath);
            }

            material.friction = 0f;
            material.bounciness = 0f;
            EditorUtility.SetDirty(material);
            return material;
        }

        private static AnimationClip CreateSpriteAnimation(
            string assetPath,
            IReadOnlyList<Sprite> sprites,
            float frameRate)
        {
            AnimationClip clip = new AnimationClip
            {
                frameRate = frameRate,
                name = System.IO.Path.GetFileNameWithoutExtension(assetPath)
            };
            EditorCurveBinding binding = new EditorCurveBinding
            {
                path = string.Empty,
                propertyName = "m_Sprite",
                type = typeof(SpriteRenderer)
            };
            ObjectReferenceKeyframe[] keyframes = new ObjectReferenceKeyframe[sprites.Count + 1];

            for (int i = 0; i < sprites.Count; i++)
            {
                keyframes[i] = new ObjectReferenceKeyframe
                {
                    time = i / frameRate,
                    value = sprites[i]
                };
            }

            keyframes[sprites.Count] = new ObjectReferenceKeyframe
            {
                time = sprites.Count / frameRate,
                value = sprites[0]
            };

            AnimationUtility.SetObjectReferenceCurve(clip, binding, keyframes);
            AnimationClipSettings settings = AnimationUtility.GetAnimationClipSettings(clip);
            settings.loopTime = true;
            AnimationUtility.SetAnimationClipSettings(clip, settings);
            AssetDatabase.CreateAsset(clip, assetPath);
            return clip;
        }

        private static void DeleteAsset(string assetPath)
        {
            if (AssetDatabase.LoadMainAssetAtPath(assetPath) != null)
                AssetDatabase.DeleteAsset(assetPath);
        }

        private static void CreateCoinPrefab()
        {
            DeleteAsset(CoinPrefabPath);

            GameObject coinObject = new GameObject("Coin");
            SpriteRenderer renderer = coinObject.AddComponent<SpriteRenderer>();
            renderer.sprite = LoadSprite(PlatformerArtGenerator.CoinPath);
            renderer.sortingOrder = 5;

            CircleCollider2D collider = coinObject.AddComponent<CircleCollider2D>();
            collider.isTrigger = true;
            collider.radius = 0.36f;

            coinObject.AddComponent<Coin>();
            GameObject prefabObject = PrefabUtility.SaveAsPrefabAsset(coinObject, CoinPrefabPath);
            UnityEngine.Object.DestroyImmediate(coinObject);

            if (prefabObject == null || prefabObject.GetComponent<Coin>() == null)
                throw new InvalidOperationException("Coin prefab could not be loaded after creation.");
        }

        private static CoinWallet CreatePlayer(
            AnimatorController controller,
            PhysicsMaterial2D playerPhysicsMaterial,
            int groundLayer)
        {
            GameObject playerObject = new GameObject("Player");
            playerObject.transform.position = new Vector3(-11f, -2.45f, 0f);

            SpriteRenderer renderer = playerObject.AddComponent<SpriteRenderer>();
            renderer.sprite = LoadSprite(PlatformerArtGenerator.PlayerIdlePath);
            renderer.sortingOrder = 10;

            Animator animator = playerObject.AddComponent<Animator>();
            animator.runtimeAnimatorController = controller;
            animator.updateMode = AnimatorUpdateMode.Fixed;

            Rigidbody2D rigidbody = playerObject.AddComponent<Rigidbody2D>();
            rigidbody.gravityScale = 4f;
            rigidbody.freezeRotation = true;
            rigidbody.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            rigidbody.interpolation = RigidbodyInterpolation2D.Interpolate;

            CapsuleCollider2D collider = playerObject.AddComponent<CapsuleCollider2D>();
            collider.size = new Vector2(0.62f, 1.35f);
            collider.offset = new Vector2(0f, -0.02f);
            collider.sharedMaterial = playerPhysicsMaterial;

            GameObject checkObject = new GameObject("Ground Check");
            checkObject.transform.SetParent(playerObject.transform);
            checkObject.transform.localPosition = new Vector3(0f, -0.72f, 0f);

            playerObject.AddComponent<PlayerInputReader>();
            GroundDetector detector = playerObject.AddComponent<GroundDetector>();
            PlayerMover mover = playerObject.AddComponent<PlayerMover>();
            playerObject.AddComponent<PlayerAnimator>();
            playerObject.AddComponent<Player>();
            CoinWallet wallet = playerObject.AddComponent<CoinWallet>();

            SetObjectReference(detector, "_checkPoint", checkObject.transform);
            SetLayerMask(detector, "_groundLayer", 1 << groundLayer);
            SetFloat(mover, "_speed", 7f);
            SetFloat(mover, "_jumpVelocity", 15f);
            return wallet;
        }

        private static void CreateEnemies()
        {
            GameObject enemies = new GameObject("Enemies");
            CreateEnemy(enemies.transform, "Enemy Ground", new Vector2(-2f, -2.82f), -4.5f, 3f, 2f);
            CreateEnemy(enemies.transform, "Enemy High", new Vector2(6f, 2.05f), 4.2f, 7.8f, 1.65f);
        }

        private static void CreateEnemy(
            Transform parent,
            string objectName,
            Vector2 position,
            float leftBoundary,
            float rightBoundary,
            float speed)
        {
            GameObject leftPoint = new GameObject(objectName + " Left Point");
            GameObject rightPoint = new GameObject(objectName + " Right Point");
            leftPoint.transform.SetParent(parent);
            rightPoint.transform.SetParent(parent);
            leftPoint.transform.position = new Vector3(leftBoundary, position.y, 0f);
            rightPoint.transform.position = new Vector3(rightBoundary, position.y, 0f);

            GameObject enemyObject = new GameObject(objectName);
            enemyObject.transform.SetParent(parent);
            enemyObject.transform.position = position;

            SpriteRenderer renderer = enemyObject.AddComponent<SpriteRenderer>();
            renderer.sprite = LoadSprite(PlatformerArtGenerator.EnemyPath);
            renderer.sortingOrder = 8;

            Rigidbody2D rigidbody = enemyObject.AddComponent<Rigidbody2D>();
            rigidbody.bodyType = RigidbodyType2D.Kinematic;
            rigidbody.freezeRotation = true;
            rigidbody.interpolation = RigidbodyInterpolation2D.Interpolate;

            BoxCollider2D collider = enemyObject.AddComponent<BoxCollider2D>();
            collider.size = new Vector2(0.72f, 0.62f);
            collider.offset = new Vector2(0f, -0.1f);

            EnemyPatrol patrol = enemyObject.AddComponent<EnemyPatrol>();
            SetObjectReference(patrol, "_leftPoint", leftPoint.transform);
            SetObjectReference(patrol, "_rightPoint", rightPoint.transform);
            SetFloat(patrol, "_speed", speed);
        }

        private static void CreateCoinSpawner()
        {
            GameObject spawnerObject = new GameObject("Coin Spawner");
            CoinSpawner spawner = spawnerObject.AddComponent<CoinSpawner>();
            GameObject coinPrefabObject = AssetDatabase.LoadAssetAtPath<GameObject>(CoinPrefabPath);
            Coin coinPrefab = coinPrefabObject.GetComponent<Coin>();
            Vector2[] positions =
            {
                new Vector2(-9f, -2f),
                new Vector2(-7f, -0.65f),
                new Vector2(-5f, -0.65f),
                new Vector2(-1f, 0.85f),
                new Vector2(1f, 0.85f),
                new Vector2(4.5f, 2.35f),
                new Vector2(6f, 2.55f),
                new Vector2(7.5f, 2.35f),
                new Vector2(10.5f, 0.35f),
                new Vector2(12.5f, 0.35f),
                new Vector2(4f, -2f),
                new Vector2(9f, -2f)
            };
            Transform[] spawnPoints = new Transform[positions.Length];

            for (int i = 0; i < positions.Length; i++)
            {
                GameObject point = new GameObject("Spawn Point " + (i + 1));
                point.transform.SetParent(spawnerObject.transform);
                point.transform.position = positions[i];
                spawnPoints[i] = point.transform;
            }

            SetObjectReference(spawner, "_coinPrefab", coinPrefab);
            SetObjectReferenceArray(spawner, "_spawnPoints", spawnPoints);
            SetInteger(spawner, "_maxActiveCoins", 6);
            SetFloat(spawner, "_respawnDelay", 3f);
        }

        private static Sprite LoadSprite(string assetPath)
        {
            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);

            if (sprite == null)
                throw new InvalidOperationException("Sprite is missing: " + assetPath);

            return sprite;
        }

        private static void ValidateCount<T>(int expectedCount) where T : Component
        {
            T[] components = UnityEngine.Object.FindObjectsByType<T>(FindObjectsInactive.Include);

            if (components.Length != expectedCount)
            {
                throw new InvalidOperationException(
                    typeof(T).Name + " count is " + components.Length + ", expected " + expectedCount + ".");
            }
        }

        private static void SetObjectReference(UnityEngine.Object target, string propertyName, UnityEngine.Object value)
        {
            SerializedObject serializedObject = new SerializedObject(target);
            SerializedProperty property = serializedObject.FindProperty(propertyName);
            property.objectReferenceValue = value;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void SetObjectReferenceArray(
            UnityEngine.Object target,
            string propertyName,
            IReadOnlyList<Transform> values)
        {
            SerializedObject serializedObject = new SerializedObject(target);
            SerializedProperty property = serializedObject.FindProperty(propertyName);
            property.arraySize = values.Count;

            for (int i = 0; i < values.Count; i++)
                property.GetArrayElementAtIndex(i).objectReferenceValue = values[i];

            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void SetFloat(UnityEngine.Object target, string propertyName, float value)
        {
            SerializedObject serializedObject = new SerializedObject(target);
            serializedObject.FindProperty(propertyName).floatValue = value;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void SetInteger(UnityEngine.Object target, string propertyName, int value)
        {
            SerializedObject serializedObject = new SerializedObject(target);
            serializedObject.FindProperty(propertyName).intValue = value;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void SetLayerMask(UnityEngine.Object target, string propertyName, int value)
        {
            SerializedObject serializedObject = new SerializedObject(target);
            serializedObject.FindProperty(propertyName).intValue = value;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}
