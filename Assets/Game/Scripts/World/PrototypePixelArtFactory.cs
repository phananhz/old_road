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
        public static Sprite CabinInteriorFloor() => GetOrCreate("cabin.interior.floor.v3", 176, 112, PaintCabinInteriorFloor, new Vector2(0.5f, 0.5f));
        public static Sprite CabinInteriorWall() => GetOrCreate("cabin.interior.wall.v3", 176, 48, PaintCabinInteriorWall, new Vector2(0.5f, 0.5f));
        public static Sprite CottageFloor1() => GetOrCreate("cottage.floor.f1", 176, 112, PaintCottageFloor1, new Vector2(0.5f, 0.5f));
        public static Sprite CottageWall1() => GetOrCreate("cottage.wall.f1", 176, 48, PaintCottageWall1, new Vector2(0.5f, 0.5f));
        public static Sprite CottageFloor2() => GetOrCreate("cottage.floor.f2", 176, 112, PaintCottageFloor2, new Vector2(0.5f, 0.5f));
        public static Sprite CottageWall2() => GetOrCreate("cottage.wall.f2", 176, 48, PaintCottageWall2, new Vector2(0.5f, 0.5f));
        public static Sprite ManorFloor1() => GetOrCreate("manor.floor.f1", 176, 112, PaintManorFloor1, new Vector2(0.5f, 0.5f));
        public static Sprite ManorWall1() => GetOrCreate("manor.wall.f1", 176, 48, PaintManorWall1, new Vector2(0.5f, 0.5f));
        public static Sprite ManorFloor2() => GetOrCreate("manor.floor.f2", 176, 112, PaintManorFloor2, new Vector2(0.5f, 0.5f));
        public static Sprite ManorWall2() => GetOrCreate("manor.wall.f2", 176, 48, PaintManorWall2, new Vector2(0.5f, 0.5f));
        public static Sprite WindmillFloor1() => GetOrCreate("windmill.floor.f1", 176, 112, PaintWindmillFloor1, new Vector2(0.5f, 0.5f));
        public static Sprite WindmillWall1() => GetOrCreate("windmill.wall.f1", 176, 48, PaintWindmillWall1, new Vector2(0.5f, 0.5f));
        public static Sprite WindmillFloor2() => GetOrCreate("windmill.floor.f2", 176, 112, PaintWindmillFloor2, new Vector2(0.5f, 0.5f));
        public static Sprite WindmillWall2() => GetOrCreate("windmill.wall.f2", 176, 48, PaintWindmillWall2, new Vector2(0.5f, 0.5f));
        public static Sprite GreenhouseFloor() => GetOrCreate("greenhouse.floor", 176, 112, PaintGreenhouseFloor, new Vector2(0.5f, 0.5f));
        public static Sprite GreenhouseWall() => GetOrCreate("greenhouse.wall", 176, 48, PaintGreenhouseWall, new Vector2(0.5f, 0.5f));
        public static Sprite TentFloor() => GetOrCreate("tent.floor", 176, 112, PaintTentFloor, new Vector2(0.5f, 0.5f));
        public static Sprite TentWall() => GetOrCreate("tent.wall", 176, 48, PaintTentWall, new Vector2(0.5f, 0.5f));
        public static Sprite StorageShedFloor() => GetOrCreate("storage.shed.floor", 176, 112, PaintStorageShedFloor, new Vector2(0.5f, 0.5f));
        public static Sprite StorageShedWall() => GetOrCreate("storage.shed.wall", 176, 48, PaintStorageShedWall, new Vector2(0.5f, 0.5f));
        public static Sprite HerbalistFloor() => GetOrCreate("herbalist.floor", 176, 112, PaintHerbalistFloor, new Vector2(0.5f, 0.5f));
        public static Sprite HerbalistWall() => GetOrCreate("herbalist.wall", 176, 48, PaintHerbalistWall, new Vector2(0.5f, 0.5f));
        public static Sprite StairsWoodUp() => GetOrCreate("stairs.wood.up", 32, 36, PaintStairsWoodUp, new Vector2(0.5f, 0.15f));
        public static Sprite StairsWoodDown() => GetOrCreate("stairs.wood.down", 32, 36, PaintStairsWoodDown, new Vector2(0.5f, 0.15f));
        public static Sprite StairsGrandManorUp() => GetOrCreate("stairs.grand.manor.up", 42, 40, PaintStairsGrandManorUp, new Vector2(0.5f, 0.15f));
        public static Sprite StairsGrandManorDown() => GetOrCreate("stairs.grand.manor.down", 42, 40, PaintStairsGrandManorDown, new Vector2(0.5f, 0.15f));
        public static Sprite MillerLadder() => GetOrCreate("miller.ladder", 22, 40, PaintMillerLadder, new Vector2(0.5f, 0.15f));
        public static Sprite RoyalCanopyBed() => GetOrCreate("bed.royal.canopy", 44, 34, PaintRoyalCanopyBed, new Vector2(0.5f, 0.15f));
        public static Sprite MillstoneGrinder() => GetOrCreate("millstone.grinder", 38, 34, PaintMillstoneGrinder, new Vector2(0.5f, 0.15f));
        public static Sprite IndoorPlanterBed() => GetOrCreate("indoor.planter.bed", 38, 24, PaintIndoorPlanterBed, new Vector2(0.5f, 0.15f));
        public static Sprite ApothecaryStation() => GetOrCreate("apothecary.station", 38, 28, PaintApothecaryStation, new Vector2(0.5f, 0.15f));
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
        // --- Dynamic Category Buildings ---
        // 0: Housing & Lodges
        public static Sprite TentComplete() => GetOrCreate("building.tent", 40, 32, PaintTent, new Vector2(0.5f, 0.12f));
        public static Sprite ManorComplete() => GetOrCreate("building.manor", 76, 60, PaintManor, new Vector2(0.5f, 0.10f));
        public static Sprite GreenhouseComplete() => GetOrCreate("building.greenhouse", 56, 46, PaintGreenhouse, new Vector2(0.5f, 0.12f));
        public static Sprite SiloComplete() => GetOrCreate("building.silo", 36, 56, PaintSilo, new Vector2(0.5f, 0.08f));

        // 1: Fire & Lighting
        public static Sprite StreetLampComplete() => GetOrCreate("building.street-lamp", 18, 36, PaintStreetLamp, new Vector2(0.5f, 0.08f));
        public static Sprite GroundTorchComplete() => GetOrCreate("building.ground-torch", 14, 26, PaintGroundTorch, new Vector2(0.5f, 0.12f));
        public static Sprite LanternPoleComplete() => GetOrCreate("building.lantern-pole", 18, 38, PaintLanternPole, new Vector2(0.5f, 0.08f));
        public static Sprite StoneFireplaceComplete() => GetOrCreate("building.stone-fireplace", 36, 40, PaintStoneFireplace, new Vector2(0.5f, 0.10f));

        // 2: Animal Husbandry
        public static Sprite SheepPastureComplete() => GetOrCreate("building.sheep-pasture", 56, 42, PaintSheepPasture, new Vector2(0.5f, 0.12f));
        public static Sprite HenCoopComplete() => GetOrCreate("building.hen-coop", 44, 36, PaintHenCoop, new Vector2(0.5f, 0.12f));

        // 3: Fences & Walls
        public static Sprite StoneWallComplete() => GetOrCreate("building.stone-wall", 24, 24, PaintStoneWall, new Vector2(0.5f, 0.15f));
        public static Sprite IronGateComplete() => GetOrCreate("building.iron-gate", 32, 28, PaintIronGate, new Vector2(0.5f, 0.15f));
        public static Sprite LogPalisadeComplete() => GetOrCreate("building.log-palisade", 24, 30, PaintLogPalisade, new Vector2(0.5f, 0.15f));
        public static Sprite PerimeterFencePreview() => GetOrCreate("building.perimeter-fence.preview", 36, 28, PaintPerimeterFencePreview, new Vector2(0.5f, 0.15f));

        // 4: Paths & Bridges
        public static Sprite PathWoodTile() => GetOrCreate("tile.path.wood", 16, 16, PaintPathWood, new Vector2(0.5f, 0.5f));
        public static Sprite PathStoneTile() => GetOrCreate("tile.path.stone-tile", 16, 16, PaintPathStoneTile, new Vector2(0.5f, 0.5f));
        public static Sprite WoodBridgeComplete() => GetOrCreate("building.wood-bridge", 48, 26, PaintWoodBridge, new Vector2(0.5f, 0.15f));

        // 5: Furniture & Living
        public static Sprite StrawBedComplete() => GetOrCreate("building.straw-bed", 32, 24, PaintStrawBed, new Vector2(0.5f, 0.15f));
        public static Sprite OakTableComplete() => GetOrCreate("building.oak-table", 30, 22, PaintOakTable, new Vector2(0.5f, 0.15f));
        public static Sprite LeatherChairComplete() => GetOrCreate("building.leather-chair", 20, 24, PaintLeatherChair, new Vector2(0.5f, 0.15f));
        public static Sprite BookshelfComplete() => GetOrCreate("building.bookshelf", 26, 38, PaintBookshelf, new Vector2(0.5f, 0.10f));
        public static Sprite WovenRugComplete() => GetOrCreate("building.woven-rug", 36, 24, PaintWovenRug, new Vector2(0.5f, 0.5f));

        // 6: Artisan & Processing
        public static Sprite CheesePressComplete() => GetOrCreate("building.cheese-press", 26, 28, PaintCheesePress, new Vector2(0.5f, 0.15f));
        public static Sprite LoomComplete() => GetOrCreate("building.loom", 30, 30, PaintLoom, new Vector2(0.5f, 0.15f));
        public static Sprite KegComplete() => GetOrCreate("building.keg", 28, 26, PaintKeg, new Vector2(0.5f, 0.15f));
        public static Sprite WindmillComplete() => GetOrCreate("building.windmill", 48, 62, PaintWindmill, new Vector2(0.5f, 0.08f));
        public static Sprite BlacksmithForgeComplete() => GetOrCreate("building.blacksmith-forge", 42, 36, PaintBlacksmithForge, new Vector2(0.5f, 0.12f));
        public static Sprite CarpenterBenchComplete() => GetOrCreate("building.carpenter-bench", 32, 24, PaintCarpenterBench, new Vector2(0.5f, 0.15f));

        // 7: Storage & Logistics
        public static Sprite WoodChestComplete() => GetOrCreate("building.wood-chest", 24, 20, PaintWoodChest, new Vector2(0.5f, 0.15f));
        public static Sprite StoneVaultComplete() => GetOrCreate("building.stone-vault", 30, 28, PaintStoneVault, new Vector2(0.5f, 0.15f));
        public static Sprite CompostBinComplete() => GetOrCreate("building.compost-bin", 26, 24, PaintCompostBin, new Vector2(0.5f, 0.15f));
        public static Sprite BarrelRackComplete() => GetOrCreate("building.barrel-rack", 36, 22, PaintBarrelRack, new Vector2(0.5f, 0.15f));

        // 8: Gardening & Greenery
        public static Sprite GrapeTrellisComplete() => GetOrCreate("building.grape-trellis", 36, 28, PaintGrapeTrellis, new Vector2(0.5f, 0.15f));
        public static Sprite PumpkinPatchComplete() => GetOrCreate("building.pumpkin-patch", 34, 28, PaintPumpkinPatch, new Vector2(0.5f, 0.15f));
        public static Sprite FlowerPlanterComplete() => GetOrCreate("building.flower-planter", 28, 20, PaintFlowerPlanter, new Vector2(0.5f, 0.15f));
        public static Sprite GardenHedgeComplete() => GetOrCreate("building.garden-hedge", 22, 22, PaintGardenHedge, new Vector2(0.5f, 0.15f));

        // 9: Water & Irrigation
        public static Sprite AncientWellComplete() => GetOrCreate("building.ancient-well", 36, 40, PaintAncientWell, new Vector2(0.5f, 0.10f));
        public static Sprite WaterAqueductComplete() => GetOrCreate("building.water-aqueduct", 34, 22, PaintWaterAqueduct, new Vector2(0.5f, 0.15f));
        public static Sprite StoneFountainComplete() => GetOrCreate("building.stone-fountain", 36, 38, PaintStoneFountain, new Vector2(0.5f, 0.12f));
        public static Sprite HotBathComplete() => GetOrCreate("building.hot-bath", 34, 28, PaintHotBath, new Vector2(0.5f, 0.15f));

        // 10: Monuments & Relics
        public static Sprite KnightStatueComplete() => GetOrCreate("building.knight-statue", 28, 44, PaintKnightStatue, new Vector2(0.5f, 0.08f));
        public static Sprite GuardianShrineComplete() => GetOrCreate("building.guardian-shrine", 30, 40, PaintGuardianShrine, new Vector2(0.5f, 0.10f));
        public static Sprite BellPillarComplete() => GetOrCreate("building.bell-pillar", 24, 40, PaintBellPillar, new Vector2(0.5f, 0.08f));

        // 11: Trade & Commerce
        public static Sprite MarketStallComplete() => GetOrCreate("building.market-stall", 40, 36, PaintMarketStall, new Vector2(0.5f, 0.12f));

        // 12: Defense & Security
        public static Sprite SpikeTrapComplete() => GetOrCreate("building.spike-trap", 26, 20, PaintSpikeTrap, new Vector2(0.5f, 0.15f));
        public static Sprite WoodenBarricadeComplete() => GetOrCreate("building.wooden-barricade", 30, 24, PaintWoodenBarricade, new Vector2(0.5f, 0.15f));
        public static Sprite AlarmBellComplete() => GetOrCreate("building.alarm-bell", 24, 38, PaintAlarmBell, new Vector2(0.5f, 0.08f));

        // 13: Leisure & Camping
        public static Sprite WoodSwingComplete() => GetOrCreate("building.wood-swing", 28, 36, PaintWoodSwing, new Vector2(0.5f, 0.08f));
        public static Sprite ChessTableComplete() => GetOrCreate("building.chess-table", 26, 24, PaintChessTable, new Vector2(0.5f, 0.15f));
        public static Sprite HammockComplete() => GetOrCreate("building.hammock", 36, 20, PaintHammock, new Vector2(0.5f, 0.15f));
        public static Sprite BbqGrillComplete() => GetOrCreate("building.bbq-grill", 28, 26, PaintBbqGrill, new Vector2(0.5f, 0.15f));

        // 14: Festivals & Ornaments
        public static Sprite FestivalBannerComplete() => GetOrCreate("building.festival-banner", 34, 24, PaintFestivalBanner, new Vector2(0.5f, 0.15f));
        public static Sprite SkyLanternComplete() => GetOrCreate("building.sky-lantern", 20, 28, PaintSkyLantern, new Vector2(0.5f, 0.15f));
        public static Sprite FireflyJarComplete() => GetOrCreate("building.firefly-jar", 18, 24, PaintFireflyJar, new Vector2(0.5f, 0.15f));

        public static Sprite BuildingComplete(string buildingId)
        {
            if (string.IsNullOrEmpty(buildingId)) return CabinComplete();
            if (buildingId.Contains("perimeter-fence")) return null;

            switch (buildingId)
            {
                // Category 0: Housing
                case "building.cabin": return CabinComplete();
                case "building.stone-cottage": return StoneCottage();
                case "building.farm-barn": return HappyFarmBarn();
                case "building.storage-shed": return StorageShed();
                case "building.herbalist-hut": return HerbalistHut();
                case "building.lookout-tower": return LookoutTower();
                case "building.tent": return TentComplete();
                case "building.manor": return ManorComplete();
                case "building.greenhouse": return GreenhouseComplete();
                case "building.silo": return SiloComplete();

                // Category 1: Lighting
                case "building.campfire": return Campfire();
                case "building.cooking-hearth": return CookingHearthOutdoor();
                case "building.street-lamp": return StreetLampComplete();
                case "building.ground-torch": return GroundTorchComplete();
                case "building.lantern-pole": return LanternPoleComplete();
                case "building.stone-fireplace": return StoneFireplaceComplete();

                // Category 2: Husbandry
                case "building.animal-pen-small": return AnimalPenSmall();
                case "building.animal-pen-long": return AnimalPenLong();
                case "building.sheep-pasture": return SheepPastureComplete();
                case "building.hen-coop": return HenCoopComplete();
                case "building.feed-trough": return FeedingTrough();
                case "building.water-trough": return WaterTrough();

                // Category 3: Fences
                case "building.fence": return WoodFenceHorizontal();
                case "building.fence-vertical": return WoodFenceVertical();
                case "building.gate": return WoodGate(false);
                case "building.stone-wall": return StoneWallComplete();
                case "building.iron-gate": return IronGateComplete();
                case "building.log-palisade": return LogPalisadeComplete();

                // Category 4: Paths
                case "building.path-dirt": return PathDirtTile();
                case "building.path-cobblestone": return PathCobblestoneTile();
                case "building.path-wood": return PathWoodTile();
                case "building.path-stone-tile": return PathStoneTile();
                case "building.wood-bridge": return WoodBridgeComplete();
                case "building.scarecrow": return Scarecrow();

                // Category 5: Furniture
                case "building.straw-bed": return StrawBedComplete();
                case "building.oak-table": return OakTableComplete();
                case "building.leather-chair": return LeatherChairComplete();
                case "building.bookshelf": return BookshelfComplete();
                case "building.woven-rug": return WovenRugComplete();

                // Category 6: Processing
                case "building.cheese-press": return CheesePressComplete();
                case "building.loom": return LoomComplete();
                case "building.keg": return KegComplete();
                case "building.windmill": return WindmillComplete();
                case "building.blacksmith-forge": return BlacksmithForgeComplete();
                case "building.carpenter-bench": return CarpenterBenchComplete();

                // Category 7: Storage
                case "building.wood-chest": return WoodChestComplete();
                case "building.stone-vault": return StoneVaultComplete();
                case "building.compost-bin": return CompostBinComplete();
                case "building.barrel-rack": return BarrelRackComplete();

                // Category 8: Gardening
                case "building.grape-trellis": return GrapeTrellisComplete();
                case "building.pumpkin-patch": return PumpkinPatchComplete();
                case "building.flower-planter": return FlowerPlanterComplete();
                case "building.garden-hedge": return GardenHedgeComplete();

                // Category 9: Water
                case "building.ancient-well": return AncientWellComplete();
                case "building.water-aqueduct": return WaterAqueductComplete();
                case "building.stone-fountain": return StoneFountainComplete();
                case "building.hot-bath": return HotBathComplete();

                // Category 10: Monuments
                case "building.knight-statue": return KnightStatueComplete();
                case "building.guardian-shrine": return GuardianShrineComplete();
                case "building.bell-pillar": return BellPillarComplete();

                // Category 11: Commerce
                case "building.market-stall": return MarketStallComplete();
                case "building.farm-sign":
                case "building.farm-signboard": return FarmSignboard();
                case "building.travel-cart": return TravelCartIcon();

                // Category 12: Defense
                case "building.spike-trap": return SpikeTrapComplete();
                case "building.wooden-barricade": return WoodenBarricadeComplete();
                case "building.alarm-bell": return AlarmBellComplete();

                // Category 13: Leisure
                case "building.wood-swing": return WoodSwingComplete();
                case "building.chess-table": return ChessTableComplete();
                case "building.hammock": return HammockComplete();
                case "building.bbq-grill": return BbqGrillComplete();

                // Category 14: Festivals
                case "building.festival-banner": return FestivalBannerComplete();
                case "building.sky-lantern": return SkyLanternComplete();
                case "building.firefly-jar": return FireflyJarComplete();

                default: return CabinComplete();
            }
        }

        public static Sprite BuildingCatalogIcon(string buildingId)
        {
            if (string.IsNullOrEmpty(buildingId)) return CabinComplete();
            if (buildingId.Contains("perimeter-fence")) return PerimeterFencePreview();
            return BuildingComplete(buildingId);
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
        public static Sprite Crop(string cropId, int stage) => GetOrCreate("crop." + cropId + "." + Mathf.Clamp(stage, 0, 4), 24, 28, pixels => PaintCrop(pixels, cropId, Mathf.Clamp(stage, 0, 4)), new Vector2(0.5f, 0.15f));
        public static Sprite HeartEmote() => GetOrCreate("emote.heart", 16, 16, PaintHeartEmote, new Vector2(0.5f, 0.5f));
        public static Sprite SilverCoinIcon() => GetOrCreate("currency.silver.coin", 16, 16, PaintSilverCoin, new Vector2(0.5f, 0.5f));
        public static Sprite NightMonsterSprite(int frame) => GetOrCreate("enemy.nightmonster." + Mathf.Abs(frame % 4), 22, 22, pixels => PaintNightMonster(pixels, Mathf.Abs(frame % 4)), new Vector2(0.5f, 0.15f));
        public static Sprite BellTowerRuins() => GetOrCreate("environment.belltower.ruins", 64, 64, PaintBellTowerRuins, new Vector2(0.5f, 0.10f));
        public static Sprite PuzzlePedestal(int symbol, bool active) => GetOrCreate("environment.pedestal." + symbol + "." + (active ? "1" : "0"), 22, 24, pixels => PaintPuzzlePedestal(pixels, symbol, active), new Vector2(0.5f, 0.15f));
        public static Sprite MerchantCart() => GetOrCreate("environment.merchant.cart", 52, 40, PaintMerchantCart, new Vector2(0.5f, 0.12f));
        public static Sprite TravelCartIcon() => MerchantCart();
        public static Sprite HappyFarmBarn() => GetOrCreate("environment.happyfarm.barn", 68, 52, PaintHappyFarmBarn, new Vector2(0.5f, 0.12f));
        public static Sprite DairyCow() => GetOrCreate("animal.dairycow", 28, 20, PaintDairyCow, new Vector2(0.5f, 0.20f));
        public static Sprite FluffySheep() => GetOrCreate("animal.sheep", 24, 18, PaintFluffySheep, new Vector2(0.5f, 0.20f));
        public static Sprite HenSprite() => GetOrCreate("animal.hen", 14, 14, PaintHen, new Vector2(0.5f, 0.20f));
        public static Sprite StrawNest(bool hasEgg) => GetOrCreate("environment.straw.nest." + (hasEgg ? "1" : "0"), 22, 16, pixels => PaintStrawNest(pixels, hasEgg), new Vector2(0.5f, 0.30f));
        public static Sprite FarmDog() => GetOrCreate("animal.farmdog", 18, 16, PaintFarmDog, new Vector2(0.5f, 0.20f));
        public static Sprite HayBalePile() => GetOrCreate("environment.haybale.pile", 28, 22, PaintHayBalePile, new Vector2(0.5f, 0.15f));
        public static Sprite FeedingTrough() => GetOrCreate("environment.feeding.trough", 26, 14, PaintFeedingTrough, new Vector2(0.5f, 0.20f));
        public static Sprite WaterTrough() => GetOrCreate("environment.water.trough", 26, 14, PaintWaterTrough, new Vector2(0.5f, 0.20f));
        public static Sprite FarmShopSign() => GetOrCreate("environment.farmshop.sign", 18, 22, PaintFarmShopSign, new Vector2(0.5f, 0.10f));
        public static Sprite BulletinBoard() => GetOrCreate("environment.bulletin.board", 32, 36, PaintBulletinBoard, new Vector2(0.5f, 0.10f));
        public static Sprite Mailbox(bool hasMail) => GetOrCreate("environment.mailbox." + (hasMail ? "1" : "0"), 20, 28, pixels => PaintMailbox(pixels, hasMail), new Vector2(0.5f, 0.15f));
        public static Sprite MailLetter() => GetOrCreate("ui.mail.letter", 18, 18, PaintMailLetter, new Vector2(0.5f, 0.5f));
        public static Sprite CompendiumBadge() => GetOrCreate("ui.compendium.badge", 18, 18, PaintCompendiumBadge, new Vector2(0.5f, 0.5f));
        public static Sprite RewardChestIcon() => GetOrCreate("ui.reward.chest", 18, 18, PaintRewardChestIcon, new Vector2(0.5f, 0.5f));
        public static Sprite CursorPointer() => GetOrCreate("cursor.pointer", 20, 20, PaintCursorPointer, new Vector2(0f, 1f));
        public static Sprite CursorAxe() => GetOrCreate("cursor.axe", 20, 20, PaintCursorAxe, new Vector2(0f, 1f));
        public static Sprite CursorPickaxe() => GetOrCreate("cursor.pickaxe", 20, 20, PaintCursorPickaxe, new Vector2(0f, 1f));
        public static Sprite CursorHoe() => GetOrCreate("cursor.hoe", 20, 20, PaintCursorHoe, new Vector2(0f, 1f));
        public static Sprite CursorWateringCan() => GetOrCreate("cursor.wateringcan", 20, 20, PaintCursorWateringCan, new Vector2(0f, 1f));
        public static Sprite CursorSeed() => GetOrCreate("cursor.seed", 20, 20, PaintCursorSeed, new Vector2(0f, 1f));
        public static Sprite CursorHand() => GetOrCreate("cursor.hand", 20, 20, PaintCursorHand, new Vector2(0f, 1f));
        public static Sprite CursorHarvest() => GetOrCreate("cursor.harvest", 20, 20, PaintCursorHarvest, new Vector2(0f, 1f));
        public static Sprite CursorSword() => GetOrCreate("cursor.sword", 20, 20, PaintCursorSword, new Vector2(0f, 1f));

        public static Texture2D CursorPointerTexture() => CursorPointer().texture;
        public static Texture2D CursorAxeTexture() => CursorAxe().texture;
        public static Texture2D CursorPickaxeTexture() => CursorPickaxe().texture;
        public static Texture2D CursorHoeTexture() => CursorHoe().texture;
        public static Texture2D CursorWateringCanTexture() => CursorWateringCan().texture;
        public static Texture2D CursorSeedTexture() => CursorSeed().texture;
        public static Texture2D CursorHandTexture() => CursorHand().texture;
        public static Texture2D CursorHarvestTexture() => CursorHarvest().texture;
        public static Texture2D CursorSwordTexture() => CursorSword().texture;

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
        public static Sprite ItemToolHoe() => GetOrCreate("item.tool-hoe", 16, 16, PaintItemHoe, new Vector2(0.5f, 0.5f));
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
        public static Sprite ItemSeedPotato() => GetOrCreate("item.seed-potato", 16, 16, pixels => PaintItemSeed(pixels, new Color32(185, 140, 75, 255)), new Vector2(0.5f, 0.5f));
        public static Sprite ItemSeedPineapple() => GetOrCreate("item.seed-pineapple", 16, 16, pixels => PaintItemSeed(pixels, new Color32(245, 190, 45, 255)), new Vector2(0.5f, 0.5f));
        public static Sprite ItemSeedTomato() => GetOrCreate("item.seed-tomato", 16, 16, pixels => PaintItemSeed(pixels, new Color32(230, 65, 45, 255)), new Vector2(0.5f, 0.5f));
        public static Sprite ItemWheat() => GetOrCreate("item.wheat", 16, 16, PaintItemWheat, new Vector2(0.5f, 0.5f));
        public static Sprite ItemCorn() => GetOrCreate("item.corn", 16, 16, PaintItemCorn, new Vector2(0.5f, 0.5f));
        public static Sprite ItemCarrot() => GetOrCreate("item.carrot", 16, 16, PaintItemCarrot, new Vector2(0.5f, 0.5f));
        public static Sprite ItemPotato() => GetOrCreate("item.potato", 16, 16, PaintItemPotato, new Vector2(0.5f, 0.5f));
        public static Sprite ItemPineapple() => GetOrCreate("item.pineapple", 16, 16, PaintItemPineapple, new Vector2(0.5f, 0.5f));
        public static Sprite ItemTomato() => GetOrCreate("item.tomato", 16, 16, PaintItemTomato, new Vector2(0.5f, 0.5f));
        public static Sprite ItemFenceWood() => GetOrCreate("item.fence-wood", 16, 16, PaintItemFenceWood, new Vector2(0.5f, 0.5f));
        public static Sprite ItemGateWood() => GetOrCreate("item.gate-wood", 16, 16, PaintItemGateWood, new Vector2(0.5f, 0.5f));
        public static Sprite ItemFishingRod() => GetOrCreate("item.fishing-rod", 16, 16, PaintItemFishingRod, new Vector2(0.5f, 0.5f));
        public static Sprite ItemFishingBait() => GetOrCreate("item.fishing-bait", 16, 16, PaintItemFishingBait, new Vector2(0.5f, 0.5f));
        public static Sprite ItemFishSalmon() => GetOrCreate("item.fish-salmon", 16, 16, PaintItemFishSalmon, new Vector2(0.5f, 0.5f));
        public static Sprite ItemFishCarp() => GetOrCreate("item.fish-carp", 16, 16, PaintItemFishCarp, new Vector2(0.5f, 0.5f));
        public static Sprite ItemFishGoldenPerch() => GetOrCreate("item.fish-golden-perch", 16, 16, PaintItemFishGoldenPerch, new Vector2(0.5f, 0.5f));
        public static Sprite ItemCookedFish() => GetOrCreate("item.cooked-fish", 16, 16, PaintItemCookedFish, new Vector2(0.5f, 0.5f));
        public static Sprite ItemWeaponSword() => GetOrCreate("item.weapon-sword", 16, 16, PaintItemWeaponSword, new Vector2(0.5f, 0.5f));
        public static Sprite ItemWeaponBow() => GetOrCreate("item.weapon-bow", 16, 16, PaintItemWeaponBow, new Vector2(0.5f, 0.5f));
        public static Sprite ItemAmmoArrow() => GetOrCreate("item.ammo-arrow", 16, 16, PaintItemAmmoArrow, new Vector2(0.5f, 0.5f));
        public static Sprite ItemShieldWood() => GetOrCreate("item.shield-wood", 16, 16, PaintItemShieldWood, new Vector2(0.5f, 0.5f));
        public static Sprite ItemArmorKnight() => GetOrCreate("item.armor-knight", 16, 16, PaintItemArmorKnight, new Vector2(0.5f, 0.5f));
        public static Sprite ItemMeatRaw() => GetOrCreate("item.meat-raw", 16, 16, PaintItemMeatRaw, new Vector2(0.5f, 0.5f));
        public static Sprite ItemLeather() => GetOrCreate("item.leather", 16, 16, PaintItemLeather, new Vector2(0.5f, 0.5f));
        public static Sprite ItemHay() => GetOrCreate("item.hay", 16, 16, PaintItemHay, new Vector2(0.5f, 0.5f));
        public static Sprite ItemFarmDeed() => GetOrCreate("item.farm-deed", 16, 16, PaintItemFarmDeed, new Vector2(0.5f, 0.5f));
        public static Sprite ItemFlour() => GetOrCreate("item.flour", 16, 16, PaintItemFlour, new Vector2(0.5f, 0.5f));
        public static Sprite ItemCheese() => GetOrCreate("item.cheese", 16, 16, PaintItemCheese, new Vector2(0.5f, 0.5f));
        public static Sprite ItemCloth() => GetOrCreate("item.cloth", 16, 16, PaintItemCloth, new Vector2(0.5f, 0.5f));
        public static Sprite ItemWine() => GetOrCreate("item.wine", 16, 16, PaintItemWine, new Vector2(0.5f, 0.5f));
        public static Sprite ItemJuice() => GetOrCreate("item.juice", 16, 16, PaintItemJuice, new Vector2(0.5f, 0.5f));
        public static Sprite ItemIronBar() => GetOrCreate("item.iron-bar", 16, 16, PaintItemIronBar, new Vector2(0.5f, 0.5f));
        public static Sprite ItemFertilizer() => GetOrCreate("item.fertilizer", 16, 16, PaintItemFertilizer, new Vector2(0.5f, 0.5f));
        public static Sprite ItemGrape() => GetOrCreate("item.grape", 16, 16, PaintItemGrape, new Vector2(0.5f, 0.5f));
        public static Sprite ItemPumpkin() => GetOrCreate("item.pumpkin", 16, 16, PaintItemPumpkin, new Vector2(0.5f, 0.5f));

        public static Sprite SlashArcSprite => GetOrCreate("vfx.slash.arc", 24, 24, PaintSlashArc, new Vector2(0.5f, 0.5f));
        public static Sprite ArrowSprite => GetOrCreate("vfx.arrow", 16, 8, PaintArrowSprite, new Vector2(0.5f, 0.5f));
        public static Sprite FishingBobberSprite => GetOrCreate("vfx.fishing.bobber", 12, 12, PaintFishingBobber, new Vector2(0.5f, 0.5f));
        public static Sprite WaterSplashSprite => GetOrCreate("vfx.water.splash", 16, 16, PaintWaterSplash, new Vector2(0.5f, 0.5f));
        public static Sprite WolfSprite(int frame) => GetOrCreate("enemy.wolf." + Mathf.Abs(frame % 4), 22, 16, pixels => PaintWolf(pixels, Mathf.Abs(frame % 4)), new Vector2(0.5f, 0.15f));
        public static Sprite BanditSprite(int frame) => GetOrCreate("enemy.bandit." + Mathf.Abs(frame % 4), 16, 22, pixels => PaintBandit(pixels, Mathf.Abs(frame % 4)), new Vector2(0.5f, 0.16f));
        public static Sprite ShadowStalkerSprite(int frame) => GetOrCreate("enemy.stalker." + Mathf.Abs(frame % 4), 20, 24, pixels => PaintShadowStalker(pixels, Mathf.Abs(frame % 4)), new Vector2(0.5f, 0.16f));

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
                case "item.tool-hoe": return ItemToolHoe();
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
                case "item.seed-potato": return ItemSeedPotato();
                case "item.seed-pineapple": return ItemSeedPineapple();
                case "item.seed-tomato": return ItemSeedTomato();
                case "item.wheat": return ItemWheat();
                case "item.corn": return ItemCorn();
                case "item.carrot": return ItemCarrot();
                case "item.potato": return ItemPotato();
                case "item.pineapple": return ItemPineapple();
                case "item.tomato": return ItemTomato();
                case "item.fence-wood": return ItemFenceWood();
                case "item.gate-wood": return ItemGateWood();
                case "item.fishing-rod": return ItemFishingRod();
                case "item.fishing-bait": return ItemFishingBait();
                case "item.fish-salmon": return ItemFishSalmon();
                case "item.fish-carp": return ItemFishCarp();
                case "item.fish-golden-perch": return ItemFishGoldenPerch();
                case "item.cooked-fish": return ItemCookedFish();
                case "item.weapon-sword": return ItemWeaponSword();
                case "item.weapon-bow": return ItemWeaponBow();
                case "item.ammo-arrow": return ItemAmmoArrow();
                case "item.shield-wood": return ItemShieldWood();
                case "item.armor-knight": return ItemArmorKnight();
                case "item.meat-raw": return ItemMeatRaw();
                case "item.leather": return ItemLeather();
                case "item.hay": return ItemHay();
                case "item.farm-deed": return ItemFarmDeed();
                case "item.flour": return ItemFlour();
                case "item.cheese": return ItemCheese();
                case "item.cloth": return ItemCloth();
                case "item.wine": return ItemWine();
                case "item.juice": return ItemJuice();
                case "item.iron-bar": return ItemIronBar();
                case "item.fertilizer": return ItemFertilizer();
                case "item.grape": return ItemGrape();
                case "item.pumpkin": return ItemPumpkin();
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
            Color32 floorA = new Color32(148, 102, 62, 255);
            Color32 floorB = new Color32(165, 116, 72, 255);
            Color32 seam = new Color32(108, 72, 42, 255);
            Color32 wallShadow = new Color32(65, 42, 28, 255);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    bool plank = ((y / 8) + (x / 24)) % 2 == 0;
                    pixels[y * width + x] = plank ? floorA : floorB;
                    if (y % 8 == 0 || x % 24 == 0) pixels[y * width + x] = seam;
                }
            }

            FillRect(pixels, width, height, 0, height - 8, width, 8, wallShadow);
            // Center cozy woven round rug
            FillRect(pixels, width, height, 60, 36, 56, 36, new Color32(145, 62, 62, 255));
            DrawRect(pixels, width, height, 60, 36, 115, 71, new Color32(215, 175, 85, 255));
            FillRect(pixels, width, height, 72, 44, 32, 20, new Color32(175, 82, 82, 255));
        }

        private static void PaintCottageFloor1(Color32[] pixels)
        {
            const int width = 176;
            const int height = 112;
            Color32 slateA = new Color32(105, 112, 120, 255);
            Color32 slateB = new Color32(88, 94, 102, 255);
            Color32 mortar = new Color32(52, 56, 62, 255);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    bool stone = ((y / 12) + (x / 18)) % 2 == 0;
                    pixels[y * width + x] = stone ? slateA : slateB;
                    if (y % 12 == 0 || (y / 12 % 2 == 0 ? x % 18 == 0 : (x + 9) % 18 == 0)) pixels[y * width + x] = mortar;
                }
            }
            FillRect(pixels, width, height, 0, height - 8, width, 8, new Color32(40, 44, 48, 255));
        }

        private static void PaintCottageWall1(Color32[] pixels)
        {
            const int width = 176;
            const int height = 48;
            Color32 stoneA = new Color32(82, 86, 92, 255);
            Color32 stoneB = new Color32(98, 102, 108, 255);
            Color32 mortar = new Color32(50, 52, 56, 255);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    bool brick = ((y / 8) + (x / 16)) % 2 == 0;
                    pixels[y * width + x] = brick ? stoneA : stoneB;
                    if (y % 8 == 0 || (y / 8 % 2 == 0 ? x % 16 == 0 : (x + 8) % 16 == 0)) pixels[y * width + x] = mortar;
                }
            }
            // Arched window in the middle
            FillRect(pixels, width, height, 72, 14, 32, 24, new Color32(110, 185, 235, 255));
            DrawRect(pixels, width, height, 72, 14, 103, 37, new Color32(45, 48, 52, 255));
            FillRect(pixels, width, height, 87, 14, 2, 24, new Color32(45, 48, 52, 255));
            FillRect(pixels, width, height, 72, 25, 32, 2, new Color32(45, 48, 52, 255));
        }

        private static void PaintCottageFloor2(Color32[] pixels)
        {
            const int width = 176;
            const int height = 112;
            Color32 woodA = new Color32(165, 125, 82, 255);
            Color32 woodB = new Color32(182, 142, 98, 255);
            Color32 seam = new Color32(120, 88, 55, 255);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    bool diag = ((x + y) / 10) % 2 == 0;
                    pixels[y * width + x] = diag ? woodA : woodB;
                    if ((x + y) % 10 == 0 || (x - y + 500) % 10 == 0) pixels[y * width + x] = seam;
                }
            }
            FillRect(pixels, width, height, 0, height - 8, width, 8, new Color32(75, 52, 32, 255));
            // Master bedroom Persian carpet
            FillRect(pixels, width, height, 38, 26, 100, 56, new Color32(138, 48, 62, 255));
            DrawRect(pixels, width, height, 38, 26, 137, 81, new Color32(225, 185, 95, 255));
            FillRect(pixels, width, height, 50, 36, 76, 36, new Color32(168, 62, 78, 255));
            DrawRect(pixels, width, height, 50, 36, 125, 71, new Color32(240, 210, 130, 255));
        }

        private static void PaintCottageWall2(Color32[] pixels)
        {
            const int width = 176;
            const int height = 48;
            Color32 wallA = new Color32(135, 98, 64, 255);
            Color32 wallB = new Color32(152, 114, 78, 255);
            Color32 beam = new Color32(88, 58, 35, 255);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    pixels[y * width + x] = ((x / 12 + y / 6) % 2 == 0) ? wallA : wallB;
                    if (x % 36 < 3 || y < 4 || y > height - 5) pixels[y * width + x] = beam;
                }
            }
            // Double balcony arched window looking out at sky and green pine mountains
            FillRect(pixels, width, height, 60, 10, 56, 30, new Color32(135, 205, 250, 255));
            DrawRect(pixels, width, height, 60, 10, 115, 39, new Color32(65, 42, 25, 255));
            FillRect(pixels, width, height, 87, 10, 2, 30, new Color32(65, 42, 25, 255));
            // Distant mountain silhouette in window
            FillRect(pixels, width, height, 61, 10, 26, 10, new Color32(75, 135, 95, 255));
            FillRect(pixels, width, height, 89, 10, 26, 12, new Color32(65, 125, 85, 255));
        }

        private static void PaintManorFloor1(Color32[] pixels)
        {
            const int width = 176;
            const int height = 112;
            Color32 marbleA = new Color32(235, 238, 242, 255);
            Color32 marbleB = new Color32(70, 75, 85, 255);
            Color32 goldTrim = new Color32(215, 175, 65, 255);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    bool checker = ((x / 14) + (y / 14)) % 2 == 0;
                    pixels[y * width + x] = checker ? marbleA : marbleB;
                    if (x % 14 == 0 || y % 14 == 0) pixels[y * width + x] = new Color32(140, 145, 155, 255);
                }
            }
            FillRect(pixels, width, height, 0, height - 8, width, 8, new Color32(35, 38, 45, 255));
            DrawRect(pixels, width, height, 12, 12, width - 13, height - 13, goldTrim);
        }

        private static void PaintManorWall1(Color32[] pixels)
        {
            const int width = 176;
            const int height = 48;
            Color32 velvetRed = new Color32(125, 28, 38, 255);
            Color32 darkRed = new Color32(95, 18, 28, 255);
            Color32 goldGilt = new Color32(225, 185, 70, 255);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    pixels[y * width + x] = (x / 8 % 2 == 0) ? velvetRed : darkRed;
                    if (y < 4 || y > height - 5) pixels[y * width + x] = goldGilt;
                }
            }
            // Royal golden crown & drapery banner in center
            FillRect(pixels, width, height, 76, 12, 24, 26, new Color32(185, 35, 45, 255));
            DrawRect(pixels, width, height, 76, 12, 99, 37, goldGilt);
            FillRect(pixels, width, height, 84, 24, 8, 8, goldGilt);
        }

        private static void PaintManorFloor2(Color32[] pixels)
        {
            const int width = 176;
            const int height = 112;
            Color32 royalCarpetA = new Color32(145, 28, 42, 255);
            Color32 royalCarpetB = new Color32(165, 35, 50, 255);
            Color32 goldBorder = new Color32(230, 190, 75, 255);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    pixels[y * width + x] = (y % 4 == 0) ? royalCarpetA : royalCarpetB;
                }
            }
            FillRect(pixels, width, height, 0, height - 8, width, 8, new Color32(65, 15, 22, 255));
            DrawRect(pixels, width, height, 8, 8, width - 9, height - 9, goldBorder);
            DrawRect(pixels, width, height, 12, 12, width - 13, height - 13, goldBorder);
        }

        private static void PaintManorWall2(Color32[] pixels)
        {
            const int width = 176;
            const int height = 48;
            Color32 wallBase = new Color32(68, 48, 62, 255);
            Color32 goldGilt = new Color32(225, 185, 70, 255);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    pixels[y * width + x] = wallBase;
                    if (y < 4 || y > height - 5) pixels[y * width + x] = goldGilt;
                }
            }
            // Stained glass cathedral window
            FillRect(pixels, width, height, 68, 8, 40, 34, new Color32(60, 140, 210, 255));
            DrawRect(pixels, width, height, 68, 8, 107, 41, goldGilt);
            FillRect(pixels, width, height, 76, 16, 24, 18, new Color32(220, 80, 110, 255));
            FillRect(pixels, width, height, 84, 22, 8, 8, new Color32(245, 215, 80, 255));
        }

        private static void PaintWindmillFloor1(Color32[] pixels)
        {
            const int width = 176;
            const int height = 112;
            Color32 stone = new Color32(115, 105, 95, 255);
            Color32 flourDust = new Color32(195, 185, 170, 255);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    pixels[y * width + x] = (x + y) % 12 < 2 ? flourDust : stone;
                }
            }
            FillRect(pixels, width, height, 0, height - 8, width, 8, new Color32(55, 50, 45, 255));
            // Giant wooden gear on the ground
            FillRect(pixels, width, height, 64, 32, 48, 48, new Color32(125, 82, 45, 255));
            DrawRect(pixels, width, height, 64, 32, 111, 79, new Color32(75, 48, 25, 255));
            FillRect(pixels, width, height, 80, 48, 16, 16, new Color32(65, 65, 70, 255));
        }

        private static void PaintWindmillWall1(Color32[] pixels)
        {
            const int width = 176;
            const int height = 48;
            Color32 stone = new Color32(95, 90, 85, 255);
            Color32 gearWood = new Color32(115, 75, 42, 255);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    pixels[y * width + x] = stone;
                    if (y > 20 && y < 28) pixels[y * width + x] = gearWood; // Shaft
                }
            }
        }

        private static void PaintWindmillFloor2(Color32[] pixels)
        {
            const int width = 176;
            const int height = 112;
            Color32 woodA = new Color32(142, 102, 65, 255);
            Color32 woodB = new Color32(125, 88, 55, 255);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    pixels[y * width + x] = (y / 6 % 2 == 0) ? woodA : woodB;
                }
            }
            FillRect(pixels, width, height, 0, height - 8, width, 8, new Color32(65, 45, 28, 255));
        }

        private static void PaintWindmillWall2(Color32[] pixels)
        {
            const int width = 176;
            const int height = 48;
            Color32 timber = new Color32(105, 72, 45, 255);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    pixels[y * width + x] = timber;
                }
            }
            // Round porthole window looking at golden wheat valley
            FillRect(pixels, width, height, 76, 12, 24, 24, new Color32(235, 195, 85, 255));
            DrawRect(pixels, width, height, 76, 12, 99, 35, new Color32(55, 38, 24, 255));
        }

        private static void PaintGreenhouseFloor(Color32[] pixels)
        {
            const int width = 176;
            const int height = 112;
            Color32 brickA = new Color32(165, 95, 68, 255);
            Color32 brickB = new Color32(145, 80, 55, 255);
            Color32 richSoil = new Color32(58, 38, 24, 255);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    pixels[y * width + x] = (x / 8 % 2 == 0) ? brickA : brickB;
                }
            }
            // 4 Large indoor soil garden beds
            FillRect(pixels, width, height, 16, 18, 56, 32, richSoil);
            DrawRect(pixels, width, height, 16, 18, 71, 49, brickB);
            FillRect(pixels, width, height, 104, 18, 56, 32, richSoil);
            DrawRect(pixels, width, height, 104, 18, 159, 49, brickB);
            FillRect(pixels, width, height, 16, 62, 56, 32, richSoil);
            DrawRect(pixels, width, height, 16, 62, 71, 93, brickB);
            FillRect(pixels, width, height, 104, 62, 56, 32, richSoil);
            DrawRect(pixels, width, height, 104, 62, 159, 93, brickB);
        }

        private static void PaintGreenhouseWall(Color32[] pixels)
        {
            const int width = 176;
            const int height = 48;
            Color32 glass = new Color32(145, 215, 245, 255);
            Color32 frame = new Color32(65, 115, 75, 255);
            Color32 ivy = new Color32(45, 135, 55, 255);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    pixels[y * width + x] = glass;
                    if (x % 22 < 3 || y < 4 || y > height - 5) pixels[y * width + x] = frame;
                    if (y > 32 && (x % 8 < 4)) pixels[y * width + x] = ivy;
                }
            }
        }

        private static void PaintTentFloor(Color32[] pixels)
        {
            const int width = 176;
            const int height = 112;
            Color32 canvas = new Color32(138, 125, 95, 255);
            Color32 sheepskin = new Color32(225, 218, 198, 255);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    pixels[y * width + x] = canvas;
                }
            }
            FillRect(pixels, width, height, 48, 28, 80, 56, sheepskin);
            DrawRect(pixels, width, height, 48, 28, 127, 83, new Color32(185, 175, 155, 255));
        }

        private static void PaintTentWall(Color32[] pixels)
        {
            const int width = 176;
            const int height = 48;
            Color32 canvas = new Color32(120, 108, 82, 255);
            Color32 pole = new Color32(75, 55, 35, 255);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    pixels[y * width + x] = canvas;
                    if (x > 84 && x < 92) pixels[y * width + x] = pole;
                }
            }
        }

        private static void PaintStorageShedFloor(Color32[] pixels)
        {
            const int width = 176;
            const int height = 112;
            Color32 timberA = new Color32(115, 78, 48, 255);
            Color32 timberB = new Color32(98, 65, 38, 255);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    pixels[y * width + x] = (x / 16 % 2 == 0) ? timberA : timberB;
                }
            }
            FillRect(pixels, width, height, 0, height - 8, width, 8, new Color32(50, 32, 18, 255));
        }

        private static void PaintStorageShedWall(Color32[] pixels)
        {
            const int width = 176;
            const int height = 48;
            Color32 wall = new Color32(85, 58, 35, 255);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    pixels[y * width + x] = wall;
                    if (y % 6 == 0 && x % 8 == 0) pixels[y * width + x] = new Color32(40, 25, 15, 255); // Peg holes
                }
            }
            // Hanging saw & hammer
            FillRect(pixels, width, height, 36, 16, 20, 4, new Color32(165, 170, 175, 255));
            FillRect(pixels, width, height, 80, 14, 4, 20, new Color32(120, 80, 45, 255));
            FillRect(pixels, width, height, 76, 30, 12, 6, new Color32(65, 70, 75, 255));
        }

        private static void PaintHerbalistFloor(Color32[] pixels)
        {
            const int width = 176;
            const int height = 112;
            Color32 parquetA = new Color32(135, 95, 62, 255);
            Color32 parquetB = new Color32(118, 82, 52, 255);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    pixels[y * width + x] = (y / 8 % 2 == 0) ? parquetA : parquetB;
                }
            }
            FillRect(pixels, width, height, 0, height - 8, width, 8, new Color32(55, 38, 24, 255));
        }

        private static void PaintHerbalistWall(Color32[] pixels)
        {
            const int width = 176;
            const int height = 48;
            Color32 thatch = new Color32(115, 88, 55, 255);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    pixels[y * width + x] = thatch;
                }
            }
            // Hanging lavender & herbs
            FillRect(pixels, width, height, 24, 18, 6, 18, new Color32(145, 115, 195, 255));
            FillRect(pixels, width, height, 48, 18, 6, 18, new Color32(85, 165, 95, 255));
            FillRect(pixels, width, height, 120, 18, 6, 18, new Color32(215, 185, 75, 255));
            FillRect(pixels, width, height, 144, 18, 6, 18, new Color32(195, 85, 75, 255));
        }

        private static void PaintStairsWoodUp(Color32[] pixels)
        {
            const int w = 32;
            const int h = 36;
            for (int step = 0; step < 6; step++)
            {
                int y0 = step * 6;
                FillRect(pixels, w, h, 4, y0, 24, 4, new Color32(175, 125, 75, 255));
                FillRect(pixels, w, h, 4, y0 + 4, 24, 2, new Color32(115, 75, 42, 255));
            }
            // Handrail
            FillRect(pixels, w, h, 2, 0, 3, h, new Color32(85, 55, 30, 255));
            FillRect(pixels, w, h, 27, 0, 3, h, new Color32(85, 55, 30, 255));
        }

        private static void PaintStairsWoodDown(Color32[] pixels)
        {
            PaintStairsWoodUp(pixels);
        }

        private static void PaintStairsGrandManorUp(Color32[] pixels)
        {
            const int w = 42;
            const int h = 40;
            for (int step = 0; step < 7; step++)
            {
                int y0 = step * 5;
                FillRect(pixels, w, h, 2, y0, 38, 3, new Color32(230, 230, 235, 255)); // Marble
                FillRect(pixels, w, h, 12, y0, 18, 3, new Color32(175, 35, 45, 255)); // Red carpet runner
            }
            // Gold balustrade
            FillRect(pixels, w, h, 2, 0, 3, h, new Color32(225, 185, 70, 255));
            FillRect(pixels, w, h, 37, 0, 3, h, new Color32(225, 185, 70, 255));
        }

        private static void PaintStairsGrandManorDown(Color32[] pixels)
        {
            PaintStairsGrandManorUp(pixels);
        }

        private static void PaintMillerLadder(Color32[] pixels)
        {
            const int w = 22;
            const int h = 40;
            FillRect(pixels, w, h, 3, 0, 3, h, new Color32(125, 82, 45, 255));
            FillRect(pixels, w, h, 16, 0, 3, h, new Color32(125, 82, 45, 255));
            for (int y = 4; y < h; y += 6)
            {
                FillRect(pixels, w, h, 3, y, 16, 3, new Color32(165, 115, 65, 255));
            }
        }

        private static void PaintRoyalCanopyBed(Color32[] pixels)
        {
            const int w = 44;
            const int h = 34;
            // 4 golden posts
            FillRect(pixels, w, h, 2, 0, 3, h, new Color32(225, 185, 70, 255));
            FillRect(pixels, w, h, 39, 0, 3, h, new Color32(225, 185, 70, 255));
            FillRect(pixels, w, h, 2, h - 4, 40, 4, new Color32(225, 185, 70, 255));
            // Velvet mattress & pillows
            FillRect(pixels, w, h, 5, 4, 34, 18, new Color32(165, 35, 48, 255));
            FillRect(pixels, w, h, 8, 14, 12, 7, new Color32(245, 240, 230, 255));
            FillRect(pixels, w, h, 24, 14, 12, 7, new Color32(245, 240, 230, 255));
            FillRect(pixels, w, h, 5, 4, 34, 10, new Color32(135, 25, 38, 255));
        }

        private static void PaintMillstoneGrinder(Color32[] pixels)
        {
            const int w = 38;
            const int h = 34;
            FillRect(pixels, w, h, 4, 2, 30, 16, new Color32(115, 118, 122, 255));
            FillRect(pixels, w, h, 8, 18, 22, 8, new Color32(145, 148, 152, 255));
            FillRect(pixels, w, h, 14, 26, 10, 6, new Color32(185, 135, 75, 255)); // Wooden hopper
            FillRect(pixels, w, h, 2, 2, 8, 4, new Color32(245, 245, 235, 255)); // Flour pile
        }

        private static void PaintIndoorPlanterBed(Color32[] pixels)
        {
            const int w = 38;
            const int h = 24;
            FillRect(pixels, w, h, 2, 2, 34, 20, new Color32(125, 78, 45, 255));
            FillRect(pixels, w, h, 4, 4, 30, 16, new Color32(65, 42, 24, 255)); // Soil
            // Green sprouts
            FillRect(pixels, w, h, 8, 10, 6, 8, new Color32(75, 185, 65, 255));
            FillRect(pixels, w, h, 18, 12, 6, 6, new Color32(95, 215, 85, 255));
            FillRect(pixels, w, h, 26, 9, 6, 9, new Color32(65, 175, 55, 255));
        }

        private static void PaintApothecaryStation(Color32[] pixels)
        {
            const int w = 38;
            const int h = 28;
            FillRect(pixels, w, h, 2, 2, 34, 14, new Color32(105, 68, 38, 255));
            // Glowing potion bottles
            FillRect(pixels, w, h, 6, 16, 6, 10, new Color32(65, 215, 185, 255));
            FillRect(pixels, w, h, 16, 16, 6, 8, new Color32(235, 75, 125, 255));
            FillRect(pixels, w, h, 26, 16, 8, 8, new Color32(185, 188, 192, 255)); // Mortar
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

        private static void PaintItemHoe(Color32[] pixels)
        {
            Color32 haftDark = new Color32(66, 40, 22, 255);
            Color32 haft = new Color32(124, 78, 42, 255);
            Color32 steelDark = new Color32(60, 68, 78, 255);
            Color32 steel = new Color32(116, 128, 142, 255);
            Color32 blade = new Color32(204, 218, 232, 255);

            for (int i = 0; i < 11; i++)
            {
                SetPixel(pixels, 16, 16, 3 + i, 2 + i, haftDark);
                SetPixel(pixels, 16, 16, 4 + i, 2 + i, haft);
            }

            FillRect(pixels, 16, 16, 11, 11, 3, 3, steelDark);
            FillRect(pixels, 16, 16, 11, 8, 4, 3, steel);
            FillRect(pixels, 16, 16, 12, 6, 3, 3, blade);
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
                // Stage 0: Tiny seed sprout in dirt
                FillRect(pixels, 24, 28, 11, 4, 2, 3, sproutGreen);
                SetPixel(pixels, 24, 28, 10, 6, sproutGreen);
                SetPixel(pixels, 24, 28, 13, 6, sproutGreen);
                return;
            }

            if (stage == 1)
            {
                // Stage 1: Small growing 2-leaf shoot
                FillRect(pixels, 24, 28, 11, 4, 2, 7, stalkGreen);
                FillRect(pixels, 24, 28, 8, 7, 4, 3, sproutGreen);
                FillRect(pixels, 24, 28, 12, 9, 5, 3, sproutGreen);
                return;
            }

            if (cropId == "crop.wheat" || cropId == "wheat")
            {
                Color32 golden = stage >= 4 ? new Color32(245, 205, 75, 255) : (stage >= 3 ? new Color32(220, 185, 60, 255) : new Color32(165, 190, 60, 255));
                Color32 wheatDark = stage >= 4 ? new Color32(195, 140, 35, 255) : (stage >= 3 ? new Color32(175, 130, 30, 255) : darkGreen);

                FillRect(pixels, 24, 28, 10, 4, 4, 14, stalkGreen);
                FillRect(pixels, 24, 28, 7, 8, 4, 2, darkGreen);
                FillRect(pixels, 24, 28, 13, 11, 4, 2, darkGreen);
                if (stage >= 2)
                {
                    FillRect(pixels, 24, 28, 8, 14, 8, 6, golden);
                    FillRect(pixels, 24, 28, 9, 16, 6, 4, wheatDark);
                }
                if (stage >= 3)
                {
                    FillRect(pixels, 24, 28, 8, 16, 8, 8, golden);
                    FillRect(pixels, 24, 28, 9, 18, 6, 6, wheatDark);
                    FillRect(pixels, 24, 28, 10, 23, 4, 4, golden);
                }
                if (stage >= 4)
                {
                    // Full golden glow and extra grain stalks
                    FillRect(pixels, 24, 28, 6, 18, 12, 8, golden);
                    FillRect(pixels, 24, 28, 7, 20, 10, 6, wheatDark);
                    FillRect(pixels, 24, 28, 9, 25, 6, 3, new Color32(255, 225, 110, 255));
                }
            }
            else if (cropId == "crop.potato" || cropId == "potato")
            {
                Color32 potatoSkin = stage >= 4 ? new Color32(205, 160, 95, 255) : new Color32(175, 130, 75, 255);
                Color32 potatoDark = new Color32(140, 95, 45, 255);
                Color32 flowerWhite = new Color32(245, 245, 250, 255);
                Color32 flowerYellow = new Color32(245, 215, 50, 255);

                // Bushy dark green potato foliage
                FillRect(pixels, 24, 28, 8, 4, 8, 12, darkGreen);
                FillRect(pixels, 24, 28, 5, 8, 14, 6, sproutGreen);
                FillRect(pixels, 24, 28, 4, 11, 16, 5, darkGreen);

                if (stage >= 2)
                {
                    // Small potato flowers blooming
                    SetPixel(pixels, 24, 28, 7, 17, flowerWhite);
                    SetPixel(pixels, 24, 28, 8, 17, flowerYellow);
                    SetPixel(pixels, 24, 28, 15, 16, flowerWhite);
                    SetPixel(pixels, 24, 28, 16, 16, flowerYellow);
                }

                if (stage >= 3)
                {
                    // Potato tubers emerging at soil level
                    FillRect(pixels, 24, 28, 6, 3, 5, 4, potatoDark);
                    FillRect(pixels, 24, 28, 7, 4, 3, 2, potatoSkin);
                }

                if (stage >= 4)
                {
                    // Big ripe potatoes plump and ready to harvest
                    FillRect(pixels, 24, 28, 5, 2, 7, 5, potatoDark);
                    FillRect(pixels, 24, 28, 6, 3, 5, 3, potatoSkin);
                    SetPixel(pixels, 24, 28, 8, 5, new Color32(235, 195, 130, 255));

                    FillRect(pixels, 24, 28, 13, 2, 6, 5, potatoDark);
                    FillRect(pixels, 24, 28, 14, 3, 4, 3, potatoSkin);
                }
            }
            else if (cropId == "crop.corn" || cropId == "corn")
            {
                Color32 cornYellow = new Color32(245, 215, 45, 255);
                Color32 husk = new Color32(120, 185, 55, 255);

                FillRect(pixels, 24, 28, 10, 4, 4, 18, darkGreen);
                FillRect(pixels, 24, 28, 4, 8, 7, 3, husk);
                FillRect(pixels, 24, 28, 13, 11, 7, 3, husk);
                FillRect(pixels, 24, 28, 5, 15, 6, 3, husk);

                if (stage >= 2)
                {
                    FillRect(pixels, 24, 28, 12, 14, 5, 7, cornYellow);
                    FillRect(pixels, 24, 28, 11, 13, 4, 4, husk);
                }
                if (stage >= 3)
                {
                    FillRect(pixels, 24, 28, 7, 18, 5, 7, cornYellow);
                    FillRect(pixels, 24, 28, 13, 20, 3, 2, new Color32(185, 100, 40, 255));
                }
                if (stage >= 4)
                {
                    FillRect(pixels, 24, 28, 6, 17, 6, 8, cornYellow);
                    FillRect(pixels, 24, 28, 12, 13, 6, 8, cornYellow);
                    FillRect(pixels, 24, 28, 8, 24, 4, 3, new Color32(255, 235, 80, 255));
                }
            }
            else if (cropId == "crop.pineapple" || cropId == "pineapple")
            {
                Color32 pineGold = new Color32(245, 185, 35, 255);
                Color32 pineDark = new Color32(190, 130, 20, 255);
                Color32 pineSpike = new Color32(85, 190, 50, 255);

                FillRect(pixels, 24, 28, 4, 3, 5, 4, pineSpike);
                FillRect(pixels, 24, 28, 15, 3, 5, 4, pineSpike);
                FillRect(pixels, 24, 28, 2, 7, 6, 3, pineSpike);
                FillRect(pixels, 24, 28, 16, 7, 6, 3, pineSpike);

                if (stage >= 2)
                {
                    FillRect(pixels, 24, 28, 7, 6, 10, 11, pineDark);
                    FillRect(pixels, 24, 28, 8, 7, 8, 9, pineGold);
                    SetPixel(pixels, 24, 28, 9, 13, pineDark);
                    SetPixel(pixels, 24, 28, 11, 14, pineDark);
                    SetPixel(pixels, 24, 28, 13, 13, pineDark);
                    SetPixel(pixels, 24, 28, 10, 10, pineDark);
                    SetPixel(pixels, 24, 28, 12, 10, pineDark);

                    FillRect(pixels, 24, 28, 10, 17, 4, 7, pineSpike);
                    FillRect(pixels, 24, 28, 7, 18, 4, 5, pineSpike);
                    FillRect(pixels, 24, 28, 13, 18, 4, 5, pineSpike);
                }
                if (stage >= 4)
                {
                    FillRect(pixels, 24, 28, 6, 6, 12, 13, pineDark);
                    FillRect(pixels, 24, 28, 7, 7, 10, 11, new Color32(255, 205, 50, 255));
                }
            }
            else if (cropId == "crop.tomato" || cropId == "tomato")
            {
                Color32 tomatoRed = new Color32(235, 45, 30, 255);
                Color32 tomatoDark = new Color32(175, 25, 20, 255);
                Color32 calyx = new Color32(85, 185, 50, 255);

                FillRect(pixels, 24, 28, 6, 4, 12, 14, darkGreen);
                FillRect(pixels, 24, 28, 4, 8, 16, 8, stalkGreen);

                if (stage >= 2)
                {
                    FillRect(pixels, 24, 28, 6, 6, 5, 5, tomatoDark);
                    FillRect(pixels, 24, 28, 7, 7, 4, 4, tomatoRed);
                    SetPixel(pixels, 24, 28, 7, 10, calyx);
                }
                if (stage >= 3)
                {
                    FillRect(pixels, 24, 28, 13, 9, 6, 6, tomatoDark);
                    FillRect(pixels, 24, 28, 14, 10, 5, 5, tomatoRed);
                    SetPixel(pixels, 24, 28, 15, 14, calyx);
                }
                if (stage >= 4)
                {
                    FillRect(pixels, 24, 28, 9, 13, 7, 7, tomatoDark);
                    FillRect(pixels, 24, 28, 10, 14, 6, 6, new Color32(255, 55, 35, 255));
                    SetPixel(pixels, 24, 28, 11, 19, calyx);
                }
            }
            else if (cropId == "crop.strawberry" || cropId == "strawberry")
            {
                Color32 strawRed = new Color32(245, 35, 55, 255);
                Color32 strawDark = new Color32(185, 20, 35, 255);
                Color32 whiteSeed = new Color32(255, 245, 210, 255);

                FillRect(pixels, 24, 28, 6, 4, 12, 10, darkGreen);
                FillRect(pixels, 24, 28, 4, 6, 16, 6, sproutGreen);

                if (stage >= 2)
                {
                    FillRect(pixels, 24, 28, 7, 5, 4, 4, strawDark);
                    FillRect(pixels, 24, 28, 8, 6, 3, 3, strawRed);
                }
                if (stage >= 3)
                {
                    FillRect(pixels, 24, 28, 13, 7, 5, 5, strawDark);
                    FillRect(pixels, 24, 28, 14, 8, 4, 4, strawRed);
                    SetPixel(pixels, 24, 28, 15, 9, whiteSeed);
                }
                if (stage >= 4)
                {
                    FillRect(pixels, 24, 28, 9, 8, 7, 7, strawDark);
                    FillRect(pixels, 24, 28, 10, 9, 6, 6, strawRed);
                    SetPixel(pixels, 24, 28, 11, 11, whiteSeed);
                    SetPixel(pixels, 24, 28, 13, 13, whiteSeed);
                }
            }
            else if (cropId == "crop.pumpkin" || cropId == "pumpkin")
            {
                Color32 pumpOrange = new Color32(255, 145, 20, 255);
                Color32 pumpDark = new Color32(195, 90, 10, 255);

                FillRect(pixels, 24, 28, 4, 4, 16, 12, darkGreen);
                FillRect(pixels, 24, 28, 6, 14, 12, 6, sproutGreen);

                if (stage >= 2)
                {
                    FillRect(pixels, 24, 28, 8, 4, 8, 6, pumpDark);
                    FillRect(pixels, 24, 28, 9, 5, 6, 5, pumpOrange);
                }
                if (stage >= 3)
                {
                    FillRect(pixels, 24, 28, 6, 4, 12, 10, pumpDark);
                    FillRect(pixels, 24, 28, 7, 5, 10, 8, pumpOrange);
                }
                if (stage >= 4)
                {
                    // Giant Pumpkin
                    FillRect(pixels, 24, 28, 4, 3, 16, 14, pumpDark);
                    FillRect(pixels, 24, 28, 5, 4, 14, 12, pumpOrange);
                    FillRect(pixels, 24, 28, 11, 16, 2, 4, darkGreen); // stem
                }
            }
            else if (cropId == "crop.apple-tree" || cropId == "apple-tree")
            {
                Color32 trunk = new Color32(110, 70, 40, 255);
                Color32 appleRed = new Color32(235, 30, 30, 255);

                FillRect(pixels, 24, 28, 10, 2, 4, 16, trunk);
                FillRect(pixels, 24, 28, 5, 12, 14, 12, darkGreen);
                FillRect(pixels, 24, 28, 7, 14, 10, 10, sproutGreen);

                if (stage >= 4)
                {
                    // Apples hanging on tree
                    FillRect(pixels, 24, 28, 7, 15, 3, 3, appleRed);
                    FillRect(pixels, 24, 28, 14, 16, 3, 3, appleRed);
                    FillRect(pixels, 24, 28, 10, 20, 3, 3, appleRed);
                }
            }
            else // Carrot
            {
                Color32 carrotOrange = stage >= 4 ? new Color32(255, 125, 30, 255) : new Color32(235, 105, 25, 255);
                Color32 carrotDark = new Color32(185, 70, 15, 255);

                FillRect(pixels, 24, 28, 7, 10, 10, 8, sproutGreen);
                FillRect(pixels, 24, 28, 9, 14, 6, 6, darkGreen);
                FillRect(pixels, 24, 28, 6, 12, 4, 4, sproutGreen);
                FillRect(pixels, 24, 28, 14, 12, 4, 4, sproutGreen);

                if (stage >= 2)
                {
                    FillRect(pixels, 24, 28, 10, 4, 4, 5, carrotOrange);
                    FillRect(pixels, 24, 28, 11, 2, 2, 3, carrotDark);
                }
                if (stage >= 3)
                {
                    FillRect(pixels, 24, 28, 9, 4, 6, 6, carrotOrange);
                    SetPixel(pixels, 24, 28, 10, 9, new Color32(255, 165, 80, 255));
                }
                if (stage >= 4)
                {
                    // Big bright carrot top popping out of soil
                    FillRect(pixels, 24, 28, 8, 3, 8, 7, carrotDark);
                    FillRect(pixels, 24, 28, 9, 4, 6, 6, carrotOrange);
                    SetPixel(pixels, 24, 28, 10, 9, new Color32(255, 185, 90, 255));
                    FillRect(pixels, 24, 28, 5, 14, 14, 8, darkGreen);
                    FillRect(pixels, 24, 28, 7, 18, 10, 6, sproutGreen);
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

        private static void PaintItemPotato(Color32[] pixels)
        {
            Color32 skin = new Color32(185, 140, 75, 255);
            Color32 dark = new Color32(135, 95, 45, 255);
            Color32 eye = new Color32(95, 60, 25, 255);

            FillRect(pixels, 16, 16, 3, 4, 10, 8, dark);
            FillRect(pixels, 16, 16, 4, 5, 8, 6, skin);
            SetPixel(pixels, 16, 16, 3, 4, new Color32(0, 0, 0, 0));
            SetPixel(pixels, 16, 16, 12, 11, new Color32(0, 0, 0, 0));

            // Potato eyes
            SetPixel(pixels, 16, 16, 6, 8, eye);
            SetPixel(pixels, 16, 16, 9, 6, eye);
            SetPixel(pixels, 16, 16, 7, 5, eye);
        }

        private static void PaintItemFishingRod(Color32[] pixels)
        {
            Color32 bamboo = new Color32(185, 150, 80, 255);
            Color32 line = new Color32(230, 235, 240, 255);
            Color32 reel = new Color32(90, 95, 105, 255);

            for (int i = 0; i < 11; i++)
            {
                SetPixel(pixels, 16, 16, 3 + i, 2 + i, bamboo);
            }
            FillRect(pixels, 16, 16, 4, 3, 3, 3, reel);
            SetPixel(pixels, 16, 16, 13, 11, line);
            SetPixel(pixels, 16, 16, 13, 8, line);
            SetPixel(pixels, 16, 16, 13, 5, new Color32(240, 60, 40, 255));
        }

        private static void PaintItemFishingBait(Color32[] pixels)
        {
            Color32 worm = new Color32(215, 110, 90, 255);
            Color32 soil = new Color32(95, 60, 35, 255);
            FillRect(pixels, 16, 16, 3, 2, 10, 4, soil);
            SetPixel(pixels, 16, 16, 5, 5, worm);
            SetPixel(pixels, 16, 16, 6, 6, worm);
            SetPixel(pixels, 16, 16, 7, 7, worm);
            SetPixel(pixels, 16, 16, 8, 8, worm);
            SetPixel(pixels, 16, 16, 9, 7, worm);
            SetPixel(pixels, 16, 16, 10, 8, worm);
            SetPixel(pixels, 16, 16, 11, 9, worm);
        }

        private static void PaintItemFishSalmon(Color32[] pixels)
        {
            Color32 scale = new Color32(235, 105, 85, 255);
            Color32 belly = new Color32(250, 195, 180, 255);
            Color32 fin = new Color32(195, 75, 60, 255);

            FillRect(pixels, 16, 16, 3, 5, 9, 6, scale);
            FillRect(pixels, 16, 16, 4, 4, 7, 2, belly);
            SetPixel(pixels, 16, 16, 1, 8, fin);
            SetPixel(pixels, 16, 16, 2, 7, fin);
            SetPixel(pixels, 16, 16, 2, 9, fin);
            SetPixel(pixels, 16, 16, 10, 8, Color.black);
            SetPixel(pixels, 16, 16, 11, 8, Color.white);
            SetPixel(pixels, 16, 16, 6, 10, fin);
            SetPixel(pixels, 16, 16, 7, 10, fin);
        }

        private static void PaintItemFishCarp(Color32[] pixels)
        {
            Color32 scale = new Color32(110, 155, 100, 255);
            Color32 belly = new Color32(210, 225, 185, 255);
            Color32 fin = new Color32(85, 125, 75, 255);

            FillRect(pixels, 16, 16, 3, 5, 9, 6, scale);
            FillRect(pixels, 16, 16, 4, 4, 7, 2, belly);
            SetPixel(pixels, 16, 16, 1, 8, fin);
            SetPixel(pixels, 16, 16, 2, 7, fin);
            SetPixel(pixels, 16, 16, 2, 9, fin);
            SetPixel(pixels, 16, 16, 10, 8, Color.black);
            SetPixel(pixels, 16, 16, 6, 10, fin);
        }

        private static void PaintItemFishGoldenPerch(Color32[] pixels)
        {
            Color32 gold = new Color32(245, 205, 45, 255);
            Color32 shine = new Color32(255, 245, 160, 255);
            Color32 fin = new Color32(215, 140, 25, 255);

            FillRect(pixels, 16, 16, 3, 5, 9, 6, gold);
            FillRect(pixels, 16, 16, 5, 7, 5, 2, shine);
            SetPixel(pixels, 16, 16, 1, 8, fin);
            SetPixel(pixels, 16, 16, 2, 7, fin);
            SetPixel(pixels, 16, 16, 2, 9, fin);
            SetPixel(pixels, 16, 16, 10, 8, Color.black);
            SetPixel(pixels, 16, 16, 11, 8, Color.white);
            SetPixel(pixels, 16, 16, 6, 11, fin);
        }

        private static void PaintItemCookedFish(Color32[] pixels)
        {
            Color32 plate = new Color32(215, 210, 195, 255);
            Color32 grilled = new Color32(195, 120, 50, 255);
            Color32 herb = new Color32(85, 165, 55, 255);

            FillRect(pixels, 16, 16, 2, 2, 12, 3, plate);
            FillRect(pixels, 16, 16, 3, 5, 10, 5, grilled);
            SetPixel(pixels, 16, 16, 5, 7, new Color32(110, 60, 20, 255));
            SetPixel(pixels, 16, 16, 8, 7, new Color32(110, 60, 20, 255));
            SetPixel(pixels, 16, 16, 6, 9, herb);
            SetPixel(pixels, 16, 16, 7, 9, herb);
        }

        private static void PaintItemWeaponSword(Color32[] pixels)
        {
            Color32 steel = new Color32(215, 225, 235, 255);
            Color32 hilt = new Color32(215, 175, 50, 255);
            Color32 grip = new Color32(105, 55, 30, 255);

            for (int i = 0; i < 9; i++)
            {
                SetPixel(pixels, 16, 16, 5 + i, 5 + i, steel);
                SetPixel(pixels, 16, 16, 5 + i, 6 + i, Color.white);
            }
            SetPixel(pixels, 16, 16, 4, 6, hilt);
            SetPixel(pixels, 16, 16, 6, 4, hilt);
            SetPixel(pixels, 16, 16, 5, 5, hilt);
            SetPixel(pixels, 16, 16, 3, 3, grip);
            SetPixel(pixels, 16, 16, 2, 2, hilt);
        }

        private static void PaintItemWeaponBow(Color32[] pixels)
        {
            Color32 wood = new Color32(145, 85, 40, 255);
            Color32 stringCol = new Color32(230, 230, 230, 255);

            SetPixel(pixels, 16, 16, 4, 13, wood);
            SetPixel(pixels, 16, 16, 6, 12, wood);
            SetPixel(pixels, 16, 16, 7, 10, wood);
            SetPixel(pixels, 16, 16, 7, 6, wood);
            SetPixel(pixels, 16, 16, 6, 4, wood);
            SetPixel(pixels, 16, 16, 4, 3, wood);

            for (int y = 3; y <= 13; y++)
            {
                SetPixel(pixels, 16, 16, 3, y, stringCol);
            }
        }

        private static void PaintItemAmmoArrow(Color32[] pixels)
        {
            Color32 shaft = new Color32(175, 125, 75, 255);
            Color32 tip = new Color32(195, 205, 215, 255);
            Color32 feather = new Color32(245, 245, 245, 255);

            for (int i = 0; i < 9; i++) SetPixel(pixels, 16, 16, 3 + i, 3 + i, shaft);
            SetPixel(pixels, 16, 16, 12, 12, tip);
            SetPixel(pixels, 16, 16, 11, 13, tip);
            SetPixel(pixels, 16, 16, 13, 11, tip);
            SetPixel(pixels, 16, 16, 2, 4, feather);
            SetPixel(pixels, 16, 16, 4, 2, feather);
        }

        private static void PaintItemShieldWood(Color32[] pixels)
        {
            Color32 wood = new Color32(145, 95, 50, 255);
            Color32 ironRim = new Color32(75, 80, 90, 255);
            Color32 boss = new Color32(215, 175, 50, 255);

            FillRect(pixels, 16, 16, 3, 3, 10, 10, wood);
            DrawRect(pixels, 16, 16, 3, 3, 12, 12, ironRim);
            FillRect(pixels, 16, 16, 7, 7, 2, 2, boss);
        }

        private static void PaintItemArmorKnight(Color32[] pixels)
        {
            Color32 plate = new Color32(195, 205, 218, 255);
            Color32 goldTrim = new Color32(235, 185, 45, 255);
            Color32 shadow = new Color32(95, 105, 120, 255);

            FillRect(pixels, 16, 16, 4, 3, 8, 9, plate);
            FillRect(pixels, 16, 16, 2, 8, 3, 4, shadow);
            FillRect(pixels, 16, 16, 11, 8, 3, 4, shadow);
            FillRect(pixels, 16, 16, 5, 11, 6, 2, goldTrim);
            SetPixel(pixels, 16, 16, 7, 7, goldTrim);
            SetPixel(pixels, 16, 16, 8, 7, goldTrim);
        }

        private static void PaintItemMeatRaw(Color32[] pixels)
        {
            Color32 meat = new Color32(215, 55, 60, 255);
            Color32 fat = new Color32(245, 220, 220, 255);
            Color32 bone = new Color32(250, 250, 245, 255);

            FillRect(pixels, 16, 16, 4, 4, 8, 7, meat);
            FillRect(pixels, 16, 16, 5, 8, 6, 2, fat);
            FillRect(pixels, 16, 16, 2, 5, 3, 3, bone);
        }

        private static void PaintItemLeather(Color32[] pixels)
        {
            Color32 leather = new Color32(155, 95, 50, 255);
            Color32 dark = new Color32(110, 65, 30, 255);

            FillRect(pixels, 16, 16, 3, 3, 10, 10, leather);
            SetPixel(pixels, 16, 16, 3, 3, new Color32(0, 0, 0, 0));
            SetPixel(pixels, 16, 16, 12, 12, new Color32(0, 0, 0, 0));
            DrawRect(pixels, 16, 16, 4, 4, 11, 11, dark);
        }

        private static void PaintItemHay(Color32[] pixels)
        {
            Color32 hayGold = new Color32(235, 195, 60, 255);
            Color32 hayDark = new Color32(185, 145, 35, 255);
            Color32 rope = new Color32(120, 75, 35, 255);

            FillRect(pixels, 16, 16, 3, 4, 10, 8, hayGold);
            DrawRect(pixels, 16, 16, 3, 4, 12, 11, hayDark);
            FillRect(pixels, 16, 16, 7, 4, 2, 8, rope);
        }

        private static void PaintItemFarmDeed(Color32[] pixels)
        {
            Color32 parchment = new Color32(240, 225, 185, 255);
            Color32 seal = new Color32(195, 35, 35, 255);
            Color32 text = new Color32(95, 75, 55, 255);

            FillRect(pixels, 16, 16, 3, 2, 10, 12, parchment);
            FillRect(pixels, 16, 16, 5, 10, 6, 1, text);
            FillRect(pixels, 16, 16, 5, 8, 6, 1, text);
            FillRect(pixels, 16, 16, 5, 6, 6, 1, text);
            FillRect(pixels, 16, 16, 6, 3, 4, 3, seal);
        }

        private static void PaintItemFlour(Color32[] pixels)
        {
            Color32 sack = new Color32(195, 165, 120, 255);
            Color32 sackDark = new Color32(145, 115, 75, 255);
            Color32 flour = new Color32(250, 250, 245, 255);
            Color32 rope = new Color32(90, 55, 25, 255);

            FillRect(pixels, 16, 16, 3, 2, 10, 10, sack);
            DrawRect(pixels, 16, 16, 3, 2, 10, 10, sackDark);
            FillRect(pixels, 16, 16, 5, 11, 6, 3, flour);
            FillRect(pixels, 16, 16, 4, 9, 8, 1, rope);
        }

        private static void PaintItemCheese(Color32[] pixels)
        {
            Color32 yellow = new Color32(245, 205, 50, 255);
            Color32 darkYellow = new Color32(205, 155, 30, 255);
            Color32 hole = new Color32(180, 130, 20, 255);

            FillRect(pixels, 16, 16, 2, 3, 12, 8, yellow);
            DrawRect(pixels, 16, 16, 2, 3, 12, 8, darkYellow);
            SetPixel(pixels, 16, 16, 5, 6, hole);
            SetPixel(pixels, 16, 16, 8, 8, hole);
            SetPixel(pixels, 16, 16, 10, 5, hole);
        }

        private static void PaintItemCloth(Color32[] pixels)
        {
            Color32 cloth = new Color32(110, 175, 235, 255);
            Color32 shadow = new Color32(70, 125, 185, 255);
            Color32 thread = new Color32(235, 245, 255, 255);

            FillRect(pixels, 16, 16, 2, 4, 12, 8, cloth);
            DrawRect(pixels, 16, 16, 2, 4, 12, 8, shadow);
            FillRect(pixels, 16, 16, 5, 6, 6, 4, thread);
        }

        private static void PaintItemWine(Color32[] pixels)
        {
            Color32 glass = new Color32(45, 75, 45, 255);
            Color32 wine = new Color32(165, 25, 45, 255);
            Color32 cork = new Color32(195, 145, 85, 255);
            Color32 label = new Color32(235, 220, 185, 255);

            FillRect(pixels, 16, 16, 5, 2, 6, 8, wine);
            FillRect(pixels, 16, 16, 7, 10, 2, 4, glass);
            SetPixel(pixels, 16, 16, 7, 14, cork);
            SetPixel(pixels, 16, 16, 8, 14, cork);
            FillRect(pixels, 16, 16, 6, 4, 4, 3, label);
            DrawRect(pixels, 16, 16, 5, 2, 6, 8, glass);
        }

        private static void PaintItemJuice(Color32[] pixels)
        {
            Color32 glass = new Color32(215, 235, 250, 255);
            Color32 juice = new Color32(235, 55, 105, 255);
            Color32 straw = new Color32(245, 215, 60, 255);

            FillRect(pixels, 16, 16, 4, 2, 8, 10, juice);
            DrawRect(pixels, 16, 16, 4, 2, 8, 10, glass);
            SetPixel(pixels, 16, 16, 9, 12, straw);
            SetPixel(pixels, 16, 16, 10, 14, straw);
        }

        private static void PaintItemIronBar(Color32[] pixels)
        {
            Color32 iron = new Color32(185, 195, 205, 255);
            Color32 ironDark = new Color32(105, 115, 125, 255);
            Color32 shine = new Color32(245, 250, 255, 255);

            FillRect(pixels, 16, 16, 2, 4, 12, 7, iron);
            DrawRect(pixels, 16, 16, 2, 4, 12, 7, ironDark);
            FillRect(pixels, 16, 16, 4, 8, 8, 1, shine);
        }

        private static void PaintItemFertilizer(Color32[] pixels)
        {
            Color32 sack = new Color32(130, 95, 55, 255);
            Color32 sprout = new Color32(65, 185, 65, 255);
            Color32 dark = new Color32(85, 60, 35, 255);

            FillRect(pixels, 16, 16, 3, 2, 10, 11, sack);
            DrawRect(pixels, 16, 16, 3, 2, 10, 11, dark);
            FillRect(pixels, 16, 16, 7, 6, 2, 4, sprout);
            SetPixel(pixels, 16, 16, 6, 8, sprout);
            SetPixel(pixels, 16, 16, 9, 8, sprout);
        }

        private static void PaintItemGrape(Color32[] pixels)
        {
            Color32 purple = new Color32(145, 45, 175, 255);
            Color32 darkPurple = new Color32(95, 25, 125, 255);
            Color32 stem = new Color32(85, 145, 45, 255);

            SetPixel(pixels, 16, 16, 8, 13, stem);
            SetPixel(pixels, 16, 16, 7, 12, stem);
            SetPixel(pixels, 16, 16, 9, 12, stem);

            FillRect(pixels, 16, 16, 6, 9, 5, 3, purple);
            FillRect(pixels, 16, 16, 7, 6, 4, 3, darkPurple);
            FillRect(pixels, 16, 16, 8, 3, 2, 3, purple);
        }

        private static void PaintItemPumpkin(Color32[] pixels)
        {
            Color32 orange = new Color32(235, 125, 25, 255);
            Color32 darkOrange = new Color32(175, 80, 15, 255);
            Color32 stem = new Color32(75, 135, 35, 255);

            FillRect(pixels, 16, 16, 2, 2, 12, 10, orange);
            DrawRect(pixels, 16, 16, 2, 2, 12, 10, darkOrange);
            FillRect(pixels, 16, 16, 6, 2, 4, 10, darkOrange);
            SetPixel(pixels, 16, 16, 8, 12, stem);
            SetPixel(pixels, 16, 16, 8, 13, stem);
        }

        private static void PaintFishingBobber(Color32[] pixels)
        {
            Color32 red = new Color32(235, 45, 35, 255);
            Color32 white = new Color32(245, 245, 250, 255);
            Color32 tip = new Color32(35, 35, 35, 255);

            FillRect(pixels, 12, 12, 4, 6, 4, 4, red);
            FillRect(pixels, 12, 12, 4, 2, 4, 4, white);
            SetPixel(pixels, 12, 12, 5, 10, tip);
            SetPixel(pixels, 12, 12, 6, 10, tip);
        }

        private static void PaintWaterSplash(Color32[] pixels)
        {
            Color32 foam = new Color32(240, 248, 255, 230);
            Color32 water = new Color32(120, 200, 255, 200);

            DrawRect(pixels, 16, 16, 2, 4, 13, 11, foam);
            DrawRect(pixels, 16, 16, 4, 6, 11, 9, water);
            SetPixel(pixels, 16, 16, 7, 13, foam);
            SetPixel(pixels, 16, 16, 8, 14, Color.white);
            SetPixel(pixels, 16, 16, 9, 13, foam);
        }

        private static void PaintArrowSprite(Color32[] pixels)
        {
            Color32 shaft = new Color32(175, 125, 75, 255);
            Color32 tip = new Color32(220, 230, 240, 255);
            Color32 feather = new Color32(245, 245, 245, 255);

            for (int x = 2; x < 13; x++)
            {
                SetPixel(pixels, 16, 8, x, 3, shaft);
                SetPixel(pixels, 16, 8, x, 4, shaft);
            }
            SetPixel(pixels, 16, 8, 13, 2, tip);
            SetPixel(pixels, 16, 8, 14, 3, tip);
            SetPixel(pixels, 16, 8, 15, 3, Color.white);
            SetPixel(pixels, 16, 8, 15, 4, Color.white);
            SetPixel(pixels, 16, 8, 14, 4, tip);
            SetPixel(pixels, 16, 8, 13, 5, tip);
            SetPixel(pixels, 16, 8, 1, 1, feather);
            SetPixel(pixels, 16, 8, 2, 2, feather);
            SetPixel(pixels, 16, 8, 1, 6, feather);
            SetPixel(pixels, 16, 8, 2, 5, feather);
        }

        private static void PaintShadowStalker(Color32[] pixels, int frame)
        {
            Color32 shadow = new Color32(25, 18, 38, 255);
            Color32 deepShadow = new Color32(12, 8, 20, 255);
            Color32 eyeGlow = new Color32(235, 55, 75, 255);
            Color32 mist = new Color32(110, 45, 145, 160);

            int bob = (frame % 2 == 0) ? 0 : 1;

            FillRect(pixels, 20, 24, 5, 4 + bob, 10, 14, deepShadow);
            FillRect(pixels, 20, 24, 6, 6 + bob, 8, 11, shadow);

            SetPixel(pixels, 20, 24, 8, 15 + bob, eyeGlow);
            SetPixel(pixels, 20, 24, 11, 15 + bob, eyeGlow);

            SetPixel(pixels, 20, 24, 5, 2 + (frame % 3), mist);
            SetPixel(pixels, 20, 24, 8, 1 + ((frame + 1) % 3), mist);
            SetPixel(pixels, 20, 24, 12, 2 + (frame % 2), mist);
            SetPixel(pixels, 20, 24, 14, 1 + (frame % 3), mist);
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

        private static void PaintFluffySheep(Color32[] pixels)
        {
            Color32 wool = new Color32(245, 245, 240, 255);
            Color32 woolShade = new Color32(205, 205, 195, 255);
            Color32 blackFace = new Color32(45, 42, 45, 255);
            Color32 hoof = new Color32(30, 28, 30, 255);

            // Fluffy Body
            FillRect(pixels, 24, 18, 5, 4, 15, 10, wool);
            FillRect(pixels, 24, 18, 4, 6, 17, 7, wool);
            FillRect(pixels, 24, 18, 6, 3, 13, 2, woolShade);
            FillRect(pixels, 24, 18, 19, 7, 2, 5, woolShade);

            // Wool texture puffs
            SetPixel(pixels, 24, 18, 8, 14, wool);
            SetPixel(pixels, 24, 18, 11, 14, wool);
            SetPixel(pixels, 24, 18, 14, 14, wool);
            SetPixel(pixels, 24, 18, 17, 13, wool);

            // Legs
            FillRect(pixels, 24, 18, 6, 0, 2, 4, blackFace);
            FillRect(pixels, 24, 18, 6, 0, 2, 1, hoof);
            FillRect(pixels, 24, 18, 10, 0, 2, 4, blackFace);
            FillRect(pixels, 24, 18, 10, 0, 2, 1, hoof);
            FillRect(pixels, 24, 18, 15, 0, 2, 4, blackFace);
            FillRect(pixels, 24, 18, 15, 0, 2, 1, hoof);
            FillRect(pixels, 24, 18, 18, 0, 2, 4, blackFace);
            FillRect(pixels, 24, 18, 18, 0, 2, 1, hoof);

            // Head
            FillRect(pixels, 24, 18, 1, 7, 5, 6, blackFace);
            SetPixel(pixels, 24, 18, 2, 10, Color.white);
            SetPixel(pixels, 24, 18, 3, 10, Color.black);
            FillRect(pixels, 24, 18, 4, 12, 3, 3, wool); // Forelock wool

            // Cute drooping ears
            SetPixel(pixels, 24, 18, 5, 8, blackFace);
            SetPixel(pixels, 24, 18, 5, 7, blackFace);
        }

        private static void PaintHen(Color32[] pixels)
        {
            Color32 brown = new Color32(185, 120, 60, 255);
            Color32 darkBrown = new Color32(135, 80, 35, 255);
            Color32 lightBreast = new Color32(230, 180, 120, 255);
            Color32 redComb = new Color32(225, 45, 40, 255);
            Color32 yellowBeak = new Color32(245, 200, 50, 255);
            Color32 yellowLeg = new Color32(235, 175, 40, 255);

            // Body
            FillRect(pixels, 14, 14, 2, 3, 8, 6, brown);
            FillRect(pixels, 14, 14, 3, 4, 4, 4, lightBreast);
            FillRect(pixels, 14, 14, 7, 6, 4, 4, darkBrown); // Wing

            // Tail feathers pointing up
            SetPixel(pixels, 14, 14, 9, 8, darkBrown);
            SetPixel(pixels, 14, 14, 10, 9, darkBrown);
            SetPixel(pixels, 14, 14, 10, 10, brown);

            // Legs
            FillRect(pixels, 14, 14, 4, 0, 1, 3, yellowLeg);
            FillRect(pixels, 14, 14, 7, 0, 1, 3, yellowLeg);
            SetPixel(pixels, 14, 14, 3, 0, yellowLeg);
            SetPixel(pixels, 14, 14, 6, 0, yellowLeg);

            // Head & Comb
            FillRect(pixels, 14, 14, 1, 8, 4, 4, brown);
            FillRect(pixels, 14, 14, 2, 12, 2, 2, redComb);
            SetPixel(pixels, 14, 14, 1, 7, redComb); // Wattle
            SetPixel(pixels, 14, 14, 0, 9, yellowBeak); // Beak
            SetPixel(pixels, 14, 14, 2, 10, Color.black); // Eye
        }

        private static void PaintHayBalePile(Color32[] pixels)
        {
            Color32 straw = new Color32(225, 185, 60, 255);
            Color32 strawDark = new Color32(165, 125, 35, 255);
            Color32 strawLight = new Color32(245, 215, 100, 255);
            Color32 twine = new Color32(105, 65, 30, 255);

            // Bottom Left Bale (14x12)
            FillRect(pixels, 28, 22, 1, 1, 13, 10, strawDark);
            FillRect(pixels, 28, 22, 2, 2, 11, 8, straw);
            FillRect(pixels, 28, 22, 4, 4, 7, 5, strawLight);
            FillRect(pixels, 28, 22, 4, 2, 1, 8, twine);
            FillRect(pixels, 28, 22, 10, 2, 1, 8, twine);

            // Bottom Right Bale (14x12)
            FillRect(pixels, 28, 22, 14, 1, 13, 10, strawDark);
            FillRect(pixels, 28, 22, 15, 2, 11, 8, straw);
            FillRect(pixels, 28, 22, 17, 4, 7, 5, strawLight);
            FillRect(pixels, 28, 22, 17, 2, 1, 8, twine);
            FillRect(pixels, 28, 22, 23, 2, 1, 8, twine);

            // Top Centered Bale (14x10)
            FillRect(pixels, 28, 22, 7, 11, 14, 10, strawDark);
            FillRect(pixels, 28, 22, 8, 12, 12, 8, straw);
            FillRect(pixels, 28, 22, 10, 14, 8, 5, strawLight);
            FillRect(pixels, 28, 22, 10, 12, 1, 8, twine);
            FillRect(pixels, 28, 22, 17, 12, 1, 8, twine);
        }

        private static void PaintFeedingTrough(Color32[] pixels)
        {
            Color32 wood = new Color32(115, 75, 40, 255);
            Color32 woodDark = new Color32(75, 45, 25, 255);
            Color32 hay = new Color32(230, 190, 65, 255);
            Color32 hayLight = new Color32(250, 220, 100, 255);

            // Wooden legs
            FillRect(pixels, 26, 14, 2, 0, 3, 5, woodDark);
            FillRect(pixels, 26, 14, 21, 0, 3, 5, woodDark);

            // Trough Box
            FillRect(pixels, 26, 14, 1, 4, 24, 9, woodDark);
            FillRect(pixels, 26, 14, 2, 5, 22, 7, wood);

            // Golden Hay Fill
            FillRect(pixels, 26, 14, 3, 7, 20, 5, hay);
            FillRect(pixels, 26, 14, 5, 9, 16, 4, hayLight);
            SetPixel(pixels, 26, 14, 4, 12, hayLight);
            SetPixel(pixels, 26, 14, 8, 13, hay);
            SetPixel(pixels, 26, 14, 14, 13, hayLight);
            SetPixel(pixels, 26, 14, 19, 12, hay);
        }

        private static void PaintWaterTrough(Color32[] pixels)
        {
            Color32 wood = new Color32(115, 75, 40, 255);
            Color32 woodDark = new Color32(75, 45, 25, 255);
            Color32 waterDark = new Color32(45, 110, 190, 255);
            Color32 water = new Color32(70, 165, 235, 255);
            Color32 waterLight = new Color32(175, 225, 255, 255);

            // Wooden legs
            FillRect(pixels, 26, 14, 2, 0, 3, 5, woodDark);
            FillRect(pixels, 26, 14, 21, 0, 3, 5, woodDark);

            // Trough Box
            FillRect(pixels, 26, 14, 1, 4, 24, 9, woodDark);
            FillRect(pixels, 26, 14, 2, 5, 22, 7, wood);

            // Water
            FillRect(pixels, 26, 14, 3, 6, 20, 6, waterDark);
            FillRect(pixels, 26, 14, 4, 7, 18, 4, water);
            FillRect(pixels, 26, 14, 6, 9, 8, 2, waterLight);
            SetPixel(pixels, 26, 14, 17, 9, waterLight);
        }

        private static void PaintBulletinBoard(Color32[] pixels)
        {
            Color32 woodDark = new Color32(85, 50, 25, 255);
            Color32 woodMid = new Color32(145, 95, 45, 255);
            Color32 woodRoof = new Color32(110, 65, 30, 255);
            Color32 cork = new Color32(205, 160, 95, 255);
            Color32 paper1 = new Color32(245, 240, 220, 255);
            Color32 paper2 = new Color32(250, 245, 230, 255);
            Color32 redSeal = new Color32(215, 45, 35, 255);
            Color32 goldPin = new Color32(245, 210, 60, 255);

            // Two support posts (32x36)
            FillRect(pixels, 32, 36, 4, 0, 3, 20, woodDark);
            FillRect(pixels, 32, 36, 25, 0, 3, 20, woodDark);

            // Wooden Board Backing (28x22)
            FillRect(pixels, 32, 36, 2, 10, 28, 22, woodDark);
            FillRect(pixels, 32, 36, 4, 12, 24, 18, cork);

            // Little Gabled Roof Overhang (32x5)
            FillRect(pixels, 32, 36, 0, 31, 32, 4, woodRoof);
            FillRect(pixels, 32, 36, 1, 32, 30, 2, woodMid);

            // Notice Paper 1 (Left - 9x11)
            FillRect(pixels, 32, 36, 6, 15, 9, 11, paper1);
            SetPixel(pixels, 32, 36, 10, 25, redSeal); // Red wax seal pin
            // Scribbled lines
            FillRect(pixels, 32, 36, 8, 21, 5, 1, new Color32(120, 100, 70, 255));
            FillRect(pixels, 32, 36, 8, 19, 5, 1, new Color32(120, 100, 70, 255));
            FillRect(pixels, 32, 36, 8, 17, 4, 1, new Color32(120, 100, 70, 255));

            // Notice Paper 2 (Right - 10x12)
            FillRect(pixels, 32, 36, 17, 14, 10, 12, paper2);
            SetPixel(pixels, 32, 36, 21, 25, goldPin); // Golden pin
            // Scribbled lines & quest star
            FillRect(pixels, 32, 36, 19, 21, 6, 1, new Color32(100, 80, 50, 255));
            FillRect(pixels, 32, 36, 19, 19, 6, 1, new Color32(100, 80, 50, 255));
            FillRect(pixels, 32, 36, 19, 17, 4, 1, new Color32(100, 80, 50, 255));
        }

        private static void PaintMailbox(Color32[] pixels, bool hasMail)
        {
            Color32 post = new Color32(95, 60, 30, 255);
            Color32 boxDark = new Color32(130, 85, 45, 255);
            Color32 boxMid = new Color32(175, 120, 65, 255);
            Color32 redFlag = new Color32(230, 45, 40, 255);
            Color32 white = new Color32(250, 250, 245, 255);
            Color32 yellowWax = new Color32(245, 200, 40, 255);

            // Post (20x28)
            FillRect(pixels, 20, 28, 8, 0, 4, 16, post);

            // Mailbox Barrel (14x12)
            FillRect(pixels, 20, 28, 3, 14, 14, 11, boxDark);
            FillRect(pixels, 20, 28, 4, 15, 12, 9, boxMid);

            // Mailbox door curve
            FillRect(pixels, 20, 28, 3, 18, 14, 4, boxDark);

            if (hasMail)
            {
                // Red Flag UP
                FillRect(pixels, 20, 28, 1, 16, 2, 8, redFlag);
                FillRect(pixels, 20, 28, 0, 22, 4, 3, redFlag);

                // Letter Peeking Out
                FillRect(pixels, 20, 28, 7, 16, 8, 6, white);
                SetPixel(pixels, 20, 28, 10, 18, yellowWax);
            }
            else
            {
                // Red Flag DOWN
                FillRect(pixels, 20, 28, 1, 16, 2, 2, redFlag);
                FillRect(pixels, 20, 28, 0, 14, 3, 3, redFlag);
            }
        }

        private static void PaintMailLetter(Color32[] pixels)
        {
            Color32 parchment = new Color32(250, 245, 230, 255);
            Color32 border = new Color32(185, 160, 120, 255);
            Color32 seal = new Color32(220, 45, 40, 255);
            Color32 gold = new Color32(250, 215, 60, 255);

            // Envelope (18x18)
            FillRect(pixels, 18, 18, 1, 2, 16, 13, border);
            FillRect(pixels, 18, 18, 2, 3, 14, 11, parchment);

            // Envelope fold lines
            SetPixel(pixels, 18, 18, 3, 12, border);
            SetPixel(pixels, 18, 18, 4, 11, border);
            SetPixel(pixels, 18, 18, 5, 10, border);
            SetPixel(pixels, 18, 18, 6, 9, border);
            SetPixel(pixels, 18, 18, 14, 12, border);
            SetPixel(pixels, 18, 18, 13, 11, border);
            SetPixel(pixels, 18, 18, 12, 10, border);
            SetPixel(pixels, 18, 18, 11, 9, border);

            // Wax Seal
            FillRect(pixels, 18, 18, 7, 6, 4, 4, seal);
            SetPixel(pixels, 18, 18, 8, 7, gold);
        }

        private static void PaintCompendiumBadge(Color32[] pixels)
        {
            Color32 gold = new Color32(255, 215, 55, 255);
            Color32 goldDark = new Color32(195, 145, 25, 255);
            Color32 ribbonBlue = new Color32(45, 115, 220, 255);
            Color32 ribbonDark = new Color32(25, 65, 150, 255);

            // Ribbon Tails
            FillRect(pixels, 18, 18, 4, 1, 3, 6, ribbonDark);
            FillRect(pixels, 18, 18, 11, 1, 3, 6, ribbonDark);
            SetPixel(pixels, 18, 18, 5, 0, Color.clear);
            SetPixel(pixels, 18, 18, 12, 0, Color.clear);

            // Star Medal
            FillRect(pixels, 18, 18, 4, 6, 10, 10, goldDark);
            FillRect(pixels, 18, 18, 5, 7, 8, 8, gold);
            FillRect(pixels, 18, 18, 7, 9, 4, 4, ribbonBlue);
        }

        private static void PaintRewardChestIcon(Color32[] pixels)
        {
            Color32 wood = new Color32(165, 110, 50, 255);
            Color32 woodDark = new Color32(105, 65, 25, 255);
            Color32 gold = new Color32(255, 215, 50, 255);

            // Chest Base
            FillRect(pixels, 18, 18, 2, 2, 14, 12, woodDark);
            FillRect(pixels, 18, 18, 3, 3, 12, 10, wood);

            // Gold Trims
            FillRect(pixels, 18, 18, 2, 2, 14, 2, gold);
            FillRect(pixels, 18, 18, 2, 9, 14, 2, gold);
            FillRect(pixels, 18, 18, 8, 6, 2, 3, gold); // Keyhole
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

        private static void PaintCursorPointer(Color32[] pixels)
        {
            Color32 shadow = new Color32(20, 15, 12, 220);
            Color32 outline = new Color32(40, 30, 20, 255);
            Color32 gold = new Color32(245, 195, 60, 255);
            Color32 fill = new Color32(255, 250, 235, 255);
            Color32 inner = new Color32(230, 220, 195, 255);

            // Classic gauntlet fantasy arrow pointer (Top-left at x=1, y=18)
            for (int y = 0; y < 16; y++)
            {
                int len = Mathf.Max(1, 14 - y);
                int xStart = 2 + y / 3;
                FillRect(pixels, 20, 20, xStart + 1, 17 - y - 1, len, 1, shadow);
            }

            // Outline & Gold Border
            for (int row = 0; row < 13; row++)
            {
                int w = Mathf.Max(1, 11 - row);
                FillRect(pixels, 20, 20, 1 + row / 3, 17 - row, w + 1, 1, outline);
            }

            // Core fill
            for (int row = 1; row < 11; row++)
            {
                int w = Mathf.Max(1, 9 - row);
                FillRect(pixels, 20, 20, 2 + row / 3, 17 - row, w, 1, row == 1 ? gold : (row < 4 ? fill : inner));
            }

            // Gold accent tip & gem
            SetPixel(pixels, 20, 20, 1, 17, gold);
            SetPixel(pixels, 20, 20, 2, 16, fill);
            SetPixel(pixels, 20, 20, 3, 15, gold);
            SetPixel(pixels, 20, 20, 4, 14, fill);
        }

        private static void PaintCursorAxe(Color32[] pixels)
        {
            Color32 wood = new Color32(145, 95, 45, 255);
            Color32 woodLight = new Color32(195, 140, 75, 255);
            Color32 steel = new Color32(180, 195, 205, 255);
            Color32 steelLight = new Color32(240, 245, 255, 255);
            Color32 steelDark = new Color32(95, 110, 125, 255);

            // Diagonal wood handle
            for (int i = 0; i < 14; i++)
            {
                SetPixel(pixels, 20, 20, 2 + i, 2 + i, wood);
                SetPixel(pixels, 20, 20, 2 + i, 3 + i, woodLight);
            }

            // Axe blade head at top-right
            FillRect(pixels, 20, 20, 11, 12, 6, 6, steelDark);
            FillRect(pixels, 20, 20, 12, 13, 5, 5, steel);
            FillRect(pixels, 20, 20, 15, 15, 3, 3, steelLight);
            SetPixel(pixels, 20, 20, 17, 17, Color.white);
            SetPixel(pixels, 20, 20, 17, 16, steelLight);
            SetPixel(pixels, 20, 20, 16, 17, steelLight);
        }

        private static void PaintCursorPickaxe(Color32[] pixels)
        {
            Color32 wood = new Color32(145, 95, 45, 255);
            Color32 steel = new Color32(180, 195, 205, 255);
            Color32 steelLight = new Color32(245, 250, 255, 255);
            Color32 steelDark = new Color32(85, 100, 115, 255);

            // Handle
            for (int i = 0; i < 14; i++)
            {
                SetPixel(pixels, 20, 20, 2 + i, 2 + i, wood);
            }

            // Curved pick head
            FillRect(pixels, 20, 20, 9, 13, 8, 3, steelDark);
            FillRect(pixels, 20, 20, 13, 9, 3, 8, steelDark);
            FillRect(pixels, 20, 20, 10, 14, 6, 2, steel);
            FillRect(pixels, 20, 20, 14, 10, 2, 6, steel);
            SetPixel(pixels, 20, 20, 17, 16, steelLight);
            SetPixel(pixels, 20, 20, 16, 17, steelLight);
        }

        private static void PaintCursorHoe(Color32[] pixels)
        {
            Color32 wood = new Color32(145, 95, 45, 255);
            Color32 steel = new Color32(180, 195, 205, 255);
            Color32 steelLight = new Color32(245, 250, 255, 255);

            // Handle
            for (int i = 0; i < 14; i++)
            {
                SetPixel(pixels, 20, 20, 2 + i, 2 + i, wood);
            }

            // Hoe flat blade
            FillRect(pixels, 20, 20, 10, 15, 7, 3, steel);
            FillRect(pixels, 20, 20, 11, 16, 6, 2, steelLight);
        }

        private static void PaintCursorWateringCan(Color32[] pixels)
        {
            Color32 blue = new Color32(50, 150, 225, 255);
            Color32 blueLight = new Color32(120, 205, 255, 255);
            Color32 blueDark = new Color32(25, 90, 155, 255);
            Color32 waterDrop = new Color32(180, 240, 255, 255);

            // Pot body
            FillRect(pixels, 20, 20, 6, 4, 9, 8, blueDark);
            FillRect(pixels, 20, 20, 7, 5, 7, 6, blue);
            FillRect(pixels, 20, 20, 8, 8, 4, 2, blueLight);

            // Handle & Spout
            FillRect(pixels, 20, 20, 12, 9, 4, 2, blueDark);
            FillRect(pixels, 20, 20, 14, 11, 2, 4, blue);
            FillRect(pixels, 20, 20, 2, 10, 5, 3, blueLight);

            // Water droplets
            SetPixel(pixels, 20, 20, 2, 8, waterDrop);
            SetPixel(pixels, 20, 20, 1, 5, waterDrop);
            SetPixel(pixels, 20, 20, 3, 3, waterDrop);
        }

        private static void PaintCursorSeed(Color32[] pixels)
        {
            Color32 sack = new Color32(185, 140, 85, 255);
            Color32 sackDark = new Color32(130, 90, 50, 255);
            Color32 rope = new Color32(235, 200, 110, 255);
            Color32 sprout = new Color32(95, 215, 65, 255);

            FillRect(pixels, 20, 20, 4, 3, 10, 9, sackDark);
            FillRect(pixels, 20, 20, 5, 4, 8, 7, sack);
            FillRect(pixels, 20, 20, 6, 10, 6, 2, rope);
            // Green leaf sprout coming out
            FillRect(pixels, 20, 20, 7, 12, 3, 4, sprout);
            SetPixel(pixels, 20, 20, 6, 15, sprout);
            SetPixel(pixels, 20, 20, 10, 15, sprout);
        }

        private static void PaintCursorHand(Color32[] pixels)
        {
            Color32 skin = new Color32(245, 210, 175, 255);
            Color32 skinDark = new Color32(195, 150, 115, 255);
            Color32 heart = new Color32(245, 60, 85, 255);

            FillRect(pixels, 20, 20, 5, 3, 9, 8, skinDark);
            FillRect(pixels, 20, 20, 6, 4, 7, 6, skin);
            // Pointing index finger
            FillRect(pixels, 20, 20, 9, 11, 3, 6, skin);

            // Small love heart above
            SetPixel(pixels, 20, 20, 13, 15, heart);
            SetPixel(pixels, 20, 20, 15, 15, heart);
            FillRect(pixels, 20, 20, 13, 14, 3, 1, heart);
            SetPixel(pixels, 20, 20, 14, 13, heart);
        }

        private static void PaintCursorHarvest(Color32[] pixels)
        {
            Color32 gold = new Color32(245, 205, 65, 255);
            Color32 goldLight = new Color32(255, 240, 140, 255);
            Color32 wood = new Color32(135, 85, 40, 255);

            // Sickle handle
            FillRect(pixels, 20, 20, 3, 3, 3, 5, wood);

            // Curved golden blade
            FillRect(pixels, 20, 20, 5, 8, 4, 3, gold);
            FillRect(pixels, 20, 20, 8, 11, 4, 4, gold);
            FillRect(pixels, 20, 20, 11, 14, 5, 3, goldLight);
            SetPixel(pixels, 20, 20, 16, 13, goldLight);
            SetPixel(pixels, 20, 20, 15, 11, gold);
        }

        private static void PaintCursorSword(Color32[] pixels)
        {
            Color32 steel = new Color32(205, 220, 235, 255);
            Color32 steelLight = new Color32(255, 255, 255, 255);
            Color32 gold = new Color32(245, 195, 55, 255);

            // Hilt & Pommel
            FillRect(pixels, 20, 20, 2, 2, 4, 4, gold);
            FillRect(pixels, 20, 20, 5, 4, 3, 6, gold);

            // Long blade pointing top-right
            for (int i = 0; i < 11; i++)
            {
                SetPixel(pixels, 20, 20, 6 + i, 6 + i, steel);
                SetPixel(pixels, 20, 20, 6 + i, 7 + i, steelLight);
            }
            SetPixel(pixels, 20, 20, 17, 17, Color.white);
        }

        // ----------------------------------------------------
        // --- PROTOTYPE PIXEL ART: DYNAMIC CATEGORY BUILDINGS ---
        // ----------------------------------------------------

        private static void PaintTent(Color32[] pixels)
        {
            const int w = 40, h = 32;
            Color32 canvas = new Color32(230, 215, 175, 255);
            Color32 canvasShadow = new Color32(185, 168, 130, 255);
            Color32 wood = new Color32(110, 70, 40, 255);
            Color32 darkEntrance = new Color32(40, 26, 16, 255);
            Color32 lanternGlow = new Color32(255, 220, 90, 255);
            Color32 rope = new Color32(140, 110, 70, 255);

            // Ground base stakes
            FillRect(pixels, w, h, 2, 2, 4, 3, wood);
            FillRect(pixels, w, h, 34, 2, 4, 3, wood);

            // Triangle tent shape
            for (int y = 3; y < 27; y++)
            {
                int span = (27 - y) * 16 / 24;
                int left = 20 - span;
                int right = 20 + span;
                for (int x = left; x <= right; x++)
                {
                    Color32 c = (x < 20) ? canvas : canvasShadow;
                    SetPixel(pixels, w, h, x, y, c);
                }
            }

            // Dark entrance opening
            for (int y = 3; y < 18; y++)
            {
                int openSpan = (18 - y) * 7 / 15;
                for (int x = 20 - openSpan; x <= 20 + openSpan; x++)
                {
                    SetPixel(pixels, w, h, x, y, darkEntrance);
                }
            }

            // Warm interior lantern hanging
            FillRect(pixels, w, h, 19, 10, 3, 4, lanternGlow);
            SetPixel(pixels, w, h, 20, 14, wood);

            // Guide ropes
            for (int i = 0; i < 6; i++)
            {
                SetPixel(pixels, w, h, 4 + i, 3 + i * 2, rope);
                SetPixel(pixels, w, h, 36 - i, 3 + i * 2, rope);
            }
        }

        private static void PaintManor(Color32[] pixels)
        {
            const int w = 76, h = 60;
            Color32 stone = new Color32(130, 135, 145, 255);
            Color32 stoneDark = new Color32(85, 90, 100, 255);
            Color32 brick = new Color32(165, 80, 60, 255);
            Color32 brickDark = new Color32(120, 50, 40, 255);
            Color32 slateRoof = new Color32(55, 65, 80, 255);
            Color32 slateLight = new Color32(85, 100, 120, 255);
            Color32 woodDoor = new Color32(95, 55, 30, 255);
            Color32 glass = new Color32(150, 220, 250, 255);
            Color32 goldTrim = new Color32(240, 195, 60, 255);

            // Ground Floor (Stone)
            FillRect(pixels, w, h, 6, 2, 64, 22, stone);
            DrawRect(pixels, w, h, 6, 2, 69, 23, stoneDark);

            // Arched Double Entrance
            FillRect(pixels, w, h, 32, 2, 12, 16, woodDoor);
            DrawRect(pixels, w, h, 31, 2, 44, 18, goldTrim);
            SetPixel(pixels, w, h, 36, 10, goldTrim);
            SetPixel(pixels, w, h, 39, 10, goldTrim);

            // Ground floor windows
            FillRect(pixels, w, h, 14, 8, 8, 10, glass);
            DrawRect(pixels, w, h, 13, 7, 22, 18, stoneDark);
            FillRect(pixels, w, h, 54, 8, 8, 10, glass);
            DrawRect(pixels, w, h, 53, 7, 62, 18, stoneDark);

            // Second Floor (Brick)
            FillRect(pixels, w, h, 8, 24, 60, 18, brick);
            DrawRect(pixels, w, h, 8, 24, 67, 41, brickDark);

            // Balcony in center
            FillRect(pixels, w, h, 30, 24, 16, 4, stone);
            for (int bx = 30; bx <= 45; bx += 3)
            {
                FillRect(pixels, w, h, bx, 28, 2, 5, stoneDark);
            }
            FillRect(pixels, w, h, 30, 33, 16, 2, stoneDark);
            FillRect(pixels, w, h, 34, 28, 8, 10, glass);

            // Second floor side windows
            FillRect(pixels, w, h, 16, 28, 8, 10, glass);
            DrawRect(pixels, w, h, 15, 27, 24, 38, brickDark);
            FillRect(pixels, w, h, 52, 28, 8, 10, glass);
            DrawRect(pixels, w, h, 51, 27, 60, 38, brickDark);

            // Mansard Roof with Slate Tiles
            for (int y = 42; y < 56; y++)
            {
                int inset = (y - 42) * 2;
                FillRect(pixels, w, h, 6 + inset, y, 64 - inset * 2, 1, (y % 2 == 0) ? slateRoof : slateLight);
            }

            // Chimneys
            FillRect(pixels, w, h, 12, 46, 6, 12, brick);
            FillRect(pixels, w, h, 58, 46, 6, 12, brick);
            FillRect(pixels, w, h, 11, 57, 8, 2, brickDark);
            FillRect(pixels, w, h, 57, 57, 8, 2, brickDark);
        }

        private static void PaintGreenhouse(Color32[] pixels)
        {
            const int w = 56, h = 46;
            Color32 stone = new Color32(110, 115, 120, 255);
            Color32 frameWhite = new Color32(245, 240, 230, 255);
            Color32 glass = new Color32(165, 225, 245, 255);
            Color32 glassHighlight = new Color32(215, 245, 255, 255);
            Color32 plantGreen = new Color32(45, 160, 60, 255);
            Color32 plantDark = new Color32(25, 95, 35, 255);
            Color32 flowerRed = new Color32(235, 60, 60, 255);
            Color32 potTerra = new Color32(195, 95, 50, 255);

            // Stone base
            FillRect(pixels, w, h, 4, 2, 48, 6, stone);
            DrawRect(pixels, w, h, 4, 2, 51, 7, new Color32(75, 80, 85, 255));

            // Glass walls
            FillRect(pixels, w, h, 6, 8, 44, 20, glass);

            // Plants visible inside
            FillRect(pixels, w, h, 10, 8, 6, 4, potTerra);
            FillRect(pixels, w, h, 8, 12, 10, 12, plantGreen);
            SetPixel(pixels, w, h, 12, 20, flowerRed);
            SetPixel(pixels, w, h, 15, 18, flowerRed);

            FillRect(pixels, w, h, 24, 8, 8, 14, plantDark);
            FillRect(pixels, w, h, 26, 12, 6, 10, plantGreen);

            FillRect(pixels, w, h, 40, 8, 6, 4, potTerra);
            FillRect(pixels, w, h, 38, 12, 10, 12, plantGreen);
            SetPixel(pixels, w, h, 42, 21, flowerRed);

            // Vertical & Horizontal Frame struts
            for (int fx = 6; fx <= 50; fx += 11)
            {
                FillRect(pixels, w, h, fx, 8, 2, 20, frameWhite);
            }
            FillRect(pixels, w, h, 6, 18, 44, 2, frameWhite);
            FillRect(pixels, w, h, 6, 27, 44, 2, frameWhite);

            // Peaked Glass Roof
            for (int y = 28; y < 44; y++)
            {
                int span = (44 - y) * 22 / 16;
                FillRect(pixels, w, h, 28 - span, y, span * 2, 1, glass);
                SetPixel(pixels, w, h, 28 - span, y, frameWhite);
                SetPixel(pixels, w, h, 28 + span, y, frameWhite);
                if (y % 4 == 0)
                {
                    SetPixel(pixels, w, h, 28 - span / 2, y, glassHighlight);
                    SetPixel(pixels, w, h, 28 + span / 2, y, glassHighlight);
                }
            }
            FillRect(pixels, w, h, 27, 42, 3, 3, frameWhite);
        }

        private static void PaintSilo(Color32[] pixels)
        {
            const int w = 36, h = 56;
            Color32 stone = new Color32(120, 125, 130, 255);
            Color32 metalLight = new Color32(200, 205, 215, 255);
            Color32 metalMid = new Color32(150, 155, 168, 255);
            Color32 metalDark = new Color32(95, 100, 110, 255);
            Color32 ironBand = new Color32(60, 65, 75, 255);
            Color32 roofDome = new Color32(180, 75, 60, 255);

            // Stone base
            FillRect(pixels, w, h, 6, 2, 24, 8, stone);

            // Cylinder Body
            for (int y = 10; y < 44; y++)
            {
                FillRect(pixels, w, h, 6, y, 6, 1, metalLight);
                FillRect(pixels, w, h, 12, y, 12, 1, metalMid);
                FillRect(pixels, w, h, 24, y, 6, 1, metalDark);
            }

            // Horizontal Reinforcing Bands
            for (int by = 12; by < 44; by += 8)
            {
                FillRect(pixels, w, h, 6, by, 24, 2, ironBand);
            }

            // Access Ladder on side
            for (int ly = 6; ly < 44; ly += 3)
            {
                SetPixel(pixels, w, h, 22, ly, Color.black);
                SetPixel(pixels, w, h, 25, ly, Color.black);
                FillRect(pixels, w, h, 22, ly, 4, 1, Color.black);
            }

            // Domed Cap Roof
            for (int y = 44; y < 54; y++)
            {
                int span = Mathf.Clamp(12 - (y - 44), 2, 12);
                FillRect(pixels, w, h, 18 - span, y, span * 2, 1, roofDome);
            }
            FillRect(pixels, w, h, 16, 53, 4, 2, ironBand);
        }

        private static void PaintStreetLamp(Color32[] pixels)
        {
            const int w = 18, h = 36;
            Color32 iron = new Color32(40, 42, 48, 255);
            Color32 glow = new Color32(255, 230, 110, 255);
            Color32 glowAura = new Color32(255, 190, 60, 255);

            // Fluted Base
            FillRect(pixels, w, h, 5, 2, 8, 4, iron);
            FillRect(pixels, w, h, 7, 6, 4, 2, iron);

            // Pole
            FillRect(pixels, w, h, 8, 8, 2, 16, iron);

            // Lantern Head Bracket
            FillRect(pixels, w, h, 6, 24, 6, 2, iron);
            FillRect(pixels, w, h, 5, 26, 8, 6, glow);
            FillRect(pixels, w, h, 6, 27, 6, 4, glowAura);

            // Iron frame around glass
            DrawRect(pixels, w, h, 5, 26, 12, 31, iron);

            // Pointed Cap
            FillRect(pixels, w, h, 6, 32, 6, 2, iron);
            FillRect(pixels, w, h, 8, 34, 2, 2, iron);
        }

        private static void PaintGroundTorch(Color32[] pixels)
        {
            const int w = 14, h = 26;
            Color32 wood = new Color32(110, 75, 45, 255);
            Color32 fireCore = new Color32(255, 245, 160, 255);
            Color32 fireMid = new Color32(255, 140, 40, 255);
            Color32 fireDark = new Color32(210, 45, 20, 255);

            // Wooden stake
            FillRect(pixels, w, h, 6, 2, 2, 14, wood);
            FillRect(pixels, w, h, 5, 14, 4, 3, new Color32(60, 40, 25, 255));

            // Burning Flame
            FillRect(pixels, w, h, 5, 17, 4, 5, fireMid);
            FillRect(pixels, w, h, 6, 18, 2, 3, fireCore);
            SetPixel(pixels, w, h, 4, 19, fireDark);
            SetPixel(pixels, w, h, 9, 20, fireDark);
            SetPixel(pixels, w, h, 6, 23, fireMid);
            SetPixel(pixels, w, h, 7, 24, fireDark);
        }

        private static void PaintLanternPole(Color32[] pixels)
        {
            const int w = 18, h = 38;
            Color32 wood = new Color32(105, 68, 38, 255);
            Color32 iron = new Color32(45, 45, 50, 255);
            Color32 glow = new Color32(255, 225, 95, 255);

            // Pole
            FillRect(pixels, w, h, 4, 2, 3, 30, wood);
            // Curved arm
            FillRect(pixels, w, h, 4, 30, 8, 3, wood);
            FillRect(pixels, w, h, 11, 28, 2, 3, iron);

            // Hanging Lantern
            FillRect(pixels, w, h, 10, 21, 5, 6, glow);
            DrawRect(pixels, w, h, 9, 20, 15, 27, iron);
            FillRect(pixels, w, h, 10, 27, 4, 2, iron);
        }

        private static void PaintStoneFireplace(Color32[] pixels)
        {
            const int w = 36, h = 40;
            Color32 stone = new Color32(125, 130, 138, 255);
            Color32 stoneDark = new Color32(75, 80, 88, 255);
            Color32 fire = new Color32(255, 160, 40, 255);
            Color32 fireCore = new Color32(255, 240, 130, 255);
            Color32 wood = new Color32(95, 60, 30, 255);

            // Hearth Base
            FillRect(pixels, w, h, 4, 2, 28, 20, stone);
            DrawRect(pixels, w, h, 4, 2, 31, 21, stoneDark);

            // Firebox Arched Opening
            FillRect(pixels, w, h, 10, 4, 16, 12, new Color32(30, 25, 22, 255));
            FillRect(pixels, w, h, 12, 4, 12, 3, wood);
            FillRect(pixels, w, h, 13, 7, 10, 6, fire);
            FillRect(pixels, w, h, 15, 8, 6, 4, fireCore);

            // Mantle
            FillRect(pixels, w, h, 2, 22, 32, 3, wood);

            // Tapered Chimney
            for (int y = 25; y < 38; y++)
            {
                int inset = (y - 25) * 5 / 13;
                FillRect(pixels, w, h, 10 + inset, y, 16 - inset * 2, 1, stone);
            }
            FillRect(pixels, w, h, 13, 37, 10, 3, stoneDark);
        }

        private static void PaintSheepPasture(Color32[] pixels)
        {
            const int w = 56, h = 42;
            Color32 grass = new Color32(95, 165, 75, 255);
            Color32 grassLight = new Color32(120, 195, 90, 255);
            Color32 whiteFence = new Color32(240, 240, 245, 255);
            Color32 wood = new Color32(115, 75, 45, 255);
            Color32 straw = new Color32(225, 200, 100, 255);

            // Lush Pasture Grass
            FillRect(pixels, w, h, 4, 4, 48, 34, grass);
            for (int x = 8; x < 48; x += 6)
            {
                FillRect(pixels, w, h, x, 10 + (x % 5), 3, 2, grassLight);
            }

            // Small Wooden Lean-to Shelter in Top Left
            FillRect(pixels, w, h, 6, 22, 16, 14, wood);
            FillRect(pixels, w, h, 8, 24, 12, 4, straw);

            // White Post & Rail Fence Perimeter
            for (int px = 4; px <= 50; px += 10)
            {
                FillRect(pixels, w, h, px, 4, 3, 16, whiteFence);
                FillRect(pixels, w, h, px, 22, 3, 16, whiteFence);
            }
            FillRect(pixels, w, h, 4, 8, 48, 2, whiteFence);
            FillRect(pixels, w, h, 4, 14, 48, 2, whiteFence);
            FillRect(pixels, w, h, 4, 28, 48, 2, whiteFence);
            FillRect(pixels, w, h, 4, 34, 48, 2, whiteFence);
        }

        private static void PaintHenCoop(Color32[] pixels)
        {
            const int w = 44, h = 36;
            Color32 wood = new Color32(160, 95, 55, 255);
            Color32 woodDark = new Color32(100, 55, 30, 255);
            Color32 roofRed = new Color32(190, 50, 45, 255);
            Color32 straw = new Color32(235, 210, 110, 255);

            // Stilt Posts
            FillRect(pixels, w, h, 6, 2, 3, 10, woodDark);
            FillRect(pixels, w, h, 35, 2, 3, 10, woodDark);

            // Coop Box Body
            FillRect(pixels, w, h, 4, 10, 36, 16, wood);
            DrawRect(pixels, w, h, 4, 10, 39, 25, woodDark);

            // Chicken Ramp
            for (int i = 0; i < 8; i++)
            {
                FillRect(pixels, w, h, 14 + i * 2, 2 + i, 3, 2, woodDark);
            }

            // Door Opening & Nest Box
            FillRect(pixels, w, h, 28, 12, 8, 10, new Color32(40, 20, 10, 255));
            FillRect(pixels, w, h, 29, 12, 6, 3, straw);

            // Red Gabled Roof
            for (int y = 26; y < 34; y++)
            {
                int span = (34 - y) * 20 / 8;
                FillRect(pixels, w, h, 22 - span, y, span * 2, 1, roofRed);
            }
            // Weather vane on peak
            FillRect(pixels, w, h, 21, 33, 2, 3, Color.black);
        }

        private static void PaintStoneWall(Color32[] pixels)
        {
            const int w = 24, h = 24;
            Color32 stone = new Color32(135, 140, 145, 255);
            Color32 stoneDark = new Color32(85, 90, 95, 255);
            Color32 moss = new Color32(80, 145, 65, 255);

            FillRect(pixels, w, h, 2, 2, 20, 18, stone);
            DrawRect(pixels, w, h, 2, 2, 21, 19, stoneDark);

            // Stone brick pattern
            FillRect(pixels, w, h, 2, 8, 20, 1, stoneDark);
            FillRect(pixels, w, h, 2, 14, 20, 1, stoneDark);
            SetPixel(pixels, w, h, 8, 5, stoneDark);
            SetPixel(pixels, w, h, 15, 11, stoneDark);

            // Capstones on top
            FillRect(pixels, w, h, 1, 19, 22, 3, stone);
            DrawRect(pixels, w, h, 1, 19, 22, 21, stoneDark);

            // Moss details
            SetPixel(pixels, w, h, 4, 3, moss);
            SetPixel(pixels, w, h, 5, 3, moss);
            SetPixel(pixels, w, h, 17, 20, moss);
        }

        private static void PaintIronGate(Color32[] pixels)
        {
            const int w = 32, h = 28;
            Color32 stone = new Color32(120, 125, 130, 255);
            Color32 iron = new Color32(35, 38, 45, 255);
            Color32 gold = new Color32(230, 185, 50, 255);

            // Stone Gateposts
            FillRect(pixels, w, h, 2, 2, 6, 22, stone);
            FillRect(pixels, w, h, 1, 23, 8, 3, stone);
            FillRect(pixels, w, h, 24, 2, 6, 22, stone);
            FillRect(pixels, w, h, 23, 23, 8, 3, stone);

            // Iron Bars
            for (int x = 9; x <= 22; x += 3)
            {
                FillRect(pixels, w, h, x, 2, 2, 18, iron);
                SetPixel(pixels, w, h, x, 20, gold);
            }
            FillRect(pixels, w, h, 8, 4, 16, 2, iron);
            FillRect(pixels, w, h, 8, 12, 16, 2, iron);
            FillRect(pixels, w, h, 8, 18, 16, 2, iron);
        }

        private static void PaintLogPalisade(Color32[] pixels)
        {
            const int w = 24, h = 30;
            Color32 wood = new Color32(130, 85, 45, 255);
            Color32 woodDark = new Color32(85, 50, 25, 255);
            Color32 woodLight = new Color32(170, 115, 65, 255);

            for (int i = 0; i < 4; i++)
            {
                int lx = 2 + i * 5;
                FillRect(pixels, w, h, lx, 2, 4, 20 + (i % 2) * 4, wood);
                DrawRect(pixels, w, h, lx, 2, lx + 3, 20 + (i % 2) * 4, woodDark);
                SetPixel(pixels, w, h, lx + 1, 21 + (i % 2) * 4, woodLight);
                SetPixel(pixels, w, h, lx + 2, 21 + (i % 2) * 4, woodLight);
                SetPixel(pixels, w, h, lx + 1, 22 + (i % 2) * 4, woodLight);
            }

            FillRect(pixels, w, h, 1, 6, 22, 3, woodDark);
            FillRect(pixels, w, h, 1, 16, 22, 3, woodDark);
        }

        private static void PaintPerimeterFencePreview(Color32[] pixels)
        {
            const int w = 36, h = 28;
            Color32 wood = new Color32(140, 90, 50, 255);

            FillRect(pixels, w, h, 2, 2, 4, 10, wood);
            FillRect(pixels, w, h, 30, 2, 4, 10, wood);
            FillRect(pixels, w, h, 2, 16, 4, 10, wood);
            FillRect(pixels, w, h, 30, 16, 4, 10, wood);

            FillRect(pixels, w, h, 4, 4, 28, 2, wood);
            FillRect(pixels, w, h, 4, 8, 28, 2, wood);
            FillRect(pixels, w, h, 4, 18, 28, 2, wood);
            FillRect(pixels, w, h, 4, 22, 28, 2, wood);
        }

        private static void PaintPathWood(Color32[] pixels)
        {
            const int w = 16, h = 16;
            Color32 wood = new Color32(145, 95, 55, 255);
            Color32 woodDark = new Color32(95, 60, 30, 255);
            Color32 nail = new Color32(60, 65, 70, 255);

            FillRect(pixels, w, h, 0, 0, 16, 16, wood);
            FillRect(pixels, w, h, 0, 3, 16, 1, woodDark);
            FillRect(pixels, w, h, 0, 7, 16, 1, woodDark);
            FillRect(pixels, w, h, 0, 11, 16, 1, woodDark);
            FillRect(pixels, w, h, 0, 15, 16, 1, woodDark);

            SetPixel(pixels, w, h, 1, 1, nail);
            SetPixel(pixels, w, h, 14, 1, nail);
            SetPixel(pixels, w, h, 1, 5, nail);
            SetPixel(pixels, w, h, 14, 5, nail);
        }

        private static void PaintPathStoneTile(Color32[] pixels)
        {
            const int w = 16, h = 16;
            Color32 tile = new Color32(165, 170, 175, 255);
            Color32 tileLight = new Color32(195, 200, 205, 255);
            Color32 grout = new Color32(90, 95, 100, 255);

            FillRect(pixels, w, h, 0, 0, 16, 16, grout);
            FillRect(pixels, w, h, 1, 1, 6, 6, tile);
            FillRect(pixels, w, h, 8, 1, 7, 6, tileLight);
            FillRect(pixels, w, h, 1, 8, 7, 7, tileLight);
            FillRect(pixels, w, h, 9, 8, 6, 7, tile);
        }

        private static void PaintWoodBridge(Color32[] pixels)
        {
            const int w = 48, h = 26;
            Color32 wood = new Color32(150, 100, 55, 255);
            Color32 woodDark = new Color32(95, 60, 30, 255);
            Color32 water = new Color32(50, 130, 200, 255);

            FillRect(pixels, w, h, 0, 0, 48, 6, water);

            for (int x = 2; x < 46; x++)
            {
                int archY = 6 + (int)(Mathf.Sin((x - 2) * Mathf.PI / 44f) * 4f);
                FillRect(pixels, w, h, x, archY, 1, 4, wood);
                SetPixel(pixels, w, h, x, archY + 4, woodDark);
                SetPixel(pixels, w, h, x, archY + 10, woodDark);
                if (x % 6 == 0)
                {
                    FillRect(pixels, w, h, x, archY + 4, 1, 6, woodDark);
                }
            }
        }

        private static void PaintStrawBed(Color32[] pixels)
        {
            const int w = 32, h = 24;
            Color32 wood = new Color32(120, 75, 40, 255);
            Color32 straw = new Color32(235, 205, 95, 255);
            Color32 linen = new Color32(245, 245, 240, 255);
            Color32 pillow = new Color32(220, 230, 240, 255);

            FillRect(pixels, w, h, 2, 2, 28, 18, wood);
            FillRect(pixels, w, h, 4, 4, 24, 14, straw);
            FillRect(pixels, w, h, 4, 4, 16, 14, linen);
            FillRect(pixels, w, h, 22, 6, 5, 10, pillow);
        }

        private static void PaintOakTable(Color32[] pixels)
        {
            const int w = 30, h = 22;
            Color32 oak = new Color32(165, 110, 60, 255);
            Color32 oakDark = new Color32(105, 65, 30, 255);
            Color32 cloth = new Color32(210, 45, 45, 255);

            FillRect(pixels, w, h, 4, 2, 3, 10, oakDark);
            FillRect(pixels, w, h, 23, 2, 3, 10, oakDark);
            FillRect(pixels, w, h, 2, 12, 26, 6, oak);
            DrawRect(pixels, w, h, 2, 12, 27, 17, oakDark);
            FillRect(pixels, w, h, 11, 14, 8, 3, cloth);
        }

        private static void PaintLeatherChair(Color32[] pixels)
        {
            const int w = 20, h = 24;
            Color32 leather = new Color32(145, 75, 40, 255);
            Color32 leatherDark = new Color32(95, 45, 20, 255);
            Color32 wood = new Color32(75, 40, 20, 255);

            FillRect(pixels, w, h, 3, 2, 2, 6, wood);
            FillRect(pixels, w, h, 15, 2, 2, 6, wood);
            FillRect(pixels, w, h, 2, 8, 16, 5, leather);
            DrawRect(pixels, w, h, 2, 8, 17, 12, leatherDark);
            FillRect(pixels, w, h, 3, 13, 14, 9, leather);
            DrawRect(pixels, w, h, 3, 13, 16, 21, leatherDark);
        }

        private static void PaintBookshelf(Color32[] pixels)
        {
            const int w = 26, h = 38;
            Color32 wood = new Color32(110, 65, 35, 255);
            Color32 woodDark = new Color32(70, 40, 20, 255);
            Color32 redBook = new Color32(195, 45, 45, 255);
            Color32 blueBook = new Color32(45, 110, 195, 255);
            Color32 greenBook = new Color32(45, 155, 65, 255);
            Color32 goldBook = new Color32(235, 190, 55, 255);

            FillRect(pixels, w, h, 2, 2, 22, 34, wood);
            DrawRect(pixels, w, h, 2, 2, 23, 35, woodDark);

            FillRect(pixels, w, h, 4, 12, 18, 2, woodDark);
            FillRect(pixels, w, h, 4, 22, 18, 2, woodDark);

            FillRect(pixels, w, h, 5, 4, 3, 8, redBook);
            FillRect(pixels, w, h, 9, 4, 4, 8, blueBook);
            FillRect(pixels, w, h, 14, 4, 3, 8, goldBook);
            FillRect(pixels, w, h, 18, 4, 3, 8, greenBook);

            FillRect(pixels, w, h, 5, 14, 4, 8, greenBook);
            FillRect(pixels, w, h, 10, 14, 3, 8, redBook);
            FillRect(pixels, w, h, 14, 14, 4, 8, blueBook);

            FillRect(pixels, w, h, 6, 24, 3, 8, goldBook);
            FillRect(pixels, w, h, 10, 24, 4, 8, redBook);
            FillRect(pixels, w, h, 15, 24, 3, 8, greenBook);
        }

        private static void PaintWovenRug(Color32[] pixels)
        {
            const int w = 36, h = 24;
            Color32 navy = new Color32(35, 55, 95, 255);
            Color32 crimson = new Color32(175, 40, 45, 255);
            Color32 gold = new Color32(235, 195, 65, 255);
            Color32 fringe = new Color32(245, 240, 225, 255);

            FillRect(pixels, w, h, 3, 2, 30, 20, navy);
            DrawRect(pixels, w, h, 4, 3, 31, 20, gold);
            FillRect(pixels, w, h, 8, 6, 20, 12, crimson);
            FillRect(pixels, w, h, 16, 10, 4, 4, gold);

            for (int y = 3; y <= 20; y += 2)
            {
                SetPixel(pixels, w, h, 1, y, fringe);
                SetPixel(pixels, w, h, 2, y, fringe);
                SetPixel(pixels, w, h, 33, y, fringe);
                SetPixel(pixels, w, h, 34, y, fringe);
            }
        }

        private static void PaintCheesePress(Color32[] pixels)
        {
            const int w = 26, h = 28;
            Color32 wood = new Color32(135, 85, 45, 255);
            Color32 metal = new Color32(75, 80, 90, 255);
            Color32 cheese = new Color32(245, 215, 75, 255);

            FillRect(pixels, w, h, 4, 2, 18, 4, wood);
            FillRect(pixels, w, h, 4, 6, 3, 16, wood);
            FillRect(pixels, w, h, 19, 6, 3, 16, wood);
            FillRect(pixels, w, h, 4, 20, 18, 3, wood);

            FillRect(pixels, w, h, 9, 6, 8, 8, cheese);
            DrawRect(pixels, w, h, 8, 5, 17, 14, metal);

            FillRect(pixels, w, h, 12, 14, 2, 10, metal);
            FillRect(pixels, w, h, 8, 24, 10, 2, metal);
        }

        private static void PaintLoom(Color32[] pixels)
        {
            const int w = 30, h = 30;
            Color32 wood = new Color32(140, 90, 50, 255);
            Color32 warp = new Color32(235, 235, 230, 255);
            Color32 clothBlue = new Color32(55, 120, 205, 255);

            FillRect(pixels, w, h, 3, 2, 4, 26, wood);
            FillRect(pixels, w, h, 23, 2, 4, 26, wood);
            FillRect(pixels, w, h, 3, 2, 24, 4, wood);
            FillRect(pixels, w, h, 3, 24, 24, 4, wood);

            for (int x = 8; x <= 21; x += 2)
            {
                FillRect(pixels, w, h, x, 6, 1, 18, warp);
            }
            FillRect(pixels, w, h, 7, 6, 16, 7, clothBlue);
        }

        private static void PaintKeg(Color32[] pixels)
        {
            const int w = 28, h = 26;
            Color32 wood = new Color32(145, 95, 55, 255);
            Color32 iron = new Color32(50, 55, 65, 255);
            Color32 stand = new Color32(95, 60, 30, 255);
            Color32 brass = new Color32(235, 195, 60, 255);

            FillRect(pixels, w, h, 4, 2, 5, 6, stand);
            FillRect(pixels, w, h, 19, 2, 5, 6, stand);

            FillRect(pixels, w, h, 5, 7, 18, 14, wood);
            DrawRect(pixels, w, h, 5, 7, 22, 20, iron);

            FillRect(pixels, w, h, 8, 7, 2, 14, iron);
            FillRect(pixels, w, h, 18, 7, 2, 14, iron);

            FillRect(pixels, w, h, 22, 11, 3, 2, brass);
            FillRect(pixels, w, h, 24, 9, 2, 2, brass);
        }

        private static void PaintWindmill(Color32[] pixels)
        {
            const int w = 48, h = 62;
            Color32 stone = new Color32(130, 135, 140, 255);
            Color32 stoneDark = new Color32(85, 90, 95, 255);
            Color32 wood = new Color32(145, 95, 50, 255);
            Color32 sailCanvas = new Color32(245, 245, 240, 255);
            Color32 hub = new Color32(50, 50, 55, 255);

            FillRect(pixels, w, h, 14, 2, 20, 16, stone);
            DrawRect(pixels, w, h, 14, 2, 33, 17, stoneDark);
            FillRect(pixels, w, h, 21, 2, 6, 8, wood);

            for (int y = 18; y < 42; y++)
            {
                int inset = (y - 18) * 4 / 24;
                FillRect(pixels, w, h, 15 + inset, y, 18 - inset * 2, 1, wood);
            }

            for (int y = 42; y < 48; y++)
            {
                int span = (48 - y) * 9 / 6;
                FillRect(pixels, w, h, 24 - span, y, span * 2, 1, stoneDark);
            }

            FillRect(pixels, w, h, 22, 42, 4, 4, hub);
            FillRect(pixels, w, h, 23, 46, 2, 14, wood);
            FillRect(pixels, w, h, 25, 47, 5, 12, sailCanvas);
            FillRect(pixels, w, h, 23, 28, 2, 14, wood);
            FillRect(pixels, w, h, 18, 29, 5, 12, sailCanvas);
            FillRect(pixels, w, h, 26, 43, 14, 2, wood);
            FillRect(pixels, w, h, 27, 38, 12, 5, sailCanvas);
            FillRect(pixels, w, h, 8, 43, 14, 2, wood);
            FillRect(pixels, w, h, 9, 45, 12, 5, sailCanvas);
        }

        private static void PaintBlacksmithForge(Color32[] pixels)
        {
            const int w = 42, h = 36;
            Color32 stone = new Color32(115, 120, 125, 255);
            Color32 fire = new Color32(255, 145, 35, 255);
            Color32 fireCore = new Color32(255, 245, 120, 255);
            Color32 anvil = new Color32(45, 48, 55, 255);
            Color32 wood = new Color32(95, 60, 30, 255);
            Color32 water = new Color32(65, 140, 215, 255);

            FillRect(pixels, w, h, 4, 2, 22, 16, stone);
            FillRect(pixels, w, h, 8, 8, 14, 8, fire);
            FillRect(pixels, w, h, 11, 10, 8, 5, fireCore);

            for (int y = 18; y < 34; y++)
            {
                int inset = (y - 18) * 5 / 16;
                FillRect(pixels, w, h, 8 + inset, y, 14 - inset * 2, 1, stone);
            }

            FillRect(pixels, w, h, 28, 2, 8, 7, wood);
            FillRect(pixels, w, h, 26, 9, 12, 6, anvil);
            FillRect(pixels, w, h, 36, 11, 4, 3, anvil);

            FillRect(pixels, w, h, 2, 2, 5, 8, wood);
            FillRect(pixels, w, h, 3, 5, 3, 4, water);
        }

        private static void PaintCarpenterBench(Color32[] pixels)
        {
            const int w = 32, h = 24;
            Color32 wood = new Color32(150, 100, 55, 255);
            Color32 woodDark = new Color32(95, 60, 30, 255);
            Color32 iron = new Color32(70, 75, 85, 255);
            Color32 shavings = new Color32(235, 205, 120, 255);

            FillRect(pixels, w, h, 4, 2, 4, 10, woodDark);
            FillRect(pixels, w, h, 24, 2, 4, 10, woodDark);
            FillRect(pixels, w, h, 2, 12, 28, 6, wood);
            DrawRect(pixels, w, h, 2, 12, 29, 17, woodDark);

            FillRect(pixels, w, h, 2, 10, 3, 6, iron);
            FillRect(pixels, w, h, 14, 18, 8, 2, iron);
            SetPixel(pixels, w, h, 22, 18, woodDark);

            SetPixel(pixels, w, h, 10, 2, shavings);
            SetPixel(pixels, w, h, 15, 3, shavings);
            SetPixel(pixels, w, h, 18, 2, shavings);
        }

        private static void PaintWoodChest(Color32[] pixels)
        {
            const int w = 24, h = 20;
            Color32 wood = new Color32(145, 90, 45, 255);
            Color32 iron = new Color32(50, 55, 65, 255);
            Color32 brass = new Color32(235, 195, 60, 255);

            FillRect(pixels, w, h, 2, 2, 20, 14, wood);
            DrawRect(pixels, w, h, 2, 2, 21, 15, iron);

            FillRect(pixels, w, h, 6, 2, 2, 14, iron);
            FillRect(pixels, w, h, 16, 2, 2, 14, iron);
            FillRect(pixels, w, h, 2, 10, 20, 2, iron);
            FillRect(pixels, w, h, 11, 8, 3, 4, brass);
        }

        private static void PaintStoneVault(Color32[] pixels)
        {
            const int w = 30, h = 28;
            Color32 stone = new Color32(120, 125, 135, 255);
            Color32 iron = new Color32(45, 48, 55, 255);
            Color32 dialGold = new Color32(235, 190, 50, 255);

            FillRect(pixels, w, h, 3, 2, 24, 22, stone);
            DrawRect(pixels, w, h, 3, 2, 26, 23, iron);
            FillRect(pixels, w, h, 6, 5, 18, 16, iron);
            DrawRect(pixels, w, h, 6, 5, 23, 20, stone);

            FillRect(pixels, w, h, 13, 11, 4, 4, dialGold);
            FillRect(pixels, w, h, 12, 12, 6, 2, dialGold);
        }

        private static void PaintCompostBin(Color32[] pixels)
        {
            const int w = 26, h = 24;
            Color32 wood = new Color32(130, 80, 40, 255);
            Color32 compost = new Color32(60, 40, 20, 255);
            Color32 greenLeaf = new Color32(75, 155, 55, 255);

            FillRect(pixels, w, h, 4, 2, 18, 14, compost);
            FillRect(pixels, w, h, 6, 12, 14, 4, greenLeaf);

            for (int sy = 3; sy < 18; sy += 4)
            {
                FillRect(pixels, w, h, 2, sy, 22, 2, wood);
            }
            FillRect(pixels, w, h, 2, 2, 3, 18, wood);
            FillRect(pixels, w, h, 21, 2, 3, 18, wood);
        }

        private static void PaintBarrelRack(Color32[] pixels)
        {
            const int w = 36, h = 22;
            Color32 rackWood = new Color32(100, 60, 30, 255);
            Color32 barrelWood = new Color32(150, 100, 55, 255);
            Color32 iron = new Color32(45, 50, 60, 255);

            FillRect(pixels, w, h, 2, 2, 32, 3, rackWood);
            FillRect(pixels, w, h, 3, 2, 3, 16, rackWood);
            FillRect(pixels, w, h, 30, 2, 3, 16, rackWood);

            for (int b = 0; b < 3; b++)
            {
                int bx = 6 + b * 9;
                FillRect(pixels, w, h, bx, 5, 7, 12, barrelWood);
                DrawRect(pixels, w, h, bx, 5, bx + 6, 16, iron);
                FillRect(pixels, w, h, bx + 2, 5, 3, 12, iron);
            }
        }

        private static void PaintGrapeTrellis(Color32[] pixels)
        {
            const int w = 36, h = 28;
            Color32 wood = new Color32(120, 75, 40, 255);
            Color32 vines = new Color32(55, 145, 45, 255);
            Color32 grapes = new Color32(135, 45, 175, 255);

            FillRect(pixels, w, h, 4, 2, 3, 22, wood);
            FillRect(pixels, w, h, 29, 2, 3, 22, wood);
            FillRect(pixels, w, h, 4, 12, 28, 2, wood);
            FillRect(pixels, w, h, 4, 22, 28, 2, wood);

            FillRect(pixels, w, h, 4, 14, 28, 8, vines);
            FillRect(pixels, w, h, 6, 6, 6, 8, vines);
            FillRect(pixels, w, h, 24, 6, 6, 8, vines);

            FillRect(pixels, w, h, 10, 10, 4, 5, grapes);
            FillRect(pixels, w, h, 18, 12, 4, 5, grapes);
            FillRect(pixels, w, h, 24, 10, 4, 5, grapes);
        }

        private static void PaintPumpkinPatch(Color32[] pixels)
        {
            const int w = 34, h = 28;
            Color32 soil = new Color32(90, 60, 35, 255);
            Color32 leaves = new Color32(65, 155, 50, 255);
            Color32 orange = new Color32(240, 125, 25, 255);
            Color32 stem = new Color32(75, 110, 45, 255);

            FillRect(pixels, w, h, 2, 2, 30, 18, soil);
            DrawRect(pixels, w, h, 2, 2, 31, 19, new Color32(120, 80, 45, 255));
            FillRect(pixels, w, h, 4, 6, 26, 12, leaves);

            FillRect(pixels, w, h, 6, 6, 10, 8, orange);
            SetPixel(pixels, w, h, 10, 14, stem);

            FillRect(pixels, w, h, 19, 8, 10, 8, orange);
            SetPixel(pixels, w, h, 23, 16, stem);
        }

        private static void PaintFlowerPlanter(Color32[] pixels)
        {
            const int w = 28, h = 20;
            Color32 wood = new Color32(135, 85, 45, 255);
            Color32 soil = new Color32(70, 45, 25, 255);
            Color32 stem = new Color32(60, 150, 50, 255);
            Color32 red = new Color32(230, 45, 50, 255);
            Color32 yellow = new Color32(245, 215, 60, 255);
            Color32 blue = new Color32(60, 130, 240, 255);

            FillRect(pixels, w, h, 3, 2, 22, 8, wood);
            DrawRect(pixels, w, h, 3, 2, 24, 9, new Color32(85, 50, 25, 255));
            FillRect(pixels, w, h, 4, 8, 20, 2, soil);

            for (int fx = 5; fx <= 21; fx += 4)
            {
                FillRect(pixels, w, h, fx + 1, 10, 1, 5, stem);
            }
            FillRect(pixels, w, h, 5, 14, 3, 3, red);
            FillRect(pixels, w, h, 9, 15, 3, 3, yellow);
            FillRect(pixels, w, h, 13, 14, 3, 3, blue);
            FillRect(pixels, w, h, 17, 15, 3, 3, red);
            FillRect(pixels, w, h, 21, 14, 3, 3, yellow);
        }

        private static void PaintGardenHedge(Color32[] pixels)
        {
            const int w = 22, h = 22;
            Color32 hedgeGreen = new Color32(50, 135, 45, 255);
            Color32 hedgeDark = new Color32(30, 85, 30, 255);
            Color32 flowerWhite = new Color32(245, 245, 235, 255);

            FillRect(pixels, w, h, 2, 2, 18, 18, hedgeGreen);
            DrawRect(pixels, w, h, 2, 2, 19, 19, hedgeDark);

            SetPixel(pixels, w, h, 5, 6, flowerWhite);
            SetPixel(pixels, w, h, 14, 12, flowerWhite);
            SetPixel(pixels, w, h, 8, 15, flowerWhite);
        }

        private static void PaintAncientWell(Color32[] pixels)
        {
            const int w = 36, h = 40;
            Color32 stone = new Color32(125, 130, 135, 255);
            Color32 stoneDark = new Color32(75, 80, 85, 255);
            Color32 water = new Color32(45, 120, 205, 255);
            Color32 wood = new Color32(130, 80, 40, 255);
            Color32 roofTile = new Color32(165, 55, 45, 255);

            FillRect(pixels, w, h, 6, 2, 24, 12, stone);
            DrawRect(pixels, w, h, 6, 2, 29, 13, stoneDark);
            FillRect(pixels, w, h, 10, 8, 16, 4, water);

            FillRect(pixels, w, h, 7, 14, 3, 16, wood);
            FillRect(pixels, w, h, 26, 14, 3, 16, wood);
            FillRect(pixels, w, h, 7, 26, 22, 2, wood);
            FillRect(pixels, w, h, 16, 22, 4, 4, new Color32(195, 165, 95, 255));

            for (int y = 30; y < 38; y++)
            {
                int span = (38 - y) * 16 / 8;
                FillRect(pixels, w, h, 18 - span, y, span * 2, 1, roofTile);
            }
        }

        private static void PaintWaterAqueduct(Color32[] pixels)
        {
            const int w = 34, h = 22;
            Color32 wood = new Color32(130, 80, 40, 255);
            Color32 woodDark = new Color32(80, 45, 20, 255);
            Color32 water = new Color32(65, 155, 235, 255);

            FillRect(pixels, w, h, 4, 2, 3, 10, woodDark);
            FillRect(pixels, w, h, 27, 2, 3, 10, woodDark);
            FillRect(pixels, w, h, 2, 12, 30, 6, wood);
            FillRect(pixels, w, h, 2, 14, 30, 3, water);
            DrawRect(pixels, w, h, 2, 12, 31, 17, woodDark);
        }

        private static void PaintStoneFountain(Color32[] pixels)
        {
            const int w = 36, h = 38;
            Color32 stone = new Color32(145, 150, 155, 255);
            Color32 stoneDark = new Color32(90, 95, 100, 255);
            Color32 water = new Color32(75, 175, 245, 255);
            Color32 waterLight = new Color32(190, 235, 255, 255);

            FillRect(pixels, w, h, 4, 2, 28, 10, stone);
            DrawRect(pixels, w, h, 4, 2, 31, 11, stoneDark);
            FillRect(pixels, w, h, 8, 6, 20, 5, water);

            FillRect(pixels, w, h, 15, 12, 6, 12, stone);

            FillRect(pixels, w, h, 10, 22, 16, 6, stone);
            DrawRect(pixels, w, h, 10, 22, 25, 27, stoneDark);
            FillRect(pixels, w, h, 12, 24, 12, 3, water);

            FillRect(pixels, w, h, 17, 28, 2, 6, waterLight);
            SetPixel(pixels, w, h, 15, 33, waterLight);
            SetPixel(pixels, w, h, 20, 33, waterLight);
        }

        private static void PaintHotBath(Color32[] pixels)
        {
            const int w = 34, h = 28;
            Color32 cedar = new Color32(145, 85, 45, 255);
            Color32 copper = new Color32(185, 110, 55, 255);
            Color32 water = new Color32(80, 185, 235, 255);
            Color32 steam = new Color32(235, 245, 255, 190);

            FillRect(pixels, w, h, 4, 2, 26, 14, cedar);
            DrawRect(pixels, w, h, 4, 2, 29, 15, new Color32(95, 55, 25, 255));
            FillRect(pixels, w, h, 4, 6, 26, 2, copper);
            FillRect(pixels, w, h, 4, 11, 26, 2, copper);

            FillRect(pixels, w, h, 7, 10, 20, 5, water);

            SetPixel(pixels, w, h, 10, 17, steam);
            SetPixel(pixels, w, h, 11, 19, steam);
            SetPixel(pixels, w, h, 18, 18, steam);
            SetPixel(pixels, w, h, 17, 21, steam);
            SetPixel(pixels, w, h, 24, 17, steam);
            SetPixel(pixels, w, h, 25, 20, steam);
        }

        private static void PaintKnightStatue(Color32[] pixels)
        {
            const int w = 28, h = 44;
            Color32 stone = new Color32(140, 145, 150, 255);
            Color32 stoneDark = new Color32(85, 90, 95, 255);
            Color32 stoneLight = new Color32(185, 190, 195, 255);

            FillRect(pixels, w, h, 3, 2, 22, 6, stone);
            DrawRect(pixels, w, h, 3, 2, 24, 7, stoneDark);
            FillRect(pixels, w, h, 6, 8, 16, 4, stone);

            FillRect(pixels, w, h, 10, 12, 8, 12, stone);
            FillRect(pixels, w, h, 8, 22, 12, 10, stoneLight);
            FillRect(pixels, w, h, 10, 32, 8, 8, stone);
            DrawRect(pixels, w, h, 11, 34, 16, 36, stoneDark);

            FillRect(pixels, w, h, 13, 14, 2, 16, stoneLight);
            FillRect(pixels, w, h, 11, 28, 6, 2, stoneDark);
        }

        private static void PaintGuardianShrine(Color32[] pixels)
        {
            const int w = 30, h = 40;
            Color32 stone = new Color32(110, 115, 125, 255);
            Color32 stoneDark = new Color32(65, 70, 80, 255);
            Color32 runeCyan = new Color32(65, 235, 245, 255);
            Color32 crystal = new Color32(120, 240, 255, 255);

            FillRect(pixels, w, h, 4, 2, 22, 8, stone);
            DrawRect(pixels, w, h, 4, 2, 25, 9, stoneDark);

            for (int y = 10; y < 28; y++)
            {
                int inset = (y - 10) * 4 / 18;
                FillRect(pixels, w, h, 8 + inset, y, 14 - inset * 2, 1, stone);
            }

            SetPixel(pixels, w, h, 14, 14, runeCyan);
            SetPixel(pixels, w, h, 15, 17, runeCyan);
            SetPixel(pixels, w, h, 14, 20, runeCyan);

            FillRect(pixels, w, h, 13, 30, 4, 7, crystal);
            SetPixel(pixels, w, h, 12, 33, crystal);
            SetPixel(pixels, w, h, 17, 33, crystal);
            SetPixel(pixels, w, h, 14, 38, runeCyan);
        }

        private static void PaintBellPillar(Color32[] pixels)
        {
            const int w = 24, h = 40;
            Color32 stone = new Color32(125, 130, 138, 255);
            Color32 stoneDark = new Color32(75, 80, 88, 255);
            Color32 bronze = new Color32(205, 145, 55, 255);

            FillRect(pixels, w, h, 4, 2, 16, 6, stone);
            FillRect(pixels, w, h, 7, 8, 10, 26, stone);
            DrawRect(pixels, w, h, 7, 8, 16, 33, stoneDark);

            FillRect(pixels, w, h, 8, 20, 8, 10, new Color32(25, 28, 35, 255));
            FillRect(pixels, w, h, 10, 22, 4, 6, bronze);
            FillRect(pixels, w, h, 9, 21, 6, 2, bronze);
        }

        private static void PaintMarketStall(Color32[] pixels)
        {
            const int w = 40, h = 36;
            Color32 wood = new Color32(145, 95, 50, 255);
            Color32 woodDark = new Color32(95, 60, 30, 255);
            Color32 canopyRed = new Color32(215, 45, 45, 255);
            Color32 canopyWhite = new Color32(245, 245, 240, 255);
            Color32 apples = new Color32(230, 50, 50, 255);
            Color32 bread = new Color32(225, 175, 85, 255);

            FillRect(pixels, w, h, 4, 2, 32, 14, wood);
            DrawRect(pixels, w, h, 4, 2, 35, 15, woodDark);

            FillRect(pixels, w, h, 6, 14, 8, 4, apples);
            FillRect(pixels, w, h, 26, 14, 8, 4, bread);

            FillRect(pixels, w, h, 4, 16, 3, 14, woodDark);
            FillRect(pixels, w, h, 33, 16, 3, 14, woodDark);

            for (int x = 2; x < 38; x++)
            {
                Color32 c = (x / 4 % 2 == 0) ? canopyRed : canopyWhite;
                FillRect(pixels, w, h, x, 28, 1, 7, c);
            }
        }

        private static void PaintSpikeTrap(Color32[] pixels)
        {
            const int w = 26, h = 20;
            Color32 wood = new Color32(110, 70, 35, 255);
            Color32 iron = new Color32(185, 195, 205, 255);
            Color32 grass = new Color32(75, 150, 55, 255);

            FillRect(pixels, w, h, 2, 2, 22, 5, wood);

            for (int sx = 4; sx <= 20; sx += 4)
            {
                FillRect(pixels, w, h, sx, 6, 2, 7, wood);
                SetPixel(pixels, w, h, sx, 13, iron);
                SetPixel(pixels, w, h, sx + 1, 14, iron);
            }

            SetPixel(pixels, w, h, 3, 5, grass);
            SetPixel(pixels, w, h, 12, 4, grass);
            SetPixel(pixels, w, h, 21, 5, grass);
        }

        private static void PaintWoodenBarricade(Color32[] pixels)
        {
            const int w = 30, h = 24;
            Color32 wood = new Color32(130, 80, 45, 255);
            Color32 woodDark = new Color32(80, 45, 25, 255);
            Color32 rope = new Color32(185, 155, 90, 255);

            for (int i = 0; i < 18; i++)
            {
                FillRect(pixels, w, h, 3 + i, 3 + i, 3, 3, wood);
                FillRect(pixels, w, h, 24 - i, 3 + i, 3, 3, woodDark);
            }
            FillRect(pixels, w, h, 12, 10, 6, 6, rope);
        }

        private static void PaintAlarmBell(Color32[] pixels)
        {
            const int w = 24, h = 38;
            Color32 wood = new Color32(120, 75, 40, 255);
            Color32 woodDark = new Color32(75, 45, 25, 255);
            Color32 bell = new Color32(215, 160, 55, 255);

            for (int i = 0; i < 28; i++)
            {
                int inset = i * 6 / 28;
                SetPixel(pixels, w, h, 4 + inset, 2 + i, wood);
                SetPixel(pixels, w, h, 19 - inset, 2 + i, wood);
            }
            FillRect(pixels, w, h, 7, 28, 10, 3, woodDark);

            FillRect(pixels, w, h, 9, 20, 6, 6, bell);
            FillRect(pixels, w, h, 8, 18, 8, 3, bell);
            for (int y = 4; y < 18; y += 2)
            {
                SetPixel(pixels, w, h, 12, y, new Color32(200, 180, 120, 255));
            }
        }

        private static void PaintWoodSwing(Color32[] pixels)
        {
            const int w = 28, h = 36;
            Color32 wood = new Color32(125, 80, 45, 255);
            Color32 rope = new Color32(195, 170, 110, 255);

            for (int y = 2; y < 32; y++)
            {
                int inset = (32 - y) * 8 / 30;
                SetPixel(pixels, w, h, 14 - inset, y, wood);
                SetPixel(pixels, w, h, 14 + inset, y, wood);
            }
            FillRect(pixels, w, h, 6, 31, 16, 3, wood);

            for (int ry = 8; ry < 31; ry++)
            {
                SetPixel(pixels, w, h, 10, ry, rope);
                SetPixel(pixels, w, h, 18, ry, rope);
            }
            FillRect(pixels, w, h, 8, 6, 12, 3, wood);
        }

        private static void PaintChessTable(Color32[] pixels)
        {
            const int w = 26, h = 24;
            Color32 stone = new Color32(130, 135, 140, 255);
            Color32 stoneDark = new Color32(80, 85, 90, 255);
            Color32 whiteSquare = new Color32(245, 245, 240, 255);
            Color32 blackSquare = new Color32(40, 45, 50, 255);

            FillRect(pixels, w, h, 2, 2, 5, 8, stone);
            FillRect(pixels, w, h, 19, 2, 5, 8, stone);

            FillRect(pixels, w, h, 8, 2, 10, 6, stone);
            FillRect(pixels, w, h, 6, 8, 14, 12, stone);
            DrawRect(pixels, w, h, 6, 8, 19, 19, stoneDark);

            for (int y = 10; y < 18; y++)
            {
                for (int x = 8; x < 16; x++)
                {
                    Color32 c = ((x + y) % 2 == 0) ? whiteSquare : blackSquare;
                    SetPixel(pixels, w, h, x, y, c);
                }
            }
        }

        private static void PaintHammock(Color32[] pixels)
        {
            const int w = 36, h = 20;
            Color32 wood = new Color32(110, 70, 35, 255);
            Color32 fabricRed = new Color32(215, 60, 60, 255);
            Color32 fabricGold = new Color32(245, 205, 70, 255);

            FillRect(pixels, w, h, 3, 2, 3, 16, wood);
            FillRect(pixels, w, h, 30, 2, 3, 16, wood);

            for (int x = 5; x < 31; x++)
            {
                int sag = (int)(Mathf.Sin((x - 5) * Mathf.PI / 26f) * 6f);
                Color32 c = (x / 3 % 2 == 0) ? fabricRed : fabricGold;
                FillRect(pixels, w, h, x, 12 - sag, 1, 3, c);
            }
        }

        private static void PaintBbqGrill(Color32[] pixels)
        {
            const int w = 28, h = 26;
            Color32 iron = new Color32(45, 48, 55, 255);
            Color32 coals = new Color32(245, 75, 30, 255);
            Color32 skewer = new Color32(190, 80, 50, 255);

            FillRect(pixels, w, h, 5, 2, 2, 10, iron);
            FillRect(pixels, w, h, 21, 2, 2, 10, iron);

            FillRect(pixels, w, h, 4, 11, 20, 6, iron);
            FillRect(pixels, w, h, 6, 14, 16, 3, coals);

            FillRect(pixels, w, h, 3, 17, 22, 2, iron);
            FillRect(pixels, w, h, 6, 19, 16, 2, skewer);
        }

        private static void PaintFestivalBanner(Color32[] pixels)
        {
            const int w = 34, h = 24;
            Color32 wood = new Color32(110, 70, 40, 255);
            Color32 rope = new Color32(210, 190, 130, 255);
            Color32[] flagColors = { new Color32(225, 45, 45, 255), new Color32(245, 200, 50, 255), new Color32(45, 120, 225, 255), new Color32(45, 175, 70, 255) };

            FillRect(pixels, w, h, 2, 2, 2, 20, wood);
            FillRect(pixels, w, h, 30, 2, 2, 20, wood);

            for (int x = 3; x < 31; x++)
            {
                int sag = (int)(Mathf.Sin((x - 3) * Mathf.PI / 28f) * 4f);
                SetPixel(pixels, w, h, x, 18 - sag, rope);
            }

            for (int f = 0; f < 4; f++)
            {
                int fx = 6 + f * 6;
                int sag = (int)(Mathf.Sin((fx - 3) * Mathf.PI / 28f) * 4f);
                Color32 c = flagColors[f % flagColors.Length];
                for (int fy = 0; fy < 5; fy++)
                {
                    int span = 5 - fy;
                    FillRect(pixels, w, h, fx + (5 - span) / 2, 17 - sag - fy, span, 1, c);
                }
            }
        }

        private static void PaintSkyLantern(Color32[] pixels)
        {
            const int w = 20, h = 28;
            Color32 paper = new Color32(255, 180, 70, 255);
            Color32 candle = new Color32(255, 245, 160, 255);
            Color32 rim = new Color32(165, 75, 30, 255);

            FillRect(pixels, w, h, 4, 6, 12, 16, paper);
            DrawRect(pixels, w, h, 4, 6, 15, 21, rim);
            FillRect(pixels, w, h, 8, 8, 4, 6, candle);
        }

        private static void PaintFireflyJar(Color32[] pixels)
        {
            const int w = 18, h = 24;
            Color32 glass = new Color32(190, 235, 245, 255);
            Color32 cork = new Color32(160, 110, 60, 255);
            Color32 firefly = new Color32(220, 255, 75, 255);
            Color32 wood = new Color32(95, 60, 30, 255);

            FillRect(pixels, w, h, 3, 2, 12, 4, wood);
            FillRect(pixels, w, h, 4, 6, 10, 12, glass);
            DrawRect(pixels, w, h, 4, 6, 13, 17, new Color32(120, 175, 195, 255));
            FillRect(pixels, w, h, 6, 18, 6, 3, cork);

            SetPixel(pixels, w, h, 6, 9, firefly);
            SetPixel(pixels, w, h, 10, 12, firefly);
            SetPixel(pixels, w, h, 8, 15, firefly);
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
