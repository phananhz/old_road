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
        public static Sprite CabinComplete() => GetOrCreate("cabin.complete", 36, 32, pixels => PaintCabin(pixels, 36, 32, 1f), new Vector2(0.5f, 0.12f));
        public static Sprite CabinConstruction(int stage) => GetOrCreate("cabin.stage." + Mathf.Clamp(stage, 0, 4), 36, 32, pixels => PaintCabin(pixels, 36, 32, Mathf.Clamp01(stage / 4f)), new Vector2(0.5f, 0.12f));
        public static Sprite PlacementPreview() => GetOrCreate("placement.preview", 32, 32, PaintPlacementPreview, new Vector2(0.5f, 0.5f));
        public static Sprite ValenOutskirtsGround() => GetOrCreate("valen.outskirts.ground", 320, 192, PaintValenOutskirtsGround, new Vector2(0.5f, 0.5f));

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
            const int width = 320;
            const int height = 192;
            Color32 grassA = new Color32(50, 82, 45, 255);
            Color32 grassB = new Color32(61, 94, 50, 255);
            Color32 pathA = new Color32(119, 91, 52, 255);
            Color32 pathB = new Color32(150, 113, 61, 255);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    bool path = Mathf.Abs(y - 86 - Mathf.Sin(x * 0.05f) * 14f) < 16f;
                    bool dither = ((x / 4) + (y / 4)) % 2 == 0;
                    pixels[y * width + x] = path ? (dither ? pathA : pathB) : (dither ? grassA : grassB);
                }
            }

            FillRect(pixels, width, height, 0, 150, width, 42, new Color32(35, 55, 47, 255));
            FillRect(pixels, width, height, 0, 0, width, 8, new Color32(27, 40, 34, 255));
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
