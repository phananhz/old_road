using System;
using UnityEngine;
using UnityEngine.Rendering;
using TheOldRoad.Player;
using TheOldRoad.Time;
using TheOldRoad.Audio;
using TheOldRoad.UI;

namespace TheOldRoad.World
{
    /// <summary>
    /// Master multi-building and multi-story interior controller.
    /// Supports distinct, tailored layouts for ALL buildings in The Old Road,
    /// with seamless 2-floor navigation (Stairs & Ladders) for multi-story buildings.
    /// </summary>
    public sealed class CabinInteriorController : MonoBehaviour
    {
        private static readonly Vector3 InteriorOriginF1 = new Vector3(200f, 200f, 0f);
        private static readonly Vector3 InteriorOriginF2 = new Vector3(200f, 230f, 0f);

        private static readonly Vector2 CameraCenterF1 = new Vector2(200f, 200f);
        private static readonly Vector2 CameraCenterF2 = new Vector2(200f, 230f);

        private static readonly Vector2 BoundsMinF1 = new Vector2(193.35f, 195.25f);
        private static readonly Vector2 BoundsMaxF1 = new Vector2(206.65f, 203.35f);
        private static readonly Vector2 BoundsMinF2 = new Vector2(193.35f, 225.25f);
        private static readonly Vector2 BoundsMaxF2 = new Vector2(206.65f, 233.35f);

        private const int InteriorPlayerSortingOrder = 9500;

        private GameObject interiorRoot;
        private Transform bedTransform;
        private Transform doorTransform;
        private Transform chestTransform;
        private Transform stairsTransform;
        private Vector3 lastOutdoorPosition;
        private string currentBuildingType = "building.cabin";
        private int currentFloor = 1;
        private bool inside;
        private string status = "Interior ready.";

        public bool IsInside => inside;
        public int CurrentFloor => currentFloor;
        public Transform BedTransform => bedTransform;
        public Transform DoorTransform => doorTransform;
        public Transform ChestTransform => chestTransform;
        public Transform StairsTransform => stairsTransform;
        public string CurrentBuildingType => currentBuildingType;
        public string Status => status;

        public void EnsureBuilt(string buildingType = "building.cabin", int floor = 1)
        {
            currentBuildingType = string.IsNullOrEmpty(buildingType) ? "building.cabin" : buildingType;
            currentFloor = Mathf.Clamp(floor, 1, 2);

            if (interiorRoot != null)
            {
                Destroy(interiorRoot);
            }

            interiorRoot = new GameObject($"Interior - {currentBuildingType} - F{currentFloor}");
            interiorRoot.transform.position = currentFloor == 2 ? InteriorOriginF2 : InteriorOriginF1;
            interiorRoot.SetActive(false);

            bedTransform = null;
            doorTransform = null;
            chestTransform = null;
            stairsTransform = null;

            switch (currentBuildingType)
            {
                case "building.stone-cottage":
                    if (currentFloor == 2) BuildStoneCottageFloor2();
                    else BuildStoneCottageFloor1();
                    break;

                case "building.manor":
                    if (currentFloor == 2) BuildManorFloor2();
                    else BuildManorFloor1();
                    break;

                case "building.windmill":
                    if (currentFloor == 2) BuildWindmillFloor2();
                    else BuildWindmillFloor1();
                    break;

                case "building.lookout-tower":
                    if (currentFloor == 2) BuildLookoutTowerFloor2();
                    else BuildLookoutTowerFloor1();
                    break;

                case "building.herbalist-hut":
                    BuildHerbalistHutInterior();
                    break;

                case "building.greenhouse":
                    BuildGreenhouseInterior();
                    break;

                case "building.tent":
                    BuildTentInterior();
                    break;

                case "building.storage-shed":
                    BuildStorageShedInterior();
                    break;

                case "building.farm-barn":
                    BuildFarmBarnInterior();
                    break;

                default:
                    BuildStarterCabinInterior();
                    break;
            }
        }

        #region Distinct Interior Layout Builders

        private void BuildStarterCabinInterior()
        {
            // Starter Log Cabin: Warm golden oak floor, log wall with window
            CreateInteriorSprite("Floor - Wood Planks", PrototypePixelArtFactory.CabinInteriorFloor(), new Vector3(0f, -0.45f, 0f), -9300, new Vector3(1.25f, 1.25f, 1f));
            CreateInteriorSprite("Wall - Log Backwall", PrototypePixelArtFactory.CabinInteriorWall(), new Vector3(0f, 3.55f, 0f), -9200, new Vector3(1.25f, 1f, 1f));

            // Left: Bedroom
            bedTransform = CreateInteriorSprite("Bed - Rustic Nook", PrototypePixelArtFactory.CabinBed(), new Vector3(-4.55f, -1.55f, 0f), -8900, Vector3.one).transform;
            chestTransform = CreateInteriorSprite("Personal Storage Chest", PrototypePixelArtFactory.ChestClosed(), new Vector3(-4.55f, 1.25f, 0f), -8920, new Vector3(1.2f, 1.2f, 1f)).transform;

            // Center: Living Room
            CreateInteriorSprite("Table - Living Room", PrototypePixelArtFactory.CabinTable(), new Vector3(0f, 0f, 0f), -8945, Vector3.one);
            CreateInteriorSprite("Bench - Living Room", PrototypePixelArtFactory.CabinBench(), new Vector3(0f, 1.15f, 0f), -8950, Vector3.one);

            // Right: Kitchen & Hearth
            CreateInteriorSprite("Kitchen Counter", PrototypePixelArtFactory.CabinKitchenCounter(), new Vector3(4.25f, -0.55f, 0f), -8950, Vector3.one);
            CreateInteriorSprite("Hearth - Kitchen", PrototypePixelArtFactory.CabinHearth(), new Vector3(4.50f, 1.55f, 0f), -8940, Vector3.one);

            doorTransform = CreateInteriorSprite("Cabin Door", PrototypePixelArtFactory.CabinDoorMarker(), new Vector3(0f, -4.25f, 0f), -8850, Vector3.one).transform;
        }

        private void BuildStoneCottageFloor1()
        {
            // Stone Cottage Floor 1: Slate cobblestone tiles, grand fireplace, stairs up
            CreateInteriorSprite("Floor - Slate Flagstone", PrototypePixelArtFactory.CottageFloor1(), new Vector3(0f, -0.45f, 0f), -9300, new Vector3(1.35f, 1.25f, 1f));
            CreateInteriorSprite("Wall - Cobblestone Masonry", PrototypePixelArtFactory.CottageWall1(), new Vector3(0f, 3.55f, 0f), -9200, new Vector3(1.35f, 1f, 1f));

            // Left: Roadwarden Study & Bookcase
            CreateInteriorSprite("Roadwarden Bookcase", PrototypePixelArtFactory.CabinKitchenCounter(), new Vector3(-4.55f, 1.25f, 0f), -8950, Vector3.one);
            CreateInteriorSprite("Study Table", PrototypePixelArtFactory.CabinTable(), new Vector3(-4.55f, -1.05f, 0f), -8945, Vector3.one);

            // Center: Grand Hearth & Dining
            CreateInteriorSprite("Grand Hearth", PrototypePixelArtFactory.CabinHearth(), new Vector3(0f, 1.85f, 0f), -8940, new Vector3(1.3f, 1.3f, 1f));
            CreateInteriorSprite("Dining Table", PrototypePixelArtFactory.CabinTable(), new Vector3(0f, -0.55f, 0f), -8945, new Vector3(1.3f, 1.2f, 1f));
            CreateInteriorSprite("Hall Bench Left", PrototypePixelArtFactory.CabinBench(), new Vector3(-1.45f, -0.55f, 0f), -8950, Vector3.one);
            CreateInteriorSprite("Hall Bench Right", PrototypePixelArtFactory.CabinBench(), new Vector3(1.45f, -0.55f, 0f), -8950, Vector3.one);

            // Right: Wooden Staircase leading to Floor 2
            stairsTransform = CreateInteriorSprite("Wooden Stairs Up", PrototypePixelArtFactory.StairsWoodUp(), new Vector3(4.55f, 1.25f, 0f), -8900, Vector3.one).transform;

            doorTransform = CreateInteriorSprite("Cottage Arch Door", PrototypePixelArtFactory.CabinDoorMarker(), new Vector3(0f, -4.25f, 0f), -8850, Vector3.one).transform;
        }

        private void BuildStoneCottageFloor2()
        {
            // Stone Cottage Floor 2: Master Suite, Persian rug, mountain balcony window, stairs down
            CreateInteriorSprite("Floor - Upper Timber Parquet", PrototypePixelArtFactory.CottageFloor2(), new Vector3(0f, -0.45f, 0f), -9300, new Vector3(1.35f, 1.25f, 1f));
            CreateInteriorSprite("Wall - Mountain Balcony Window", PrototypePixelArtFactory.CottageWall2(), new Vector3(0f, 3.55f, 0f), -9200, new Vector3(1.35f, 1f, 1f));

            // Left: Master Royal Bed & Heirloom Chest
            bedTransform = CreateInteriorSprite("Royal Master Bed", PrototypePixelArtFactory.RoyalCanopyBed(), new Vector3(-4.25f, -0.55f, 0f), -8900, Vector3.one).transform;
            chestTransform = CreateInteriorSprite("Heirloom Wardrobe Chest", PrototypePixelArtFactory.ChestClosed(), new Vector3(-4.25f, 1.85f, 0f), -8920, new Vector3(1.3f, 1.3f, 1f)).transform;

            // Center: Balcony Desk & Window View
            CreateInteriorSprite("Balcony Window Desk", PrototypePixelArtFactory.CabinTable(), new Vector3(0f, 1.35f, 0f), -8945, new Vector3(1.3f, 1.2f, 1f));
            CreateInteriorSprite("Plush Armchair", PrototypePixelArtFactory.CabinBench(), new Vector3(0f, -0.15f, 0f), -8950, Vector3.one);

            // Right: Stairwell Down to Floor 1
            stairsTransform = CreateInteriorSprite("Stairs Down to Floor 1", PrototypePixelArtFactory.StairsWoodDown(), new Vector3(4.55f, -2.05f, 0f), -8900, Vector3.one).transform;

            doorTransform = null; // Exit door is on Floor 1
        }

        private void BuildManorFloor1()
        {
            // Royal Manor Floor 1: Checkered Marble Hall, Dual Hearths, Grand Double Staircase
            CreateInteriorSprite("Floor - Grand Checkered Marble", PrototypePixelArtFactory.ManorFloor1(), new Vector3(0f, -0.45f, 0f), -9300, new Vector3(1.5f, 1.35f, 1f));
            CreateInteriorSprite("Wall - Velvet Crimson Palace", PrototypePixelArtFactory.ManorWall1(), new Vector3(0f, 3.85f, 0f), -9200, new Vector3(1.5f, 1.1f, 1f));

            // Dual Grand Hearths
            CreateInteriorSprite("West Grand Hearth", PrototypePixelArtFactory.CabinHearth(), new Vector3(-5.25f, 2.05f, 0f), -8940, new Vector3(1.3f, 1.3f, 1f));
            CreateInteriorSprite("East Grand Hearth", PrototypePixelArtFactory.CabinHearth(), new Vector3(5.25f, 2.05f, 0f), -8940, new Vector3(1.3f, 1.3f, 1f));

            // Center: Grand Banquet Feast Table
            CreateInteriorSprite("Banquet Feast Table", PrototypePixelArtFactory.CabinTable(), new Vector3(0f, 0.25f, 0f), -8945, new Vector3(1.6f, 1.3f, 1f));
            CreateInteriorSprite("Royal Chairs Left", PrototypePixelArtFactory.CabinBench(), new Vector3(-1.85f, 0.25f, 0f), -8950, Vector3.one);
            CreateInteriorSprite("Royal Chairs Right", PrototypePixelArtFactory.CabinBench(), new Vector3(1.85f, 0.25f, 0f), -8950, Vector3.one);

            // Grand Double Staircase Up to Floor 2
            stairsTransform = CreateInteriorSprite("Grand Double Staircase Up", PrototypePixelArtFactory.StairsGrandManorUp(), new Vector3(4.85f, -1.85f, 0f), -8900, Vector3.one).transform;

            doorTransform = CreateInteriorSprite("Manor Double Doors", PrototypePixelArtFactory.CabinDoorMarker(), new Vector3(0f, -4.25f, 0f), -8850, Vector3.one).transform;
        }

        private void BuildManorFloor2()
        {
            // Royal Manor Floor 2: Crimson velvet carpet, King Canopy Bed, Stained Glass
            CreateInteriorSprite("Floor - Velvet Royal Carpet", PrototypePixelArtFactory.ManorFloor2(), new Vector3(0f, -0.45f, 0f), -9300, new Vector3(1.5f, 1.35f, 1f));
            CreateInteriorSprite("Wall - Stained Glass Suite", PrototypePixelArtFactory.ManorWall2(), new Vector3(0f, 3.85f, 0f), -9200, new Vector3(1.5f, 1.1f, 1f));

            // Left: King 4-Poster Canopy Bed
            bedTransform = CreateInteriorSprite("King Canopy Bed", PrototypePixelArtFactory.RoyalCanopyBed(), new Vector3(-4.65f, -0.45f, 0f), -8900, new Vector3(1.2f, 1.2f, 1f)).transform;
            chestTransform = CreateInteriorSprite("Royal Vault Treasure Chest", PrototypePixelArtFactory.ChestClosed(), new Vector3(-4.65f, 2.05f, 0f), -8920, new Vector3(1.5f, 1.5f, 1f)).transform;

            // Center: War Council Table
            CreateInteriorSprite("Council Map Table", PrototypePixelArtFactory.CabinTable(), new Vector3(0f, 1.15f, 0f), -8945, new Vector3(1.4f, 1.3f, 1f));
            CreateInteriorSprite("Council Bench", PrototypePixelArtFactory.CabinBench(), new Vector3(0f, -0.35f, 0f), -8950, Vector3.one);

            // Right: Grand Stairs Down
            stairsTransform = CreateInteriorSprite("Grand Stairs Down", PrototypePixelArtFactory.StairsGrandManorDown(), new Vector3(4.85f, -2.05f, 0f), -8900, Vector3.one).transform;

            doorTransform = null;
        }

        private void BuildWindmillFloor1()
        {
            // Windmill Floor 1: Milling Room with Gear, Millstone, Miller's Ladder
            CreateInteriorSprite("Floor - Circular Stone Flour", PrototypePixelArtFactory.WindmillFloor1(), new Vector3(0f, -0.45f, 0f), -9300, new Vector3(1.25f, 1.25f, 1f));
            CreateInteriorSprite("Wall - Mill Tower Shaft", PrototypePixelArtFactory.WindmillWall1(), new Vector3(0f, 3.55f, 0f), -9200, new Vector3(1.25f, 1f, 1f));

            // Center: Millstone Grinder
            CreateInteriorSprite("Millstone Grinder", PrototypePixelArtFactory.MillstoneGrinder(), new Vector3(0f, 1.45f, 0f), -8940, Vector3.one);
            CreateInteriorSprite("Flour Sack Pile", PrototypePixelArtFactory.CabinBench(), new Vector3(-3.25f, 0.45f, 0f), -8950, Vector3.one);
            CreateInteriorSprite("Grain Sacks", PrototypePixelArtFactory.CabinBench(), new Vector3(-3.25f, -1.85f, 0f), -8950, Vector3.one);

            // Right: Miller's Ladder Up to Roost
            stairsTransform = CreateInteriorSprite("Miller Ladder Up", PrototypePixelArtFactory.MillerLadder(), new Vector3(4.05f, 1.15f, 0f), -8900, Vector3.one).transform;

            doorTransform = CreateInteriorSprite("Mill Door", PrototypePixelArtFactory.CabinDoorMarker(), new Vector3(0f, -4.25f, 0f), -8850, Vector3.one).transform;
        }

        private void BuildWindmillFloor2()
        {
            // Windmill Floor 2: Miller's Attic Roost, Round Window, Straw Bed
            CreateInteriorSprite("Floor - Attic Planks", PrototypePixelArtFactory.WindmillFloor2(), new Vector3(0f, -0.45f, 0f), -9300, new Vector3(1.2f, 1.2f, 1f));
            CreateInteriorSprite("Wall - Conical Roof Window", PrototypePixelArtFactory.WindmillWall2(), new Vector3(0f, 3.55f, 0f), -9200, new Vector3(1.2f, 1f, 1f));

            // Left: Cozy Straw Bed & Tool Chest
            bedTransform = CreateInteriorSprite("Miller Straw Bed", PrototypePixelArtFactory.CabinBed(), new Vector3(-3.45f, -0.65f, 0f), -8900, Vector3.one).transform;
            chestTransform = CreateInteriorSprite("Miller Tool Chest", PrototypePixelArtFactory.ChestClosed(), new Vector3(-3.45f, 1.45f, 0f), -8920, Vector3.one).transform;

            // Center: Workdesk
            CreateInteriorSprite("Maintenance Workdesk", PrototypePixelArtFactory.CabinTable(), new Vector3(0f, 1.15f, 0f), -8945, Vector3.one);

            // Right: Ladder Down to Milling Room
            stairsTransform = CreateInteriorSprite("Ladder Down to Milling Room", PrototypePixelArtFactory.MillerLadder(), new Vector3(3.85f, -1.95f, 0f), -8900, Vector3.one).transform;

            doorTransform = null;
        }

        private void BuildLookoutTowerFloor1()
        {
            // Lookout Tower Floor 1: Armory, Crossbows, Shields, Ladder Up
            CreateInteriorSprite("Floor - Tower Stone", PrototypePixelArtFactory.CottageFloor1(), new Vector3(0f, -0.45f, 0f), -9300, new Vector3(1.15f, 1.15f, 1f));
            CreateInteriorSprite("Wall - Stone Masonry", PrototypePixelArtFactory.CottageWall1(), new Vector3(0f, 3.55f, 0f), -9200, new Vector3(1.15f, 1f, 1f));

            CreateInteriorSprite("Weapon Rack", PrototypePixelArtFactory.CabinKitchenCounter(), new Vector3(-3.45f, 1.25f, 0f), -8950, Vector3.one);
            CreateInteriorSprite("Shield Crate", PrototypePixelArtFactory.ChestClosed(), new Vector3(-3.45f, -1.25f, 0f), -8920, Vector3.one);

            stairsTransform = CreateInteriorSprite("Observation Ladder Up", PrototypePixelArtFactory.MillerLadder(), new Vector3(3.45f, 1.25f, 0f), -8900, Vector3.one).transform;
            doorTransform = CreateInteriorSprite("Tower Gate Door", PrototypePixelArtFactory.CabinDoorMarker(), new Vector3(0f, -4.25f, 0f), -8850, Vector3.one).transform;
        }

        private void BuildLookoutTowerFloor2()
        {
            // Lookout Tower Floor 2: Top Observation Scout Deck with Telescope
            CreateInteriorSprite("Floor - Crow's Nest", PrototypePixelArtFactory.CottageFloor2(), new Vector3(0f, -0.45f, 0f), -9300, new Vector3(1.1f, 1.1f, 1f));
            CreateInteriorSprite("Wall - Open Battlements", PrototypePixelArtFactory.CottageWall2(), new Vector3(0f, 3.55f, 0f), -9200, new Vector3(1.1f, 1f, 1f));

            CreateInteriorSprite("Scout Telescope", PrototypePixelArtFactory.CabinTable(), new Vector3(0f, 1.45f, 0f), -8945, Vector3.one);
            stairsTransform = CreateInteriorSprite("Ladder Down to Armory", PrototypePixelArtFactory.MillerLadder(), new Vector3(3.25f, -2.05f, 0f), -8900, Vector3.one).transform;

            doorTransform = null;
        }

        private void BuildHerbalistHutInterior()
        {
            // Herbalist Hut: Herbal floor, drying racks, Apothecary station
            CreateInteriorSprite("Floor - Herbal Wood", PrototypePixelArtFactory.HerbalistFloor(), new Vector3(0f, -0.45f, 0f), -9300, new Vector3(1.25f, 1.25f, 1f));
            CreateInteriorSprite("Wall - Thatch Herb Wall", PrototypePixelArtFactory.HerbalistWall(), new Vector3(0f, 3.55f, 0f), -9200, new Vector3(1.25f, 1f, 1f));

            // Apothecary Lab Station & Herb Counter
            CreateInteriorSprite("Alchemy Lab Station", PrototypePixelArtFactory.ApothecaryStation(), new Vector3(-4.0f, 1.45f, 0f), -8940, Vector3.one);
            CreateInteriorSprite("Herb Drying Counter", PrototypePixelArtFactory.CabinKitchenCounter(), new Vector3(4.0f, 1.25f, 0f), -8950, Vector3.one);

            chestTransform = CreateInteriorSprite("Apothecary Medicine Chest", PrototypePixelArtFactory.ChestClosed(), new Vector3(-4.0f, -1.45f, 0f), -8920, new Vector3(1.2f, 1.2f, 1f)).transform;
            CreateInteriorSprite("Herbalist Tea Table", PrototypePixelArtFactory.CabinTable(), new Vector3(0f, -0.25f, 0f), -8945, Vector3.one);

            doorTransform = CreateInteriorSprite("Herbalist Door", PrototypePixelArtFactory.CabinDoorMarker(), new Vector3(0f, -4.25f, 0f), -8850, Vector3.one).transform;
        }

        private void BuildGreenhouseInterior()
        {
            // Greenhouse: Terracotta floor with 4 garden beds, Glass ceiling with ivy
            CreateInteriorSprite("Floor - Terracotta Soil Beds", PrototypePixelArtFactory.GreenhouseFloor(), new Vector3(0f, -0.45f, 0f), -9300, new Vector3(1.35f, 1.25f, 1f));
            CreateInteriorSprite("Wall - Glass Panes & Ivy", PrototypePixelArtFactory.GreenhouseWall(), new Vector3(0f, 3.55f, 0f), -9200, new Vector3(1.35f, 1f, 1f));

            // 4 Indoor Planters
            CreateInteriorSprite("Indoor Planter 1", PrototypePixelArtFactory.IndoorPlanterBed(), new Vector3(-3.8f, 0.95f, 0f), -8950, Vector3.one);
            CreateInteriorSprite("Indoor Planter 2", PrototypePixelArtFactory.IndoorPlanterBed(), new Vector3(-3.8f, -1.85f, 0f), -8950, Vector3.one);
            CreateInteriorSprite("Indoor Planter 3", PrototypePixelArtFactory.IndoorPlanterBed(), new Vector3(3.8f, 0.95f, 0f), -8950, Vector3.one);
            CreateInteriorSprite("Indoor Planter 4", PrototypePixelArtFactory.IndoorPlanterBed(), new Vector3(3.8f, -1.85f, 0f), -8950, Vector3.one);

            // Center Seedling Table
            CreateInteriorSprite("Seedling Worktable", PrototypePixelArtFactory.CabinTable(), new Vector3(0f, 0.45f, 0f), -8945, Vector3.one);

            doorTransform = CreateInteriorSprite("Glass Door", PrototypePixelArtFactory.CabinDoorMarker(), new Vector3(0f, -4.25f, 0f), -8850, Vector3.one).transform;
        }

        private void BuildTentInterior()
        {
            // Tent: Canvas ground with sheepskin rug, camp sleeping bag
            CreateInteriorSprite("Floor - Canvas Rug", PrototypePixelArtFactory.TentFloor(), new Vector3(0f, -0.45f, 0f), -9300, new Vector3(0.9f, 0.9f, 1f));
            CreateInteriorSprite("Wall - Tent Canvas", PrototypePixelArtFactory.TentWall(), new Vector3(0f, 2.85f, 0f), -9200, new Vector3(0.9f, 0.8f, 1f));

            bedTransform = CreateInteriorSprite("Camp Sleeping Bag", PrototypePixelArtFactory.CabinBed(), new Vector3(-2.25f, 0.05f, 0f), -8900, new Vector3(0.9f, 0.9f, 1f)).transform;
            CreateInteriorSprite("Camp Ember Pit", PrototypePixelArtFactory.CabinHearth(), new Vector3(2.25f, 0.85f, 0f), -8940, new Vector3(0.8f, 0.8f, 1f));
            chestTransform = CreateInteriorSprite("Travel Backpack", PrototypePixelArtFactory.ChestClosed(), new Vector3(2.25f, -1.25f, 0f), -8920, Vector3.one).transform;

            doorTransform = CreateInteriorSprite("Tent Flap", PrototypePixelArtFactory.CabinDoorMarker(), new Vector3(0f, -3.25f, 0f), -8850, Vector3.one).transform;
        }

        private void BuildStorageShedInterior()
        {
            // Storage Shed: Heavy Timber Floor, Tool Pegboard Wall, 3 Vault Chests
            CreateInteriorSprite("Floor - Heavy Timber", PrototypePixelArtFactory.StorageShedFloor(), new Vector3(0f, -0.45f, 0f), -9300, new Vector3(1.25f, 1.25f, 1f));
            CreateInteriorSprite("Wall - Tool Pegboard", PrototypePixelArtFactory.StorageShedWall(), new Vector3(0f, 3.55f, 0f), -9200, new Vector3(1.25f, 1f, 1f));

            chestTransform = CreateInteriorSprite("Primary Warehouse Chest", PrototypePixelArtFactory.ChestClosed(), new Vector3(-4.55f, 0.65f, 0f), -8920, new Vector3(1.4f, 1.4f, 1f)).transform;
            CreateInteriorSprite("Secondary Material Chest", PrototypePixelArtFactory.ChestClosed(), new Vector3(-4.55f, -1.85f, 0f), -8920, new Vector3(1.3f, 1.3f, 1f));
            CreateInteriorSprite("Dry Goods Chest", PrototypePixelArtFactory.ChestClosed(), new Vector3(4.55f, 0.65f, 0f), -8920, new Vector3(1.3f, 1.3f, 1f));

            CreateInteriorSprite("Carpenter Worktable", PrototypePixelArtFactory.CabinTable(), new Vector3(0f, 0.25f, 0f), -8945, new Vector3(1.3f, 1.2f, 1f));
            CreateInteriorSprite("Tool Rack", PrototypePixelArtFactory.CabinKitchenCounter(), new Vector3(0f, 2.35f, 0f), -8950, Vector3.one);

            doorTransform = CreateInteriorSprite("Shed Barn Door", PrototypePixelArtFactory.CabinDoorMarker(), new Vector3(0f, -4.25f, 0f), -8850, Vector3.one).transform;
        }

        private void BuildFarmBarnInterior()
        {
            // Farm Barn: Straw floor, hay bales, barn chest
            CreateInteriorSprite("Floor - Barn Hay", PrototypePixelArtFactory.CabinInteriorFloor(), new Vector3(0f, -0.45f, 0f), -9300, new Vector3(1.35f, 1.25f, 1f));
            CreateInteriorSprite("Wall - Red Barn Wood", PrototypePixelArtFactory.CabinInteriorWall(), new Vector3(0f, 3.55f, 0f), -9200, new Vector3(1.35f, 1f, 1f));

            CreateInteriorSprite("Hay Bales Left", PrototypePixelArtFactory.CabinBench(), new Vector3(-4.55f, 1.25f, 0f), -8950, Vector3.one);
            CreateInteriorSprite("Hay Bales Right", PrototypePixelArtFactory.CabinBench(), new Vector3(4.55f, 1.25f, 0f), -8950, Vector3.one);
            chestTransform = CreateInteriorSprite("Barn Tool Chest", PrototypePixelArtFactory.ChestClosed(), new Vector3(-4.55f, -1.85f, 0f), -8920, Vector3.one).transform;

            doorTransform = CreateInteriorSprite("Barn Gate", PrototypePixelArtFactory.CabinDoorMarker(), new Vector3(0f, -4.25f, 0f), -8850, Vector3.one).transform;
        }

        #endregion

        public void Enter(PlayerMovement player, Vector3 outdoorBuildingPosition, string buildingType = "building.cabin")
        {
            if (player == null) return;
            currentFloor = 1;
            EnsureBuilt(buildingType, 1);

            lastOutdoorPosition = outdoorBuildingPosition + new Vector3(0f, -2.2f, 0f);
            inside = true;
            interiorRoot.SetActive(true);
            player.transform.position = InteriorOriginF1 + new Vector3(0f, -3.15f, 0f);
            SetPlayerInteriorRenderMode(player.gameObject, true);
            ConfigureInteriorCamera(player.transform, 1);
            status = "Entered " + LocalizationRuntime.BuildingName(buildingType) + ".";
        }

        public void Exit(PlayerMovement player)
        {
            if (player == null) return;

            inside = false;
            currentFloor = 1;
            if (interiorRoot != null) interiorRoot.SetActive(false);
            SetPlayerInteriorRenderMode(player.gameObject, false);
            player.transform.position = lastOutdoorPosition;
            ConfigureWorldCamera(player.transform);
            status = "Exited building.";
        }

        public void ToggleFloor(PlayerMovement player)
        {
            if (player == null || !inside) return;

            int targetFloor = currentFloor == 1 ? 2 : 1;
            EnsureBuilt(currentBuildingType, targetFloor);
            interiorRoot.SetActive(true);

            Vector3 newOrigin = targetFloor == 2 ? InteriorOriginF2 : InteriorOriginF1;
            player.transform.position = newOrigin + (targetFloor == 2 ? new Vector3(4.55f, -1.25f, 0f) : new Vector3(4.55f, 0.25f, 0f));
            ConfigureInteriorCamera(player.transform, targetFloor);

            AudioManager.PlayFootstep();
            status = LocalizationRuntime.IsVietnamese
                ? (targetFloor == 2 ? "Đã lên Tầng 2!" : "Đã xuống Tầng 1!")
                : (targetFloor == 2 ? "Moved up to 2nd Floor!" : "Moved down to 1st Floor!");
            
            PlayerSpeechBubble.Say(status);
        }

        public void SleepEightHours(GameTimeController gameTime)
        {
            if (gameTime == null)
            {
                status = "No game clock found.";
                return;
            }

            gameTime.AdvanceHours(8f);
            status = "Slept 8 hours. " + gameTime.ClockText + ".";
        }

        public bool IsNearBed(Transform actor)
        {
            return inside && actor != null && bedTransform != null && Vector2.Distance(actor.position, bedTransform.position) <= 1.65f;
        }

        public bool IsNearChest(Transform actor)
        {
            return inside && actor != null && chestTransform != null && Vector2.Distance(actor.position, chestTransform.position) <= 1.85f;
        }

        public bool IsNearStairs(Transform actor)
        {
            return inside && actor != null && stairsTransform != null && Vector2.Distance(actor.position, stairsTransform.position) <= 1.75f;
        }

        public bool IsNearDoor(Transform actor)
        {
            return inside && currentFloor == 1 && actor != null && doorTransform != null && Vector2.Distance(actor.position, doorTransform.position) <= 1.85f;
        }

        public void ConstrainActorInside(Transform actor, ref Vector3 previousValidPosition)
        {
            if (!inside || actor == null) return;

            Vector2 boundsMin = currentFloor == 2 ? BoundsMinF2 : BoundsMinF1;
            Vector2 boundsMax = currentFloor == 2 ? BoundsMaxF2 : BoundsMaxF1;

            Vector3 position = actor.position;
            position.x = Mathf.Clamp(position.x, boundsMin.x, boundsMax.x);
            position.y = Mathf.Clamp(position.y, boundsMin.y, boundsMax.y);

            actor.position = position;
            previousValidPosition = actor.position;
        }

        private GameObject CreateInteriorSprite(string name, Sprite sprite, Vector3 localPosition, int sortingOrder, Vector3 scale)
        {
            GameObject obj = new GameObject(name);
            obj.transform.SetParent(interiorRoot.transform, false);
            obj.transform.localPosition = localPosition;
            obj.transform.localScale = scale;
            SpriteRenderer renderer = obj.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.sortingOrder = sortingOrder;
            return obj;
        }

        private static void ConfigureInteriorCamera(Transform target, int floor)
        {
            Camera camera = Camera.main;
            if (camera == null) return;

            Vector2 center = floor == 2 ? CameraCenterF2 : CameraCenterF1;
            camera.orthographicSize = 5.65f;
            camera.transform.position = new Vector3(center.x, center.y, -10f);

            CameraFollow2D follow = camera.GetComponent<CameraFollow2D>();
            if (follow != null)
            {
                follow.Configure(target, center, center, 0f);
            }
        }

        private static void ConfigureWorldCamera(Transform target)
        {
            Camera camera = Camera.main;
            if (camera == null) return;

            camera.orthographicSize = 6f;
            camera.transform.position = new Vector3(target.position.x, target.position.y, -10f);

            CameraFollow2D follow = camera.GetComponent<CameraFollow2D>();
            if (follow != null)
            {
                follow.Configure(target, new Vector2(-53f, -31f), new Vector2(53f, 31f), 0.12f);
            }
        }

        private static void SetPlayerInteriorRenderMode(GameObject player, bool interiorMode)
        {
            if (player == null) return;

            YSortSprite sorter = player.GetComponent<YSortSprite>();
            if (sorter != null) sorter.enabled = !interiorMode;

            SpriteRenderer spriteRenderer = player.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.enabled = true;
                spriteRenderer.sortingOrder = interiorMode ? InteriorPlayerSortingOrder : 50;
                spriteRenderer.shadowCastingMode = ShadowCastingMode.Off;
                spriteRenderer.receiveShadows = false;
            }

            Renderer[] childRenderers = player.GetComponentsInChildren<Renderer>(true);
            foreach (Renderer renderer in childRenderers)
            {
                if (renderer == null || renderer == spriteRenderer) continue;

                renderer.shadowCastingMode = ShadowCastingMode.Off;
                renderer.receiveShadows = false;

                if (renderer is MeshRenderer || renderer.name.ToLowerInvariant().Contains("shadow"))
                {
                    renderer.enabled = false;
                }
            }
        }
    }
}
