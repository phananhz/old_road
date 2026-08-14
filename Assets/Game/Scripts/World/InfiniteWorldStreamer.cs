using System.Collections.Generic;
using UnityEngine;
using TheOldRoad.NPC;
using TheOldRoad.Player;

namespace TheOldRoad.World
{
    /// <summary>Deterministic chunk-streamed prototype world around the player.</summary>
    public sealed class InfiniteWorldStreamer : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField, Min(8f)] private float chunkSize = 32f;
        [SerializeField, Min(1)] private int loadRadius = 2;
        [SerializeField, Min(1)] private int unloadRadius = 3;
        [SerializeField] private int worldSeed = 43129;

        private readonly Dictionary<Vector2Int, GameObject> loadedChunks = new Dictionary<Vector2Int, GameObject>();
        private Vector2Int lastCenter = new Vector2Int(int.MinValue, int.MinValue);
        private float nextRefreshTime;

        public int LoadedChunkCount => loadedChunks.Count;

        public void Configure(Transform target, int worldSeed, float chunkSize, int loadRadius, int unloadRadius)
        {
            this.target = target;
            this.worldSeed = worldSeed;
            this.chunkSize = Mathf.Max(8f, chunkSize);
            this.loadRadius = Mathf.Max(1, loadRadius);
            this.unloadRadius = Mathf.Max(this.loadRadius, unloadRadius);
            Refresh(true);
        }

        private void Update()
        {
            if (target == null)
            {
                PlayerMovement player = FindAnyObjectByType<PlayerMovement>();
                if (player != null) target = player.transform;
            }

            if (target == null || UnityEngine.Time.unscaledTime < nextRefreshTime) return;
            nextRefreshTime = UnityEngine.Time.unscaledTime + 0.35f;
            Refresh(false);
        }

        private void Refresh(bool force)
        {
            if (target == null) return;

            Vector2Int center = WorldToChunk(target.position);
            if (!force && center == lastCenter) return;
            lastCenter = center;

            for (int y = center.y - loadRadius; y <= center.y + loadRadius; y++)
            {
                for (int x = center.x - loadRadius; x <= center.x + loadRadius; x++)
                {
                    Vector2Int coord = new Vector2Int(x, y);
                    if (!loadedChunks.ContainsKey(coord)) loadedChunks[coord] = CreateChunk(coord);
                }
            }

            List<Vector2Int> toUnload = new List<Vector2Int>();
            foreach (Vector2Int coord in loadedChunks.Keys)
            {
                int distance = Mathf.Max(Mathf.Abs(coord.x - center.x), Mathf.Abs(coord.y - center.y));
                if (distance > unloadRadius) toUnload.Add(coord);
            }

            for (int i = 0; i < toUnload.Count; i++)
            {
                Vector2Int coord = toUnload[i];
                if (!loadedChunks.TryGetValue(coord, out GameObject chunk)) continue;
                loadedChunks.Remove(coord);
                if (chunk != null) Destroy(chunk);
            }
        }

        private GameObject CreateChunk(Vector2Int coord)
        {
            GameObject chunk = new GameObject("World Chunk " + coord.x + "," + coord.y);
            chunk.transform.SetParent(transform, false);
            chunk.transform.position = ChunkCenter(coord);

            SpriteRenderer ground = chunk.AddComponent<SpriteRenderer>();
            ground.sprite = PrototypePixelArtFactory.WorldChunkGround(coord.x, coord.y, worldSeed);
            ground.sortingOrder = -10050;
            ground.transform.localScale = Vector3.one * (chunkSize / 16f);

            AddChunkDecorations(chunk.transform, coord);
            if (ShouldSpawnVillage(coord)) AddVillage(chunk.transform, coord);
            return chunk;
        }

        private void AddChunkDecorations(Transform parent, Vector2Int coord)
        {
            int count = 7 + Mathf.FloorToInt(Hash01(coord.x, coord.y, 19) * 8f);
            for (int i = 0; i < count; i++)
            {
                float x = ChunkMin(coord).x + 3f + Hash01(coord.x, coord.y, 100 + i) * (chunkSize - 6f);
                float y = ChunkMin(coord).y + 3f + Hash01(coord.x, coord.y, 200 + i) * (chunkSize - 6f);
                float roll = Hash01(coord.x, coord.y, 300 + i);
                Sprite sprite = roll < 0.46f
                    ? PrototypePixelArtFactory.Tree()
                    : roll < 0.70f
                        ? PrototypePixelArtFactory.Rock()
                        : roll < 0.82f
                            ? PrototypePixelArtFactory.BerryBush()
                            : roll < 0.92f
                                ? PrototypePixelArtFactory.MushroomCluster()
                                : PrototypePixelArtFactory.HerbPatch();

                GameObject decoration = CreateSprite("Wild " + i, sprite, new Vector3(x, y, 0f), 0);
                decoration.transform.SetParent(parent, true);
            }
        }

        private void AddVillage(Transform parent, Vector2Int coord)
        {
            string villageName = GetVillageName(coord);
            GameObject village = new GameObject("Village - " + villageName);
            village.transform.SetParent(parent, false);
            village.transform.position = ChunkCenter(coord) + new Vector3(-6f, -2f, 0f);

            Vector3 origin = village.transform.position;
            CreateVillageBuilding(village.transform, villageName + " Hall", PrototypePixelArtFactory.StoneCottage(), origin + new Vector3(0f, 2.4f, 0f), 4);
            CreateVillageBuilding(village.transform, villageName + " Cabin", PrototypePixelArtFactory.CabinComplete(), origin + new Vector3(-5.3f, -1.2f, 0f), 3);
            CreateVillageBuilding(village.transform, villageName + " Storehouse", PrototypePixelArtFactory.StorageShed(), origin + new Vector3(5.3f, -1.5f, 0f), 3);
            CreateVillageBuilding(village.transform, villageName + " Hearth", PrototypePixelArtFactory.Campfire(), origin + new Vector3(0.6f, -3.7f, 0f), 5);
            CreateVillageSign(village.transform, villageName, origin + new Vector3(-7.3f, 3.2f, 0f));

            AddVillageNpc(village.transform, villageName, "Miller", origin + new Vector3(-2f, -3.2f, 0f), new[]
            {
                origin + new Vector3(-2f, -3.2f, 0f),
                origin + new Vector3(-5.3f, -1.2f, 0f),
                origin + new Vector3(0.6f, -3.7f, 0f)
            });
            AddVillageNpc(village.transform, villageName, "Woodcutter", origin + new Vector3(4.6f, 1.4f, 0f), new[]
            {
                origin + new Vector3(4.6f, 1.4f, 0f),
                origin + new Vector3(7.4f, 3.6f, 0f),
                origin + new Vector3(5.3f, -1.5f, 0f)
            });
            AddVillageNpc(village.transform, villageName, "Herbalist", origin + new Vector3(-6.6f, 3.5f, 0f), new[]
            {
                origin + new Vector3(-6.6f, 3.5f, 0f),
                origin + new Vector3(-3.6f, 4.5f, 0f),
                origin + new Vector3(-5.3f, -1.2f, 0f)
            });
        }

        private void CreateVillageBuilding(Transform parent, string name, Sprite sprite, Vector3 position, int sortingOffset)
        {
            GameObject building = CreateSprite(name, sprite, position, sortingOffset);
            building.transform.SetParent(parent, true);
        }

        private void CreateVillageSign(Transform parent, string villageName, Vector3 position)
        {
            GameObject sign = CreateSprite(villageName + " Sign", PrototypePixelArtFactory.RoadSign(), position, 6);
            sign.transform.SetParent(parent, true);
        }

        private void AddVillageNpc(Transform parent, string villageName, string job, Vector3 position, Vector3[] points)
        {
            string npcName = GetNpcName(villageName, job);
            GameObject npc = CreateSprite(npcName, PrototypePixelArtFactory.Villager(Mathf.Abs(StableStringHash(npcName + job)) % 4, 0), position, 45);
            npc.transform.SetParent(parent, true);
            VillagerNpcController controller = npc.AddComponent<VillagerNpcController>();
            controller.Configure(npcName, job, points, 0.75f + Hash01(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y), 7) * 0.35f);
        }

        private GameObject CreateSprite(string name, Sprite sprite, Vector3 position, int sortingOffset)
        {
            GameObject gameObject = new GameObject(name);
            gameObject.transform.position = position;
            SpriteRenderer renderer = gameObject.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            gameObject.AddComponent<YSortSprite>().Configure(sortingOffset);
            return gameObject;
        }

        private Vector2Int WorldToChunk(Vector3 position)
        {
            return new Vector2Int(Mathf.FloorToInt(position.x / chunkSize), Mathf.FloorToInt(position.y / chunkSize));
        }

        private Vector3 ChunkCenter(Vector2Int coord)
        {
            return new Vector3(coord.x * chunkSize + chunkSize * 0.5f, coord.y * chunkSize + chunkSize * 0.5f, 0f);
        }

        private Vector2 ChunkMin(Vector2Int coord)
        {
            return new Vector2(coord.x * chunkSize, coord.y * chunkSize);
        }

        private bool ShouldSpawnVillage(Vector2Int coord)
        {
            if (Mathf.Abs(coord.x) <= 1 && Mathf.Abs(coord.y) <= 1) return false;
            int regionX = Mathf.FloorToInt(coord.x / 4f);
            int regionY = Mathf.FloorToInt(coord.y / 4f);
            Vector2Int villageCoord = new Vector2Int(regionX * 4 + Mathf.FloorToInt(Hash01(regionX, regionY, 4) * 4f), regionY * 4 + Mathf.FloorToInt(Hash01(regionX, regionY, 9) * 4f));
            return coord == villageCoord;
        }

        private string GetVillageName(Vector2Int coord)
        {
            string[] prefixes = { "Ash", "Bell", "Dun", "Grey", "Moss", "Raven", "Stone", "Willow" };
            string[] suffixes = { "ford", "mere", "hollow", "wick", "stead", "brook", "watch", "gate" };
            int first = Mathf.FloorToInt(Hash01(coord.x, coord.y, 31) * prefixes.Length) % prefixes.Length;
            int second = Mathf.FloorToInt(Hash01(coord.x, coord.y, 32) * suffixes.Length) % suffixes.Length;
            return prefixes[first] + suffixes[second];
        }

        private string GetNpcName(string villageName, string job)
        {
            string[] names = { "Aren", "Bryn", "Cala", "Dain", "Edda", "Finn", "Mira", "Tor" };
            int index = Mathf.FloorToInt(Hash01(StableStringHash(villageName), StableStringHash(job), 41) * names.Length) % names.Length;
            return names[index] + " of " + villageName;
        }

        private static int StableStringHash(string value)
        {
            unchecked
            {
                int hash = 17;
                if (value == null) return hash;
                for (int i = 0; i < value.Length; i++) hash = hash * 31 + value[i];
                return hash;
            }
        }

        private float Hash01(int x, int y, int salt)
        {
            unchecked
            {
                int hash = worldSeed;
                hash = hash * 73856093 ^ x * 19349663 ^ y * 83492791 ^ salt * 374761393;
                hash ^= hash >> 13;
                hash *= 1274126177;
                hash ^= hash >> 16;
                return (hash & 0x7fffffff) / 2147483647f;
            }
        }
    }
}
