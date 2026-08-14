using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using TheOldRoad.Building;
using TheOldRoad.Core;
using TheOldRoad.Crafting;
using TheOldRoad.Items;

namespace TheOldRoad.Editor
{
    public static class ProjectSetup
    {
        private const string BootstrapScenePath = "Assets/Game/Scenes/Bootstrap/Bootstrap.unity";

        [MenuItem("The Old Road/Rebuild Prototype Project")]
        public static void RebuildPrototypeProject()
        {
            EnsureFolders();
            CreateItem("Assets/Game/Data/Items/Wood.asset", "item.wood", "Wood", 99);
            CreateItem("Assets/Game/Data/Items/Stone.asset", "item.stone", "Stone", 99);
            CreateItem("Assets/Game/Data/Items/CabinPlank.asset", "item.cabin-plank", "Cabin Plank", 99);
            CreateItem("Assets/Game/Data/Items/CookedMeal.asset", "item.cooked-meal", "Cooked Meal", 20);
            CreateItem("Assets/Game/Data/Items/Egg.asset", "item.egg", "Egg", 50);
            CreateItem("Assets/Game/Data/Items/Wool.asset", "item.wool", "Wool", 50);
            CreateItem("Assets/Game/Data/Items/Milk.asset", "item.milk", "Milk", 50);
            CreateRecipe();
            CreateCabin();
            CreatePrototypeBuilding(
                "Assets/Game/Data/Buildings/Campfire.asset",
                "building.campfire",
                new Vector2Int(1, 1),
                new[]
                {
                    new BuildCostEntry { itemId = "item.wood", quantity = 2 },
                    new BuildCostEntry { itemId = "item.stone", quantity = 2 }
                },
                12f,
                new[] { "Ring", "Kindling", "Flame" });
            CreatePrototypeBuilding(
                "Assets/Game/Data/Buildings/CookingHearth.asset",
                "building.cooking-hearth",
                new Vector2Int(2, 1),
                new[]
                {
                    new BuildCostEntry { itemId = "item.stone", quantity = 5 },
                    new BuildCostEntry { itemId = "item.wood", quantity = 2 },
                    new BuildCostEntry { itemId = "item.iron-ore", quantity = 1 }
                },
                25f,
                new[] { "Base", "Chamber", "Ready" });
            CreatePrototypeBuilding(
                "Assets/Game/Data/Buildings/SmallAnimalPen.asset",
                "building.animal-pen-small",
                new Vector2Int(3, 2),
                new[]
                {
                    new BuildCostEntry { itemId = "item.wood", quantity = 6 },
                    new BuildCostEntry { itemId = "item.stone", quantity = 2 }
                },
                35f,
                new[] { "Posts", "Rails", "Gate", "Ready" });
            CreatePrototypeBuilding(
                "Assets/Game/Data/Buildings/LongAnimalPen.asset",
                "building.animal-pen-long",
                new Vector2Int(4, 2),
                new[]
                {
                    new BuildCostEntry { itemId = "item.wood", quantity = 10 },
                    new BuildCostEntry { itemId = "item.stone", quantity = 4 }
                },
                45f,
                new[] { "Posts", "Rails", "Gate", "Ready" });
            CreatePrototypeBuilding(
                "Assets/Game/Data/Buildings/StorageShed.asset",
                "building.storage-shed",
                new Vector2Int(2, 2),
                new[]
                {
                    new BuildCostEntry { itemId = "item.wood", quantity = 5 },
                    new BuildCostEntry { itemId = "item.cabin-plank", quantity = 1 },
                    new BuildCostEntry { itemId = "item.stone", quantity = 2 }
                },
                30f,
                new[] { "Foundation", "Frame", "Roof", "Ready" });
            CreatePrototypeBuilding(
                "Assets/Game/Data/Buildings/StoneCottage.asset",
                "building.stone-cottage",
                new Vector2Int(3, 2),
                new[]
                {
                    new BuildCostEntry { itemId = "item.stone", quantity = 8 },
                    new BuildCostEntry { itemId = "item.wood", quantity = 8 },
                    new BuildCostEntry { itemId = "item.cabin-plank", quantity = 2 }
                },
                60f,
                new[] { "Foundation", "Frame", "Walls", "Roof", "Complete" });
            CreateBootstrapScene();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static void EnsureFolders()
        {
            string[] folders =
            {
                "Assets/Game/Art/Characters",
                "Assets/Game/Art/Environment",
                "Assets/Game/Art/Buildings",
                "Assets/Game/Art/Items",
                "Assets/Game/Art/UI",
                "Assets/Game/Art/VFX",
                "Assets/Game/Audio/Music",
                "Assets/Game/Audio/Ambient",
                "Assets/Game/Audio/SFX",
                "Assets/Game/Data/Items",
                "Assets/Game/Data/Recipes",
                "Assets/Game/Data/Buildings",
                "Assets/Game/Data/NPCs",
                "Assets/Game/Data/Quests",
                "Assets/Game/Prefabs/Player",
                "Assets/Game/Prefabs/Resources",
                "Assets/Game/Prefabs/Buildings",
                "Assets/Game/Prefabs/NPCs",
                "Assets/Game/Prefabs/UI",
                "Assets/Game/Scenes/Bootstrap",
                "Assets/Game/Scenes/World",
                "Assets/Game/Scenes/Interiors",
                "Assets/Game/Scenes/Test",
                "Assets/Game/Tests/EditMode",
                "Assets/Game/Tests/PlayMode"
            };

            foreach (string folder in folders)
            {
                Directory.CreateDirectory(folder);
            }
        }

        private static void CreateItem(string path, string itemId, string displayName, int maxStack)
        {
            ItemDefinition item = LoadOrCreate<ItemDefinition>(path);
            item.ConfigureForPrototype(itemId, displayName, maxStack);
            EditorUtility.SetDirty(item);
        }

        private static void CreateRecipe()
        {
            RecipeDefinition recipe = LoadOrCreate<RecipeDefinition>("Assets/Game/Data/Recipes/CabinPlanks.asset");
            recipe.ConfigureForPrototype(
                "recipe.cabin-planks",
                new[] { new IngredientRequirement { itemId = "item.wood", quantity = 2 } },
                "item.cabin-plank",
                1,
                0f,
                string.Empty);
            EditorUtility.SetDirty(recipe);
        }

        private static void CreateCabin()
        {
            CreatePrototypeBuilding(
                "Assets/Game/Data/Buildings/Cabin.asset",
                "building.cabin",
                new Vector2Int(2, 2),
                new[]
                {
                    new BuildCostEntry { itemId = "item.wood", quantity = 3 },
                    new BuildCostEntry { itemId = "item.stone", quantity = 2 }
                },
                30f,
                new[] { "Foundation", "Frame", "Walls", "Roof", "Complete" });
        }

        private static void CreatePrototypeBuilding(string path, string buildingId, Vector2Int footprint, BuildCostEntry[] costs, float duration, string[] stages)
        {
            BuildingDefinition building = LoadOrCreate<BuildingDefinition>(path);
            building.ConfigureForPrototype(buildingId, footprint, costs, duration, stages);
            EditorUtility.SetDirty(building);
        }

        private static void CreateBootstrapScene()
        {
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            GameObject bootstrap = new GameObject("GameBootstrap");
            bootstrap.AddComponent<GameBootstrap>();

            EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(), BootstrapScenePath);
            string guid = AssetDatabase.AssetPathToGUID(BootstrapScenePath);
            EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(BootstrapScenePath, true) };
            Debug.Log("The Old Road prototype scene rebuilt. Scene GUID: " + guid);
        }

        private static T LoadOrCreate<T>(string path) where T : ScriptableObject
        {
            T asset = AssetDatabase.LoadAssetAtPath<T>(path);
            if (asset != null) return asset;

            Directory.CreateDirectory(Path.GetDirectoryName(path));
            asset = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(asset, path);
            return asset;
        }
    }
}
