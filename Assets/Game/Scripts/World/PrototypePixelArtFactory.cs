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

        public static Sprite Player() => PlayerWalk(0);
        public static Sprite PlayerWalk(int frame) => GetOrCreate("player.walk." + Mathf.Abs(frame % 4), 16, 24, pixels => PaintPlayer(pixels, Mathf.Abs(frame % 4)), new Vector2(0.5f, 0.18f));
        public static Sprite Tree() => GetOrCreate("tree", 24, 32, PaintTree, new Vector2(0.5f, 0.12f));
        public static Sprite Rock() => GetOrCreate("rock", 18, 14, PaintRock, new Vector2(0.5f, 0.18f));
        public static Sprite BerryBush() => GetOrCreate("berry.bush", 22, 20, PaintBerryBush, new Vector2(0.5f, 0.12f));
        public static Sprite HerbPatch() => GetOrCreate("herb.patch", 20, 16, PaintHerbPatch, new Vector2(0.5f, 0.12f));
        public static Sprite MushroomCluster() => GetOrCreate("mushroom.cluster", 20, 16, PaintMushroomCluster, new Vector2(0.5f, 0.12f));
        public static Sprite IronOre() => GetOrCreate("iron.ore", 20, 16, PaintIronOre, new Vector2(0.5f, 0.18f));
        public static Sprite WaterRipple() => GetOrCreate("water.ripple", 24, 8, PaintWaterRipple, new Vector2(0.5f, 0.5f));
        public static Sprite CabinInteriorFloor() => GetOrCreate("cabin.interior.floor.rooms.v2", 176, 112, PaintCabinInteriorFloor, new Vector2(0.5f, 0.5f));
        public static Sprite CabinInteriorWall() => GetOrCreate("cabin.interior.wall.rooms.v2", 176, 48, PaintCabinInteriorWall, new Vector2(0.5f, 0.5f));
        public static Sprite CabinPartitionWall() => GetOrCreate("cabin.partition.wall", 18, 92, PaintCabinPartitionWall, new Vector2(0.5f, 0.5f));
        public static Sprite CabinBed() => GetOrCreate("cabin.bed", 34, 24, PaintCabinBed, new Vector2(0.5f, 0.15f));
        public static Sprite CabinHearth() => GetOrCreate("cabin.hearth", 28, 28, PaintCabinHearth, new Vector2(0.5f, 0.12f));
        public static Sprite CabinTable() => GetOrCreate("cabin.table", 30, 22, PaintCabinTable, new Vector2(0.5f, 0.15f));
        public static Sprite CabinBench() => GetOrCreate("cabin.bench", 28, 14, PaintCabinBench, new Vector2(0.5f, 0.15f));
        public static Sprite CabinKitchenCounter() => GetOrCreate("cabin.kitchen.counter", 38, 22, PaintCabinKitchenCounter, new Vector2(0.5f, 0.15f));
        public static Sprite CabinDoorMarker() => GetOrCreate("cabin.door.marker", 26, 12, PaintCabinDoorMarker, new Vector2(0.5f, 0.5f));
        public static Sprite TorchGlow() => GetOrCreate("torch.glow", 96, 96, PaintTorchGlow, new Vector2(0.5f, 0.5f));
        public static Sprite SolidPixel() => GetOrCreate("solid.pixel", 1, 1, PaintSolidPixel, new Vector2(0.5f, 0.5f));
        public static Sprite ChestClosed() => GetOrCreate("chest.closed", 22, 18, PaintChestClosed, new Vector2(0.5f, 0.12f));
        public static Sprite ChestOpen() => GetOrCreate("chest.open", 22, 18, PaintChestOpen, new Vector2(0.5f, 0.12f));
        public static Sprite Waystone() => GetOrCreate("waystone", 18, 28, PaintWaystone, new Vector2(0.5f, 0.12f));
        public static Sprite RoadSign() => GetOrCreate("road.sign", 22, 22, PaintRoadSign, new Vector2(0.5f, 0.12f));
        public static Sprite RuinedArch() => GetOrCreate("ruined.arch", 46, 36, PaintRuinedArch, new Vector2(0.5f, 0.08f));
        public static Sprite Footbridge() => GetOrCreate("footbridge", 48, 18, PaintFootbridge, new Vector2(0.5f, 0.5f));
        public static Sprite Campfire() => GetOrCreate("campfire", 28, 22, PaintCampfire, new Vector2(0.5f, 0.12f));
        public static Sprite CookingHearthOutdoor() => GetOrCreate("cooking.hearth.outdoor", 34, 30, PaintCookingHearthOutdoor, new Vector2(0.5f, 0.12f));
        public static Sprite AnimalPenSmall() => GetOrCreate("animal.pen.small", 46, 34, pixels => PaintAnimalPen(pixels, 46, 34, false), new Vector2(0.5f, 0.12f));
        public static Sprite AnimalPenLong() => GetOrCreate("animal.pen.long", 64, 34, pixels => PaintAnimalPen(pixels, 64, 34, true), new Vector2(0.5f, 0.12f));
        public static Sprite StorageShed() => GetOrCreate("storage.shed", 42, 34, PaintStorageShed, new Vector2(0.5f, 0.12f));
        public static Sprite StoneCottage() => GetOrCreate("stone.cottage", 58, 46, PaintStoneCottage, new Vector2(0.5f, 0.12f));
        public static Sprite CabinComplete() => GetOrCreate("cabin.complete.v2", 52, 44, pixels => PaintCabin(pixels, 52, 44, 1f), new Vector2(0.5f, 0.12f));
        public static Sprite CabinConstruction(int stage) => GetOrCreate("cabin.stage.v2." + Mathf.Clamp(stage, 0, 4), 52, 44, pixels => PaintCabin(pixels, 52, 44, Mathf.Clamp01(stage / 4f)), new Vector2(0.5f, 0.12f));
        public static Sprite BuildingComplete(string buildingId)
        {
            switch (buildingId)
            {
                case "building.campfire": return Campfire();
                case "building.cooking-hearth": return CookingHearthOutdoor();
                case "building.animal-pen-small": return AnimalPenSmall();
                case "building.animal-pen-long": return AnimalPenLong();
                case "building.storage-shed": return StorageShed();
                case "building.stone-cottage": return StoneCottage();
                default: return CabinComplete();
            }
        }

        public static Sprite BuildingConstruction(string buildingId, int stage)
        {
            return CabinConstruction(stage);
        }
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

        private static void PaintPlayer(Color32[] pixels, int frame)
        {
            int step = frame == 1 ? 1 : frame == 3 ? -1 : 0;
            int capeSwing = frame == 1 || frame == 2 ? 1 : 0;

            FillRect(pixels, 16, 24, 5, 1, 3, 5 + Mathf.Max(0, step), new Color32(37, 28, 26, 255));
            FillRect(pixels, 16, 24, 8, 1, 3, 5 + Mathf.Max(0, -step), new Color32(37, 28, 26, 255));
            FillRect(pixels, 16, 24, 4, 6, 8, 9, new Color32(48, 55, 68, 255));
            FillRect(pixels, 16, 24, 5, 7, 6, 7, new Color32(36, 42, 54, 255));
            FillRect(pixels, 16, 24, 5, 15, 6, 5, new Color32(86, 89, 96, 255));
            FillRect(pixels, 16, 24, 6, 16, 4, 2, new Color32(151, 157, 165, 255));
            FillRect(pixels, 16, 24, 3, 8 + capeSwing, 2, 8, new Color32(94, 34, 37, 255));
            FillRect(pixels, 16, 24, 11, 8 - capeSwing, 2, 8, new Color32(94, 34, 37, 255));
            SetPixel(pixels, 16, 24, 4, 7 + capeSwing, new Color32(142, 48, 52, 255));
            SetPixel(pixels, 16, 24, 11, 7 - capeSwing, new Color32(142, 48, 52, 255));
            FillRect(pixels, 16, 24, 7, 0, 2, 22, new Color32(168, 142, 76, 255));
            SetPixel(pixels, 16, 24, 7, 19, new Color32(220, 209, 157, 255));
            SetPixel(pixels, 16, 24, 9, 19, new Color32(220, 209, 157, 255));
            FillRect(pixels, 16, 24, 6, 20, 4, 2, new Color32(58, 62, 70, 255));
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

        private static void PaintBerryBush(Color32[] pixels)
        {
            FillRect(pixels, 22, 20, 4, 3, 14, 8, new Color32(30, 86, 42, 255));
            FillRect(pixels, 22, 20, 6, 9, 10, 6, new Color32(42, 119, 52, 255));
            FillRect(pixels, 22, 20, 2, 7, 6, 5, new Color32(24, 74, 37, 255));
            FillRect(pixels, 22, 20, 14, 6, 6, 6, new Color32(24, 74, 37, 255));
            SetPixel(pixels, 22, 20, 7, 11, new Color32(178, 24, 38, 255));
            SetPixel(pixels, 22, 20, 12, 13, new Color32(212, 39, 55, 255));
            SetPixel(pixels, 22, 20, 16, 9, new Color32(178, 24, 38, 255));
            SetPixel(pixels, 22, 20, 9, 6, new Color32(235, 66, 74, 255));
        }

        private static void PaintHerbPatch(Color32[] pixels)
        {
            FillRect(pixels, 20, 16, 3, 1, 14, 3, new Color32(25, 72, 36, 255));
            FillRect(pixels, 20, 16, 5, 4, 2, 8, new Color32(55, 143, 66, 255));
            FillRect(pixels, 20, 16, 10, 3, 2, 10, new Color32(63, 170, 74, 255));
            FillRect(pixels, 20, 16, 14, 4, 2, 7, new Color32(46, 126, 58, 255));
            FillRect(pixels, 20, 16, 4, 9, 5, 2, new Color32(86, 194, 92, 255));
            FillRect(pixels, 20, 16, 11, 11, 5, 2, new Color32(108, 216, 113, 255));
        }

        private static void PaintMushroomCluster(Color32[] pixels)
        {
            FillRect(pixels, 20, 16, 4, 1, 3, 7, new Color32(223, 199, 157, 255));
            FillRect(pixels, 20, 16, 12, 1, 3, 6, new Color32(213, 187, 145, 255));
            FillRect(pixels, 20, 16, 2, 7, 8, 5, new Color32(138, 63, 54, 255));
            FillRect(pixels, 20, 16, 10, 6, 8, 5, new Color32(169, 78, 61, 255));
            FillRect(pixels, 20, 16, 4, 10, 4, 2, new Color32(215, 126, 92, 255));
            FillRect(pixels, 20, 16, 12, 9, 3, 2, new Color32(231, 144, 98, 255));
        }

        private static void PaintIronOre(Color32[] pixels)
        {
            FillRect(pixels, 20, 16, 2, 2, 16, 8, new Color32(59, 63, 68, 255));
            FillRect(pixels, 20, 16, 5, 8, 10, 5, new Color32(82, 87, 94, 255));
            FillRect(pixels, 20, 16, 4, 4, 4, 2, new Color32(135, 112, 84, 255));
            FillRect(pixels, 20, 16, 11, 5, 5, 2, new Color32(151, 126, 91, 255));
            FillRect(pixels, 20, 16, 7, 10, 4, 2, new Color32(191, 160, 111, 255));
            DrawRect(pixels, 20, 16, 2, 2, 18, 12, new Color32(36, 39, 44, 255));
        }

        private static void PaintWaterRipple(Color32[] pixels)
        {
            FillRect(pixels, 24, 8, 2, 3, 8, 1, new Color32(130, 202, 225, 150));
            FillRect(pixels, 24, 8, 12, 5, 9, 1, new Color32(92, 170, 198, 120));
            FillRect(pixels, 24, 8, 6, 1, 12, 1, new Color32(189, 226, 238, 95));
        }

        private static void PaintCabinInteriorFloor(Color32[] pixels)
        {
            const int width = 176;
            const int height = 112;
            Color32 floorA = new Color32(88, 58, 34, 255);
            Color32 floorB = new Color32(104, 68, 38, 255);
            Color32 seam = new Color32(58, 36, 24, 255);
            Color32 wallShadow = new Color32(43, 30, 24, 255);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    bool plank = (y / 8) % 2 == 0;
                    pixels[y * width + x] = plank ? floorA : floorB;
                    if (y % 8 == 0 || x % 32 == 0) pixels[y * width + x] = seam;
                }
            }

            FillRect(pixels, width, height, 0, height - 10, width, 10, wallShadow);
            FillRect(pixels, width, height, 0, 0, width, 3, new Color32(36, 25, 20, 255));
            FillRect(pixels, width, height, 4, 14, 46, 34, new Color32(78, 38, 38, 255));
            DrawRect(pixels, width, height, 4, 14, 49, 47, new Color32(132, 76, 50, 255));
            FillRect(pixels, width, height, 66, 18, 42, 26, new Color32(68, 78, 62, 255));
            DrawRect(pixels, width, height, 66, 18, 107, 43, new Color32(132, 105, 58, 255));
            FillRect(pixels, width, height, 128, 12, 38, 30, new Color32(67, 58, 45, 255));
            DrawRect(pixels, width, height, 128, 12, 165, 41, new Color32(118, 91, 52, 255));
        }

        private static void PaintCabinInteriorWall(Color32[] pixels)
        {
            const int width = 176;
            const int height = 48;
            Color32 wallA = new Color32(64, 42, 31, 255);
            Color32 wallB = new Color32(80, 52, 35, 255);
            Color32 beam = new Color32(38, 26, 20, 255);
            Color32 shelf = new Color32(108, 70, 37, 255);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    pixels[y * width + x] = ((x / 10 + y / 6) % 2 == 0) ? wallA : wallB;
                    if (x % 44 < 3 || y < 4 || y > height - 5) pixels[y * width + x] = beam;
                }
            }

            FillRect(pixels, width, height, 15, 19, 28, 12, new Color32(31, 52, 62, 255));
            DrawRect(pixels, width, height, 15, 19, 42, 30, new Color32(35, 24, 17, 255));
            FillRect(pixels, width, height, 70, 24, 34, 4, shelf);
            FillRect(pixels, width, height, 74, 29, 6, 8, new Color32(130, 50, 42, 255));
            FillRect(pixels, width, height, 84, 29, 5, 8, new Color32(76, 94, 70, 255));
            FillRect(pixels, width, height, 94, 29, 6, 8, new Color32(156, 119, 58, 255));
            FillRect(pixels, width, height, 126, 17, 34, 11, new Color32(32, 44, 54, 255));
            DrawRect(pixels, width, height, 126, 17, 159, 27, new Color32(35, 24, 17, 255));
        }

        private static void PaintCabinPartitionWall(Color32[] pixels)
        {
            const int width = 18;
            const int height = 92;
            Color32 timber = new Color32(74, 45, 27, 255);
            Color32 edge = new Color32(37, 24, 18, 255);
            Color32 plaster = new Color32(83, 62, 44, 255);

            FillRect(pixels, width, height, 6, 0, 6, height, timber);
            FillRect(pixels, width, height, 2, 0, 2, height, edge);
            FillRect(pixels, width, height, 14, 0, 2, height, edge);
            for (int y = 8; y < height - 8; y += 16)
            {
                FillRect(pixels, width, height, 3, y, 12, 3, edge);
            }

            FillRect(pixels, width, height, 4, 18, 10, 17, new Color32(0, 0, 0, 0));
            FillRect(pixels, width, height, 4, 52, 10, 18, plaster);
            DrawRect(pixels, width, height, 4, 52, 13, 69, edge);
        }

        private static void PaintCabinBed(Color32[] pixels)
        {
            FillRect(pixels, 34, 24, 3, 2, 28, 5, new Color32(72, 42, 28, 255));
            FillRect(pixels, 34, 24, 4, 7, 26, 13, new Color32(96, 36, 42, 255));
            FillRect(pixels, 34, 24, 7, 15, 20, 4, new Color32(142, 55, 60, 255));
            FillRect(pixels, 34, 24, 7, 8, 8, 6, new Color32(214, 190, 146, 255));
            DrawRect(pixels, 34, 24, 3, 2, 31, 20, new Color32(38, 25, 18, 255));
        }

        private static void PaintCabinHearth(Color32[] pixels)
        {
            FillRect(pixels, 28, 28, 4, 2, 20, 6, new Color32(58, 59, 61, 255));
            FillRect(pixels, 28, 28, 6, 8, 16, 12, new Color32(75, 76, 78, 255));
            FillRect(pixels, 28, 28, 9, 10, 10, 5, new Color32(27, 20, 16, 255));
            FillRect(pixels, 28, 28, 12, 14, 4, 8, new Color32(219, 79, 35, 255));
            FillRect(pixels, 28, 28, 13, 16, 2, 7, new Color32(252, 185, 64, 255));
            FillRect(pixels, 28, 28, 8, 21, 12, 3, new Color32(46, 47, 49, 255));
        }

        private static void PaintCabinTable(Color32[] pixels)
        {
            FillRect(pixels, 30, 22, 5, 7, 20, 9, new Color32(104, 67, 36, 255));
            FillRect(pixels, 30, 22, 7, 14, 16, 3, new Color32(153, 99, 48, 255));
            FillRect(pixels, 30, 22, 7, 2, 3, 7, new Color32(66, 43, 28, 255));
            FillRect(pixels, 30, 22, 20, 2, 3, 7, new Color32(66, 43, 28, 255));
            FillRect(pixels, 30, 22, 13, 16, 4, 2, new Color32(214, 172, 82, 255));
        }

        private static void PaintCabinBench(Color32[] pixels)
        {
            FillRect(pixels, 28, 14, 4, 7, 20, 4, new Color32(113, 72, 38, 255));
            FillRect(pixels, 28, 14, 3, 10, 22, 3, new Color32(153, 98, 49, 255));
            FillRect(pixels, 28, 14, 6, 2, 3, 6, new Color32(55, 35, 24, 255));
            FillRect(pixels, 28, 14, 19, 2, 3, 6, new Color32(55, 35, 24, 255));
            DrawRect(pixels, 28, 14, 3, 2, 25, 13, new Color32(38, 24, 18, 255));
        }

        private static void PaintCabinKitchenCounter(Color32[] pixels)
        {
            FillRect(pixels, 38, 22, 3, 5, 32, 10, new Color32(82, 57, 39, 255));
            FillRect(pixels, 38, 22, 3, 15, 32, 4, new Color32(139, 101, 58, 255));
            FillRect(pixels, 38, 22, 6, 7, 7, 6, new Color32(54, 39, 30, 255));
            FillRect(pixels, 38, 22, 16, 8, 7, 5, new Color32(65, 68, 67, 255));
            FillRect(pixels, 38, 22, 27, 7, 5, 7, new Color32(112, 79, 42, 255));
            FillRect(pixels, 38, 22, 17, 12, 5, 2, new Color32(138, 154, 150, 255));
            DrawRect(pixels, 38, 22, 3, 5, 35, 19, new Color32(35, 23, 17, 255));
        }

        private static void PaintCabinDoorMarker(Color32[] pixels)
        {
            FillRect(pixels, 26, 12, 2, 2, 22, 7, new Color32(68, 42, 28, 255));
            FillRect(pixels, 26, 12, 4, 5, 18, 3, new Color32(125, 78, 39, 255));
            DrawRect(pixels, 26, 12, 2, 2, 24, 9, new Color32(35, 22, 17, 255));
        }

        private static void PaintTorchGlow(Color32[] pixels)
        {
            const int width = 96;
            const int height = 96;
            Vector2 center = new Vector2((width - 1) * 0.5f, (height - 1) * 0.5f);
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float distance = Vector2.Distance(new Vector2(x, y), center) / 48f;
                    byte alpha = (byte)Mathf.RoundToInt(Mathf.Clamp01(1f - distance) * 120f);
                    pixels[y * width + x] = new Color32(255, 176, 72, alpha);
                }
            }
        }

        private static void PaintSolidPixel(Color32[] pixels)
        {
            pixels[0] = new Color32(255, 255, 255, 255);
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

        private static void PaintCookingHearthOutdoor(Color32[] pixels)
        {
            FillRect(pixels, 34, 30, 5, 2, 24, 6, new Color32(64, 61, 58, 255));
            FillRect(pixels, 34, 30, 7, 8, 20, 14, new Color32(88, 84, 78, 255));
            FillRect(pixels, 34, 30, 10, 10, 14, 7, new Color32(27, 20, 16, 255));
            FillRect(pixels, 34, 30, 13, 15, 8, 8, new Color32(215, 72, 32, 255));
            FillRect(pixels, 34, 30, 15, 17, 4, 7, new Color32(250, 174, 48, 255));
            FillRect(pixels, 34, 30, 9, 22, 16, 4, new Color32(47, 45, 43, 255));
            FillRect(pixels, 34, 30, 25, 15, 5, 10, new Color32(51, 48, 45, 255));
            DrawRect(pixels, 34, 30, 5, 2, 29, 25, new Color32(33, 31, 29, 255));
        }

        private static void PaintAnimalPen(Color32[] pixels, int width, int height, bool longPen)
        {
            Color32 grass = new Color32(42, 82, 38, 255);
            Color32 grassB = new Color32(52, 96, 45, 255);
            Color32 wood = new Color32(120, 75, 36, 255);
            Color32 woodDark = new Color32(62, 38, 24, 255);
            for (int y = 3; y < height - 4; y++)
            {
                for (int x = 5; x < width - 5; x++)
                {
                    pixels[y * width + x] = ((x / 4 + y / 4) % 2 == 0) ? grass : grassB;
                }
            }

            DrawRect(pixels, width, height, 4, 4, width - 5, height - 5, woodDark);
            DrawRect(pixels, width, height, 7, 7, width - 8, height - 8, wood);
            for (int x = 8; x < width - 8; x += 8)
            {
                FillRect(pixels, width, height, x, 4, 3, height - 8, wood);
            }

            for (int y = 8; y < height - 8; y += 8)
            {
                FillRect(pixels, width, height, 4, y, width - 8, 3, wood);
            }

            int gateX = longPen ? width / 2 - 5 : width - 15;
            FillRect(pixels, width, height, gateX, 4, 10, 5, grass);
            FillRect(pixels, width, height, gateX + 1, 4, 8, 2, new Color32(40, 30, 22, 255));
            FillRect(pixels, width, height, width / 3, height / 2, 5, 4, new Color32(239, 231, 205, 255));
            FillRect(pixels, width, height, width / 3 + 1, height / 2 + 3, 3, 3, new Color32(45, 42, 37, 255));
        }

        private static void PaintStorageShed(Color32[] pixels)
        {
            FillRect(pixels, 42, 34, 5, 2, 32, 5, new Color32(36, 25, 20, 150));
            FillRect(pixels, 42, 34, 8, 6, 26, 15, new Color32(86, 55, 33, 255));
            for (int x = 10; x < 32; x += 6) FillRect(pixels, 42, 34, x, 7, 2, 14, new Color32(52, 34, 24, 255));
            FillRect(pixels, 42, 34, 5, 21, 32, 4, new Color32(58, 35, 28, 255));
            FillRect(pixels, 42, 34, 9, 25, 24, 4, new Color32(112, 44, 38, 255));
            FillRect(pixels, 42, 34, 17, 7, 8, 11, new Color32(40, 26, 20, 255));
            FillRect(pixels, 42, 34, 27, 13, 5, 5, new Color32(52, 80, 86, 255));
            DrawRect(pixels, 42, 34, 8, 6, 34, 21, new Color32(34, 22, 16, 255));
        }

        private static void PaintStoneCottage(Color32[] pixels)
        {
            FillRect(pixels, 58, 46, 5, 2, 48, 6, new Color32(32, 25, 22, 150));
            FillRect(pixels, 58, 46, 8, 8, 42, 22, new Color32(86, 84, 78, 255));
            for (int y = 10; y < 29; y += 5)
            {
                for (int x = 10; x < 48; x += 9)
                {
                    FillRect(pixels, 58, 46, x, y, 6, 3, new Color32(118, 112, 104, 255));
                }
            }

            FillRect(pixels, 58, 46, 5, 30, 48, 5, new Color32(55, 40, 35, 255));
            FillRect(pixels, 58, 46, 9, 35, 40, 4, new Color32(103, 42, 38, 255));
            FillRect(pixels, 58, 46, 15, 39, 28, 4, new Color32(149, 58, 44, 255));
            FillRect(pixels, 58, 46, 25, 8, 8, 14, new Color32(44, 31, 24, 255));
            FillRect(pixels, 58, 46, 13, 17, 8, 6, new Color32(64, 86, 92, 255));
            FillRect(pixels, 58, 46, 38, 17, 8, 6, new Color32(64, 86, 92, 255));
            FillRect(pixels, 58, 46, 43, 34, 6, 9, new Color32(50, 45, 42, 255));
            DrawRect(pixels, 58, 46, 8, 8, 50, 30, new Color32(39, 37, 35, 255));
        }

        private static void PaintCabin(Color32[] pixels, int width, int height, float progress)
        {
            Color32 shadow = new Color32(36, 26, 20, 150);
            Color32 stone = new Color32(76, 72, 68, 255);
            Color32 stoneLight = new Color32(118, 112, 104, 255);
            Color32 woodDark = new Color32(62, 38, 25, 255);
            Color32 wood = new Color32(112, 70, 38, 255);
            Color32 woodLight = new Color32(158, 102, 53, 255);
            Color32 roofDark = new Color32(67, 26, 25, 255);
            Color32 roof = new Color32(122, 45, 38, 255);
            Color32 roofLight = new Color32(168, 67, 49, 255);

            FillRect(pixels, width, height, 4, 1, width - 8, 5, shadow);
            FillRect(pixels, width, height, 7, 5, width - 14, 6, stone);
            for (int x = 8; x < width - 8; x += 7)
            {
                FillRect(pixels, width, height, x, 8, 4, 2, stoneLight);
            }
            if (progress < 0.25f) return;

            FillRect(pixels, width, height, 10, 11, width - 20, 17, woodDark);
            for (int y = 13; y < 28; y += 5)
            {
                FillRect(pixels, width, height, 12, y, width - 24, 3, wood);
                FillRect(pixels, width, height, 15, y + 1, width - 30, 1, woodLight);
            }

            FillRect(pixels, width, height, 14, 11, 4, 17, new Color32(48, 31, 22, 255));
            FillRect(pixels, width, height, width - 18, 11, 4, 17, new Color32(48, 31, 22, 255));
            FillRect(pixels, width, height, 23, 11, 8, 12, new Color32(49, 31, 23, 255));
            FillRect(pixels, width, height, 25, 13, 4, 8, new Color32(80, 47, 28, 255));
            if (progress < 0.5f) return;

            FillRect(pixels, width, height, 6, 28, width - 12, 5, roofDark);
            FillRect(pixels, width, height, 9, 33, width - 18, 4, roof);
            FillRect(pixels, width, height, 13, 37, width - 26, 3, roofLight);
            FillRect(pixels, width, height, 18, 40, width - 36, 3, roof);
            FillRect(pixels, width, height, 22, 42, width - 44, 2, roofDark);
            if (progress < 0.75f) return;

            FillRect(pixels, width, height, 13, 17, 7, 6, new Color32(64, 86, 92, 255));
            FillRect(pixels, width, height, width - 20, 17, 7, 6, new Color32(64, 86, 92, 255));
            DrawRect(pixels, width, height, 13, 17, 19, 22, new Color32(34, 24, 18, 255));
            DrawRect(pixels, width, height, width - 20, 17, width - 14, 22, new Color32(34, 24, 18, 255));
            FillRect(pixels, width, height, 15, 19, 3, 2, new Color32(227, 169, 72, 255));
            FillRect(pixels, width, height, width - 18, 19, 3, 2, new Color32(227, 169, 72, 255));
            FillRect(pixels, width, height, width - 16, 34, 6, 9, new Color32(54, 45, 39, 255));
            FillRect(pixels, width, height, width - 15, 41, 4, 2, new Color32(104, 99, 92, 255));
            FillRect(pixels, width, height, 7, 29, width - 14, 2, new Color32(43, 24, 21, 255));
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
