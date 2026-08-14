using UnityEngine;

namespace TheOldRoad.Building
{
    [System.Serializable]
    public struct BuildCostEntry
    {
        public string itemId;
        [Min(1)] public int quantity;
    }

    [CreateAssetMenu(menuName = "The Old Road/Building/Building Definition")]
    public sealed class BuildingDefinition : ScriptableObject
    {
        [SerializeField] private string buildingId;
        [SerializeField] private Vector2Int footprint = Vector2Int.one;
        [SerializeField] private BuildCostEntry[] constructionCosts;
        [SerializeField, Min(0f)] private float constructionDurationSeconds;
        [SerializeField] private string[] constructionStages = { "Foundation", "Frame", "Walls", "Roof", "Complete" };

        public string BuildingId => buildingId;
        public Vector2Int Footprint => footprint;
        public BuildCostEntry[] ConstructionCosts => constructionCosts;
        public float ConstructionDurationSeconds => constructionDurationSeconds;
        public string[] ConstructionStages => constructionStages;

        public void ConfigureForPrototype(
            string buildingId,
            Vector2Int footprint,
            BuildCostEntry[] constructionCosts,
            float constructionDurationSeconds,
            string[] constructionStages)
        {
            this.buildingId = buildingId;
            this.footprint = new Vector2Int(Mathf.Max(1, footprint.x), Mathf.Max(1, footprint.y));
            this.constructionCosts = constructionCosts ?? System.Array.Empty<BuildCostEntry>();
            this.constructionDurationSeconds = Mathf.Max(0f, constructionDurationSeconds);
            this.constructionStages = constructionStages == null || constructionStages.Length == 0
                ? new[] { "Foundation", "Frame", "Walls", "Roof", "Complete" }
                : constructionStages;
        }
    }
}
