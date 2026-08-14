using UnityEngine;
using TheOldRoad.Building;
using TheOldRoad.Core;
using TheOldRoad.Input;
using TheOldRoad.Inventory;
using TheOldRoad.Player;

namespace TheOldRoad.UI
{
    public sealed class InventoryDebugHud : MonoBehaviour
    {
        [SerializeField] private InventorySession inventorySession;
        [SerializeField] private BuildingPlacementController placementController;
        [SerializeField] private VerticalSliceController sliceController;

        private TextMesh textMesh;

        public void Configure(
            InventorySession inventorySession,
            BuildingPlacementController placementController = null,
            VerticalSliceController sliceController = null)
        {
            this.inventorySession = inventorySession;
            this.placementController = placementController;
            this.sliceController = sliceController;
        }

        private void Update()
        {
            EnsureTextMesh();
            if (inventorySession == null || inventorySession.Runtime == null) return;

            int wood = inventorySession.Runtime.GetQuantity("item.wood");
            int stone = inventorySession.Runtime.GetQuantity("item.stone");

            string text = "The Old Road - Valen Outskirts\n" +
                          "Wood: " + wood + " | Stone: " + stone + "\n" +
                          "Move: WASD/Arrows or hold Right Mouse | Gather: E | Build: B\n" +
                          "Save: automatic on gather/build/quit";

            text += "\n" + BuildInputDebugText();

            if (placementController != null && placementController.BuildingDefinition != null)
            {
                text += "\nCabin cost: " + FormatCosts(placementController.BuildingDefinition) +
                        "\n" + placementController.LastStatus;
            }

            if (sliceController != null)
            {
                text += "\nConstruction jobs: " + sliceController.ActiveConstructionCount +
                        "\n" + sliceController.SaveStatus;
            }

            textMesh.text = text;
        }

        private static string BuildInputDebugText()
        {
            KeyboardPlayerInputSource input = FindAnyObjectByType<KeyboardPlayerInputSource>();
            PlayerMovement player = FindAnyObjectByType<PlayerMovement>();

            string move = input != null
                ? $"move=({input.LastMove.x:0.00},{input.LastMove.y:0.00})"
                : "move=input-missing";

            string position = player != null
                ? $"player=({player.transform.position.x:0.00},{player.transform.position.y:0.00})"
                : "player=missing";

            return PrototypeInput.Diagnostics + " | " + move + " | " + position;
        }

        private void EnsureTextMesh()
        {
            if (textMesh != null) return;

            GameObject textObject = new GameObject("Development HUD Text");
            Camera mainCamera = Camera.main;
            if (mainCamera != null)
            {
                textObject.transform.SetParent(mainCamera.transform, false);
                textObject.transform.localPosition = new Vector3(-4.75f, 4.4f, 10f);
            }
            else
            {
                textObject.transform.position = new Vector3(-4.75f, 4.4f, 0f);
            }

            textObject.transform.localScale = new Vector3(0.16f, 0.16f, 0.16f);
            textMesh = textObject.AddComponent<TextMesh>();
            textMesh.anchor = TextAnchor.UpperLeft;
            textMesh.alignment = TextAlignment.Left;
            textMesh.fontSize = 28;
            textMesh.color = Color.white;
        }

        private static string FormatCosts(BuildingDefinition definition)
        {
            if (definition.ConstructionCosts == null || definition.ConstructionCosts.Length == 0) return "none";

            string text = string.Empty;
            for (int i = 0; i < definition.ConstructionCosts.Length; i++)
            {
                BuildCostEntry cost = definition.ConstructionCosts[i];
                if (i > 0) text += ", ";
                text += cost.quantity + " " + cost.itemId;
            }

            return text;
        }
    }
}
