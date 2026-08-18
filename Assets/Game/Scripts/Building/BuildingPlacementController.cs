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
        [SerializeField] private Vector2Int buildAreaMin = new Vector2Int(-60, -36);
        [SerializeField] private Vector2Int buildAreaMax = new Vector2Int(60, 36);
        [SerializeField, Min(0.1f)] private float gridSize = 1f;

        private GameObject preview;
        private bool placementMode;
        private bool demolishMode;
        private Vector2Int currentCell;
        private bool isDragging;
        private Vector2Int dragStartCell;
        private Vector2Int dragCurrentFootprint = new Vector2Int(10, 8);

        public bool IsPlacementMode => placementMode;
        public bool IsDemolishMode => demolishMode;
        public string LastStatus { get; private set; } = "Press B to open the build catalog. Press X to demolish.";
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
            if (TheOldRoad.Input.PrototypeInput.GetKeyDown(KeyCode.X))
            {
                if (demolishMode) CancelDemolish();
                else BeginDemolish();
                return;
            }

            if (demolishMode)
            {
                if (worldCamera == null) return;
                Vector3 mouseWorld = worldCamera.ScreenToWorldPoint(TheOldRoad.Input.PrototypeInput.MousePosition);
                mouseWorld.z = 0f;
                UpdateDemolishPreview(mouseWorld);

                if (TheOldRoad.Input.PrototypeInput.GetMouseButtonDown(0))
                {
                    if (sliceController != null && sliceController.TryDemolishBuilding(mouseWorld, out string status))
                    {
                        LastStatus = status;
                    }
                }

                if (TheOldRoad.Input.PrototypeInput.GetMouseButtonDown(1) || TheOldRoad.Input.PrototypeInput.GetKeyDown(KeyCode.Escape))
                {
                    CancelDemolish();
                }
                return;
            }

            if (!placementMode || worldCamera == null) return;

            currentCell = GetMouseCell();

            bool isDragFence = buildingDefinition != null && buildingDefinition.BuildingId.Contains("perimeter-fence-drag");

            if (isDragFence)
            {
                if (TheOldRoad.Input.PrototypeInput.GetMouseButtonDown(0))
                {
                    isDragging = true;
                    dragStartCell = currentCell;
                }

                if (isDragging)
                {
                    int minX = Mathf.Min(dragStartCell.x, currentCell.x);
                    int maxX = Mathf.Max(dragStartCell.x, currentCell.x);
                    int minY = Mathf.Min(dragStartCell.y, currentCell.y);
                    int maxY = Mathf.Max(dragStartCell.y, currentCell.y);

                    int w = Mathf.Max(4, maxX - minX + 1);
                    int h = Mathf.Max(3, maxY - minY + 1);
                    dragCurrentFootprint = new Vector2Int(w, h);
                    currentCell = new Vector2Int(minX, minY);
                }

                UpdateDragPreview(currentCell, dragCurrentFootprint);

                if (TheOldRoad.Input.PrototypeInput.GetMouseButtonUp(0) && isDragging)
                {
                    isDragging = false;
                    if (IsCurrentPlacementValid()) ConfirmPlacement();
                }
            }
            else
            {
                UpdatePreview(currentCell);

                if (TheOldRoad.Input.PrototypeInput.GetMouseButtonDown(0) && IsCurrentPlacementValid()) ConfirmPlacement();
            }

            if (TheOldRoad.Input.PrototypeInput.GetMouseButtonDown(1)) CancelPlacement();
        }

        public void BeginDemolish()
        {
            CancelPlacement();
            demolishMode = true;
            CreateDemolishPreview();
            LastStatus = LocalizationRuntime.IsVietnamese
                ? "Chế độ Xóa: Click chuột trái vào công trình để Phá dỡ & Nhận lại 100% vật phẩm. Click phải / X để thoát."
                : "Demolish Mode: Left click building to Demolish & Refund 100% materials. Right click / X to cancel.";
            PlayerSpeechBubble.Say(LastStatus);
        }

        public void CancelDemolish()
        {
            demolishMode = false;
            if (preview != null) Destroy(preview);
            LastStatus = "Press B to open the build catalog. Press X to demolish.";
        }

        private void CreateDemolishPreview()
        {
            if (preview != null) Destroy(preview);

            preview = new GameObject("Demolish Preview");
            preview.transform.localScale = Vector3.one * 1.5f;
            SpriteRenderer renderer = preview.AddComponent<SpriteRenderer>();
            renderer.sprite = PrototypePixelArtFactory.PlacementPreview();
            renderer.color = new Color(1f, 0.2f, 0.2f, 0.65f);
            renderer.sortingOrder = 9000;
        }

        private void UpdateDemolishPreview(Vector3 worldPos)
        {
            if (preview == null) return;
            preview.transform.position = worldPos;
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
            isDragging = false;
            CreatePreview();
            LastStatus = "Select a valid grid cell, then left click. Right click cancels.";
            PlayerSpeechBubble.Say("speech.build_begin");
        }

        public void CancelPlacement()
        {
            placementMode = false;
            isDragging = false;
            if (preview != null) Destroy(preview);
            LastStatus = "Press B to open the build catalog.";
        }

        private void CreatePreview()
        {
            if (preview != null) Destroy(preview);

            preview = new GameObject("Building Placement Preview");
            preview.name = GetBuildingName() + " Placement Preview";
            preview.transform.localScale = GetFootprint();
            SpriteRenderer renderer = preview.AddComponent<SpriteRenderer>();
            renderer.sprite = PrototypePixelArtFactory.PlacementPreview();
            renderer.sortingOrder = 9000;
        }

        private void UpdatePreview(Vector2Int cell)
        {
            if (preview == null) return;
            preview.transform.localScale = GetFootprint();
            preview.transform.position = new Vector3(cell.x * gridSize, cell.y * gridSize, 0);
            preview.GetComponent<SpriteRenderer>().color = IsCurrentPlacementValid()
                ? new Color(0.2f, 0.9f, 0.3f, 0.55f)
                : new Color(0.9f, 0.2f, 0.2f, 0.55f);
        }

        private void UpdateDragPreview(Vector2Int cell, Vector2Int footprint)
        {
            if (preview == null) return;
            preview.transform.localScale = new Vector3(footprint.x * gridSize, footprint.y * gridSize, 1f);
            preview.transform.position = new Vector3((cell.x + footprint.x * 0.5f - 0.5f) * gridSize, (cell.y + footprint.y * 0.5f - 0.5f) * gridSize, 0f);
            preview.GetComponent<SpriteRenderer>().color = IsCurrentPlacementValid()
                ? new Color(0.2f, 0.7f, 1.0f, 0.55f)
                : new Color(0.9f, 0.2f, 0.2f, 0.55f);
        }

        private void ConfirmPlacement()
        {
            if (sliceController == null)
            {
                LastStatus = "World controller is not ready.";
                return;
            }

            Vector2Int footprint = GetFootprintInCells();
            bool started = sliceController.TryBeginConstruction(buildingDefinition, currentCell, out string status, footprint);
            if (started)
            {
                TheOldRoad.Audio.AudioManager.PlayBuildPlace();
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
            if (buildingDefinition != null && buildingDefinition.BuildingId.Contains("perimeter-fence-drag"))
            {
                return dragCurrentFootprint;
            }
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
