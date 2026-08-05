using System.IO;
using UnityEditor;
using UnityEngine;

namespace Platformer.Editor
{
    public static class PlatformerArtGenerator
    {
        public const string CloudPath = "Assets/Platformer/Art/Cloud.png";
        public const string CoinPath = "Assets/Platformer/Art/Coin.png";
        public const string EnemyPath = "Assets/Platformer/Art/Enemy.png";
        public const string HillPath = "Assets/Platformer/Art/Hill.png";
        public const string PlatformPath = "Assets/Platformer/Art/Platform.png";
        public const string PlayerIdlePath = "Assets/Platformer/Art/Player_Idle.png";
        public const string PlayerRunOnePath = "Assets/Platformer/Art/Player_Run_1.png";
        public const string PlayerRunTwoPath = "Assets/Platformer/Art/Player_Run_2.png";
        public const string PlayerRunThreePath = "Assets/Platformer/Art/Player_Run_3.png";
        public const string PlayerRunFourPath = "Assets/Platformer/Art/Player_Run_4.png";

        private const int DefaultPixelsPerUnit = 32;

        public static void Generate()
        {
            CreatePlayerSprite(PlayerIdlePath, 0);
            CreatePlayerSprite(PlayerRunOnePath, 1);
            CreatePlayerSprite(PlayerRunTwoPath, 2);
            CreatePlayerSprite(PlayerRunThreePath, 3);
            CreatePlayerSprite(PlayerRunFourPath, 4);
            CreateEnemySprite();
            CreateCoinSprite();
            CreatePlatformSprite();
            CreateCloudSprite();
            CreateHillSprite();

            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);

            ConfigureSprite(PlayerIdlePath, DefaultPixelsPerUnit);
            ConfigureSprite(PlayerRunOnePath, DefaultPixelsPerUnit);
            ConfigureSprite(PlayerRunTwoPath, DefaultPixelsPerUnit);
            ConfigureSprite(PlayerRunThreePath, DefaultPixelsPerUnit);
            ConfigureSprite(PlayerRunFourPath, DefaultPixelsPerUnit);
            ConfigureSprite(EnemyPath, DefaultPixelsPerUnit);
            ConfigureSprite(CoinPath, DefaultPixelsPerUnit);
            ConfigureSprite(PlatformPath, DefaultPixelsPerUnit);
            ConfigureSprite(CloudPath, DefaultPixelsPerUnit);
            ConfigureSprite(HillPath, DefaultPixelsPerUnit);
        }

        private static void CreatePlayerSprite(string assetPath, int frame)
        {
            const int width = 32;
            const int height = 48;

            Color32[] pixels = CreateTransparentPixels(width, height);
            Color32 outline = new Color32(34, 30, 53, 255);
            Color32 skin = new Color32(255, 196, 145, 255);
            Color32 hair = new Color32(73, 48, 68, 255);
            Color32 shirt = new Color32(60, 170, 255, 255);
            Color32 shirtLight = new Color32(125, 217, 255, 255);
            Color32 trousers = new Color32(54, 71, 112, 255);
            Color32 shoe = new Color32(248, 244, 218, 255);

            DrawRect(pixels, width, 9, 28, 22, 43, outline);
            DrawRect(pixels, width, 11, 29, 20, 41, skin);
            DrawRect(pixels, width, 9, 38, 22, 43, hair);
            DrawRect(pixels, width, 19, 34, 22, 36, outline);
            DrawRect(pixels, width, 20, 35, 21, 35, new Color32(255, 255, 255, 255));

            DrawRect(pixels, width, 7, 14, 24, 30, outline);
            DrawRect(pixels, width, 9, 16, 22, 29, shirt);
            DrawRect(pixels, width, 11, 24, 20, 28, shirtLight);
            DrawRect(pixels, width, 9, 14, 22, 17, trousers);

            DrawPlayerArms(pixels, width, frame, outline, skin, shirt);
            DrawPlayerLegs(pixels, width, frame, outline, trousers, shoe);
            WriteTexture(assetPath, width, height, pixels);
        }

        private static void DrawPlayerArms(
            Color32[] pixels,
            int width,
            int frame,
            Color32 outline,
            Color32 skin,
            Color32 shirt)
        {
            int leftOffset = frame == 1 || frame == 4 ? -2 : 1;
            int rightOffset = frame == 1 || frame == 4 ? 2 : -1;

            DrawRect(pixels, width, 4 + leftOffset, 15, 9 + leftOffset, 27, outline);
            DrawRect(pixels, width, 6 + leftOffset, 17, 8 + leftOffset, 25, shirt);
            DrawRect(pixels, width, 6 + leftOffset, 14, 8 + leftOffset, 17, skin);
            DrawRect(pixels, width, 22 + rightOffset, 15, 27 + rightOffset, 27, outline);
            DrawRect(pixels, width, 23 + rightOffset, 17, 25 + rightOffset, 25, shirt);
            DrawRect(pixels, width, 23 + rightOffset, 14, 25 + rightOffset, 17, skin);
        }

        private static void DrawPlayerLegs(
            Color32[] pixels,
            int width,
            int frame,
            Color32 outline,
            Color32 trousers,
            Color32 shoe)
        {
            int leftOffset = 0;
            int rightOffset = 0;
            int leftBottom = 3;
            int rightBottom = 3;

            if (frame == 1 || frame == 4)
            {
                leftOffset = -4;
                rightOffset = 2;
                rightBottom = 7;
            }
            else if (frame == 2)
            {
                leftOffset = -1;
                rightOffset = 1;
                leftBottom = 5;
                rightBottom = 5;
            }
            else if (frame == 3)
            {
                leftOffset = 2;
                rightOffset = -4;
                leftBottom = 7;
            }

            DrawRect(pixels, width, 8 + leftOffset, leftBottom, 14 + leftOffset, 17, outline);
            DrawRect(pixels, width, 10 + leftOffset, leftBottom + 2, 13 + leftOffset, 16, trousers);
            DrawRect(pixels, width, 6 + leftOffset, leftBottom, 13 + leftOffset, leftBottom + 3, shoe);
            DrawRect(pixels, width, 18 + rightOffset, rightBottom, 24 + rightOffset, 17, outline);
            DrawRect(pixels, width, 19 + rightOffset, rightBottom + 2, 22 + rightOffset, 16, trousers);
            DrawRect(pixels, width, 19 + rightOffset, rightBottom, 26 + rightOffset, rightBottom + 3, shoe);
        }

        private static void CreateEnemySprite()
        {
            const int size = 32;

            Color32[] pixels = CreateTransparentPixels(size, size);
            Color32 outline = new Color32(58, 31, 58, 255);
            Color32 body = new Color32(239, 83, 108, 255);
            Color32 highlight = new Color32(255, 137, 134, 255);
            Color32 eye = new Color32(255, 251, 226, 255);

            DrawCircle(pixels, size, 16, 15, 12, outline);
            DrawCircle(pixels, size, 16, 16, 10, body);
            DrawRect(pixels, size, 8, 6, 24, 13, body);
            DrawRect(pixels, size, 10, 20, 14, 23, highlight);
            DrawRect(pixels, size, 10, 13, 14, 18, eye);
            DrawRect(pixels, size, 18, 13, 22, 18, eye);
            DrawRect(pixels, size, 12, 14, 14, 17, outline);
            DrawRect(pixels, size, 18, 14, 20, 17, outline);
            DrawRect(pixels, size, 13, 8, 19, 9, outline);
            WriteTexture(EnemyPath, size, size, pixels);
        }

        private static void CreateCoinSprite()
        {
            const int size = 24;

            Color32[] pixels = CreateTransparentPixels(size, size);
            Color32 outline = new Color32(116, 66, 34, 255);
            Color32 gold = new Color32(255, 194, 52, 255);
            Color32 light = new Color32(255, 244, 129, 255);
            Color32 shadow = new Color32(222, 126, 33, 255);

            DrawCircle(pixels, size, 12, 12, 10, outline);
            DrawCircle(pixels, size, 12, 12, 8, gold);
            DrawRect(pixels, size, 8, 7, 10, 17, light);
            DrawRect(pixels, size, 14, 6, 16, 16, shadow);
            DrawRect(pixels, size, 10, 8, 14, 15, gold);
            WriteTexture(CoinPath, size, size, pixels);
        }

        private static void CreatePlatformSprite()
        {
            const int size = 32;

            Color32[] pixels = CreateTransparentPixels(size, size);
            Color32 darkSoil = new Color32(82, 54, 50, 255);
            Color32 soil = new Color32(133, 83, 58, 255);
            Color32 lightSoil = new Color32(170, 112, 68, 255);
            Color32 grass = new Color32(82, 190, 104, 255);
            Color32 grassLight = new Color32(155, 226, 112, 255);

            DrawRect(pixels, size, 0, 0, 31, 31, darkSoil);
            DrawRect(pixels, size, 2, 2, 29, 24, soil);
            DrawRect(pixels, size, 5, 5, 10, 10, lightSoil);
            DrawRect(pixels, size, 20, 13, 27, 18, lightSoil);
            DrawRect(pixels, size, 0, 25, 31, 31, grass);
            DrawRect(pixels, size, 0, 29, 31, 31, grassLight);
            WriteTexture(PlatformPath, size, size, pixels);
        }

        private static void CreateCloudSprite()
        {
            const int width = 64;
            const int height = 32;

            Color32[] pixels = CreateTransparentPixels(width, height);
            Color32 shadow = new Color32(187, 222, 237, 255);
            Color32 cloud = new Color32(241, 251, 255, 255);

            DrawCircle(pixels, width, 20, 14, 10, shadow);
            DrawCircle(pixels, width, 34, 18, 13, shadow);
            DrawCircle(pixels, width, 47, 13, 9, shadow);
            DrawRect(pixels, width, 13, 6, 53, 17, shadow);
            DrawCircle(pixels, width, 20, 17, 8, cloud);
            DrawCircle(pixels, width, 34, 21, 11, cloud);
            DrawCircle(pixels, width, 47, 16, 7, cloud);
            DrawRect(pixels, width, 14, 10, 52, 19, cloud);
            WriteTexture(CloudPath, width, height, pixels);
        }

        private static void CreateHillSprite()
        {
            const int width = 64;
            const int height = 48;

            Color32[] pixels = CreateTransparentPixels(width, height);
            Color32 hill = new Color32(97, 170, 137, 255);
            Color32 hillLight = new Color32(142, 202, 154, 255);

            DrawCircle(pixels, width, 32, 6, 38, hill);
            DrawCircle(pixels, width, 24, 5, 27, hillLight);
            DrawRect(pixels, width, 0, 0, 63, 10, hill);
            WriteTexture(HillPath, width, height, pixels);
        }

        private static Color32[] CreateTransparentPixels(int width, int height)
        {
            return new Color32[width * height];
        }

        private static void DrawRect(
            Color32[] pixels,
            int width,
            int left,
            int bottom,
            int right,
            int top,
            Color32 color)
        {
            int height = pixels.Length / width;
            int clampedLeft = Mathf.Clamp(left, 0, width - 1);
            int clampedRight = Mathf.Clamp(right, 0, width - 1);
            int clampedBottom = Mathf.Clamp(bottom, 0, height - 1);
            int clampedTop = Mathf.Clamp(top, 0, height - 1);

            for (int y = clampedBottom; y <= clampedTop; y++)
            {
                for (int x = clampedLeft; x <= clampedRight; x++)
                    pixels[y * width + x] = color;
            }
        }

        private static void DrawCircle(
            Color32[] pixels,
            int width,
            int centerX,
            int centerY,
            int radius,
            Color32 color)
        {
            int radiusSquared = radius * radius;

            for (int y = centerY - radius; y <= centerY + radius; y++)
            {
                for (int x = centerX - radius; x <= centerX + radius; x++)
                {
                    int deltaX = x - centerX;
                    int deltaY = y - centerY;

                    if (deltaX * deltaX + deltaY * deltaY <= radiusSquared)
                        DrawRect(pixels, width, x, y, x, y, color);
                }
            }
        }

        private static void WriteTexture(string assetPath, int width, int height, Color32[] pixels)
        {
            Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
            texture.SetPixels32(pixels);
            texture.Apply();

            byte[] png = texture.EncodeToPNG();
            Object.DestroyImmediate(texture);

            string projectPath = Directory.GetParent(Application.dataPath).FullName;
            string absolutePath = Path.Combine(projectPath, assetPath);
            string directoryPath = Path.GetDirectoryName(absolutePath);
            Directory.CreateDirectory(directoryPath);
            File.WriteAllBytes(absolutePath, png);
        }

        private static void ConfigureSprite(string assetPath, int pixelsPerUnit)
        {
            TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;

            if (importer == null)
                throw new InvalidDataException("Texture importer was not created for " + assetPath);

            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.spritePixelsPerUnit = pixelsPerUnit;
            importer.filterMode = FilterMode.Point;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.mipmapEnabled = false;
            importer.alphaIsTransparency = true;
            importer.SaveAndReimport();
        }
    }
}
