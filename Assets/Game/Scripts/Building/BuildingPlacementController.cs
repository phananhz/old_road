using UnityEngine;
using TheOldRoad.Core;
using TheOldRoad.Inventory;
using TheOldRoad.UI;
using TheOldRoad.World;

namespace TheOldRoad.Building
{
    /// <summary>Development placement preview for the selected prototype building.</summary>
    public sealed class BuildingPlacementController : MonoBehaviour
    {
        [SerializeField] private Camera worldCamera;
        [SerializeField] private InventorySession inventorySession;
        [SerializeField] private VerticalSliceController sliceController;
        [SerializeField] private BuildingDefinition buildingDefinition;
        [SerializeField] private Vector2Int buildAreaMin = new Vector2Int(-8, -5);
        [SerializeField] private Vector2Int buildAreaMax = new Vector2Int(8, 5);
        [SerializeField, Min(0.1f)] private float gridSize = 1f;

        private GameObject preview;
        private bool placementMode;
        private Vector2Int currentCell;

        public bool IsPlacementMode => placementMode;
        public string LastStatus { get; private set; } = "Press B to open the build catalog.";
        public BuildingDefinition BuildingDefinition => buildingDefinition;

        public void Configure(
            Camera worldCamera,
            InventorySession inventorySession,
            VerticalSliceController sliceController,
            BuildingDefinition buildingDefinition,
            Vector2Int buildAreaMin,
            Vector2Int buildAreaMax,
            float gridSize)
        {
            this.worldCamera = worldCamera;
            this.inventorySession = inventorySession;
            this.sliceController = sliceController;
            this.buildingDefinition = buildingDefinition;
            this.buildAreaMin = buildAreaMin;
            this.buildAreaMax = buildAreaMax;
            this.gridSize = Mathf.Max(0.1f, gridSize);
        }

        private void Awake()
        {
            if (worldCamera == null) worldCamera = Camera.main;
        }

        private void Update()
        {
            if (!placementMode || worldCamera == null) return;

            currentCell = GetMouseCell();
            UpdatePreview(currentCell);

            if (TheOldRoad.Input.PrototypeInput.GetMouseButtonDown(0) && IsCurrentPlacementValid()) ConfirmPlacement();
            if (TheOldRoad.Input.PrototypeInput.GetMouseButtonDown(1)) CancelPlacement();
        }

        public void BeginPlacement(BuildingDefinition selectedDefinition)
        {
            if (selectedDefinition != null) buildingDefinition = selectedDefinition;
            if (buildingDefinition == null)
            {
                LastStatus = "No building definition selected.";
                PlayerSpeechBubble.Say("speech.build_blocked");
                return;
            }

            placementMode = true;
            CreatePreview();
            LastStatus = "Select a valid grid cell, then left click. Right click cancels.";
            PlayerSpeechBubble.Say("speech.build_begin");
        }

        public void CancelPlacement()
        {
            placementMode = false;
            if (preview != null) Destroy(preview);
            LastStatus = "Press B to open the build catalog.";
        }

        private void CreatePreview()
        {
            if (preview != null) Destroy(preview);

            preview = new GameObject("Cabin Placement Preview");
            preview.name = GetBuildingName() + " Placement Preview";
            preview.transform.localScale = GetFootprint();
            SpriteRenderer renderer = preview.AddComponent<SpriteRenderer>();
            renderer.sprite = PrototypePixelArtFactory.PlacementPreview();
            renderer.sortingOrder = 9000;
        }

        private void UpdatePreview(Vector2Int cell)
        {
            preview.transform.position = new Vector3(cell.x * gridSize, cell.y * gridSize, 0);
            preview.GetComponent<SpriteRenderer>().color = IsCurrentPlacementValid()
                ? new Color(0.2f, 0.9f, 0.3f, 0.55f)
                : new Color(0.9f, 0.2f, 0.2f, 0.55f);
        }

        private void ConfirmPlacement()
        {
            if (sliceController == null)
            {
                LastStatus = "World controller is not ready.";
                return;
            }

            bool started = sliceController.TryBeginConstruction(buildingDefinition, currentCell, out string status);
            if (started)
            {
                CancelPlacement();
                PlayerSpeechBubble.Say("speech.build_started");
            }
            else
            {
                PlayerSpeechBubble.Say(status == "Invalid placement." ? "speech.build_invalid" : "speech.build_blocked");
            }

            LastStatus = status;
        }

        private bool IsCurrentPlacementValid()
        {
            Vector2Int footprint = GetFootprintInCells();
            if (sliceController != null) return sliceController.IsPlacementValid(currentCell, footprint);

            return GridPlacementValidator.IsValid(currentCell, footprint,
                new PlacementArea(buildAreaMin, buildAreaMax), false);
        }

        private Vector2Int GetFootprintInCells()
        {
            return buildingDefinition != null ? buildingDefinition.Footprint : new Vector2Int(2, 2);
        }

        private Vector3 GetFootprint() => new Vector3(GetFootprintInCells().x, GetFootprintInCells().y, 1f);

        private string GetBuildingName()
        {
            if (buildingDefinition == null || string.IsNullOrWhiteSpace(buildingDefinition.BuildingId)) return "Building";
            if (buildingDefinition.BuildingId == "building.cabin") return "Cabin";
            return buildingDefinition.BuildingId;
        }

        private Vector2Int GetMouseCell()
        {
            Vector3 world = worldCamera.ScreenToWorldPoint(TheOldRoad.Input.PrototypeInput.MousePosition);
            return new Vector2Int(Mathf.RoundToInt(world.x / gridSize), Mathf.RoundToInt(world.y / gridSize));
        }

    }
}
