using System.IO;
using UnityEditor;
using UnityEngine;
using TheOldRoad.World;

namespace TheOldRoad.Editor
{
    /// <summary>
    /// Editor utility to generate and import handcrafted prototype pixel-art sprites as PNG assets.
    /// Configures point filtering, 16 PPU, uncompressed format, and custom Y-sort pivots.
    /// </summary>
    public static class AssetSpriteGenerator
    {
        public const string ArtAssetPrefix = "Assets/Game/Art/";

        [MenuItem("The Old Road/Generate All Prototype Sprites")]
        public static void GenerateAllSprites()
        {
            string artFsRoot = Path.Combine(Application.dataPath, "Game", "Art");
            EnsureDirectories(artFsRoot);
            int count = 0;

            foreach (ExportableSprite entry in PrototypePixelArtFactory.AllExportableSprites)
            {
                string fullPath = Path.Combine(artFsRoot, entry.RelativePath);
                string directory = Path.GetDirectoryName(fullPath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                Color32[] pixels = new Color32[entry.Width * entry.Height];
                entry.Paint(pixels);

                Texture2D texture = new Texture2D(entry.Width, entry.Height, TextureFormat.RGBA32, false);
                texture.SetPixels32(pixels);
                texture.Apply();

                byte[] pngBytes = texture.EncodeToPNG();
                Object.DestroyImmediate(texture);

                File.WriteAllBytes(fullPath, pngBytes);
                count++;
            }

            AssetDatabase.Refresh();

            // Configure TextureImporters
            foreach (ExportableSprite entry in PrototypePixelArtFactory.AllExportableSprites)
            {
                string assetPath = "Assets/Game/Art/" + entry.RelativePath.Replace('\\', '/');
                TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
                if (importer != null)
                {
                    bool modified = false;

                    if (importer.textureType != TextureImporterType.Sprite)
                    {
                        importer.textureType = TextureImporterType.Sprite;
                        modified = true;
                    }

                    if (importer.spriteImportMode != SpriteImportMode.Single)
                    {
                        importer.spriteImportMode = SpriteImportMode.Single;
                        modified = true;
                    }

                    if (Mathf.Abs(importer.spritePixelsPerUnit - PrototypePixelArtFactory.PixelsPerUnit) > 0.01f)
                    {
                        importer.spritePixelsPerUnit = PrototypePixelArtFactory.PixelsPerUnit;
                        modified = true;
                    }

                    if (importer.filterMode != FilterMode.Point)
                    {
                        importer.filterMode = FilterMode.Point;
                        modified = true;
                    }

                    if (importer.textureCompression != TextureImporterCompression.Uncompressed)
                    {
                        importer.textureCompression = TextureImporterCompression.Uncompressed;
                        modified = true;
                    }

                    TextureImporterSettings settings = new TextureImporterSettings();
                    importer.ReadTextureSettings(settings);
                    if (settings.spriteAlignment != (int)SpriteAlignment.Custom || settings.spritePivot != entry.Pivot)
                    {
                        settings.spriteAlignment = (int)SpriteAlignment.Custom;
                        settings.spritePivot = entry.Pivot;
                        importer.SetTextureSettings(settings);
                        modified = true;
                    }

                    if (modified)
                    {
                        importer.SaveAndReimport();
                    }
                }
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log($"[The Old Road] Successfully generated and configured {count} prototype pixel-art sprites in {artFsRoot}");
        }

        private static void EnsureDirectories(string artFsRoot)
        {
            string[] subdirs =
            {
                "Characters",
                "Environment",
                "Buildings",
                "Items",
                "UI",
                "VFX"
            };

            foreach (string subdir in subdirs)
            {
                string fullPath = Path.Combine(artFsRoot, subdir);
                if (!Directory.Exists(fullPath))
                {
                    Directory.CreateDirectory(fullPath);
                }
            }
        }
    }
}
