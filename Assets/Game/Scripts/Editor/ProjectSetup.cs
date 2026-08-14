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
            CreateRecipe();
            CreateCabin();
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
            BuildingDefinition cabin = LoadOrCreate<BuildingDefinition>("Assets/Game/Data/Buildings/Cabin.asset");
            cabin.ConfigureForPrototype(
                "building.cabin",
                new Vector2Int(2, 2),
                new[]
                {
                    new BuildCostEntry { itemId = "item.wood", quantity = 3 },
                    new BuildCostEntry { itemId = "item.stone", quantity = 2 }
                },
                30f,
                new[] { "Foundation", "Frame", "Walls", "Roof", "Complete" });
            EditorUtility.SetDirty(cabin);
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
