using System;
using System.Collections.Generic;
using UnityEngine;

namespace TheOldRoad.World
{
    /// <summary>
    /// Runtime placeholder pixel art for the prototype. Replace with authored sprites later.
    /// Textures are point-filtered so the game reads as pixel art while systems are still being built.
    /// </summary>
    public static class PrototypePixelArtFactory
    {
        public const float PixelsPerUnit = 16f;

        private static readonly Dictionary<string, Sprite> Cache = new Dictionary<string, Sprite>();

        public static Sprite Player() => GetOrCreate("player", 16, 24, PaintPlayer, new Vector2(0.5f, 0.18f));
        public static Sprite Tree() => GetOrCreate("tree", 24, 32, PaintTree, new Vector2(0.5f, 0.12f));
        public static Sprite Rock() => GetOrCreate("rock", 18, 14, PaintRock, new Vector2(0.5f, 0.18f));
        public static Sprite ChestClosed() => GetOrCreate("chest.closed", 22, 18, PaintChestClosed, new Vector2(0.5f, 0.12f));
        public static Sprite ChestOpen() => GetOrCreate("chest.open", 22, 18, PaintChestOpen, new Vector2(0.5f, 0.12f));
        public static Sprite Waystone() => GetOrCreate("waystone", 18, 28, PaintWaystone, new Vector2(0.5f, 0.12f));
        public static Sprite RoadSign() => GetOrCreate("road.sign", 22, 22, PaintRoadSign, new Vector2(0.5f, 0.12f));
        public static Sprite RuinedArch() => GetOrCreate("ruined.arch", 46, 36, PaintRuinedArch, new Vector2(0.5f, 0.08f));
        public static Sprite Footbridge() => GetOrCreate("footbridge", 48, 18, PaintFootbridge, new Vector2(0.5f, 0.5f));
        public static Sprite Campfire() => GetOrCreate("campfire", 28, 22, PaintCampfire, new Vector2(0.5f, 0.12f));
        public static Sprite CabinComplete() => GetOrCreate("cabin.complete", 36, 32, pixels => PaintCabin(pixels, 36, 32, 1f), new Vector2(0.5f, 0.12f));
        public static Sprite CabinConstruction(int stage) => GetOrCreate("cabin.stage." + Mathf.Clamp(stage, 0, 4), 36, 32, pixels => PaintCabin(pixels, 36, 32, Mathf.Clamp01(stage / 4f)), new Vector2(0.5f, 0.12f));
        public static Sprite PlacementPreview() => GetOrCreate("placement.preview", 32, 32, PaintPlacementPreview, new Vector2(0.5f, 0.5f));
        public static Sprite ValenOutskirtsGround() => GetOrCreate("valen.outskirts.ground.seeded.120x72", 1920, 1152, PaintValenOutskirtsGround, new Vector2(0.5f, 0.5f));

        private static Sprite GetOrCreate(string key, int width, int height, Action<Color32[]> paint, Vector2 pivot)
        {
            if (Cache.TryGetValue(key, out Sprite cached)) return cached;

            Color32[] pixels = new Color32[width * height];
            for (int i = 0; i < pixels.Length; i++) pixels[i] = new Color32(0, 0, 0, 0);
            paint(pixels);

            Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false)
            {
                name = "Prototype Pixel Art - " + key,
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Clamp,
                hideFlags = HideFlags.DontSave
            };
            texture.SetPixels32(pixels);
            texture.Apply(false, true);

            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, width, height), pivot, PixelsPerUnit, 0, SpriteMeshType.FullRect);
            sprite.name = "Prototype Pixel Art - " + key;
            sprite.hideFlags = HideFlags.DontSave;
            Cache[key] = sprite;
            return sprite;
        }

        private static void PaintPlayer(Color32[] pixels)
        {
            FillRect(pixels, 16, 24, 5, 2, 6, 4, new Color32(37, 28, 26, 255));
            FillRect(pixels, 16, 24, 4, 6, 8, 9, new Color32(48, 55, 68, 255));
            FillRect(pixels, 16, 24, 5, 15, 6, 5, new Color32(86, 89, 96, 255));
            FillRect(pixels, 16, 24, 6, 16, 4, 2, new Color32(151, 157, 165, 255));
            FillRect(pixels, 16, 24, 3, 8, 2, 8, new Color32(94, 34, 37, 255));
            FillRect(pixels, 16, 24, 11, 8, 2, 8, new Color32(94, 34, 37, 255));
            FillRect(pixels, 16, 24, 7, 0, 2, 22, new Color32(168, 142, 76, 255));
            SetPixel(pixels, 16, 24, 7, 19, new Color32(220, 209, 157, 255));
        }

        private static void PaintTree(Color32[] pixels)
        {
            FillRect(pixels, 24, 32, 10, 0, 4, 10, new Color32(96, 56, 31, 255));
            FillRect(pixels, 24, 32, 8, 2, 8, 4, new Color32(128, 79, 42, 255));
            FillRect(pixels, 24, 32, 5, 8, 14, 8, new Color32(25, 83, 41, 255));
            FillRect(pixels, 24, 32, 3, 14, 18, 8, new Color32(18, 101, 45, 255));
            FillRect(pixels, 24, 32, 6, 22, 12, 7, new Color32(43, 129, 55, 255));
            FillRect(pixels, 24, 32, 8, 25, 4, 2, new Color32(101, 157, 77, 255));
        }

        private static void PaintRock(Color32[] pixels)
        {
            FillRect(pixels, 18, 14, 2, 2, 14, 7, new Color32(82, 84, 88, 255));
            FillRect(pixels, 18, 14, 5, 7, 9, 4, new Color32(112, 116, 122, 255));
            FillRect(pixels, 18, 14, 3, 3, 4, 2, new Color32(139, 143, 148, 255));
            FillRect(pixels, 18, 14, 10, 2, 5, 2, new Color32(52, 53, 58, 255));
        }

        private static void PaintChestClosed(Color32[] pixels)
        {
            FillRect(pixels, 22, 18, 3, 3, 16, 9, new Color32(92, 55, 28, 255));
            FillRect(pixels, 22, 18, 2, 9, 18, 5, new Color32(128, 76, 36, 255));
            FillRect(pixels, 22, 18, 4, 11, 14, 2, new Color32(178, 120, 48, 255));
            FillRect(pixels, 22, 18, 9, 7, 4, 4, new Color32(210, 164, 62, 255));
            DrawRect(pixels, 22, 18, 2, 3, 19, 14, new Color32(48, 30, 21, 255));
        }

        private static void PaintChestOpen(Color32[] pixels)
        {
            FillRect(pixels, 22, 18, 3, 2, 16, 8, new Color32(82, 48, 26, 255));
            FillRect(pixels, 22, 18, 2, 11, 18, 4, new Color32(110, 62, 30, 255));
            FillRect(pixels, 22, 18, 5, 10, 12, 2, new Color32(232, 184, 72, 255));
            FillRect(pixels, 22, 18, 9, 5, 4, 3, new Color32(178, 130, 46, 255));
            DrawRect(pixels, 22, 18, 2, 2, 19, 15, new Color32(43, 27, 19, 255));
        }

        private static void PaintWaystone(Color32[] pixels)
        {
            FillRect(pixels, 18, 28, 5, 1, 8, 20, new Color32(69, 74, 78, 255));
            FillRect(pixels, 18, 28, 4, 4, 10, 15, new Color32(92, 98, 104, 255));
            FillRect(pixels, 18, 28, 6, 19, 6, 5, new Color32(112, 119, 126, 255));
            FillRect(pixels, 18, 28, 7, 8, 4, 2, new Color32(87, 126, 141, 255));
            FillRect(pixels, 18, 28, 8, 11, 2, 5, new Color32(41, 58, 65, 255));
            FillRect(pixels, 18, 28, 3, 0, 12, 2, new Color32(39, 42, 45, 255));
        }

        private static void PaintRoadSign(Color32[] pixels)
        {
            FillRect(pixels, 22, 22, 10, 0, 3, 16, new Color32(82, 51, 28, 255));
            FillRect(pixels, 22, 22, 3, 11, 16, 6, new Color32(132, 86, 41, 255));
            FillRect(pixels, 22, 22, 5, 13, 10, 2, new Color32(186, 145, 77, 255));
            FillRect(pixels, 22, 22, 18, 13, 2, 2, new Color32(132, 86, 41, 255));
        }

        private static void PaintRuinedArch(Color32[] pixels)
        {
            Color32 stoneDark = new Color32(56, 58, 62, 255);
            Color32 stone = new Color32(90, 95, 100, 255);
            Color32 stoneLight = new Color32(128, 133, 137, 255);

            FillRect(pixels, 46, 36, 5, 0, 8, 26, stoneDark);
            FillRect(pixels, 46, 36, 33, 0, 8, 26, stoneDark);
            FillRect(pixels, 46, 36, 9, 24, 28, 7, stoneDark);
            FillRect(pixels, 46, 36, 7, 3, 5, 18, stone);
            FillRect(pixels, 46, 36, 34, 3, 5, 18, stone);
            FillRect(pixels, 46, 36, 12, 25, 22, 4, stone);
            FillRect(pixels, 46, 36, 14, 13, 18, 13, new Color32(0, 0, 0, 0));
            FillRect(pixels, 46, 36, 8, 21, 6, 3, stoneLight);
            FillRect(pixels, 46, 36, 30, 26, 5, 2, stoneLight);
            FillRect(pixels, 46, 36, 2, 0, 14, 3, new Color32(38, 41, 44, 255));
            FillRect(pixels, 46, 36, 30, 0, 14, 3, new Color32(38, 41, 44, 255));
        }

        private static void PaintFootbridge(Color32[] pixels)
        {
            FillRect(pixels, 48, 18, 2, 5, 44, 8, new Color32(96, 57, 31, 255));
            FillRect(pixels, 48, 18, 3, 9, 42, 3, new Color32(140, 85, 42, 255));
            for (int x = 6; x < 44; x += 7)
            {
                FillRect(pixels, 48, 18, x, 3, 2, 12, new Color32(55, 34, 23, 255));
            }
            FillRect(pixels, 48, 18, 0, 4, 48, 2, new Color32(46, 30, 21, 255));
        }

        private static void PaintCampfire(Color32[] pixels)
        {
            FillRect(pixels, 28, 22, 5, 2, 18, 4, new Color32(58, 35, 22, 255));
            FillRect(pixels, 28, 22, 8, 5, 12, 3, new Color32(93, 59, 32, 255));
            FillRect(pixels, 28, 22, 11, 8, 6, 9, new Color32(207, 63, 34, 255));
            FillRect(pixels, 28, 22, 12, 10, 4, 8, new Color32(238, 136, 45, 255));
            FillRect(pixels, 28, 22, 13, 12, 2, 6, new Color32(252, 215, 87, 255));
            FillRect(pixels, 28, 22, 4, 1, 5, 3, new Color32(62, 64, 66, 255));
            FillRect(pixels, 28, 22, 19, 1, 5, 3, new Color32(62, 64, 66, 255));
        }

        private static void PaintCabin(Color32[] pixels, int width, int height, float progress)
        {
            FillRect(pixels, width, height, 4, 1, 28, 5, new Color32(72, 55, 38, 255));
            if (progress < 0.25f) return;

            FillRect(pixels, width, height, 7, 6, 22, 11, new Color32(104, 69, 41, 255));
            FillRect(pixels, width, height, 10, 7, 4, 8, new Color32(50, 32, 24, 255));
            if (progress < 0.5f) return;

            FillRect(pixels, width, height, 5, 16, 26, 4, new Color32(62, 42, 34, 255));
            FillRect(pixels, width, height, 8, 20, 20, 4, new Color32(91, 38, 35, 255));
            if (progress < 0.75f) return;

            FillRect(pixels, width, height, 12, 9, 5, 4, new Color32(222, 157, 71, 255));
            FillRect(pixels, width, height, 20, 9, 5, 4, new Color32(222, 157, 71, 255));
            FillRect(pixels, width, height, 14, 24, 8, 4, new Color32(129, 45, 39, 255));
        }

        private static void PaintPlacementPreview(Color32[] pixels)
        {
            DrawRect(pixels, 32, 32, 0, 0, 31, 31, new Color32(255, 255, 255, 255));
            DrawRect(pixels, 32, 32, 1, 1, 30, 30, new Color32(255, 255, 255, 180));
        }

        private static void PaintValenOutskirtsGround(Color32[] pixels)
        {
            const int width = 1920;
            const int height = 1152;
            Color32 grassA = new Color32(50, 82, 45, 255);
            Color32 grassB = new Color32(61, 94, 50, 255);
            Color32 forestA = new Color32(25, 52, 38, 255);
            Color32 forestB = new Color32(32, 62, 43, 255);
            Color32 pathA = new Color32(119, 91, 52, 255);
            Color32 pathB = new Color32(150, 113, 61, 255);
            Color32 riverA = new Color32(38, 76, 91, 255);
            Color32 riverB = new Color32(46, 93, 111, 255);
            Color32 stone = new Color32(83, 87, 91, 255);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float worldX = (x / 16f) - 60f;
                    float worldY = (y / 16f) - 36f;
                    float roadCenter = 1.4f * Mathf.Sin(worldX * 0.34f) + 0.8f * Mathf.Sin(worldX * 0.11f);
                    float regionNoise = Hash01((x / 32) + 43129, (y / 32) - 43129);
                    bool path = Mathf.Abs(worldY - roadCenter) < 2.0f;
                    bool river = Mathf.Abs(worldY + 12.5f + Mathf.Sin(worldX * 0.16f) * 2.0f) < 1.05f && worldX < 28f;
                    bool forest = worldY > 22.5f || worldY < -28f || worldX < -54f || worldX > 54f || regionNoise > 0.76f;
                    bool dither = ((x / 4) + (y / 4)) % 2 == 0;

                    Color32 color = forest ? (dither ? forestA : forestB) : (dither ? grassA : grassB);
                    if (river) color = dither ? riverA : riverB;
                    if (path) color = dither ? pathA : pathB;
                    if (!path && !river && regionNoise < 0.055f) color = dither ? new Color32(71, 83, 48, 255) : new Color32(82, 91, 55, 255);
                    pixels[y * width + x] = color;
                }
            }

            FillRect(pixels, width, height, 0, height - 96, width, 96, new Color32(28, 48, 41, 255));
            FillRect(pixels, width, height, 0, 0, width, 24, new Color32(25, 36, 32, 255));

            for (int i = 0; i < 54; i++)
            {
                int px = 24 + Mathf.FloorToInt(Hash01(i * 17 + 3, i * 31 + 7) * (width - 48));
                int py = 24 + Mathf.FloorToInt(Hash01(i * 23 + 11, i * 47 + 13) * (height - 48));
                if (i % 3 == 0) PaintPebbleCluster(pixels, width, height, px, py, stone);
                else PaintFlowerPatch(pixels, width, height, px, py);
            }
        }

        private static float Hash01(int x, int y)
        {
            unchecked
            {
                int hash = x * 374761393 + y * 668265263;
                hash = (hash ^ (hash >> 13)) * 1274126177;
                return ((hash ^ (hash >> 16)) & 0x7fffffff) / 2147483647f;
            }
        }

        private static void PaintPebbleCluster(Color32[] pixels, int width, int height, int x, int y, Color32 color)
        {
            FillRect(pixels, width, height, x, y, 5, 3, color);
            FillRect(pixels, width, height, x + 8, y + 2, 4, 3, color);
            FillRect(pixels, width, height, x + 14, y - 2, 6, 4, new Color32(55, 58, 61, 255));
        }

        private static void PaintFlowerPatch(Color32[] pixels, int width, int height, int x, int y)
        {
            Color32 yellow = new Color32(214, 174, 66, 255);
            Color32 red = new Color32(130, 50, 52, 255);
            SetPixel(pixels, width, height, x, y, yellow);
            SetPixel(pixels, width, height, x + 6, y + 3, red);
            SetPixel(pixels, width, height, x + 12, y - 1, yellow);
            SetPixel(pixels, width, height, x + 18, y + 4, red);
        }

        private static void FillRect(Color32[] pixels, int width, int height, int x, int y, int w, int h, Color32 color)
        {
            for (int iy = y; iy < y + h; iy++)
            {
                for (int ix = x; ix < x + w; ix++) SetPixel(pixels, width, height, ix, iy, color);
            }
        }

        private static void DrawRect(Color32[] pixels, int width, int height, int left, int bottom, int right, int top, Color32 color)
        {
            for (int x = left; x <= right; x++)
            {
                SetPixel(pixels, width, height, x, bottom, color);
                SetPixel(pixels, width, height, x, top, color);
            }

            for (int y = bottom; y <= top; y++)
            {
                SetPixel(pixels, width, height, left, y, color);
                SetPixel(pixels, width, height, right, y, color);
            }
        }

        private static void SetPixel(Color32[] pixels, int width, int height, int x, int y, Color32 color)
        {
            if (x < 0 || x >= width || y < 0 || y >= height) return;
            pixels[y * width + x] = color;
        }
    }
}
