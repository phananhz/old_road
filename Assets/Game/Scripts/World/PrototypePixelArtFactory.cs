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
        public static Sprite Villager(int variant, int frame) => GetOrCreate("villager." + Mathf.Abs(variant % 4) + "." + Mathf.Abs(frame % 4), 16, 22, pixels => PaintVillager(pixels, Mathf.Abs(variant % 4), Mathf.Abs(frame % 4)), new Vector2(0.5f, 0.16f));
        public static Sprite Animal(int variant, int frame) => GetOrCreate("animal." + Mathf.Abs(variant % 4) + "." + Mathf.Abs(frame % 4), 18, 14, pixels => PaintAnimal(pixels, Mathf.Abs(variant % 4), Mathf.Abs(frame % 4)), new Vector2(0.5f, 0.14f));
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
        public static Sprite SmokePuff() => GetOrCreate("smoke.puff", 12, 12, PaintSmokePuff, new Vector2(0.5f, 0.5f));
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
        public static Sprite HerbalistHut() => GetOrCreate("herbalist.hut", 54, 44, PaintHerbalistHut, new Vector2(0.5f, 0.12f));
        public static Sprite LookoutTower() => GetOrCreate("lookout.tower", 36, 56, PaintLookoutTower, new Vector2(0.5f, 0.08f));
        public static Sprite WoodFence() => GetOrCreate("fence.wood", 32, 24, PaintWoodFence, new Vector2(0.5f, 0.15f));
        public static Sprite WoodFenceHorizontal() => WoodFence();
        public static Sprite WoodFenceVertical() => GetOrCreate("fence.wood.vertical", 16, 24, PaintWoodFenceVertical, new Vector2(0.5f, 0.15f));
        public static Sprite WoodFenceCorner() => GetOrCreate("fence.wood.corner", 16, 24, PaintWoodFenceCorner, new Vector2(0.5f, 0.15f));
        public static Sprite WoodGate(bool isOpen) => GetOrCreate("gate.wood." + (isOpen ? "open" : "closed"), 32, 24, pixels => PaintWoodGate(pixels, isOpen), new Vector2(0.5f, 0.15f));
        public static Sprite GateLantern(bool isLit) => GetOrCreate("fence.lantern." + (isLit ? "lit" : "off"), 16, 24, pixels => PaintGateLantern(pixels, isLit), new Vector2(0.5f, 0.15f));
        public static Sprite FarmSignboard() => GetOrCreate("environment.farmsignboard", 20, 24, PaintFarmSignboard, new Vector2(0.5f, 0.15f));
        public static Sprite PathDirtTile() => GetOrCreate("tile.path.dirt", 16, 16, PaintPathDirt, new Vector2(0.5f, 0.5f));
        public static Sprite PathCobblestoneTile() => GetOrCreate("tile.path.cobble", 16, 16, PaintPathCobblestone, new Vector2(0.5f, 0.5f));
        public static Sprite Scarecrow() => GetOrCreate("environment.scarecrow", 24, 32, PaintScarecrow, new Vector2(0.5f, 0.12f));
        public static Sprite SelectionTileHighlight() => GetOrCreate("ui.selection.tile.highlight", 16, 16, PaintSelectionTileHighlight, new Vector2(0.5f, 0.5f));
        public static Sprite TitleKnightSunsetPanorama() => GetOrCreate("title.knight.sunset.panorama", 512, 216, PaintTitleKnightSunsetPanorama, new Vector2(0.5f, 0.5f));
        public static Texture2D TitleKnightSunsetTexture() => TitleKnightSunsetPanorama().texture;

        public static Sprite CabinComplete() => GetOrCreate("cabin.complete.v2", 52, 44, pixels => PaintCabin(pixels, 52, 44, 1f), new Vector2(0.5f, 0.12f));
        public static Sprite CabinConstruction(int stage) => GetOrCreate("cabin.stage.v2." + Mathf.Clamp(stage, 0, 4), 52, 44, pixels => PaintCabin(pixels, 52, 44, Mathf.Clamp01(stage / 4f)), new Vector2(0.5f, 0.12f));
        public static Sprite BuildingComplete(string buildingId)
        {
            if (buildingId != null && buildingId.Contains("perimeter-fence")) return null;
            switch (buildingId)
            {
                case "building.campfire": return Campfire();
                case "building.cooking-hearth": return CookingHearthOutdoor();
                case "building.animal-pen-small": return AnimalPenSmall();
                case "building.animal-pen-long": return AnimalPenLong();
                case "building.storage-shed": return StorageShed();
                case "building.stone-cottage": return StoneCottage();
                case "building.herbalist-hut": return HerbalistHut();
                case "building.lookout-tower": return LookoutTower();
                case "building.farm-barn": return HappyFarmBarn();
                case "building.fence": return WoodFenceHorizontal();
                case "building.fence-vertical": return WoodFenceVertical();
                case "building.gate": return WoodGate(false);
                case "building.scarecrow": return Scarecrow();
                case "building.farm-signboard": return FarmSignboard();
                case "building.path-dirt": return PathDirtTile();
                case "building.path-cobblestone": return PathCobblestoneTile();
                default: return CabinComplete();
            }
        }

        public static Sprite BuildingConstruction(string buildingId, int stage)
        {
            if (buildingId != null && buildingId.Contains("perimeter-fence")) return null;
            return CabinConstruction(stage);
        }
        public static Sprite PlacementPreview() => GetOrCreate("placement.preview", 32, 32, PaintPlacementPreview, new Vector2(0.5f, 0.5f));
        public static Sprite ValenOutskirtsGround() => GetOrCreate("valen.outskirts.ground.seeded.120x72", 1920, 1152, PaintValenOutskirtsGround, new Vector2(0.5f, 0.5f));
        public static Sprite WorldChunkGround(int chunkX, int chunkY, int seed) => GetOrCreate("world.chunk." + seed + "." + chunkX + "." + chunkY, 256, 256, pixels => PaintWorldChunkGround(pixels, chunkX, chunkY, seed), new Vector2(0.5f, 0.5f));

        public static Sprite TilledSoil(bool watered) => GetOrCreate("soil.tilled." + (watered ? "wet" : "dry"), 32, 32, pixels => PaintTilledSoil(pixels, watered), new Vector2(0.5f, 0.5f));
        public static Sprite Crop(string cropId, int stage) => GetOrCreate("crop." + cropId + "." + Mathf.Clamp(stage, 0, 3), 24, 28, pixels => PaintCrop(pixels, cropId, Mathf.Clamp(stage, 0, 3)), new Vector2(0.5f, 0.15f));
        public static Sprite HeartEmote() => GetOrCreate("emote.heart", 16, 16, PaintHeartEmote, new Vector2(0.5f, 0.5f));
        public static Sprite SilverCoinIcon() => GetOrCreate("currency.silver.coin", 16, 16, PaintSilverCoin, new Vector2(0.5f, 0.5f));
        public static Sprite NightMonsterSprite(int frame) => GetOrCreate("enemy.nightmonster." + Mathf.Abs(frame % 4), 22, 22, pixels => PaintNightMonster(pixels, Mathf.Abs(frame % 4)), new Vector2(0.5f, 0.15f));
        public static Sprite BellTowerRuins() => GetOrCreate("environment.belltower.ruins", 64, 64, PaintBellTowerRuins, new Vector2(0.5f, 0.10f));
        public static Sprite PuzzlePedestal(int symbol, bool active) => GetOrCreate("environment.pedestal." + symbol + "." + (active ? "1" : "0"), 22, 24, pixels => PaintPuzzlePedestal(pixels, symbol, active), new Vector2(0.5f, 0.15f));
        public static Sprite MerchantCart() => GetOrCreate("environment.merchant.cart", 52, 40, PaintMerchantCart, new Vector2(0.5f, 0.12f));
        public static Sprite HappyFarmBarn() => GetOrCreate("environment.happyfarm.barn", 68, 52, PaintHappyFarmBarn, new Vector2(0.5f, 0.12f));
        public static Sprite DairyCow() => GetOrCreate("animal.dairycow", 28, 20, PaintDairyCow, new Vector2(0.5f, 0.20f));
        public static Sprite StrawNest(bool hasEgg) => GetOrCreate("environment.straw.nest." + (hasEgg ? "1" : "0"), 22, 16, pixels => PaintStrawNest(pixels, hasEgg), new Vector2(0.5f, 0.30f));
        public static Sprite FarmDog() => GetOrCreate("animal.farmdog", 18, 16, PaintFarmDog, new Vector2(0.5f, 0.20f));
        public static Sprite FarmShopSign() => GetOrCreate("environment.farmshop.sign", 18, 22, PaintFarmShopSign, new Vector2(0.5f, 0.10f));

        public static Sprite ItemWood() => GetOrCreate("item.wood", 16, 16, PaintItemWood, new Vector2(0.5f, 0.5f));
        public static Sprite ItemStone() => GetOrCreate("item.stone", 16, 16, PaintItemStone, new Vector2(0.5f, 0.5f));
        public static Sprite ItemCabinPlank() => GetOrCreate("item.cabin-plank", 16, 16, PaintItemCabinPlank, new Vector2(0.5f, 0.5f));
        public static Sprite ItemWildBerries() => GetOrCreate("item.wild-berries", 16, 16, PaintItemWildBerries, new Vector2(0.5f, 0.5f));
        public static Sprite ItemMedicinalHerb() => GetOrCreate("item.medicinal-herb", 16, 16, PaintItemMedicinalHerb, new Vector2(0.5f, 0.5f));
        public static Sprite ItemMushroom() => GetOrCreate("item.mushroom", 16, 16, PaintItemMushroom, new Vector2(0.5f, 0.5f));
        public static Sprite ItemIronOreItem() => GetOrCreate("item.iron-ore", 16, 16, PaintItemIronOre, new Vector2(0.5f, 0.5f));
        public static Sprite ItemOldCoin() => GetOrCreate("item.old-coin", 16, 16, PaintItemOldCoin, new Vector2(0.5f, 0.5f));
        public static Sprite ItemTorch() => GetOrCreate("item.torch", 16, 16, PaintItemTorch, new Vector2(0.5f, 0.5f));
        public static Sprite ItemToolAxe() => GetOrCreate("item.tool-axe", 16, 16, PaintItemAxe, new Vector2(0.5f, 0.5f));
        public static Sprite ItemToolPickaxe() => GetOrCreate("item.tool-pickaxe", 16, 16, PaintItemPickaxe, new Vector2(0.5f, 0.5f));
        public static Sprite ItemRoadwardenPage() => GetOrCreate("item.roadwarden-page", 16, 16, PaintItemRoadwardenPage, new Vector2(0.5f, 0.5f));
        public static Sprite ItemBellFragment() => GetOrCreate("item.bell-fragment", 16, 16, PaintItemBellFragment, new Vector2(0.5f, 0.5f));
        public static Sprite ItemCookedMeal() => GetOrCreate("item.cooked-meal", 16, 16, PaintItemCookedMeal, new Vector2(0.5f, 0.5f));
        public static Sprite ItemEgg() => GetOrCreate("item.egg", 16, 16, PaintItemEgg, new Vector2(0.5f, 0.5f));
        public static Sprite ItemWool() => GetOrCreate("item.wool", 16, 16, PaintItemWool, new Vector2(0.5f, 0.5f));
        public static Sprite ItemMilk() => GetOrCreate("item.milk", 16, 16, PaintItemMilk, new Vector2(0.5f, 0.5f));
        public static Sprite ItemSilverCoin() => GetOrCreate("item.silver-coin", 16, 16, PaintSilverCoin, new Vector2(0.5f, 0.5f));
        public static Sprite ItemWateringCan() => GetOrCreate("item.watering-can", 16, 16, PaintItemWateringCan, new Vector2(0.5f, 0.5f));
        public static Sprite ItemSeedWheat() => GetOrCreate("item.seed-wheat", 16, 16, pixels => PaintItemSeed(pixels, new Color32(220, 195, 90, 255)), new Vector2(0.5f, 0.5f));
        public static Sprite ItemSeedCorn() => GetOrCreate("item.seed-corn", 16, 16, pixels => PaintItemSeed(pixels, new Color32(235, 210, 60, 255)), new Vector2(0.5f, 0.5f));
        public static Sprite ItemSeedCarrot() => GetOrCreate("item.seed-carrot", 16, 16, pixels => PaintItemSeed(pixels, new Color32(230, 120, 45, 255)), new Vector2(0.5f, 0.5f));
        public static Sprite ItemSeedPineapple() => GetOrCreate("item.seed-pineapple", 16, 16, pixels => PaintItemSeed(pixels, new Color32(245, 190, 45, 255)), new Vector2(0.5f, 0.5f));
        public static Sprite ItemSeedTomato() => GetOrCreate("item.seed-tomato", 16, 16, pixels => PaintItemSeed(pixels, new Color32(230, 65, 45, 255)), new Vector2(0.5f, 0.5f));
        public static Sprite ItemWheat() => GetOrCreate("item.wheat", 16, 16, PaintItemWheat, new Vector2(0.5f, 0.5f));
        public static Sprite ItemCorn() => GetOrCreate("item.corn", 16, 16, PaintItemCorn, new Vector2(0.5f, 0.5f));
        public static Sprite ItemCarrot() => GetOrCreate("item.carrot", 16, 16, PaintItemCarrot, new Vector2(0.5f, 0.5f));
        public static Sprite ItemPineapple() => GetOrCreate("item.pineapple", 16, 16, PaintItemPineapple, new Vector2(0.5f, 0.5f));
        public static Sprite ItemTomato() => GetOrCreate("item.tomato", 16, 16, PaintItemTomato, new Vector2(0.5f, 0.5f));
        public static Sprite ItemFenceWood() => GetOrCreate("item.fence-wood", 16, 16, PaintItemFenceWood, new Vector2(0.5f, 0.5f));
        public static Sprite ItemGateWood() => GetOrCreate("item.gate-wood", 16, 16, PaintItemGateWood, new Vector2(0.5f, 0.5f));

        public static Sprite SlashArcSprite => GetOrCreate("vfx.slash.arc", 24, 24, PaintSlashArc, new Vector2(0.5f, 0.5f));
        public static Sprite WolfSprite(int frame) => GetOrCreate("enemy.wolf." + Mathf.Abs(frame % 4), 22, 16, pixels => PaintWolf(pixels, Mathf.Abs(frame % 4)), new Vector2(0.5f, 0.15f));
        public static Sprite BanditSprite(int frame) => GetOrCreate("enemy.bandit." + Mathf.Abs(frame % 4), 16, 22, pixels => PaintBandit(pixels, Mathf.Abs(frame % 4)), new Vector2(0.5f, 0.16f));

        public static Sprite ItemIcon(string itemId)
        {
            switch (itemId)
            {
                case "item.wood": return ItemWood();
                case "item.stone": return ItemStone();
                case "item.cabin-plank": return ItemCabinPlank();
                case "item.wild-berries": return ItemWildBerries();
                case "item.medicinal-herb": return ItemMedicinalHerb();
                case "item.mushroom": return ItemMushroom();
                case "item.iron-ore": return ItemIronOreItem();
                case "item.old-coin": return ItemOldCoin();
                case "item.torch": return ItemTorch();
                case "item.tool-axe": return ItemToolAxe();
                case "item.tool-pickaxe": return ItemToolPickaxe();
                case "item.roadwarden-page": return ItemRoadwardenPage();
                case "item.bell-fragment": return ItemBellFragment();
                case "item.cooked-meal": return ItemCookedMeal();
                case "item.egg": return ItemEgg();
                case "item.wool": return ItemWool();
                case "item.milk": return ItemMilk();
                case "item.silver-coin": return ItemSilverCoin();
                case "item.watering-can": return ItemWateringCan();
                case "item.seed-wheat": return ItemSeedWheat();
                case "item.seed-corn": return ItemSeedCorn();
                case "item.seed-carrot": return ItemSeedCarrot();
                case "item.seed-pineapple": return ItemSeedPineapple();
                case "item.seed-tomato": return ItemSeedTomato();
                case "item.wheat": return ItemWheat();
                case "item.corn": return ItemCorn();
                case "item.carrot": return ItemCarrot();
                case "item.pineapple": return ItemPineapple();
                case "item.tomato": return ItemTomato();
                case "item.fence-wood": return ItemFenceWood();
                case "item.gate-wood": return ItemGateWood();
                default: return ItemStone();
            }
        }

        public static Texture2D ItemIconTexture(string itemId) => ItemIcon(itemId)?.texture;

        public static IEnumerable<ExportableSprite> AllExportableSprites
        {
            get
            {
                // Characters
                for (int i = 0; i < 4; i++)
                {
                    int frame = i;
                    yield return new ExportableSprite("Characters/Player_Walk_" + frame + ".png", 16, 24, new Vector2(0.5f, 0.18f), p => PaintPlayer(p, frame));
                }

                for (int f = 0; f < 4; f++)
                {
                    int frame = f;
                    yield return new ExportableSprite("Characters/Enemy_Wolf_" + frame + ".png", 22, 16, new Vector2(0.5f, 0.15f), p => PaintWolf(p, frame));
                    yield return new ExportableSprite("Characters/Enemy_Bandit_" + frame + ".png", 16, 22, new Vector2(0.5f, 0.16f), p => PaintBandit(p, frame));
                }

                for (int v = 0; v < 4; v++)
                {
                    for (int f = 0; f < 4; f++)
                    {
                        int variant = v;
                        int frame = f;
                        yield return new ExportableSprite("Characters/Villager_V" + variant + "_Walk_" + frame + ".png", 16, 22, new Vector2(0.5f, 0.16f), p => PaintVillager(p, variant, frame));
                    }
                }

                for (int v = 0; v < 4; v++)
                {
                    for (int f = 0; f < 4; f++)
                    {
                        int variant = v;
                        int frame = f;
                        yield return new ExportableSprite("Characters/Animal_V" + variant + "_Walk_" + frame + ".png", 18, 14, new Vector2(0.5f, 0.14f), p => PaintAnimal(p, variant, frame));
                    }
                }

                // Environment
                yield return new ExportableSprite("Environment/Tree.png", 24, 32, new Vector2(0.5f, 0.12f), PaintTree);
                yield return new ExportableSprite("Environment/Rock.png", 18, 14, new Vector2(0.5f, 0.18f), PaintRock);
                yield return new ExportableSprite("Environment/BerryBush.png", 22, 20, new Vector2(0.5f, 0.12f), PaintBerryBush);
                yield return new ExportableSprite("Environment/HerbPatch.png", 20, 16, new Vector2(0.5f, 0.12f), PaintHerbPatch);
                yield return new ExportableSprite("Environment/MushroomCluster.png", 20, 16, new Vector2(0.5f, 0.12f), PaintMushroomCluster);
                yield return new ExportableSprite("Environment/IronOre.png", 20, 16, new Vector2(0.5f, 0.18f), PaintIronOre);
                yield return new ExportableSprite("Environment/WaterRipple.png", 24, 8, new Vector2(0.5f, 0.5f), PaintWaterRipple);
                yield return new ExportableSprite("Environment/ChestClosed.png", 22, 18, new Vector2(0.5f, 0.12f), PaintChestClosed);
                yield return new ExportableSprite("Environment/ChestOpen.png", 22, 18, new Vector2(0.5f, 0.12f), PaintChestOpen);
                yield return new ExportableSprite("Environment/Waystone.png", 18, 28, new Vector2(0.5f, 0.12f), PaintWaystone);
                yield return new ExportableSprite("Environment/RoadSign.png", 22, 22, new Vector2(0.5f, 0.12f), PaintRoadSign);
                yield return new ExportableSprite("Environment/RuinedArch.png", 46, 36, new Vector2(0.5f, 0.08f), PaintRuinedArch);
                yield return new ExportableSprite("Environment/Footbridge.png", 48, 18, new Vector2(0.5f, 0.5f), PaintFootbridge);

                // Buildings
                yield return new ExportableSprite("Buildings/Campfire.png", 28, 22, new Vector2(0.5f, 0.12f), PaintCampfire);
                yield return new ExportableSprite("Buildings/CookingHearth.png", 34, 30, new Vector2(0.5f, 0.12f), PaintCookingHearthOutdoor);
                yield return new ExportableSprite("Buildings/AnimalPenSmall.png", 46, 34, new Vector2(0.5f, 0.12f), p => PaintAnimalPen(p, 46, 34, false));
                yield return new ExportableSprite("Buildings/AnimalPenLong.png", 64, 34, new Vector2(0.5f, 0.12f), p => PaintAnimalPen(p, 64, 34, true));
                yield return new ExportableSprite("Buildings/StorageShed.png", 42, 34, new Vector2(0.5f, 0.12f), PaintStorageShed);
                yield return new ExportableSprite("Buildings/StoneCottage.png", 58, 46, new Vector2(0.5f, 0.12f), PaintStoneCottage);
                yield return new ExportableSprite("Buildings/Cabin_Complete.png", 52, 44, new Vector2(0.5f, 0.12f), p => PaintCabin(p, 52, 44, 1f));
                for (int s = 0; s <= 4; s++)
                {
                    int stage = s;
                    yield return new ExportableSprite("Buildings/Cabin_Stage_" + stage + ".png", 52, 44, new Vector2(0.5f, 0.12f), p => PaintCabin(p, 52, 44, Mathf.Clamp01(stage / 4f)));
                }

                // Cabin Interior
                yield return new ExportableSprite("Buildings/CabinInteriorFloor.png", 176, 112, new Vector2(0.5f, 0.5f), PaintCabinInteriorFloor);
                yield return new ExportableSprite("Buildings/CabinInteriorWall.png", 176, 48, new Vector2(0.5f, 0.5f), PaintCabinInteriorWall);
                yield return new ExportableSprite("Buildings/CabinPartitionWall.png", 18, 92, new Vector2(0.5f, 0.5f), PaintCabinPartitionWall);
                yield return new ExportableSprite("Buildings/CabinBed.png", 34, 24, new Vector2(0.5f, 0.15f), PaintCabinBed);
                yield return new ExportableSprite("Buildings/CabinHearth.png", 28, 28, new Vector2(0.5f, 0.12f), PaintCabinHearth);
                yield return new ExportableSprite("Buildings/CabinTable.png", 30, 22, new Vector2(0.5f, 0.15f), PaintCabinTable);
                yield return new ExportableSprite("Buildings/CabinBench.png", 28, 14, new Vector2(0.5f, 0.15f), PaintCabinBench);
                yield return new ExportableSprite("Buildings/CabinKitchenCounter.png", 38, 22, new Vector2(0.5f, 0.15f), PaintCabinKitchenCounter);
                yield return new ExportableSprite("Buildings/CabinDoorMarker.png", 26, 12, new Vector2(0.5f, 0.5f), PaintCabinDoorMarker);

                // Items
                yield return new ExportableSprite("Items/Wood.png", 16, 16, new Vector2(0.5f, 0.5f), PaintItemWood);
                yield return new ExportableSprite("Items/Stone.png", 16, 16, new Vector2(0.5f, 0.5f), PaintItemStone);
                yield return new ExportableSprite("Items/CabinPlank.png", 16, 16, new Vector2(0.5f, 0.5f), PaintItemCabinPlank);
                yield return new ExportableSprite("Items/WildBerries.png", 16, 16, new Vector2(0.5f, 0.5f), PaintItemWildBerries);
                yield return new ExportableSprite("Items/MedicinalHerb.png", 16, 16, new Vector2(0.5f, 0.5f), PaintItemMedicinalHerb);
                yield return new ExportableSprite("Items/Mushroom.png", 16, 16, new Vector2(0.5f, 0.5f), PaintItemMushroom);
                yield return new ExportableSprite("Items/IronOre.png", 16, 16, new Vector2(0.5f, 0.5f), PaintItemIronOre);
                yield return new ExportableSprite("Items/OldCoin.png", 16, 16, new Vector2(0.5f, 0.5f), PaintItemOldCoin);
                yield return new ExportableSprite("Items/Torch.png", 16, 16, new Vector2(0.5f, 0.5f), PaintItemTorch);
                yield return new ExportableSprite("Items/ToolAxe.png", 16, 16, new Vector2(0.5f, 0.5f), PaintItemAxe);
                yield return new ExportableSprite("Items/ToolPickaxe.png", 16, 16, new Vector2(0.5f, 0.5f), PaintItemPickaxe);
                yield return new ExportableSprite("Items/RoadwardenPage.png", 16, 16, new Vector2(0.5f, 0.5f), PaintItemRoadwardenPage);
                yield return new ExportableSprite("Items/BellFragment.png", 16, 16, new Vector2(0.5f, 0.5f), PaintItemBellFragment);
                yield return new ExportableSprite("Items/CookedMeal.png", 16, 16, new Vector2(0.5f, 0.5f), PaintItemCookedMeal);
                yield return new ExportableSprite("Items/Egg.png", 16, 16, new Vector2(0.5f, 0.5f), PaintItemEgg);
                yield return new ExportableSprite("Items/Wool.png", 16, 16, new Vector2(0.5f, 0.5f), PaintItemWool);
                yield return new ExportableSprite("Items/Milk.png", 16, 16, new Vector2(0.5f, 0.5f), PaintItemMilk);

                // VFX & UI
                yield return new ExportableSprite("VFX/TorchGlow.png", 96, 96, new Vector2(0.5f, 0.5f), PaintTorchGlow);
                yield return new ExportableSprite("VFX/SmokePuff.png", 12, 12, new Vector2(0.5f, 0.5f), PaintSmokePuff);
                yield return new ExportableSprite("VFX/SolidPixel.png", 1, 1, new Vector2(0.5f, 0.5f), PaintSolidPixel);
                yield return new ExportableSprite("UI/PlacementPreview.png", 32, 32, new Vector2(0.5f, 0.5f), PaintPlacementPreview);
            }
        }

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
            // Full Steel Plate Knight with Visor Helm, Crimson Mantle, Gauntlets, Greaves, Sword & Shield
            int step = frame == 1 ? 1 : frame == 3 ? -1 : 0;
            int capeSwing = frame == 1 || frame == 2 ? 1 : 0;

            Color32 steelLight = new Color32(235, 242, 250, 255);
            Color32 steelMid = new Color32(165, 178, 196, 255);
            Color32 steelShadow = new Color32(95, 108, 128, 255);
            Color32 steelDark = new Color32(45, 52, 68, 255);
            Color32 visorDark = new Color32(16, 18, 26, 255);

            Color32 redLight = new Color32(220, 52, 52, 255);
            Color32 redMid = new Color32(168, 30, 34, 255);
            Color32 redDark = new Color32(105, 18, 22, 255);

            Color32 leather = new Color32(68, 40, 26, 255);
            Color32 gold = new Color32(238, 188, 62, 255);
            Color32 goldDark = new Color32(182, 128, 32, 255);

            // 1. Cape Backing (Draped behind knight)
            FillRect(pixels, 16, 24, 2, 4 + capeSwing, 3, 11, redDark);
            FillRect(pixels, 16, 24, 11, 4 - capeSwing, 3, 11, redDark);
            FillRect(pixels, 16, 24, 3, 5 + capeSwing, 2, 9, redMid);
            FillRect(pixels, 16, 24, 11, 5 - capeSwing, 2, 9, redMid);

            // 2. Greaves & Sabatons (Armored Legs & Boots)
            // Left Leg
            FillRect(pixels, 16, 24, 4, 1, 3, 5 + Mathf.Max(0, step), steelShadow);
            FillRect(pixels, 16, 24, 5, 2, 2, 3 + Mathf.Max(0, step), steelMid);
            FillRect(pixels, 16, 24, 4, 0, 4, 1, steelDark); // Sole
            SetPixel(pixels, 16, 24, 5, 5 + Mathf.Max(0, step), steelLight); // Knee poleyn shine

            // Right Leg
            FillRect(pixels, 16, 24, 9, 1, 3, 5 + Mathf.Max(0, -step), steelShadow);
            FillRect(pixels, 16, 24, 10, 2, 2, 3 + Mathf.Max(0, -step), steelMid);
            FillRect(pixels, 16, 24, 9, 0, 4, 1, steelDark); // Sole
            SetPixel(pixels, 16, 24, 10, 5 + Mathf.Max(0, -step), steelLight); // Knee poleyn shine

            // 3. Waist, Leather Belt & Faulds
            FillRect(pixels, 16, 24, 4, 6, 8, 2, leather);
            FillRect(pixels, 16, 24, 7, 6, 2, 2, gold); // Belt buckle
            SetPixel(pixels, 16, 24, 7, 6, goldDark);

            // 4. Steel Cuirass (Breastplate & Pauldrons)
            FillRect(pixels, 16, 24, 4, 8, 8, 8, steelShadow);
            FillRect(pixels, 16, 24, 5, 8, 6, 7, steelMid);
            FillRect(pixels, 16, 24, 7, 9, 2, 6, steelLight); // Center ridge specular highlight
            FillRect(pixels, 16, 24, 4, 8, 1, 7, steelDark);
            FillRect(pixels, 16, 24, 11, 8, 1, 7, steelDark);

            // Shoulder Pauldrons
            FillRect(pixels, 16, 24, 2, 13, 3, 4, steelMid);
            FillRect(pixels, 16, 24, 2, 13, 1, 4, steelDark);
            SetPixel(pixels, 16, 24, 3, 16, steelLight);

            FillRect(pixels, 16, 24, 11, 13, 3, 4, steelMid);
            FillRect(pixels, 16, 24, 13, 13, 1, 4, steelDark);
            SetPixel(pixels, 16, 24, 12, 16, steelLight);

            // Gauntlets (Armored Hands)
            FillRect(pixels, 16, 24, 2, 9, 2, 4, steelShadow);
            SetPixel(pixels, 16, 24, 2, 9, steelDark);
            FillRect(pixels, 16, 24, 12, 9, 2, 4, steelShadow);
            SetPixel(pixels, 16, 24, 13, 9, steelDark);

            // 5. Crimson Mantle / Neck Scarf
            FillRect(pixels, 16, 24, 3, 15, 10, 3, redMid);
            FillRect(pixels, 16, 24, 5, 16, 6, 2, redLight);
            FillRect(pixels, 16, 24, 4, 15, 8, 1, redDark);
            SetPixel(pixels, 16, 24, 3, 15, redDark);
            SetPixel(pixels, 16, 24, 12, 15, redDark);

            // 6. Knight Greathelm (Helmet with Visor)
            FillRect(pixels, 16, 24, 4, 18, 8, 5, steelMid);
            FillRect(pixels, 16, 24, 5, 23, 6, 1, steelMid);
            FillRect(pixels, 16, 24, 4, 18, 1, 5, steelDark);
            FillRect(pixels, 16, 24, 11, 18, 1, 5, steelDark);

            // Specular dome glint
            FillRect(pixels, 16, 24, 6, 21, 3, 2, steelLight);

            // Visor Brow & Slit
            FillRect(pixels, 16, 24, 4, 20, 8, 1, steelDark); // Brow rim
            FillRect(pixels, 16, 24, 5, 19, 6, 1, visorDark);  // Eye slit
            SetPixel(pixels, 16, 24, 7, 20, gold);            // Golden brow crest
            SetPixel(pixels, 16, 24, 8, 20, gold);

            // Lower visor breathing vents
            SetPixel(pixels, 16, 24, 6, 18, steelDark);
            SetPixel(pixels, 16, 24, 9, 18, steelDark);

            // 7. Weapon details (Sword hilt & Lion Shield edge)
            // Sword crossguard over right shoulder
            FillRect(pixels, 16, 24, 12, 19, 3, 1, gold);
            FillRect(pixels, 16, 24, 13, 20, 1, 3, leather); // Grip
            SetPixel(pixels, 16, 24, 13, 23, gold);           // Pommel

            // Shield rim on left side
            FillRect(pixels, 16, 24, 1, 8, 2, 7, steelDark);
            FillRect(pixels, 16, 24, 1, 9, 1, 5, redMid);
            SetPixel(pixels, 16, 24, 1, 11, gold); // Lion crest hint
        }

        private static void PaintVillager(Color32[] pixels, int variant, int frame)
        {
            int step = frame == 1 ? 1 : frame == 3 ? -1 : 0;
            Color32 shirt = variant == 0 ? new Color32(83, 119, 74, 255)
                : variant == 1 ? new Color32(127, 78, 50, 255)
                : variant == 2 ? new Color32(70, 94, 124, 255)
                : new Color32(126, 96, 56, 255);
            Color32 shirtDark = variant == 0 ? new Color32(45, 79, 46, 255)
                : variant == 1 ? new Color32(88, 49, 35, 255)
                : variant == 2 ? new Color32(43, 57, 84, 255)
                : new Color32(84, 61, 38, 255);

            FillRect(pixels, 16, 22, 5, 0, 3, 5 + Mathf.Max(0, step), new Color32(54, 39, 28, 255));
            FillRect(pixels, 16, 22, 8, 0, 3, 5 + Mathf.Max(0, -step), new Color32(54, 39, 28, 255));
            FillRect(pixels, 16, 22, 4, 5, 8, 9, shirt);
            FillRect(pixels, 16, 22, 5, 6, 6, 6, shirtDark);
            FillRect(pixels, 16, 22, 5, 14, 6, 5, new Color32(171, 124, 82, 255));
            FillRect(pixels, 16, 22, 4, 16, 8, 2, new Color32(84, 57, 36, 255));
            FillRect(pixels, 16, 22, 6, 18, 4, 2, new Color32(206, 158, 105, 255));
            SetPixel(pixels, 16, 22, 6, 17, new Color32(46, 35, 29, 255));
            SetPixel(pixels, 16, 22, 9, 17, new Color32(46, 35, 29, 255));
            FillRect(pixels, 16, 22, 5, 20, 6, 1, new Color32(59, 39, 28, 255));
        }

        private static void PaintAnimal(Color32[] pixels, int variant, int frame)
        {
            int step = frame == 1 ? 1 : frame == 3 ? -1 : 0;
            Color32 body = variant == 0 ? new Color32(238, 229, 197, 255)
                : variant == 1 ? new Color32(116, 78, 46, 255)
                : variant == 2 ? new Color32(72, 70, 66, 255)
                : new Color32(204, 156, 86, 255);
            Color32 bodyDark = variant == 0 ? new Color32(172, 160, 132, 255)
                : variant == 1 ? new Color32(74, 48, 31, 255)
                : variant == 2 ? new Color32(42, 42, 42, 255)
                : new Color32(139, 96, 48, 255);
            Color32 accent = variant == 0 ? new Color32(210, 58, 38, 255)
                : variant == 1 ? new Color32(232, 221, 184, 255)
                : variant == 2 ? new Color32(210, 210, 205, 255)
                : new Color32(68, 42, 24, 255);

            FillRect(pixels, 18, 14, 4, 4, 10, 5, body);
            FillRect(pixels, 18, 14, 11, 7, 4, 3, bodyDark);
            FillRect(pixels, 18, 14, 2, 6, 4, 4, body);
            FillRect(pixels, 18, 14, 3, 9, 3, 2, bodyDark);
            FillRect(pixels, 18, 14, 6, 2 + Mathf.Max(0, step), 2, 3, bodyDark);
            FillRect(pixels, 18, 14, 11, 2 + Mathf.Max(0, -step), 2, 3, bodyDark);
            SetPixel(pixels, 18, 14, 4, 8, new Color32(30, 24, 20, 255));
            SetPixel(pixels, 18, 14, 14, 10, accent);
            if (variant == 0)
            {
                FillRect(pixels, 18, 14, 1, 9, 3, 2, accent);
                FillRect(pixels, 18, 14, 2, 11, 2, 1, new Color32(226, 142, 36, 255));
            }
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

        private static void PaintSmokePuff(Color32[] pixels)
        {
            const int width = 12;
            const int height = 12;
            Color32 smoke = new Color32(172, 178, 179, 180);
            Color32 smokeSoft = new Color32(142, 149, 151, 120);
            Color32 smokeEdge = new Color32(98, 106, 110, 70);

            FillRect(pixels, width, height, 4, 2, 4, 8, smoke);
            FillRect(pixels, width, height, 2, 4, 8, 4, smoke);
            FillRect(pixels, width, height, 3, 3, 6, 6, smokeSoft);
            SetPixel(pixels, width, height, 5, 9, smokeSoft);
            SetPixel(pixels, width, height, 6, 9, smokeSoft);
            SetPixel(pixels, width, height, 2, 5, smokeEdge);
            SetPixel(pixels, width, height, 9, 6, smokeEdge);
            SetPixel(pixels, width, height, 4, 1, smokeEdge);
            SetPixel(pixels, width, height, 7, 10, smokeEdge);
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
            Color32 woodDark = new Color32(130, 80, 40, 255);
            Color32 woodMid = new Color32(185, 125, 65, 255);
            Color32 woodLight = new Color32(225, 165, 95, 255);
            Color32 iron = new Color32(45, 40, 35, 255);

            // Clear to transparent so interior is completely open and see-through
            for (int i = 0; i < pixels.Length; i++) pixels[i] = new Color32(0, 0, 0, 0);

            // --- TOP HORIZONTAL FENCE ---
            // Rails
            FillRect(pixels, width, height, 2, height - 6, width - 4, 3, woodDark);
            FillRect(pixels, width, height, 2, height - 5, width - 4, 2, woodMid);
            FillRect(pixels, width, height, 2, height - 12, width - 4, 3, woodDark);
            FillRect(pixels, width, height, 2, height - 11, width - 4, 2, woodMid);

            // --- BOTTOM HORIZONTAL FENCE ---
            int gateWidth = 14;
            int gateX = longPen ? (width - gateWidth) / 2 : width - gateWidth - 6;

            // Left segment of bottom fence
            if (gateX > 4)
            {
                FillRect(pixels, width, height, 2, 8, gateX - 2, 3, woodDark);
                FillRect(pixels, width, height, 2, 9, gateX - 2, 2, woodMid);
                FillRect(pixels, width, height, 2, 2, gateX - 2, 3, woodDark);
                FillRect(pixels, width, height, 2, 3, gateX - 2, 2, woodMid);
            }

            // Right segment of bottom fence
            int rightStart = gateX + gateWidth;
            if (width - rightStart > 4)
            {
                FillRect(pixels, width, height, rightStart, 8, width - rightStart - 2, 3, woodDark);
                FillRect(pixels, width, height, rightStart, 9, width - rightStart - 2, 2, woodMid);
                FillRect(pixels, width, height, rightStart, 2, width - rightStart - 2, 3, woodDark);
                FillRect(pixels, width, height, rightStart, 3, width - rightStart - 2, 2, woodMid);
            }

            // --- LEFT VERTICAL FENCE ---
            FillRect(pixels, width, height, 2, 2, 3, height - 4, woodDark);
            FillRect(pixels, width, height, 3, 3, 2, height - 6, woodMid);

            // --- RIGHT VERTICAL FENCE ---
            FillRect(pixels, width, height, width - 5, 2, 3, height - 4, woodDark);
            FillRect(pixels, width, height, width - 4, 3, 2, height - 6, woodMid);

            // --- VERTICAL POSTS with carved caps ---
            // Top corners & Intermediate top posts
            int postSpacing = 16;
            for (int x = 2; x <= width - 6; x += postSpacing)
            {
                FillRect(pixels, width, height, x, height - 16, 4, 16, woodDark);
                FillRect(pixels, width, height, x + 1, height - 15, 2, 14, woodMid);
                FillRect(pixels, width, height, x + 1, height - 2, 2, 2, woodLight); // cap
                SetPixel(pixels, width, height, x + 2, height - 5, iron);
                SetPixel(pixels, width, height, x + 2, height - 11, iron);
            }
            // Always post at top right
            FillRect(pixels, width, height, width - 6, height - 16, 4, 16, woodDark);
            FillRect(pixels, width, height, width - 5, height - 15, 2, 14, woodMid);
            FillRect(pixels, width, height, width - 5, height - 2, 2, 2, woodLight);

            // Bottom posts (corner & gate posts)
            FillRect(pixels, width, height, 2, 0, 4, 14, woodDark);
            FillRect(pixels, width, height, 3, 1, 2, 12, woodMid);
            FillRect(pixels, width, height, 3, 13, 2, 2, woodLight);

            FillRect(pixels, width, height, width - 6, 0, 4, 14, woodDark);
            FillRect(pixels, width, height, width - 5, 1, 2, 12, woodMid);
            FillRect(pixels, width, height, width - 5, 13, 2, 2, woodLight);

            // Gate Left & Right Posts
            FillRect(pixels, width, height, gateX, 0, 4, 14, woodDark);
            FillRect(pixels, width, height, gateX + 1, 1, 2, 12, woodMid);
            FillRect(pixels, width, height, gateX + 1, 13, 2, 2, woodLight);

            FillRect(pixels, width, height, gateX + gateWidth - 4, 0, 4, 14, woodDark);
            FillRect(pixels, width, height, gateX + gateWidth - 3, 1, 2, 12, woodMid);
            FillRect(pixels, width, height, gateX + gateWidth - 3, 13, 2, 2, woodLight);
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

        private static void PaintWorldChunkGround(Color32[] pixels, int chunkX, int chunkY, int seed)
        {
            const int width = 256;
            const int height = 256;
            Color32 grassA = new Color32(42, 82, 38, 255);
            Color32 grassB = new Color32(50, 96, 45, 255);
            Color32 forest = new Color32(24, 58, 38, 255);
            Color32 hill = new Color32(58, 72, 48, 255);
            Color32 road = new Color32(138, 91, 42, 255);
            Color32 roadEdge = new Color32(104, 70, 34, 255);
            Color32 river = new Color32(35, 94, 116, 255);
            Color32 riverLight = new Color32(65, 139, 162, 255);

            int biomeRoll = Mathf.FloorToInt(ChunkHash01(chunkX, chunkY, seed, 10) * 4f);
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int worldPixelX = chunkX * width + x;
                    int worldPixelY = chunkY * height + y;
                    Color32 color = ((worldPixelX / 8 + worldPixelY / 8) % 2 == 0) ? grassA : grassB;
                    if (biomeRoll == 1 && ((worldPixelX / 11 + worldPixelY / 7) % 3 == 0)) color = forest;
                    if (biomeRoll == 2 && ((worldPixelX / 13 + worldPixelY / 9) % 4 == 0)) color = hill;

                    float roadCenter = 128f + Mathf.Sin((chunkX * 16f + x / 16f) * 0.22f) * 28f + Mathf.Sin((chunkX * 16f + x / 16f) * 0.055f) * 44f;
                    float roadDistance = Mathf.Abs(y - roadCenter);
                    if (roadDistance < 18f) color = roadDistance > 14f ? roadEdge : road;

                    float riverCenter = 54f + Mathf.Sin((chunkX * 16f + x / 16f) * 0.18f + 2.4f) * 24f;
                    float riverDistance = Mathf.Abs(y - riverCenter);
                    if (riverDistance < 10f && ChunkHash01(chunkX, 0, seed, 77) > 0.18f)
                    {
                        color = riverDistance < 2f ? riverLight : river;
                    }

                    pixels[y * width + x] = color;
                }
            }

            for (int i = 0; i < 72; i++)
            {
                int x = Mathf.FloorToInt(ChunkHash01(chunkX, chunkY, seed, 1000 + i) * width);
                int y = Mathf.FloorToInt(ChunkHash01(chunkX, chunkY, seed, 2000 + i) * height);
                Color32 speckle = ChunkHash01(chunkX, chunkY, seed, 3000 + i) > 0.5f
                    ? new Color32(65, 112, 52, 120)
                    : new Color32(28, 62, 35, 120);
                FillRect(pixels, width, height, x, y, 3, 2, speckle);
            }
        }

        private static float ChunkHash01(int x, int y, int seed, int salt)
        {
            unchecked
            {
                int hash = seed;
                hash = hash * 73856093 ^ x * 19349663 ^ y * 83492791 ^ salt * 374761393;
                hash ^= hash >> 13;
                hash *= 1274126177;
                hash ^= hash >> 16;
                return (hash & 0x7fffffff) / 2147483647f;
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

        private static void PaintItemWood(Color32[] pixels)
        {
            Color32 bark = new Color32(58, 35, 20, 255);
            Color32 wood = new Color32(148, 93, 44, 255);
            Color32 woodLight = new Color32(188, 132, 72, 255);
            Color32 pith = new Color32(94, 56, 26, 255);
            Color32 rope = new Color32(204, 172, 88, 255);
            Color32 ropeDark = new Color32(138, 110, 48, 255);

            FillRect(pixels, 16, 16, 2, 2, 6, 6, bark);
            FillRect(pixels, 16, 16, 3, 3, 4, 4, wood);
            SetPixel(pixels, 16, 16, 4, 5, woodLight);
            SetPixel(pixels, 16, 16, 4, 4, pith);

            FillRect(pixels, 16, 16, 8, 2, 6, 6, bark);
            FillRect(pixels, 16, 16, 9, 3, 4, 4, wood);
            SetPixel(pixels, 16, 16, 10, 5, woodLight);
            SetPixel(pixels, 16, 16, 10, 4, pith);

            FillRect(pixels, 16, 16, 5, 7, 6, 6, bark);
            FillRect(pixels, 16, 16, 6, 8, 4, 4, wood);
            SetPixel(pixels, 16, 16, 7, 10, woodLight);
            SetPixel(pixels, 16, 16, 7, 9, pith);

            FillRect(pixels, 16, 16, 3, 6, 10, 2, rope);
            SetPixel(pixels, 16, 16, 3, 6, ropeDark);
            SetPixel(pixels, 16, 16, 12, 6, ropeDark);
            SetPixel(pixels, 16, 16, 8, 7, ropeDark);
        }

        private static void PaintItemStone(Color32[] pixels)
        {
            Color32 dark = new Color32(40, 42, 46, 255);
            Color32 mid = new Color32(92, 98, 106, 255);
            Color32 light = new Color32(146, 154, 164, 255);
            Color32 highlight = new Color32(202, 210, 222, 255);

            FillRect(pixels, 16, 16, 2, 2, 12, 10, dark);
            FillRect(pixels, 16, 16, 4, 12, 7, 2, dark);
            FillRect(pixels, 16, 16, 3, 3, 10, 8, mid);
            FillRect(pixels, 16, 16, 5, 11, 5, 2, mid);

            FillRect(pixels, 16, 16, 4, 8, 5, 4, light);
            SetPixel(pixels, 16, 16, 3, 9, highlight);
            SetPixel(pixels, 16, 16, 4, 10, highlight);
            SetPixel(pixels, 16, 16, 5, 11, highlight);
            SetPixel(pixels, 16, 16, 6, 12, highlight);
            SetPixel(pixels, 16, 16, 10, 6, light);
            SetPixel(pixels, 16, 16, 11, 5, light);
            SetPixel(pixels, 16, 16, 9, 3, dark);
        }

        private static void PaintItemCabinPlank(Color32[] pixels)
        {
            Color32 dark = new Color32(66, 38, 22, 255);
            Color32 wood = new Color32(158, 102, 50, 255);
            Color32 light = new Color32(204, 142, 82, 255);
            Color32 nail = new Color32(64, 70, 78, 255);
            Color32 nailGlint = new Color32(188, 196, 206, 255);

            FillRect(pixels, 16, 16, 1, 9, 14, 5, dark);
            FillRect(pixels, 16, 16, 2, 10, 12, 3, wood);
            FillRect(pixels, 16, 16, 3, 11, 9, 1, light);
            SetPixel(pixels, 16, 16, 3, 11, nail);
            SetPixel(pixels, 16, 16, 4, 12, nailGlint);
            SetPixel(pixels, 16, 16, 12, 11, nail);
            SetPixel(pixels, 16, 16, 13, 12, nailGlint);

            FillRect(pixels, 16, 16, 1, 2, 14, 5, dark);
            FillRect(pixels, 16, 16, 2, 3, 12, 3, wood);
            FillRect(pixels, 16, 16, 4, 4, 8, 1, light);
            SetPixel(pixels, 16, 16, 3, 4, nail);
            SetPixel(pixels, 16, 16, 4, 5, nailGlint);
            SetPixel(pixels, 16, 16, 12, 4, nail);
            SetPixel(pixels, 16, 16, 13, 5, nailGlint);
        }

        private static void PaintItemWildBerries(Color32[] pixels)
        {
            Color32 twig = new Color32(76, 48, 26, 255);
            Color32 leafDark = new Color32(28, 86, 38, 255);
            Color32 leaf = new Color32(52, 148, 64, 255);
            Color32 berryDark = new Color32(120, 16, 28, 255);
            Color32 berry = new Color32(204, 32, 50, 255);
            Color32 berryLight = new Color32(248, 136, 150, 255);

            FillRect(pixels, 16, 16, 7, 10, 2, 4, twig);
            FillRect(pixels, 16, 16, 3, 10, 4, 3, leafDark);
            FillRect(pixels, 16, 16, 4, 11, 3, 2, leaf);
            FillRect(pixels, 16, 16, 9, 10, 4, 3, leafDark);
            FillRect(pixels, 16, 16, 10, 11, 3, 2, leaf);

            FillRect(pixels, 16, 16, 3, 3, 5, 5, berryDark);
            FillRect(pixels, 16, 16, 4, 4, 3, 3, berry);
            SetPixel(pixels, 16, 16, 4, 6, berryLight);

            FillRect(pixels, 16, 16, 8, 3, 5, 5, berryDark);
            FillRect(pixels, 16, 16, 9, 4, 3, 3, berry);
            SetPixel(pixels, 16, 16, 9, 6, berryLight);

            FillRect(pixels, 16, 16, 5, 7, 6, 5, berryDark);
            FillRect(pixels, 16, 16, 6, 8, 4, 3, berry);
            SetPixel(pixels, 16, 16, 6, 10, berryLight);
        }

        private static void PaintItemMedicinalHerb(Color32[] pixels)
        {
            Color32 root = new Color32(174, 148, 106, 255);
            Color32 stem = new Color32(36, 96, 46, 255);
            Color32 dark = new Color32(22, 68, 30, 255);
            Color32 mid = new Color32(54, 148, 68, 255);
            Color32 light = new Color32(112, 214, 126, 255);
            Color32 flower = new Color32(244, 216, 72, 255);

            FillRect(pixels, 16, 16, 7, 1, 2, 3, root);
            FillRect(pixels, 16, 16, 7, 4, 2, 8, stem);

            FillRect(pixels, 16, 16, 2, 4, 5, 3, dark);
            FillRect(pixels, 16, 16, 3, 5, 4, 2, mid);
            SetPixel(pixels, 16, 16, 4, 6, light);

            FillRect(pixels, 16, 16, 9, 6, 5, 3, dark);
            FillRect(pixels, 16, 16, 9, 7, 4, 2, mid);
            SetPixel(pixels, 16, 16, 11, 8, light);

            FillRect(pixels, 16, 16, 3, 8, 5, 3, dark);
            FillRect(pixels, 16, 16, 3, 9, 4, 2, mid);
            SetPixel(pixels, 16, 16, 5, 10, light);

            FillRect(pixels, 16, 16, 6, 12, 4, 3, flower);
            SetPixel(pixels, 16, 16, 7, 14, new Color32(255, 248, 160, 255));
        }

        private static void PaintItemMushroom(Color32[] pixels)
        {
            Color32 capDark = new Color32(96, 28, 24, 255);
            Color32 cap = new Color32(204, 48, 38, 255);
            Color32 capLight = new Color32(236, 96, 82, 255);
            Color32 spot = new Color32(248, 240, 218, 255);
            Color32 stemDark = new Color32(166, 148, 120, 255);
            Color32 stem = new Color32(224, 210, 184, 255);
            Color32 moss = new Color32(48, 92, 42, 255);

            FillRect(pixels, 16, 16, 4, 1, 8, 2, moss);

            FillRect(pixels, 16, 16, 6, 2, 4, 7, stemDark);
            FillRect(pixels, 16, 16, 7, 3, 2, 6, stem);

            FillRect(pixels, 16, 16, 2, 7, 12, 6, capDark);
            FillRect(pixels, 16, 16, 4, 13, 8, 2, capDark);
            FillRect(pixels, 16, 16, 3, 8, 10, 5, cap);
            FillRect(pixels, 16, 16, 5, 12, 6, 2, capLight);

            SetPixel(pixels, 16, 16, 4, 10, spot);
            SetPixel(pixels, 16, 16, 8, 11, spot);
            SetPixel(pixels, 16, 16, 11, 9, spot);
        }

        private static void PaintItemIronOre(Color32[] pixels)
        {
            Color32 dark = new Color32(42, 44, 50, 255);
            Color32 rock = new Color32(76, 80, 88, 255);
            Color32 ironDark = new Color32(110, 122, 136, 255);
            Color32 ironLight = new Color32(204, 218, 234, 255);
            Color32 rust = new Color32(148, 88, 48, 255);

            FillRect(pixels, 16, 16, 2, 2, 12, 11, dark);
            FillRect(pixels, 16, 16, 3, 3, 10, 9, rock);

            FillRect(pixels, 16, 16, 3, 7, 4, 2, rust);
            FillRect(pixels, 16, 16, 6, 5, 3, 2, rust);

            FillRect(pixels, 16, 16, 8, 8, 4, 3, ironDark);
            FillRect(pixels, 16, 16, 9, 9, 2, 2, ironLight);
            FillRect(pixels, 16, 16, 4, 4, 3, 3, ironDark);
            SetPixel(pixels, 16, 16, 5, 5, ironLight);
            SetPixel(pixels, 16, 16, 11, 4, ironLight);
        }

        private static void PaintItemOldCoin(Color32[] pixels)
        {
            Color32 dark = new Color32(84, 58, 16, 255);
            Color32 gold = new Color32(208, 158, 38, 255);
            Color32 goldLight = new Color32(248, 218, 92, 255);
            Color32 highlight = new Color32(255, 248, 186, 255);

            FillRect(pixels, 16, 16, 3, 2, 10, 12, dark);
            FillRect(pixels, 16, 16, 2, 4, 12, 8, dark);

            FillRect(pixels, 16, 16, 4, 3, 8, 10, gold);
            FillRect(pixels, 16, 16, 3, 5, 10, 6, gold);

            DrawRect(pixels, 16, 16, 5, 4, 10, 11, goldLight);
            SetPixel(pixels, 16, 16, 6, 8, highlight);
            SetPixel(pixels, 16, 16, 8, 8, highlight);
            SetPixel(pixels, 16, 16, 7, 7, highlight);
            SetPixel(pixels, 16, 16, 7, 6, dark);
        }

        private static void PaintItemTorch(Color32[] pixels)
        {
            Color32 woodDark = new Color32(68, 42, 24, 255);
            Color32 wood = new Color32(118, 74, 38, 255);
            Color32 wrap = new Color32(48, 44, 42, 255);
            Color32 flameRed = new Color32(208, 48, 24, 255);
            Color32 flameOrange = new Color32(248, 138, 32, 255);
            Color32 flameYellow = new Color32(255, 236, 88, 255);

            for (int i = 0; i < 7; i++)
            {
                SetPixel(pixels, 16, 16, 2 + i, 2 + i, woodDark);
                SetPixel(pixels, 16, 16, 3 + i, 2 + i, wood);
            }

            SetPixel(pixels, 16, 16, 8, 8, wrap);
            SetPixel(pixels, 16, 16, 9, 8, wrap);
            SetPixel(pixels, 16, 16, 9, 9, wrap);

            FillRect(pixels, 16, 16, 9, 10, 5, 5, flameRed);
            FillRect(pixels, 16, 16, 10, 11, 3, 3, flameOrange);
            SetPixel(pixels, 16, 16, 11, 12, flameYellow);
            SetPixel(pixels, 16, 16, 10, 13, flameYellow);
            SetPixel(pixels, 16, 16, 12, 15, flameOrange);
            SetPixel(pixels, 16, 16, 7, 14, flameYellow);
        }

        private static void PaintItemAxe(Color32[] pixels)
        {
            Color32 haftDark = new Color32(66, 40, 22, 255);
            Color32 haft = new Color32(124, 78, 42, 255);
            Color32 steelDark = new Color32(62, 70, 80, 255);
            Color32 steel = new Color32(120, 132, 146, 255);
            Color32 blade = new Color32(212, 226, 240, 255);

            for (int i = 0; i < 11; i++)
            {
                SetPixel(pixels, 16, 16, 3 + i, 2 + i, haftDark);
                SetPixel(pixels, 16, 16, 4 + i, 2 + i, haft);
            }

            FillRect(pixels, 16, 16, 9, 10, 4, 4, steelDark);
            FillRect(pixels, 16, 16, 7, 11, 4, 3, steel);
            FillRect(pixels, 16, 16, 5, 12, 3, 3, blade);
            SetPixel(pixels, 16, 16, 4, 13, blade);
            SetPixel(pixels, 16, 16, 4, 14, blade);
        }

        private static void PaintItemPickaxe(Color32[] pixels)
        {
            Color32 haftDark = new Color32(66, 40, 22, 255);
            Color32 haft = new Color32(124, 78, 42, 255);
            Color32 steelDark = new Color32(60, 68, 78, 255);
            Color32 steel = new Color32(116, 128, 142, 255);
            Color32 tip = new Color32(204, 218, 232, 255);

            for (int i = 0; i < 11; i++)
            {
                SetPixel(pixels, 16, 16, 3 + i, 2 + i, haftDark);
                SetPixel(pixels, 16, 16, 4 + i, 2 + i, haft);
            }

            SetPixel(pixels, 16, 16, 4, 14, tip);
            SetPixel(pixels, 16, 16, 5, 13, steel);
            SetPixel(pixels, 16, 16, 6, 12, steelDark);
            SetPixel(pixels, 16, 16, 11, 11, steelDark);
            FillRect(pixels, 16, 16, 10, 10, 3, 3, steel);
            SetPixel(pixels, 16, 16, 14, 7, steel);
            SetPixel(pixels, 16, 16, 14, 6, tip);
        }

        private static void PaintItemRoadwardenPage(Color32[] pixels)
        {
            Color32 dark = new Color32(138, 116, 78, 255);
            Color32 parchment = new Color32(218, 196, 152, 255);
            Color32 light = new Color32(242, 228, 196, 255);
            Color32 ink = new Color32(96, 80, 52, 255);
            Color32 waxDark = new Color32(132, 24, 28, 255);
            Color32 wax = new Color32(204, 42, 48, 255);

            FillRect(pixels, 16, 16, 3, 2, 10, 12, dark);
            FillRect(pixels, 16, 16, 4, 3, 8, 10, parchment);
            FillRect(pixels, 16, 16, 5, 4, 6, 8, light);

            SetPixel(pixels, 16, 16, 11, 12, dark);
            SetPixel(pixels, 16, 16, 11, 13, dark);
            SetPixel(pixels, 16, 16, 10, 13, dark);

            FillRect(pixels, 16, 16, 5, 10, 5, 1, ink);
            FillRect(pixels, 16, 16, 5, 8, 6, 1, ink);
            FillRect(pixels, 16, 16, 5, 6, 4, 1, ink);

            FillRect(pixels, 16, 16, 8, 3, 3, 3, waxDark);
            SetPixel(pixels, 16, 16, 9, 4, wax);
        }

        private static void PaintItemBellFragment(Color32[] pixels)
        {
            Color32 dark = new Color32(52, 46, 34, 255);
            Color32 bronze = new Color32(118, 104, 72, 255);
            Color32 bronzeLight = new Color32(174, 156, 112, 255);
            Color32 runeDark = new Color32(36, 152, 168, 255);
            Color32 rune = new Color32(84, 226, 244, 255);
            Color32 runeGlint = new Color32(210, 250, 255, 255);

            FillRect(pixels, 16, 16, 3, 2, 10, 12, dark);
            FillRect(pixels, 16, 16, 4, 3, 8, 10, bronze);
            FillRect(pixels, 16, 16, 5, 5, 6, 7, bronzeLight);

            SetPixel(pixels, 16, 16, 3, 2, new Color32(0, 0, 0, 0));
            SetPixel(pixels, 16, 16, 12, 13, new Color32(0, 0, 0, 0));
            SetPixel(pixels, 16, 16, 11, 13, new Color32(0, 0, 0, 0));

            FillRect(pixels, 16, 16, 6, 6, 4, 5, runeDark);
            SetPixel(pixels, 16, 16, 7, 7, rune);
            SetPixel(pixels, 16, 16, 8, 8, rune);
            SetPixel(pixels, 16, 16, 7, 9, runeGlint);
        }

        private static void PaintItemCookedMeal(Color32[] pixels)
        {
            Color32 bowlDark = new Color32(76, 44, 24, 255);
            Color32 bowl = new Color32(136, 82, 46, 255);
            Color32 stew = new Color32(184, 98, 34, 255);
            Color32 meat = new Color32(104, 40, 26, 255);
            Color32 herb = new Color32(48, 132, 54, 255);
            Color32 steam = new Color32(220, 226, 230, 160);

            FillRect(pixels, 16, 16, 2, 2, 12, 6, bowlDark);
            FillRect(pixels, 16, 16, 3, 3, 10, 4, bowl);
            FillRect(pixels, 16, 16, 3, 6, 10, 3, stew);

            SetPixel(pixels, 16, 16, 5, 7, meat);
            SetPixel(pixels, 16, 16, 9, 7, meat);
            SetPixel(pixels, 16, 16, 7, 7, herb);

            SetPixel(pixels, 16, 16, 5, 10, steam);
            SetPixel(pixels, 16, 16, 6, 12, steam);
            SetPixel(pixels, 16, 16, 10, 10, steam);
            SetPixel(pixels, 16, 16, 9, 12, steam);
        }

        private static void PaintItemEgg(Color32[] pixels)
        {
            Color32 dark = new Color32(148, 130, 102, 255);
            Color32 mid = new Color32(224, 208, 178, 255);
            Color32 light = new Color32(248, 240, 224, 255);
            Color32 speckle = new Color32(174, 148, 114, 255);

            FillRect(pixels, 16, 16, 4, 2, 8, 11, dark);
            FillRect(pixels, 16, 16, 3, 4, 10, 7, dark);
            FillRect(pixels, 16, 16, 5, 13, 6, 1, dark);

            FillRect(pixels, 16, 16, 5, 3, 6, 9, mid);
            FillRect(pixels, 16, 16, 4, 5, 8, 6, mid);
            FillRect(pixels, 16, 16, 6, 6, 4, 5, light);

            SetPixel(pixels, 16, 16, 5, 5, speckle);
            SetPixel(pixels, 16, 16, 9, 6, speckle);
            SetPixel(pixels, 16, 16, 7, 3, speckle);
        }

        private static void PaintItemWool(Color32[] pixels)
        {
            Color32 dark = new Color32(148, 142, 134, 255);
            Color32 mid = new Color32(218, 214, 206, 255);
            Color32 light = new Color32(250, 248, 244, 255);
            Color32 stringColor = new Color32(138, 112, 72, 255);

            FillRect(pixels, 16, 16, 3, 3, 10, 10, dark);
            FillRect(pixels, 16, 16, 2, 5, 12, 6, dark);

            FillRect(pixels, 16, 16, 4, 4, 8, 8, mid);
            FillRect(pixels, 16, 16, 3, 6, 10, 4, mid);

            FillRect(pixels, 16, 16, 5, 6, 5, 4, light);
            SetPixel(pixels, 16, 16, 9, 9, light);

            FillRect(pixels, 16, 16, 2, 7, 12, 2, stringColor);
        }

        private static void PaintItemMilk(Color32[] pixels)
        {
            Color32 dark = new Color32(76, 44, 24, 255);
            Color32 clay = new Color32(142, 86, 50, 255);
            Color32 light = new Color32(188, 124, 78, 255);
            Color32 milk = new Color32(248, 250, 254, 255);

            FillRect(pixels, 16, 16, 4, 2, 8, 8, dark);
            FillRect(pixels, 16, 16, 5, 3, 6, 6, clay);
            FillRect(pixels, 16, 16, 6, 4, 3, 4, light);

            FillRect(pixels, 16, 16, 6, 10, 4, 3, dark);
            FillRect(pixels, 16, 16, 7, 10, 2, 2, clay);
            FillRect(pixels, 16, 16, 5, 13, 6, 2, dark);
            FillRect(pixels, 16, 16, 6, 13, 4, 1, milk);

            FillRect(pixels, 16, 16, 2, 5, 2, 5, dark);
            SetPixel(pixels, 16, 16, 3, 6, new Color32(0, 0, 0, 0));
            SetPixel(pixels, 16, 16, 3, 7, new Color32(0, 0, 0, 0));
        }

        private static void PaintSlashArc(Color32[] pixels)
        {
            Color32 white = new Color32(255, 255, 255, 255);
            Color32 lightSilver = new Color32(220, 235, 255, 210);
            Color32 trail = new Color32(160, 195, 240, 130);
            Color32 fade = new Color32(100, 140, 200, 60);

            // Arc curve from top right curving towards bottom right
            for (int i = 0; i < 18; i++)
            {
                float t = i / 17f;
                float angle = Mathf.Lerp(-0.75f, 0.75f, t) * Mathf.PI;
                int x = Mathf.Clamp(Mathf.RoundToInt(12f + Mathf.Cos(angle) * 10f), 0, 23);
                int y = Mathf.Clamp(Mathf.RoundToInt(12f + Mathf.Sin(angle) * 10f), 0, 23);

                SetPixel(pixels, 24, 24, x, y, white);
                SetPixel(pixels, 24, 24, x - 1, y, lightSilver);
                SetPixel(pixels, 24, 24, x, y - 1, lightSilver);
                SetPixel(pixels, 24, 24, x - 2, y, trail);
                SetPixel(pixels, 24, 24, x - 3, y, fade);
            }
        }

        private static void PaintWolf(Color32[] pixels, int frame)
        {
            Color32 furDark = new Color32(45, 48, 54, 255);
            Color32 fur = new Color32(82, 88, 98, 255);
            Color32 furLight = new Color32(128, 136, 148, 255);
            Color32 eyeRed = new Color32(235, 45, 35, 255);
            Color32 teeth = new Color32(245, 245, 240, 255);

            int step = (frame == 1) ? 1 : (frame == 3 ? -1 : 0);

            // Body
            FillRect(pixels, 22, 16, 5, 4, 12, 6, furDark);
            FillRect(pixels, 22, 16, 6, 5, 10, 5, fur);
            FillRect(pixels, 22, 16, 7, 7, 7, 2, furLight);

            // Head and Muzzle
            FillRect(pixels, 22, 16, 15, 6, 5, 6, furDark);
            FillRect(pixels, 22, 16, 16, 7, 4, 4, fur);
            FillRect(pixels, 22, 16, 18, 6, 3, 3, furLight);
            SetPixel(pixels, 22, 16, 17, 9, eyeRed);
            SetPixel(pixels, 22, 16, 20, 6, teeth);

            // Ears
            FillRect(pixels, 22, 16, 15, 12, 2, 3, furDark);
            SetPixel(pixels, 22, 16, 15, 13, furLight);

            // Bushy Tail
            FillRect(pixels, 22, 16, 2, 7, 4, 4, furDark);
            FillRect(pixels, 22, 16, 1, 9, 3, 3, fur);
            SetPixel(pixels, 22, 16, 1, 11, furLight);

            // Legs with running animation
            FillRect(pixels, 22, 16, 6, 1 + Math.Max(0, step), 2, 4, furDark);
            FillRect(pixels, 22, 16, 9, 1 + Math.Max(0, -step), 2, 4, furDark);
            FillRect(pixels, 22, 16, 13, 1 + Math.Max(0, -step), 2, 4, furDark);
            FillRect(pixels, 22, 16, 16, 1 + Math.Max(0, step), 2, 4, furDark);
        }

        private static void PaintBandit(Color32[] pixels, int frame)
        {
            Color32 hoodDark = new Color32(32, 28, 30, 255);
            Color32 hood = new Color32(58, 48, 50, 255);
            Color32 leather = new Color32(108, 68, 42, 255);
            Color32 iron = new Color32(148, 158, 168, 255);
            Color32 skin = new Color32(188, 142, 102, 255);

            int step = (frame == 1) ? 1 : (frame == 3 ? -1 : 0);

            // Boots / Legs
            FillRect(pixels, 16, 22, 5, 0, 3, 5 + Math.Max(0, step), hoodDark);
            FillRect(pixels, 16, 22, 8, 0, 3, 5 + Math.Max(0, -step), hoodDark);

            // Torso / Leather Jerkin
            FillRect(pixels, 16, 22, 4, 5, 8, 9, leather);
            FillRect(pixels, 16, 22, 5, 6, 6, 6, hoodDark);
            FillRect(pixels, 16, 22, 6, 8, 4, 1, iron); // Belt buckle

            // Hood and Head
            FillRect(pixels, 16, 22, 4, 14, 8, 7, hoodDark);
            FillRect(pixels, 16, 22, 5, 15, 6, 5, hood);
            FillRect(pixels, 16, 22, 6, 16, 4, 2, skin); // Eyes slit
            SetPixel(pixels, 16, 22, 7, 17, hoodDark);
            SetPixel(pixels, 16, 22, 9, 17, hoodDark);

            // Weapon (Dagger / Club)
            FillRect(pixels, 16, 22, 12, 7 + step, 2, 7, iron);
            FillRect(pixels, 16, 22, 11, 8 + step, 4, 2, hoodDark);
        }

        private static void PaintSilverCoin(Color32[] pixels)
        {
            Color32 edge = new Color32(140, 150, 165, 255);
            Color32 silver = new Color32(210, 222, 235, 255);
            Color32 shine = new Color32(255, 255, 255, 255);
            Color32 shadow = new Color32(110, 120, 135, 255);

            FillRect(pixels, 16, 16, 4, 2, 8, 12, edge);
            FillRect(pixels, 16, 16, 2, 4, 12, 8, edge);
            FillRect(pixels, 16, 16, 5, 3, 6, 10, silver);
            FillRect(pixels, 16, 16, 3, 5, 10, 6, silver);
            // Center emblem & rim shine
            FillRect(pixels, 16, 16, 6, 6, 4, 4, shadow);
            FillRect(pixels, 16, 16, 7, 7, 2, 2, shine);
            SetPixel(pixels, 16, 16, 4, 11, shine);
            SetPixel(pixels, 16, 16, 5, 12, shine);
        }

        private static void PaintHeartEmote(Color32[] pixels)
        {
            Color32 red = new Color32(235, 45, 65, 255);
            Color32 darkRed = new Color32(170, 20, 35, 255);
            Color32 shine = new Color32(255, 180, 190, 255);

            FillRect(pixels, 16, 16, 2, 8, 5, 5, darkRed);
            FillRect(pixels, 16, 16, 9, 8, 5, 5, darkRed);
            FillRect(pixels, 16, 16, 3, 4, 10, 7, darkRed);
            FillRect(pixels, 16, 16, 5, 2, 6, 4, darkRed);
            FillRect(pixels, 16, 16, 7, 0, 2, 3, darkRed);

            FillRect(pixels, 16, 16, 3, 8, 3, 4, red);
            FillRect(pixels, 16, 16, 10, 8, 3, 4, red);
            FillRect(pixels, 16, 16, 4, 4, 8, 6, red);
            FillRect(pixels, 16, 16, 6, 2, 4, 3, red);
            SetPixel(pixels, 16, 16, 7, 1, red);
            SetPixel(pixels, 16, 16, 8, 1, red);

            SetPixel(pixels, 16, 16, 4, 10, shine);
            SetPixel(pixels, 16, 16, 5, 11, shine);
        }

        private static void PaintTilledSoil(Color32[] pixels, bool watered)
        {
            Color32 edgeShadow = watered ? new Color32(32, 18, 10, 255) : new Color32(110, 80, 48, 255);
            Color32 baseSoil = watered ? new Color32(65, 42, 24, 255) : new Color32(195, 170, 118, 255);
            Color32 soilRidge = watered ? new Color32(85, 56, 32, 255) : new Color32(220, 195, 142, 255);
            Color32 fissure = watered ? new Color32(42, 26, 14, 255) : new Color32(150, 122, 78, 255);
            Color32 wetShine = new Color32(140, 175, 205, 210);

            // Outer margin / shadow border for square grid distinction
            FillRect(pixels, 32, 32, 1, 1, 30, 30, edgeShadow);
            FillRect(pixels, 32, 32, 2, 2, 28, 28, baseSoil);

            // Organic tilled earthen fissures & ridges
            for (int row = 0; row < 4; row++)
            {
                int y = 4 + row * 6;
                FillRect(pixels, 32, 32, 3, y, 26, 5, baseSoil);
                FillRect(pixels, 32, 32, 4, y + 2, 24, 2, soilRidge);

                // Soil fissures / clods
                SetPixel(pixels, 32, 32, 7 + row * 4, y + 1, fissure);
                SetPixel(pixels, 32, 32, 8 + row * 4, y + 2, fissure);
                SetPixel(pixels, 32, 32, 18 - row * 3, y + 3, fissure);
                SetPixel(pixels, 32, 32, 19 - row * 3, y + 2, fissure);

                if (watered && (row % 2 == 0))
                {
                    SetPixel(pixels, 32, 32, 9 + row * 3, y + 3, wetShine);
                    SetPixel(pixels, 32, 32, 21 - row * 2, y + 3, wetShine);
                }
            }
        }

        private static void PaintCrop(Color32[] pixels, string cropId, int stage)
        {
            Color32 sproutGreen = new Color32(95, 195, 60, 255);
            Color32 darkGreen = new Color32(45, 130, 35, 255);
            Color32 stalkGreen = new Color32(70, 160, 45, 255);

            if (stage == 0)
            {
                // Sprout / seeds in dirt
                FillRect(pixels, 24, 28, 11, 4, 2, 4, sproutGreen);
                SetPixel(pixels, 24, 28, 10, 7, sproutGreen);
                SetPixel(pixels, 24, 28, 13, 7, sproutGreen);
                return;
            }

            if (stage == 1)
            {
                // Small growing plant
                FillRect(pixels, 24, 28, 11, 4, 2, 9, stalkGreen);
                FillRect(pixels, 24, 28, 8, 8, 4, 3, sproutGreen);
                FillRect(pixels, 24, 28, 12, 10, 5, 3, sproutGreen);
                return;
            }

            if (cropId == "crop.wheat" || cropId == "wheat")
            {
                Color32 golden = stage >= 3 ? new Color32(235, 195, 65, 255) : new Color32(165, 190, 60, 255);
                Color32 wheatDark = stage >= 3 ? new Color32(185, 135, 35, 255) : darkGreen;

                FillRect(pixels, 24, 28, 10, 4, 4, 14, stalkGreen);
                FillRect(pixels, 24, 28, 7, 8, 4, 2, darkGreen);
                FillRect(pixels, 24, 28, 13, 11, 4, 2, darkGreen);
                // Plump wheat ears on top
                FillRect(pixels, 24, 28, 8, 16, 8, 8, golden);
                FillRect(pixels, 24, 28, 9, 18, 6, 6, wheatDark);
                FillRect(pixels, 24, 28, 10, 23, 4, 4, golden);
            }
            else if (cropId == "crop.corn" || cropId == "corn")
            {
                Color32 cornYellow = new Color32(245, 215, 45, 255);
                Color32 husk = new Color32(120, 185, 55, 255);

                FillRect(pixels, 24, 28, 10, 4, 4, 18, darkGreen);
                // Broad maize leaves
                FillRect(pixels, 24, 28, 4, 8, 7, 3, husk);
                FillRect(pixels, 24, 28, 13, 11, 7, 3, husk);
                FillRect(pixels, 24, 28, 5, 15, 6, 3, husk);
                // Corn cobs
                if (stage >= 2)
                {
                    FillRect(pixels, 24, 28, 12, 14, 5, 7, cornYellow);
                    FillRect(pixels, 24, 28, 11, 13, 4, 4, husk);
                    if (stage >= 3)
                    {
                        FillRect(pixels, 24, 28, 7, 18, 5, 7, cornYellow);
                        FillRect(pixels, 24, 28, 13, 20, 3, 2, new Color32(185, 100, 40, 255)); // silk
                    }
                }
            }
            else if (cropId == "crop.pineapple" || cropId == "pineapple")
            {
                Color32 pineGold = new Color32(245, 185, 35, 255);
                Color32 pineDark = new Color32(190, 130, 20, 255);
                Color32 pineSpike = new Color32(85, 190, 50, 255);

                // Spreading sharp leaves at base
                FillRect(pixels, 24, 28, 4, 3, 5, 4, pineSpike);
                FillRect(pixels, 24, 28, 15, 3, 5, 4, pineSpike);
                FillRect(pixels, 24, 28, 2, 7, 6, 3, pineSpike);
                FillRect(pixels, 24, 28, 16, 7, 6, 3, pineSpike);

                if (stage >= 2)
                {
                    // Golden Pineapple body
                    FillRect(pixels, 24, 28, 7, 6, 10, 11, pineDark);
                    FillRect(pixels, 24, 28, 8, 7, 8, 9, pineGold);
                    // Pineapple diamond cross pattern
                    SetPixel(pixels, 24, 28, 9, 13, pineDark);
                    SetPixel(pixels, 24, 28, 11, 14, pineDark);
                    SetPixel(pixels, 24, 28, 13, 13, pineDark);
                    SetPixel(pixels, 24, 28, 10, 10, pineDark);
                    SetPixel(pixels, 24, 28, 12, 10, pineDark);
                    SetPixel(pixels, 24, 28, 9, 8, pineDark);
                    SetPixel(pixels, 24, 28, 11, 8, pineDark);
                    SetPixel(pixels, 24, 28, 13, 8, pineDark);

                    // Crown of green spiky leaves on top
                    FillRect(pixels, 24, 28, 10, 17, 4, 7, pineSpike);
                    FillRect(pixels, 24, 28, 7, 18, 4, 5, pineSpike);
                    FillRect(pixels, 24, 28, 13, 18, 4, 5, pineSpike);
                    SetPixel(pixels, 24, 28, 6, 21, pineSpike);
                    SetPixel(pixels, 24, 28, 17, 21, pineSpike);
                    SetPixel(pixels, 24, 28, 11, 25, pineSpike);
                    SetPixel(pixels, 24, 28, 12, 25, pineSpike);
                }
            }
            else if (cropId == "crop.tomato" || cropId == "tomato")
            {
                Color32 tomatoRed = new Color32(235, 45, 30, 255);
                Color32 tomatoDark = new Color32(175, 25, 20, 255);
                Color32 calyx = new Color32(85, 185, 50, 255);

                // Bushy vine
                FillRect(pixels, 24, 28, 6, 4, 12, 14, darkGreen);
                FillRect(pixels, 24, 28, 4, 8, 16, 8, stalkGreen);

                if (stage >= 2)
                {
                    // Plump glossy red tomatoes hanging
                    FillRect(pixels, 24, 28, 6, 6, 5, 5, tomatoDark);
                    FillRect(pixels, 24, 28, 7, 7, 4, 4, tomatoRed);
                    SetPixel(pixels, 24, 28, 7, 10, calyx);
                    SetPixel(pixels, 24, 28, 8, 9, Color.white); // shine

                    FillRect(pixels, 24, 28, 13, 9, 6, 6, tomatoDark);
                    FillRect(pixels, 24, 28, 14, 10, 5, 5, tomatoRed);
                    SetPixel(pixels, 24, 28, 15, 14, calyx);
                    SetPixel(pixels, 24, 28, 15, 13, Color.white);

                    if (stage >= 3)
                    {
                        FillRect(pixels, 24, 28, 9, 13, 6, 6, tomatoDark);
                        FillRect(pixels, 24, 28, 10, 14, 5, 5, tomatoRed);
                        SetPixel(pixels, 24, 28, 11, 18, calyx);
                        SetPixel(pixels, 24, 28, 11, 17, Color.white);
                    }
                }
            }
            else // Carrot
            {
                Color32 carrotOrange = new Color32(245, 115, 30, 255);
                Color32 carrotDark = new Color32(195, 75, 15, 255);

                // Bushy green top
                FillRect(pixels, 24, 28, 7, 10, 10, 8, sproutGreen);
                FillRect(pixels, 24, 28, 9, 14, 6, 6, darkGreen);
                FillRect(pixels, 24, 28, 6, 12, 4, 4, sproutGreen);
                FillRect(pixels, 24, 28, 14, 12, 4, 4, sproutGreen);

                // Orange root poking above soil
                if (stage >= 2)
                {
                    FillRect(pixels, 24, 28, 10, 4, 4, 6, carrotOrange);
                    FillRect(pixels, 24, 28, 11, 2, 2, 4, carrotDark);
                    if (stage >= 3)
                    {
                        FillRect(pixels, 24, 28, 9, 5, 6, 5, carrotOrange);
                        SetPixel(pixels, 24, 28, 10, 9, new Color32(255, 165, 80, 255)); // highlight
                    }
                }
            }
        }

        private static void PaintWoodFence(Color32[] pixels)
        {
            Color32 woodDark = new Color32(130, 80, 40, 255);
            Color32 woodMid = new Color32(185, 125, 65, 255);
            Color32 woodLight = new Color32(225, 165, 95, 255);
            Color32 iron = new Color32(45, 40, 35, 255);

            // Left Post (x=0..5)
            FillRect(pixels, 32, 24, 1, 0, 5, 18, woodDark);
            FillRect(pixels, 32, 24, 2, 1, 3, 17, woodMid);
            FillRect(pixels, 32, 24, 2, 17, 3, 2, woodLight); // rounded cap

            // Right Post (x=26..31)
            FillRect(pixels, 32, 24, 26, 0, 5, 18, woodDark);
            FillRect(pixels, 32, 24, 27, 1, 3, 17, woodMid);
            FillRect(pixels, 32, 24, 27, 17, 3, 2, woodLight);

            // Top Horizontal Continuous Rail (runs all the way across 0..32)
            FillRect(pixels, 32, 24, 0, 11, 32, 4, woodDark);
            FillRect(pixels, 32, 24, 0, 12, 32, 2, woodMid);
            FillRect(pixels, 32, 24, 0, 14, 32, 1, woodLight);

            // Bottom Horizontal Continuous Rail (runs all the way across 0..32)
            FillRect(pixels, 32, 24, 0, 4, 32, 4, woodDark);
            FillRect(pixels, 32, 24, 0, 5, 32, 2, woodMid);
            FillRect(pixels, 32, 24, 0, 7, 32, 1, woodLight);

            // Fastener Nails
            SetPixel(pixels, 32, 24, 3, 13, iron);
            SetPixel(pixels, 32, 24, 3, 6, iron);
            SetPixel(pixels, 32, 24, 28, 13, iron);
            SetPixel(pixels, 32, 24, 28, 6, iron);
        }

        private static void PaintWoodGate(Color32[] pixels, bool isOpen)
        {
            Color32 postDark = new Color32(75, 48, 25, 255);
            Color32 post = new Color32(115, 78, 44, 255);
            Color32 postLight = new Color32(155, 108, 65, 255);
            Color32 ironHinge = new Color32(40, 40, 45, 255);

            // Gate Posts
            FillRect(pixels, 32, 24, 1, 0, 5, 22, postDark);
            FillRect(pixels, 32, 24, 2, 1, 3, 20, post);
            FillRect(pixels, 32, 24, 26, 0, 5, 22, postDark);
            FillRect(pixels, 32, 24, 27, 1, 3, 20, post);

            if (!isOpen)
            {
                // Closed gate swinging door
                FillRect(pixels, 32, 24, 6, 2, 20, 16, postDark);
                FillRect(pixels, 32, 24, 7, 3, 18, 14, post);
                // Slats
                for (int i = 0; i < 4; i++)
                {
                    FillRect(pixels, 32, 24, 8 + i * 4, 3, 3, 14, postLight);
                }
                // Diagonal cross brace
                for (int d = 0; d < 12; d++)
                {
                    SetPixel(pixels, 32, 24, 8 + d, 4 + d, postDark);
                    SetPixel(pixels, 32, 24, 9 + d, 4 + d, postLight);
                }
                // Hinges and latch
                FillRect(pixels, 32, 24, 5, 14, 3, 2, ironHinge);
                FillRect(pixels, 32, 24, 5, 5, 3, 2, ironHinge);
                FillRect(pixels, 32, 24, 24, 9, 3, 3, ironHinge);
            }
            else
            {
                // Open gate (swung inward)
                FillRect(pixels, 32, 24, 6, 8, 8, 12, postDark);
                FillRect(pixels, 32, 24, 7, 9, 6, 10, post);
                FillRect(pixels, 32, 24, 5, 14, 3, 2, ironHinge);
            }
        }

        private static void PaintHerbalistHut(Color32[] pixels)
        {
            Color32 timber = new Color32(95, 62, 35, 255);
            Color32 thatch = new Color32(165, 145, 85, 255);
            Color32 thatchDark = new Color32(125, 105, 55, 255);
            Color32 mossGreen = new Color32(65, 135, 55, 255);
            Color32 purpleFlower = new Color32(185, 95, 215, 255);
            Color32 plaster = new Color32(205, 195, 175, 255);
            Color32 doorWood = new Color32(65, 40, 22, 255);

            // Base walls
            FillRect(pixels, 54, 44, 4, 0, 46, 22, plaster);
            // Timber frame beams
            FillRect(pixels, 54, 44, 4, 0, 4, 22, timber);
            FillRect(pixels, 54, 44, 46, 0, 4, 22, timber);
            FillRect(pixels, 54, 44, 25, 0, 4, 22, timber);
            FillRect(pixels, 54, 44, 4, 20, 46, 3, timber);

            // Wooden Round Door
            FillRect(pixels, 54, 44, 12, 0, 10, 15, doorWood);
            FillRect(pixels, 54, 44, 14, 2, 6, 12, timber);

            // Windows with herb pots
            FillRect(pixels, 54, 44, 32, 6, 10, 8, timber);
            FillRect(pixels, 54, 44, 34, 8, 6, 5, new Color32(225, 235, 245, 255));
            FillRect(pixels, 54, 44, 32, 5, 10, 2, timber);
            SetPixel(pixels, 54, 44, 34, 7, purpleFlower);
            SetPixel(pixels, 54, 44, 38, 7, mossGreen);

            // Thatch Roof with moss & flowers
            for (int y = 0; y < 20; y++)
            {
                int inset = (int)(y * 1.3f);
                FillRect(pixels, 54, 44, 1 + inset, 22 + y, 52 - inset * 2, 2, (y % 2 == 0) ? thatch : thatchDark);
            }
            // Moss & Flowering vines draping from roof
            FillRect(pixels, 54, 44, 6, 18, 4, 6, mossGreen);
            SetPixel(pixels, 54, 44, 7, 19, purpleFlower);
            SetPixel(pixels, 54, 44, 8, 17, purpleFlower);
            FillRect(pixels, 54, 44, 42, 17, 5, 7, mossGreen);
            SetPixel(pixels, 54, 44, 44, 18, purpleFlower);
            SetPixel(pixels, 54, 44, 45, 20, purpleFlower);
        }

        private static void PaintLookoutTower(Color32[] pixels)
        {
            Color32 timber = new Color32(85, 55, 30, 255);
            Color32 timberLight = new Color32(130, 88, 50, 255);
            Color32 fireGlow = new Color32(255, 160, 40, 255);
            Color32 fireCore = new Color32(255, 240, 120, 255);

            // 4 main stilts / legs
            FillRect(pixels, 36, 56, 4, 0, 4, 42, timber);
            FillRect(pixels, 36, 56, 28, 0, 4, 42, timber);
            // Cross braces
            for (int i = 0; i < 3; i++)
            {
                int y = 6 + i * 12;
                FillRect(pixels, 36, 56, 6, y, 24, 3, timberLight);
                for (int d = 0; d < 20; d++)
                {
                    SetPixel(pixels, 36, 56, 7 + d, y + (d * 10 / 20), timber);
                }
            }
            // Ladder in center
            FillRect(pixels, 36, 56, 15, 0, 2, 42, timber);
            FillRect(pixels, 36, 56, 19, 0, 2, 42, timber);
            for (int rung = 0; rung < 10; rung++)
            {
                FillRect(pixels, 36, 56, 15, rung * 4 + 2, 6, 1, timberLight);
            }

            // Top Platform & Railings
            FillRect(pixels, 36, 56, 2, 42, 32, 4, timberLight);
            FillRect(pixels, 36, 56, 2, 46, 3, 6, timber);
            FillRect(pixels, 36, 56, 31, 46, 3, 6, timber);
            FillRect(pixels, 36, 56, 2, 51, 32, 2, timber);

            // Beacon Brazier with Fire on top
            FillRect(pixels, 36, 56, 14, 46, 8, 4, new Color32(45, 45, 50, 255));
            FillRect(pixels, 36, 56, 15, 50, 6, 5, fireGlow);
            FillRect(pixels, 36, 56, 16, 51, 4, 3, fireCore);
        }

        private static void PaintNightMonster(Color32[] pixels, int frame)
        {
            Color32 shadowDark = new Color32(18, 12, 24, 255);
            Color32 shadowMid = new Color32(42, 28, 56, 255);
            Color32 shadowAura = new Color32(75, 45, 95, 180);
            Color32 eyeGlow = new Color32(255, 35, 45, 255);
            Color32 eyeCore = new Color32(255, 220, 220, 255);

            int pulse = (frame % 2 == 0) ? 1 : -1;

            // Smokey shadowy aura
            FillRect(pixels, 22, 22, 2, 2, 18, 18, shadowAura);
            // Main shadow body
            FillRect(pixels, 22, 22, 4, 4, 14 + pulse, 14 - pulse, shadowDark);
            FillRect(pixels, 22, 22, 5, 5, 12, 12, shadowMid);

            // Horns / Spikes
            FillRect(pixels, 22, 22, 4, 17, 3, 4, shadowDark);
            FillRect(pixels, 22, 22, 15, 17, 3, 4, shadowDark);

            // Menacing Glowing Red Eyes
            FillRect(pixels, 22, 22, 6, 11, 3, 3, eyeGlow);
            SetPixel(pixels, 22, 22, 7, 12, eyeCore);
            FillRect(pixels, 22, 22, 13, 11, 3, 3, eyeGlow);
            SetPixel(pixels, 22, 22, 14, 12, eyeCore);

            // Smoky tendrils below
            FillRect(pixels, 22, 22, 5 + pulse, 0, 3, 4, shadowDark);
            FillRect(pixels, 22, 22, 10 - pulse, 0, 3, 4, shadowDark);
            FillRect(pixels, 22, 22, 14 + pulse, 0, 3, 4, shadowDark);
        }

        private static void PaintBellTowerRuins(Color32[] pixels)
        {
            Color32 stoneDark = new Color32(65, 68, 75, 255);
            Color32 stone = new Color32(110, 115, 125, 255);
            Color32 stoneLight = new Color32(155, 160, 170, 255);
            Color32 moss = new Color32(55, 115, 45, 255);
            Color32 bellBronze = new Color32(185, 145, 65, 255);

            // Foundation base
            FillRect(pixels, 64, 64, 4, 0, 56, 14, stoneDark);
            FillRect(pixels, 64, 64, 8, 2, 48, 10, stone);
            // Stone Pillars
            FillRect(pixels, 64, 64, 8, 14, 12, 38, stone);
            FillRect(pixels, 64, 64, 44, 14, 12, 38, stone);
            FillRect(pixels, 64, 64, 9, 14, 4, 36, stoneLight);
            FillRect(pixels, 64, 64, 45, 14, 4, 36, stoneLight);

            // Crumbling broken archway
            FillRect(pixels, 64, 64, 8, 50, 48, 6, stoneDark);
            FillRect(pixels, 64, 64, 10, 52, 20, 8, stone);
            // Bell belfry chamber with broken hanging bell
            FillRect(pixels, 64, 64, 26, 32, 12, 12, bellBronze);
            FillRect(pixels, 64, 64, 28, 30, 8, 4, new Color32(130, 95, 40, 255));

            // Overgrown Ivy & Moss
            FillRect(pixels, 64, 64, 14, 8, 8, 16, moss);
            FillRect(pixels, 64, 64, 42, 20, 6, 24, moss);
        }

        private static void PaintPuzzlePedestal(Color32[] pixels, int symbol, bool active)
        {
            Color32 stone = new Color32(95, 100, 110, 255);
            Color32 stoneLight = new Color32(145, 150, 160, 255);
            Color32 glowColor = active ? new Color32(80, 240, 220, 255) : new Color32(60, 70, 80, 255);

            FillRect(pixels, 22, 24, 2, 0, 18, 6, stone);
            FillRect(pixels, 22, 24, 5, 6, 12, 12, stone);
            FillRect(pixels, 22, 24, 2, 18, 18, 5, stoneLight);

            // Engraved Glyphs (0: Sun, 1: Moon, 2: Star)
            if (symbol == 0)
            {
                // Sun glyph
                FillRect(pixels, 22, 24, 8, 8, 6, 6, glowColor);
                SetPixel(pixels, 22, 24, 10, 10, stone);
                SetPixel(pixels, 22, 24, 11, 10, stone);
            }
            else if (symbol == 1)
            {
                // Moon crescent glyph
                FillRect(pixels, 22, 24, 8, 8, 6, 7, glowColor);
                FillRect(pixels, 22, 24, 11, 9, 3, 5, stone);
            }
            else
            {
                // Star glyph
                FillRect(pixels, 22, 24, 10, 8, 2, 8, glowColor);
                FillRect(pixels, 22, 24, 7, 11, 8, 2, glowColor);
            }
        }

        private static void PaintMerchantCart(Color32[] pixels)
        {
            Color32 wood = new Color32(110, 70, 40, 255);
            Color32 woodDark = new Color32(75, 45, 25, 255);
            Color32 clothRed = new Color32(205, 55, 60, 255);
            Color32 clothWhite = new Color32(235, 230, 215, 255);
            Color32 wheel = new Color32(50, 45, 40, 255);

            // Wooden wagon bed & cargo boxes
            FillRect(pixels, 52, 40, 6, 8, 40, 12, wood);
            FillRect(pixels, 52, 40, 8, 20, 10, 8, woodDark);
            FillRect(pixels, 52, 40, 20, 20, 12, 9, new Color32(145, 100, 55, 255));

            // Striped Canopy
            for (int c = 0; c < 6; c++)
            {
                FillRect(pixels, 52, 40, 4 + c * 7, 28, 7, 10, (c % 2 == 0) ? clothRed : clothWhite);
            }
            FillRect(pixels, 52, 40, 2, 26, 48, 3, clothRed);

            // Wagon wheels
            FillRect(pixels, 52, 40, 8, 0, 10, 10, wheel);
            FillRect(pixels, 52, 40, 10, 2, 6, 6, wood);
            FillRect(pixels, 52, 40, 34, 0, 10, 10, wheel);
            FillRect(pixels, 52, 40, 36, 2, 6, 6, wood);
        }

        private static void PaintItemSeed(Color32[] pixels, Color32 color)
        {
            Color32 pouch = new Color32(165, 135, 95, 255);
            Color32 tie = new Color32(85, 55, 30, 255);

            FillRect(pixels, 16, 16, 4, 1, 8, 9, pouch);
            FillRect(pixels, 16, 16, 5, 10, 6, 2, tie);
            FillRect(pixels, 16, 16, 4, 12, 8, 3, pouch);
            FillRect(pixels, 16, 16, 6, 4, 4, 4, color);
        }

        private static void PaintItemWheat(Color32[] pixels)
        {
            Color32 stalk = new Color32(195, 165, 45, 255);
            Color32 grain = new Color32(245, 215, 75, 255);

            FillRect(pixels, 16, 16, 7, 1, 2, 8, stalk);
            FillRect(pixels, 16, 16, 5, 8, 6, 6, grain);
            FillRect(pixels, 16, 16, 6, 13, 4, 2, grain);
        }

        private static void PaintItemCorn(Color32[] pixels)
        {
            Color32 yellow = new Color32(245, 215, 35, 255);
            Color32 husk = new Color32(95, 175, 55, 255);

            FillRect(pixels, 16, 16, 5, 2, 6, 11, yellow);
            FillRect(pixels, 16, 16, 4, 1, 4, 7, husk);
            FillRect(pixels, 16, 16, 8, 1, 4, 7, husk);
        }

        private static void PaintItemCarrot(Color32[] pixels)
        {
            Color32 orange = new Color32(245, 110, 25, 255);
            Color32 leaf = new Color32(85, 195, 55, 255);

            FillRect(pixels, 16, 16, 6, 1, 4, 8, orange);
            FillRect(pixels, 16, 16, 7, 0, 2, 3, orange);
            FillRect(pixels, 16, 16, 5, 9, 6, 2, orange);
            FillRect(pixels, 16, 16, 4, 11, 8, 4, leaf);
        }

        private static void PaintItemWateringCan(Color32[] pixels)
        {
            Color32 metal = new Color32(75, 145, 215, 255);
            Color32 metalLight = new Color32(145, 205, 255, 255);

            FillRect(pixels, 16, 16, 3, 2, 8, 8, metal);
            FillRect(pixels, 16, 16, 4, 9, 6, 1, metalLight);
            // Spout
            FillRect(pixels, 16, 16, 11, 6, 3, 2, metal);
            FillRect(pixels, 16, 16, 13, 8, 2, 3, metalLight);
            // Handle
            FillRect(pixels, 16, 16, 1, 4, 2, 6, metalLight);
            FillRect(pixels, 16, 16, 2, 10, 4, 2, metalLight);
        }

        private static void PaintItemFenceWood(Color32[] pixels)
        {
            Color32 wood = new Color32(115, 78, 44, 255);
            FillRect(pixels, 16, 16, 3, 1, 3, 14, wood);
            FillRect(pixels, 16, 16, 10, 1, 3, 14, wood);
            FillRect(pixels, 16, 16, 1, 10, 14, 2, wood);
            FillRect(pixels, 16, 16, 1, 4, 14, 2, wood);
        }

        private static void PaintItemGateWood(Color32[] pixels)
        {
            Color32 wood = new Color32(130, 90, 50, 255);
            Color32 iron = new Color32(40, 40, 45, 255);
            FillRect(pixels, 16, 16, 2, 1, 12, 13, wood);
            FillRect(pixels, 16, 16, 4, 3, 8, 9, new Color32(85, 55, 30, 255));
            FillRect(pixels, 16, 16, 1, 10, 3, 2, iron);
            FillRect(pixels, 16, 16, 1, 4, 3, 2, iron);
        }

        private static void PaintItemPineapple(Color32[] pixels)
        {
            Color32 gold = new Color32(245, 185, 35, 255);
            Color32 dark = new Color32(185, 125, 20, 255);
            Color32 green = new Color32(80, 185, 45, 255);

            FillRect(pixels, 16, 16, 4, 1, 8, 9, dark);
            FillRect(pixels, 16, 16, 5, 2, 6, 7, gold);
            SetPixel(pixels, 16, 16, 6, 6, dark);
            SetPixel(pixels, 16, 16, 8, 6, dark);
            SetPixel(pixels, 16, 16, 7, 4, dark);

            // Spiky crown
            FillRect(pixels, 16, 16, 6, 10, 4, 5, green);
            FillRect(pixels, 16, 16, 4, 11, 3, 3, green);
            FillRect(pixels, 16, 16, 9, 11, 3, 3, green);
        }

        private static void PaintItemTomato(Color32[] pixels)
        {
            Color32 red = new Color32(235, 40, 25, 255);
            Color32 darkRed = new Color32(170, 20, 15, 255);
            Color32 calyx = new Color32(75, 180, 45, 255);

            FillRect(pixels, 16, 16, 4, 2, 8, 8, darkRed);
            FillRect(pixels, 16, 16, 5, 3, 6, 6, red);
            SetPixel(pixels, 16, 16, 5, 7, Color.white);

            // Star calyx
            FillRect(pixels, 16, 16, 6, 10, 4, 2, calyx);
            SetPixel(pixels, 16, 16, 5, 12, calyx);
            SetPixel(pixels, 16, 16, 8, 13, calyx);
            SetPixel(pixels, 16, 16, 10, 12, calyx);
        }

        private static void PaintHappyFarmBarn(Color32[] pixels)
        {
            Color32 tinLight = new Color32(220, 228, 235, 255);
            Color32 tinMid = new Color32(175, 185, 195, 255);
            Color32 tinDark = new Color32(130, 140, 150, 255);
            Color32 concrete = new Color32(180, 185, 180, 255);
            Color32 concreteShadow = new Color32(135, 140, 135, 255);
            Color32 hayGold = new Color32(235, 185, 45, 255);
            Color32 hayDark = new Color32(185, 135, 30, 255);
            Color32 woodFence = new Color32(150, 95, 45, 255);
            Color32 woodLight = new Color32(195, 135, 75, 255);
            Color32 interiorDark = new Color32(35, 32, 38, 255);
            Color32 arrowYellow = new Color32(250, 220, 35, 255);

            // Foundation base slab
            FillRect(pixels, 68, 52, 4, 0, 60, 5, concreteShadow);
            FillRect(pixels, 68, 52, 5, 2, 58, 3, concrete);

            // Concrete Support Pillars
            FillRect(pixels, 68, 52, 5, 5, 5, 28, concrete);
            FillRect(pixels, 68, 52, 32, 5, 5, 28, concrete);
            FillRect(pixels, 68, 52, 58, 5, 5, 28, concrete);

            // Left Compartment: Giant Haystack
            FillRect(pixels, 68, 52, 10, 5, 22, 26, hayDark);
            FillRect(pixels, 68, 52, 12, 7, 18, 22, hayGold);
            for (int i = 0; i < 5; i++)
            {
                SetPixel(pixels, 68, 52, 13 + i * 3, 15 + (i % 3) * 4, Color.white);
                SetPixel(pixels, 68, 52, 14 + i * 3, 10 + (i % 2) * 5, new Color32(255, 235, 130, 255));
            }
            // Low protective wooden fence in front of haystack
            FillRect(pixels, 68, 52, 9, 6, 24, 3, woodFence);
            FillRect(pixels, 68, 52, 9, 12, 24, 3, woodFence);
            FillRect(pixels, 68, 52, 11, 5, 3, 12, woodLight);
            FillRect(pixels, 68, 52, 19, 5, 3, 12, woodLight);
            FillRect(pixels, 68, 52, 27, 5, 3, 12, woodLight);

            // Right Compartment: Open Tool Shed Interior
            FillRect(pixels, 68, 52, 37, 5, 21, 28, interiorDark);
            for (int h = 0; h < 18; h++)
            {
                SetPixel(pixels, 68, 52, 55 - (h / 4), 6 + h, new Color32(185, 125, 60, 255));
            }
            FillRect(pixels, 68, 52, 50, 22, 5, 4, new Color32(130, 85, 40, 255));

            // Top Header Beam
            FillRect(pixels, 68, 52, 4, 32, 60, 4, concrete);

            // Corrugated Tin Roof
            for (int y = 0; y < 16; y++)
            {
                for (int x = 2; x < 66; x++)
                {
                    bool isRidge = (x % 3 == 0);
                    bool isShadow = (x % 3 == 1);
                    Color32 tinCol = isRidge ? tinLight : (isShadow ? tinDark : tinMid);
                    SetPixel(pixels, 68, 52, x, 36 + y, tinCol);
                }
            }
            FillRect(pixels, 68, 52, 1, 35, 66, 3, new Color32(195, 205, 215, 255));
            FillRect(pixels, 68, 52, 1, 34, 66, 1, tinDark);

            // "VÀO" Interaction Marker Badge & Yellow Arrow
            FillRect(pixels, 68, 52, 41, 18, 14, 8, new Color32(20, 20, 24, 230));
            FillRect(pixels, 68, 52, 45, 9, 6, 5, arrowYellow);
            FillRect(pixels, 68, 52, 46, 6, 4, 3, arrowYellow);
            FillRect(pixels, 68, 52, 47, 4, 2, 2, arrowYellow);
        }

        private static void PaintDairyCow(Color32[] pixels)
        {
            Color32 white = new Color32(245, 245, 245, 255);
            Color32 blackSpot = new Color32(40, 38, 42, 255);
            Color32 pinkMuzzle = new Color32(245, 175, 185, 255);
            Color32 horn = new Color32(215, 185, 125, 255);
            Color32 hoof = new Color32(75, 60, 48, 255);

            // Body
            FillRect(pixels, 28, 20, 4, 4, 18, 10, white);
            FillRect(pixels, 28, 20, 7, 8, 6, 6, blackSpot);
            FillRect(pixels, 28, 20, 16, 6, 5, 5, blackSpot);
            FillRect(pixels, 28, 20, 10, 4, 4, 3, blackSpot);

            // Legs
            FillRect(pixels, 28, 20, 5, 0, 3, 4, white);
            FillRect(pixels, 28, 20, 5, 0, 3, 1, hoof);
            FillRect(pixels, 28, 20, 9, 0, 3, 4, blackSpot);
            FillRect(pixels, 28, 20, 9, 0, 3, 1, hoof);
            FillRect(pixels, 28, 20, 15, 0, 3, 4, white);
            FillRect(pixels, 28, 20, 15, 0, 3, 1, hoof);
            FillRect(pixels, 28, 20, 19, 0, 3, 4, blackSpot);
            FillRect(pixels, 28, 20, 19, 0, 3, 1, hoof);

            // Head
            FillRect(pixels, 28, 20, 1, 8, 7, 8, white);
            FillRect(pixels, 28, 20, 3, 12, 3, 3, blackSpot);
            FillRect(pixels, 28, 20, 0, 8, 4, 4, pinkMuzzle);
            SetPixel(pixels, 28, 20, 1, 9, Color.black);
            SetPixel(pixels, 28, 20, 4, 12, Color.black);

            // Horns & Ears
            SetPixel(pixels, 28, 20, 5, 17, horn);
            SetPixel(pixels, 28, 20, 6, 18, horn);
            FillRect(pixels, 28, 20, 7, 14, 2, 2, pinkMuzzle);

            // Tail
            SetPixel(pixels, 28, 20, 22, 11, white);
            SetPixel(pixels, 28, 20, 23, 9, white);
            FillRect(pixels, 28, 20, 23, 7, 2, 2, blackSpot);
        }

        private static void PaintStrawNest(Color32[] pixels, bool hasEgg)
        {
            Color32 straw = new Color32(225, 175, 55, 255);
            Color32 strawDark = new Color32(165, 120, 35, 255);
            Color32 eggWhite = new Color32(250, 250, 245, 255);
            Color32 eggShadow = new Color32(215, 215, 205, 255);

            FillRect(pixels, 22, 16, 2, 1, 18, 12, strawDark);
            FillRect(pixels, 22, 16, 3, 2, 16, 10, straw);
            FillRect(pixels, 22, 16, 6, 4, 10, 6, strawDark);

            if (hasEgg)
            {
                FillRect(pixels, 22, 16, 7, 5, 4, 5, eggShadow);
                FillRect(pixels, 22, 16, 8, 6, 3, 4, eggWhite);
                FillRect(pixels, 22, 16, 11, 4, 4, 5, eggShadow);
                FillRect(pixels, 22, 16, 12, 5, 3, 4, eggWhite);
            }
        }

        private static void PaintFarmDog(Color32[] pixels)
        {
            Color32 fur = new Color32(215, 160, 80, 255);
            Color32 furDark = new Color32(165, 110, 45, 255);
            Color32 white = new Color32(245, 245, 245, 255);

            FillRect(pixels, 18, 16, 4, 3, 9, 6, fur);
            FillRect(pixels, 18, 16, 6, 3, 5, 3, white);

            FillRect(pixels, 18, 16, 4, 0, 2, 3, furDark);
            FillRect(pixels, 18, 16, 11, 0, 2, 3, furDark);

            FillRect(pixels, 18, 16, 1, 6, 6, 6, fur);
            SetPixel(pixels, 18, 16, 1, 8, Color.black);
            SetPixel(pixels, 18, 16, 4, 9, Color.black);
            FillRect(pixels, 18, 16, 5, 10, 2, 3, furDark);

            SetPixel(pixels, 18, 16, 13, 8, fur);
            SetPixel(pixels, 18, 16, 14, 10, fur);
            SetPixel(pixels, 18, 16, 15, 11, fur);
        }

        private static void PaintFarmShopSign(Color32[] pixels)
        {
            Color32 wood = new Color32(185, 125, 60, 255);
            Color32 woodDark = new Color32(125, 75, 30, 255);
            Color32 leaf = new Color32(95, 195, 55, 255);
            Color32 textGold = new Color32(255, 225, 90, 255);

            FillRect(pixels, 18, 22, 8, 0, 2, 10, woodDark);
            FillRect(pixels, 18, 22, 1, 10, 16, 10, woodDark);
            FillRect(pixels, 18, 22, 2, 11, 14, 8, wood);

            // M
            FillRect(pixels, 18, 22, 4, 13, 1, 4, textGold);
            SetPixel(pixels, 18, 22, 5, 15, textGold);
            FillRect(pixels, 18, 22, 6, 13, 1, 4, textGold);
            // U
            FillRect(pixels, 18, 22, 8, 13, 1, 4, textGold);
            FillRect(pixels, 18, 22, 10, 13, 1, 4, textGold);
            SetPixel(pixels, 18, 22, 9, 13, textGold);
            // A
            FillRect(pixels, 18, 22, 12, 13, 1, 4, textGold);
            FillRect(pixels, 18, 22, 14, 13, 1, 4, textGold);
            SetPixel(pixels, 18, 22, 13, 16, textGold);
            SetPixel(pixels, 18, 22, 13, 14, textGold);

            SetPixel(pixels, 18, 22, 9, 20, leaf);
            SetPixel(pixels, 18, 22, 10, 21, leaf);
        }

        private static void PaintWoodFenceVertical(Color32[] pixels)
        {
            Color32 woodDark = new Color32(130, 80, 40, 255);
            Color32 woodMid = new Color32(185, 125, 65, 255);
            Color32 woodLight = new Color32(225, 165, 95, 255);
            Color32 iron = new Color32(45, 40, 35, 255);

            // Vertical 2.5D fence post (16x24) matching reference image
            FillRect(pixels, 16, 24, 6, 2, 4, 20, woodDark);
            FillRect(pixels, 16, 24, 7, 3, 2, 18, woodMid);
            FillRect(pixels, 16, 24, 7, 21, 2, 2, woodLight);

            // Left connecting side-rails
            FillRect(pixels, 16, 24, 0, 16, 6, 3, woodDark);
            FillRect(pixels, 16, 24, 0, 17, 6, 1, woodMid);
            FillRect(pixels, 16, 24, 0, 8, 6, 3, woodDark);
            FillRect(pixels, 16, 24, 0, 9, 6, 1, woodMid);

            // Right connecting side-rails
            FillRect(pixels, 16, 24, 10, 16, 6, 3, woodDark);
            FillRect(pixels, 16, 24, 10, 17, 6, 1, woodMid);
            FillRect(pixels, 16, 24, 10, 8, 6, 3, woodDark);
            FillRect(pixels, 16, 24, 10, 9, 6, 1, woodMid);

            // Iron nail heads
            SetPixel(pixels, 16, 24, 8, 17, iron);
            SetPixel(pixels, 16, 24, 8, 9, iron);
        }

        private static void PaintWoodFenceCorner(Color32[] pixels)
        {
            Color32 woodDark = new Color32(130, 80, 40, 255);
            Color32 woodMid = new Color32(185, 125, 65, 255);
            Color32 woodLight = new Color32(225, 165, 95, 255);
            Color32 iron = new Color32(45, 40, 35, 255);

            // Corner post (16x24)
            FillRect(pixels, 16, 24, 5, 2, 6, 20, woodDark);
            FillRect(pixels, 16, 24, 6, 3, 4, 18, woodMid);
            FillRect(pixels, 16, 24, 6, 21, 4, 2, woodLight);

            // Side rail mortise
            FillRect(pixels, 16, 24, 0, 16, 5, 3, woodDark);
            FillRect(pixels, 16, 24, 0, 17, 5, 1, woodMid);
            FillRect(pixels, 16, 24, 0, 8, 5, 3, woodDark);
            FillRect(pixels, 16, 24, 0, 9, 5, 1, woodMid);

            SetPixel(pixels, 16, 24, 7, 17, iron);
            SetPixel(pixels, 16, 24, 7, 9, iron);
        }

        private static void PaintGateLantern(Color32[] pixels, bool isLit)
        {
            Color32 woodDark = new Color32(110, 70, 35, 255);
            Color32 wood = new Color32(165, 110, 55, 255);
            Color32 iron = new Color32(45, 42, 38, 255);
            Color32 glass = new Color32(180, 215, 230, 200);
            Color32 flame = isLit ? new Color32(255, 220, 80, 255) : new Color32(120, 100, 70, 255);
            Color32 glow = new Color32(255, 180, 40, 160);

            // Post
            FillRect(pixels, 16, 24, 6, 0, 4, 18, woodDark);
            FillRect(pixels, 16, 24, 7, 1, 2, 17, wood);

            // Iron Arm
            FillRect(pixels, 16, 24, 7, 18, 5, 2, iron);
            FillRect(pixels, 16, 24, 11, 14, 2, 4, iron);

            // Lantern Body
            FillRect(pixels, 16, 24, 9, 8, 6, 7, iron);
            FillRect(pixels, 16, 24, 10, 9, 4, 5, glass);
            FillRect(pixels, 16, 24, 11, 10, 2, 3, flame);

            if (isLit)
            {
                SetPixel(pixels, 16, 24, 11, 14, glow);
                SetPixel(pixels, 16, 24, 8, 11, glow);
                SetPixel(pixels, 16, 24, 15, 11, glow);
            }
        }

        private static void PaintFarmSignboard(Color32[] pixels)
        {
            Color32 woodDark = new Color32(110, 70, 35, 255);
            Color32 wood = new Color32(170, 115, 60, 255);
            Color32 parchment = new Color32(235, 215, 175, 255);
            Color32 parchmentDark = new Color32(195, 170, 130, 255);
            Color32 leaf = new Color32(85, 180, 50, 255);

            // Posts
            FillRect(pixels, 20, 24, 4, 0, 3, 14, woodDark);
            FillRect(pixels, 20, 24, 13, 0, 3, 14, woodDark);

            // Plaque Board
            FillRect(pixels, 20, 24, 1, 10, 18, 12, woodDark);
            FillRect(pixels, 20, 24, 2, 11, 16, 10, wood);
            FillRect(pixels, 20, 24, 4, 13, 12, 6, parchment);
            DrawRect(pixels, 20, 24, 4, 13, 12, 6, parchmentDark);

            // Leaf insignia
            FillRect(pixels, 20, 24, 9, 15, 2, 2, leaf);
            SetPixel(pixels, 20, 24, 10, 17, leaf);
        }

        private static void PaintPathDirt(Color32[] pixels)
        {
            Color32 dirtA = new Color32(145, 102, 58, 255);
            Color32 dirtB = new Color32(165, 118, 68, 255);
            Color32 dirtC = new Color32(128, 88, 48, 255);
            Color32 pebble = new Color32(185, 160, 130, 255);

            for (int y = 0; y < 16; y++)
            {
                for (int x = 0; x < 16; x++)
                {
                    int pattern = (x * 7 + y * 13) % 5;
                    Color32 c = pattern == 0 ? dirtC : (pattern == 1 ? dirtB : dirtA);
                    pixels[y * 16 + x] = c;
                }
            }

            // Scatter pebbles
            pixels[3 * 16 + 4] = pebble;
            pixels[11 * 16 + 12] = pebble;
            pixels[7 * 16 + 9] = pebble;
        }

        private static void PaintPathCobblestone(Color32[] pixels)
        {
            Color32 mortar = new Color32(75, 70, 60, 255);
            Color32 stoneA = new Color32(145, 140, 130, 255);
            Color32 stoneB = new Color32(170, 165, 155, 255);
            Color32 stoneDark = new Color32(105, 100, 95, 255);

            for (int i = 0; i < pixels.Length; i++) pixels[i] = mortar;

            // Stone 1
            FillRect(pixels, 16, 16, 1, 1, 6, 6, stoneDark);
            FillRect(pixels, 16, 16, 2, 2, 4, 4, stoneA);
            pixels[4 * 16 + 4] = stoneB;

            // Stone 2
            FillRect(pixels, 16, 16, 8, 1, 7, 5, stoneDark);
            FillRect(pixels, 16, 16, 9, 2, 5, 3, stoneA);

            // Stone 3
            FillRect(pixels, 16, 16, 1, 8, 7, 7, stoneDark);
            FillRect(pixels, 16, 16, 2, 9, 5, 5, stoneA);
            pixels[11 * 16 + 4] = stoneB;

            // Stone 4
            FillRect(pixels, 16, 16, 9, 7, 6, 8, stoneDark);
            FillRect(pixels, 16, 16, 10, 8, 4, 6, stoneA);
            pixels[11 * 16 + 12] = stoneB;
        }

        private static void PaintScarecrow(Color32[] pixels)
        {
            Color32 wood = new Color32(110, 70, 35, 255);
            Color32 straw = new Color32(235, 205, 100, 255);
            Color32 coat = new Color32(65, 115, 175, 255);
            Color32 hat = new Color32(195, 150, 65, 255);
            Color32 patch = new Color32(185, 60, 45, 255);
            Color32 eyes = new Color32(35, 30, 25, 255);

            // Wooden Pole
            FillRect(pixels, 24, 32, 11, 0, 2, 30, wood);
            FillRect(pixels, 24, 32, 4, 18, 16, 2, wood); // Cross arm

            // Blue Coat Body
            FillRect(pixels, 24, 32, 8, 10, 8, 10, coat);
            FillRect(pixels, 24, 32, 5, 17, 14, 4, coat); // Coat sleeves
            FillRect(pixels, 24, 32, 12, 12, 3, 3, patch); // Red patch

            // Straw poking from sleeves & bottom
            FillRect(pixels, 24, 32, 3, 17, 2, 3, straw);
            FillRect(pixels, 24, 32, 19, 17, 2, 3, straw);
            FillRect(pixels, 24, 32, 9, 7, 6, 3, straw);

            // Head (Burlap/Straw)
            FillRect(pixels, 24, 32, 9, 21, 6, 6, straw);
            SetPixel(pixels, 24, 32, 10, 24, eyes);
            SetPixel(pixels, 24, 32, 13, 24, eyes);

            // Straw Hat
            FillRect(pixels, 24, 32, 6, 26, 12, 2, hat); // Brim
            FillRect(pixels, 24, 32, 8, 28, 8, 3, hat);  // Crown
        }

        private static void PaintSelectionTileHighlight(Color32[] pixels)
        {
            Color32 fill = new Color32(70, 160, 245, 125);
            Color32 border = new Color32(120, 215, 255, 240);

            for (int i = 0; i < pixels.Length; i++) pixels[i] = fill;

            // 1px bright border
            for (int x = 0; x < 16; x++)
            {
                pixels[0 * 16 + x] = border;
                pixels[15 * 16 + x] = border;
            }
            for (int y = 0; y < 16; y++)
            {
                pixels[y * 16 + 0] = border;
                pixels[y * 16 + 15] = border;
            }
        }

        private static void PaintTitleKnightSunsetPanorama(Color32[] pixels)
        {
            const int w = 512;
            const int h = 216;

            // Palette definitions matching reference image
            Color32 skyTop = new Color32(28, 22, 45, 255);      // Deep twilight indigo
            Color32 skyMidHigh = new Color32(65, 30, 72, 255);  // Twilight plum
            Color32 skyMid = new Color32(148, 55, 62, 255);     // Crimson sunset
            Color32 skyMidLow = new Color32(215, 95, 35, 255);   // Burning amber orange
            Color32 skyHorizon = new Color32(248, 175, 55, 255); // Radiant sunset gold
            Color32 sunCore = new Color32(255, 252, 235, 255);   // Blazing white-gold sun
            Color32 sunGlow = new Color32(252, 218, 110, 255);   // Golden aureole

            Color32 cloudDark = new Color32(75, 42, 70, 255);   // Cloud base shadow
            Color32 cloudMid = new Color32(138, 72, 85, 255);   // Cloud midtone
            Color32 cloudLit = new Color32(235, 140, 52, 255);  // Golden-orange rim light
            Color32 cloudBright = new Color32(250, 205, 105, 255);// Sun-struck cloud edge

            Color32 mtnFar = new Color32(78, 58, 85, 255);      // Distant mountain haze
            Color32 mtnMid = new Color32(52, 42, 60, 255);      // Mid mountain ridge
            Color32 mtnCastle = new Color32(40, 34, 46, 255);   // Castle silhouette base
            Color32 castleStone = new Color32(65, 55, 72, 255); // Castle walls
            Color32 castleLight = new Color32(120, 95, 90, 255);// Castle sunlit edges
            Color32 torchWindow = new Color32(255, 215, 90, 255);// Lit windows

            Color32 riverDeep = new Color32(110, 55, 45, 255);  // River water body
            Color32 riverReflect = new Color32(245, 160, 55, 255);// Sunset shimmer on river
            Color32 riverBright = new Color32(255, 220, 120, 255);// Radiant river reflection

            Color32 pineDark = new Color32(20, 32, 28, 255);    // Pine forest silhouette
            Color32 pineMid = new Color32(35, 55, 42, 255);     // Forest canopy
            Color32 pineSun = new Color32(82, 78, 42, 255);     // Sunset rim on pine needles

            Color32 hillFar = new Color32(48, 52, 38, 255);     // Rolling hills
            Color32 grassDark = new Color32(32, 40, 26, 255);   // Foreground hill shadow
            Color32 grassMid = new Color32(64, 78, 42, 255);    // Foreground turf
            Color32 grassGold = new Color32(145, 130, 55, 255); // Sun-drenched grass blades

            Color32 oakBarkDark = new Color32(22, 18, 16, 255); // Ancient oak shadow
            Color32 oakBarkMid = new Color32(46, 36, 30, 255);  // Oak trunk fissures
            Color32 oakBarkRim = new Color32(112, 75, 42, 255); // Warm sunset rim on bark
            Color32 oakFoliage = new Color32(26, 36, 24, 255);  // Oak leaves dark
            Color32 oakFoliageMid = new Color32(48, 62, 34, 255);// Oak leaves mid
            Color32 oakFoliageLit = new Color32(105, 98, 46, 255);// Oak leaves sunset glow

            Color32 steelDark = new Color32(35, 38, 48, 255);
            Color32 steelMid = new Color32(115, 125, 142, 255);
            Color32 steelLit = new Color32(205, 218, 235, 255);
            Color32 steelGold = new Color32(225, 185, 120, 255);// Golden sunset reflection on armor
            Color32 knightScarf = new Color32(165, 32, 35, 255);
            Color32 knightScarfLit = new Color32(215, 65, 55, 255);
            Color32 goldLion = new Color32(235, 185, 50, 255);

            // --- 1. Sky Gradient ---
            for (int y = 0; y < h; y++)
            {
                float normY = (float)y / h;
                Color32 c;
                if (normY > 0.75f)
                {
                    float t = (normY - 0.75f) / 0.25f;
                    c = Color32.Lerp(skyMidHigh, skyTop, t);
                }
                else if (normY > 0.55f)
                {
                    float t = (normY - 0.55f) / 0.20f;
                    c = Color32.Lerp(skyMid, skyMidHigh, t);
                }
                else if (normY > 0.40f)
                {
                    float t = (normY - 0.40f) / 0.15f;
                    c = Color32.Lerp(skyMidLow, skyMid, t);
                }
                else
                {
                    float t = normY / 0.40f;
                    c = Color32.Lerp(skyHorizon, skyMidLow, t);
                }

                for (int x = 0; x < w; x++) pixels[y * w + x] = c;
            }

            // --- 2. Sun & Radiant Aureole ---
            int sunX = 390;
            int sunY = 148;
            int sunRadius = 13;
            int glowRadius = 52;

            for (int gy = -glowRadius; gy <= glowRadius; gy++)
            {
                int py = sunY + gy;
                if (py < 0 || py >= h) continue;

                for (int gx = -glowRadius; gx <= glowRadius; gx++)
                {
                    int px = sunX + gx;
                    if (px < 0 || px >= w) continue;

                    float dist = Mathf.Sqrt(gx * gx + gy * gy);
                    if (dist <= sunRadius)
                    {
                        pixels[py * w + px] = dist <= sunRadius * 0.7f ? sunCore : sunGlow;
                    }
                    else if (dist <= glowRadius)
                    {
                        float alpha = 1f - (dist - sunRadius) / (glowRadius - sunRadius);
                        alpha = alpha * alpha; // Smooth falloff
                        Color32 cur = pixels[py * w + px];
                        pixels[py * w + px] = Color32.Lerp(cur, sunGlow, alpha * 0.72f);
                    }
                }
            }

            // --- 3. Sunset Clouds ---
            // High left clouds
            PaintCloudCluster(pixels, w, h, 60, 185, 90, 16, cloudDark, cloudMid, cloudLit, cloudBright);
            PaintCloudCluster(pixels, w, h, 175, 175, 120, 18, cloudDark, cloudMid, cloudLit, cloudBright);
            PaintCloudCluster(pixels, w, h, 290, 188, 85, 14, cloudDark, cloudMid, cloudLit, cloudBright);
            // Clouds framing sun
            PaintCloudCluster(pixels, w, h, 340, 162, 70, 12, cloudDark, cloudMid, cloudLit, cloudBright);
            PaintCloudCluster(pixels, w, h, 440, 165, 65, 14, cloudDark, cloudMid, cloudLit, cloudBright);
            PaintCloudCluster(pixels, w, h, 385, 192, 110, 18, cloudDark, cloudMid, cloudLit, cloudBright);

            // Silhouetted birds in flight
            PaintBirdSilhouette(pixels, w, h, 395, 188);
            PaintBirdSilhouette(pixels, w, h, 408, 194);
            PaintBirdSilhouette(pixels, w, h, 418, 190);

            // --- 4. Distant Mountain Ranges ---
            for (int x = 0; x < w; x++)
            {
                // Far peaks
                float peak1 = Mathf.Sin(x * 0.015f) * 18f + Mathf.Cos(x * 0.038f) * 12f + 118f;
                if (x > 320 && x < 480) peak1 += 22f; // Peak under castle
                for (int y = 0; y < (int)peak1 && y < h; y++)
                {
                    pixels[y * w + x] = mtnFar;
                }

                // Mid ridges
                float peak2 = Mathf.Sin(x * 0.022f + 1.2f) * 14f + Mathf.Cos(x * 0.018f) * 8f + 104f;
                for (int y = 0; y < (int)peak2 && y < h; y++)
                {
                    pixels[y * w + x] = mtnMid;
                }
            }

            // --- 5. Grand Mountain Fortress / Castle (Right) ---
            int cX = 390;
            int cY = 126;
            // Mountain crag under castle
            FillRect(pixels, w, h, cX - 55, 90, 115, 38, mtnCastle);
            for (int i = 0; i < 45; i++)
            {
                SetPixel(pixels, w, h, cX - 50 + i * 2, 90 + i / 2, castleStone);
            }

            // Castle main keep & curtain walls
            FillRect(pixels, w, h, cX - 35, cY, 80, 26, castleStone);
            FillRect(pixels, w, h, cX - 30, cY + 26, 68, 8, castleStone);
            // Spires & Turrets
            FillRect(pixels, w, h, cX + 18, cY + 20, 10, 32, castleStone); // Tallest central spire
            FillRect(pixels, w, h, cX + 20, cY + 52, 6, 12, castleStone);  // Spire cone
            SetPixel(pixels, w, h, cX + 22, cY + 65, castleLight);         // Spire tip & flag
            SetPixel(pixels, w, h, cX + 23, cY + 64, knightScarfLit);

            FillRect(pixels, w, h, cX - 18, cY + 18, 8, 24, castleStone);  // Left tower
            FillRect(pixels, w, h, cX - 17, cY + 42, 6, 8, castleStone);
            FillRect(pixels, w, h, cX + 42, cY + 14, 8, 22, castleStone);  // Right tower
            FillRect(pixels, w, h, cX + 43, cY + 36, 6, 8, castleStone);
            FillRect(pixels, w, h, cX - 42, cY + 6, 10, 20, castleStone);  // Outer watchtower
            FillRect(pixels, w, h, cX - 41, cY + 26, 8, 8, castleStone);

            // Crenelations (Battlements)
            for (int bx = cX - 35; bx < cX + 45; bx += 4)
            {
                FillRect(pixels, w, h, bx, cY + 34, 2, 3, castleStone);
            }

            // Sunlit edges on castle
            for (int cy = cY; cy < cY + 50; cy++)
            {
                SetPixel(pixels, w, h, cX + 18, cy, castleLight);
                SetPixel(pixels, w, h, cX + 42, cy, castleLight);
            }
            // Lit castle windows (glowing torches)
            SetPixel(pixels, w, h, cX + 22, cY + 35, torchWindow);
            SetPixel(pixels, w, h, cX + 22, cY + 25, torchWindow);
            SetPixel(pixels, w, h, cX - 14, cY + 28, torchWindow);
            SetPixel(pixels, w, h, cX + 45, cY + 22, torchWindow);
            SetPixel(pixels, w, h, cX, cY + 14, torchWindow);

            // --- 6. Rolling Hills, Pine Forests & Valley ---
            for (int x = 0; x < w; x++)
            {
                float valleyH = Mathf.Sin(x * 0.012f + 0.4f) * 16f + 78f;
                for (int y = 0; y < (int)valleyH && y < h; y++)
                {
                    pixels[y * w + x] = hillFar;
                }
            }

            // Draw dense pine forest spires across valley
            System.Random prng = new System.Random(88421);
            for (int x = 120; x < w; x += 6)
            {
                int baseY = (int)(Mathf.Sin(x * 0.012f + 0.4f) * 16f + 68f + prng.Next(-4, 5));
                int treeH = prng.Next(10, 22);
                PaintPineTreeSilhouette(pixels, w, h, x, baseY, treeH, pineDark, pineMid, pineSun);
            }

            // Tiny village rooftops nestled in trees
            FillRect(pixels, w, h, 290, 68, 8, 5, new Color32(75, 45, 35, 255));
            FillRect(pixels, w, h, 305, 65, 10, 6, new Color32(65, 40, 30, 255));
            FillRect(pixels, w, h, 322, 70, 7, 5, new Color32(80, 48, 38, 255));
            SetPixel(pixels, w, h, 292, 74, new Color32(200, 200, 200, 160)); // Smoke puffs
            SetPixel(pixels, w, h, 293, 76, new Color32(200, 200, 200, 110));

            // --- 7. Winding Sparkling River ---
            // S-curve from (350, 85) down to (220, 20)
            int[,] riverPoints = {
                { 355, 92, 4 }, { 340, 86, 5 }, { 318, 80, 7 }, { 295, 74, 9 },
                { 275, 68, 12 }, { 260, 60, 14 }, { 252, 50, 18 }, { 248, 40, 22 },
                { 245, 30, 26 }, { 240, 18, 32 }, { 235, 5, 38 }
            };

            for (int i = 0; i < riverPoints.GetLength(0) - 1; i++)
            {
                int x1 = riverPoints[i, 0];
                int y1 = riverPoints[i, 1];
                int r1 = riverPoints[i, 2];
                int x2 = riverPoints[i + 1, 0];
                int y2 = riverPoints[i + 1, 1];
                int r2 = riverPoints[i + 1, 2];

                int steps = Mathf.Max(Mathf.Abs(x2 - x1), Mathf.Abs(y2 - y1)) * 2;
                for (int s = 0; s <= steps; s++)
                {
                    float t = (float)s / Mathf.Max(1, steps);
                    int rx = (int)Mathf.Lerp(x1, x2, t);
                    int ry = (int)Mathf.Lerp(y1, y2, t);
                    int rw = (int)Mathf.Lerp(r1, r2, t);

                    for (int ox = -rw / 2; ox <= rw / 2; ox++)
                    {
                        int px = rx + ox;
                        int py = ry;
                        if (px < 0 || px >= w || py < 0 || py >= h) continue;

                        float centerFactor = 1f - Mathf.Abs(ox) / (rw * 0.5f + 1f);
                        if (centerFactor > 0.6f) pixels[py * w + px] = riverBright;
                        else if (centerFactor > 0.3f) pixels[py * w + px] = riverReflect;
                        else pixels[py * w + px] = riverDeep;
                    }
                }
            }

            // --- 8. Foreground Grassy Hillside (Rising to the Left) ---
            for (int x = 0; x < w; x++)
            {
                // Ridge rises steeply toward left where tree stands
                float hillSlope = (1f - (float)x / w);
                float hillH = 22f + hillSlope * hillSlope * 78f + Mathf.Sin(x * 0.08f) * 2.5f;

                for (int y = 0; y < (int)hillH && y < h; y++)
                {
                    float depth = y / hillH;
                    if (y >= (int)hillH - 2)
                    {
                        // Sunlit grass edge
                        pixels[y * w + x] = (x > 180 && prng.Next(0, 10) > 3) ? grassGold : grassMid;
                    }
                    else if (y >= (int)hillH - 6)
                    {
                        pixels[y * w + x] = grassMid;
                    }
                    else
                    {
                        pixels[y * w + x] = grassDark;
                    }
                }

                // Scatter grass blades & wildflowers on foreground hill
                if (x > 160 && (int)hillH < h - 4)
                {
                    int topY = (int)hillH;
                    if (prng.Next(0, 100) < 18)
                    {
                        SetPixel(pixels, w, h, x, topY + 1, grassGold);
                        SetPixel(pixels, w, h, x, topY + 2, grassGold);
                    }
                    if (prng.Next(0, 100) < 6)
                    {
                        SetPixel(pixels, w, h, x, topY + 1, new Color32(250, 240, 210, 255)); // White wildflower
                    }
                }
            }

            // Big foreground rocks
            FillRect(pixels, w, h, 360, 12, 18, 10, new Color32(50, 52, 58, 255));
            FillRect(pixels, w, h, 363, 16, 12, 6, new Color32(85, 88, 92, 255));
            FillRect(pixels, w, h, 365, 19, 7, 3, new Color32(140, 135, 110, 255)); // Sunlit rock face

            FillRect(pixels, w, h, 290, 8, 14, 8, new Color32(45, 48, 52, 255));
            FillRect(pixels, w, h, 293, 11, 9, 5, new Color32(75, 78, 82, 255));

            // --- 9. Ancient Oak Tree (Left) ---
            int trunkX = 45;
            int trunkBaseY = 48;
            int trunkW = 38;

            // Trunk mass
            for (int y = trunkBaseY; y < h; y++)
            {
                int curW = trunkW + (int)(Mathf.Sin((y - trunkBaseY) * 0.04f) * 6f);
                int curX = trunkX - curW / 2 + (int)((y - trunkBaseY) * -0.12f);

                for (int x = curX; x < curX + curW; x++)
                {
                    if (x < 0 || x >= w || y < 0 || y >= h) continue;

                    float t = (float)(x - curX) / curW;
                    if (t > 0.82f) pixels[y * w + x] = oakBarkRim;   // Golden sunset rim light
                    else if (t > 0.55f) pixels[y * w + x] = oakBarkMid;
                    else pixels[y * w + x] = oakBarkDark;
                }
            }

            // Roots sprawling into the earth
            FillRect(pixels, w, h, trunkX - 25, trunkBaseY - 12, 22, 16, oakBarkDark);
            FillRect(pixels, w, h, trunkX + 10, trunkBaseY - 10, 28, 14, oakBarkMid);
            for (int rx = trunkX + 15; rx < trunkX + 38; rx++) SetPixel(pixels, w, h, rx, trunkBaseY - 2, oakBarkRim);

            // Sprawling leafy oak canopy framing the top left
            PaintOakFoliageCluster(pixels, w, h, 30, 200, 80, 24, oakFoliage, oakFoliageMid, oakFoliageLit);
            PaintOakFoliageCluster(pixels, w, h, 95, 205, 90, 26, oakFoliage, oakFoliageMid, oakFoliageLit);
            PaintOakFoliageCluster(pixels, w, h, 160, 210, 85, 22, oakFoliage, oakFoliageMid, oakFoliageLit);
            PaintOakFoliageCluster(pixels, w, h, 215, 212, 65, 18, oakFoliage, oakFoliageMid, oakFoliageLit);

            PaintOakFoliageCluster(pixels, w, h, 15, 175, 65, 20, oakFoliage, oakFoliageMid, oakFoliageLit);
            PaintOakFoliageCluster(pixels, w, h, 70, 180, 75, 22, oakFoliage, oakFoliageMid, oakFoliageLit);
            PaintOakFoliageCluster(pixels, w, h, 130, 185, 70, 20, oakFoliage, oakFoliageMid, oakFoliageLit);

            // --- 10. The Resting Knight under the Oak Tree ---
            int kX = 135;
            int kY = 48;

            // Armor shadow underneath
            FillRect(pixels, w, h, kX - 28, kY - 8, 55, 10, new Color32(18, 16, 20, 255));

            // Torso (Plate Cuirass leaning back)
            FillRect(pixels, w, h, kX - 10, kY + 6, 20, 24, steelDark);
            FillRect(pixels, w, h, kX - 7, kY + 8, 15, 20, steelMid);
            FillRect(pixels, w, h, kX - 2, kY + 12, 8, 14, steelLit);
            FillRect(pixels, w, h, kX + 1, kY + 14, 4, 10, steelGold); // Sunset light glinting on breastplate

            // Crimson Scarf & Cape
            FillRect(pixels, w, h, kX - 14, kY + 16, 26, 12, knightScarf);
            FillRect(pixels, w, h, kX - 11, kY + 20, 18, 8, knightScarfLit);
            FillRect(pixels, w, h, kX - 16, kY + 4, 12, 18, knightScarf); // Cape draped down back

            // Knight Helmet & Visor (Tilted back drinking)
            int helmX = kX - 2;
            int helmY = kY + 30;
            FillRect(pixels, w, h, helmX - 8, helmY - 4, 16, 16, steelMid);
            FillRect(pixels, w, h, helmX - 6, helmY + 8, 12, 5, steelLit); // Dome
            FillRect(pixels, w, h, helmX - 1, helmY + 9, 5, 3, steelGold); // Sunset glint on helmet
            FillRect(pixels, w, h, helmX - 8, helmY - 2, 16, 3, steelDark); // Brow rim
            FillRect(pixels, w, h, helmX - 6, helmY - 5, 13, 2, new Color32(12, 12, 16, 255)); // Visor dark slit

            // Raised Right Arm holding bottle / drinking horn
            FillRect(pixels, w, h, kX + 8, kY + 22, 10, 7, steelMid); // Shoulder pauldron
            SetPixel(pixels, w, h, kX + 12, kY + 26, steelGold);
            FillRect(pixels, w, h, kX + 14, kY + 26, 7, 10, steelLit); // Forearm
            FillRect(pixels, w, h, kX + 17, kY + 32, 6, 6, steelMid);  // Gauntlet

            // Bottle / Flask at mouth
            FillRect(pixels, w, h, kX + 9, helmY - 2, 14, 6, new Color32(32, 58, 42, 255)); // Dark green glass
            FillRect(pixels, w, h, kX + 12, helmY - 1, 9, 3, new Color32(65, 115, 85, 255));
            SetPixel(pixels, w, h, kX + 16, helmY + 1, new Color32(180, 235, 190, 255)); // Glass highlight

            // Left Arm resting on knee
            FillRect(pixels, w, h, kX - 16, kY + 18, 8, 6, steelMid); // Left pauldron
            FillRect(pixels, w, h, kX - 14, kY + 10, 6, 12, steelDark);
            FillRect(pixels, w, h, kX - 10, kY + 8, 6, 6, steelLit);  // Left gauntlet

            // Legs (Plate Greaves, Knee Poleyns & Sabatons)
            // Left Leg (Bent knee raised up)
            FillRect(pixels, w, h, kX + 4, kY - 2, 10, 16, steelMid);   // Thigh plate
            FillRect(pixels, w, h, kX + 12, kY + 6, 8, 8, steelLit);    // Knee poleyn
            SetPixel(pixels, w, h, kX + 14, kY + 10, steelGold);        // Knee sunset highlight
            FillRect(pixels, w, h, kX + 16, kY - 6, 8, 14, steelMid);   // Shin greave
            FillRect(pixels, w, h, kX + 22, kY - 8, 10, 6, steelDark);  // Sabaton boot

            // Right Leg (Extended down slope)
            FillRect(pixels, w, h, kX + 6, kY - 8, 18, 8, steelDark);
            FillRect(pixels, w, h, kX + 20, kY - 10, 16, 7, steelMid);
            FillRect(pixels, w, h, kX + 32, kY - 12, 12, 6, steelDark); // Right boot
            SetPixel(pixels, w, h, kX + 24, kY - 7, steelGold);

            // Broadsword stuck in ground next to knight
            int swX = kX - 44;
            int swY = kY - 6;
            FillRect(pixels, w, h, swX, swY, 4, 34, steelLit);           // Blade
            FillRect(pixels, w, h, swX + 1, swY + 4, 2, 28, steelGold);  // Blade sunset reflection
            FillRect(pixels, w, h, swX - 8, swY + 34, 20, 3, steelDark); // Crossguard
            FillRect(pixels, w, h, swX - 6, swY + 35, 16, 1, goldLion);
            FillRect(pixels, w, h, swX + 1, swY + 37, 2, 8, oakBarkDark);// Leather grip
            FillRect(pixels, w, h, swX - 1, swY + 45, 6, 5, goldLion);   // Pommel

            // Heater Shield with Lion Crest resting on ground
            int shX = kX - 32;
            int shY = kY - 8;
            FillRect(pixels, w, h, shX, shY, 18, 26, steelDark);
            FillRect(pixels, w, h, shX + 2, shY + 2, 14, 22, knightScarf); // Shield crimson field
            FillRect(pixels, w, h, shX + 1, shY + 24, 16, 2, steelLit);    // Top rim
            // Golden Lion Crest
            FillRect(pixels, w, h, shX + 6, shY + 10, 6, 8, goldLion);
            FillRect(pixels, w, h, shX + 8, shY + 18, 5, 4, goldLion);
            SetPixel(pixels, w, h, shX + 12, shY + 20, new Color32(255, 230, 140, 255));

            // Leather traveler's pack beside knight
            FillRect(pixels, w, h, kX - 22, kY - 8, 14, 12, oakBarkMid);
            FillRect(pixels, w, h, kX - 20, kY - 2, 10, 3, oakBarkRim);
            SetPixel(pixels, w, h, kX - 16, kY - 3, goldLion); // Brass buckle
        }

        private static void PaintCloudCluster(Color32[] pixels, int w, int h, int cx, int cy, int cw, int ch, Color32 dark, Color32 mid, Color32 lit, Color32 bright)
        {
            for (int y = cy - ch / 2; y <= cy + ch / 2; y++)
            {
                if (y < 0 || y >= h) continue;
                for (int x = cx - cw / 2; x <= cx + cw / 2; x++)
                {
                    if (x < 0 || x >= w) continue;

                    float dx = (x - cx) / (cw * 0.5f);
                    float dy = (y - cy) / (ch * 0.5f);
                    float dSq = dx * dx + dy * dy;
                    if (dSq > 1f) continue;

                    float edge = 1f - dSq;
                    if (dy < -0.3f) pixels[y * w + x] = lit;
                    else if (dy > 0.4f) pixels[y * w + x] = (edge > 0.5f) ? bright : lit;
                    else if (edge > 0.6f) pixels[y * w + x] = mid;
                    else pixels[y * w + x] = dark;
                }
            }
        }

        private static void PaintPineTreeSilhouette(Color32[] pixels, int w, int h, int x, int baseY, int treeH, Color32 dark, Color32 mid, Color32 sun)
        {
            for (int y = baseY; y < baseY + treeH && y < h; y++)
            {
                float t = (float)(y - baseY) / treeH;
                int branchW = (int)((1f - t) * 7f + Mathf.Sin(t * 12f) * 1.5f);

                for (int bx = x - branchW; bx <= x + branchW; bx++)
                {
                    if (bx < 0 || bx >= w || y < 0 || y >= h) continue;
                    if (bx == x + branchW) pixels[y * w + bx] = sun;
                    else if (bx > x) pixels[y * w + bx] = mid;
                    else pixels[y * w + bx] = dark;
                }
            }
        }

        private static void PaintOakFoliageCluster(Color32[] pixels, int w, int h, int cx, int cy, int cw, int ch, Color32 dark, Color32 mid, Color32 lit)
        {
            for (int y = cy - ch / 2; y <= cy + ch / 2; y++)
            {
                if (y < 0 || y >= h) continue;
                for (int x = cx - cw / 2; x <= cx + cw / 2; x++)
                {
                    if (x < 0 || x >= w) continue;

                    float dx = (x - cx) / (cw * 0.5f);
                    float dy = (y - cy) / (ch * 0.5f);
                    float dSq = dx * dx + dy * dy;
                    if (dSq > 1f) continue;

                    if (dx > 0.35f || dy < -0.3f) pixels[y * w + x] = lit;
                    else if (dSq < 0.5f) pixels[y * w + x] = mid;
                    else pixels[y * w + x] = dark;
                }
            }
        }

        private static void PaintBirdSilhouette(Color32[] pixels, int w, int h, int x, int y)
        {
            Color32 b = new Color32(35, 25, 38, 255);
            SetPixel(pixels, w, h, x, y, b);
            SetPixel(pixels, w, h, x - 1, y + 1, b);
            SetPixel(pixels, w, h, x - 2, y + 1, b);
            SetPixel(pixels, w, h, x + 1, y + 1, b);
            SetPixel(pixels, w, h, x + 2, y + 1, b);
        }
    }

    public readonly struct ExportableSprite
    {
        public string RelativePath { get; }
        public int Width { get; }
        public int Height { get; }
        public Vector2 Pivot { get; }
        public Action<Color32[]> Paint { get; }

        public ExportableSprite(string relativePath, int width, int height, Vector2 pivot, Action<Color32[]> paint)
        {
            RelativePath = relativePath;
            Width = width;
            Height = height;
            Pivot = pivot;
            Paint = paint;
        }
    }
}
