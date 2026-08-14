using UnityEngine;
using UnityEngine.Rendering;
using TheOldRoad.Player;
using TheOldRoad.Time;

namespace TheOldRoad.World
{
    /// <summary>Runtime prototype cabin interior built away from the outdoor map.</summary>
    public sealed class CabinInteriorController : MonoBehaviour
    {
        private static readonly Vector3 InteriorOrigin = new Vector3(200f, 200f, 0f);
        private static readonly Vector2 InteriorCameraCenter = new Vector2(200f, 200f);
        private static readonly Vector2 InteriorBoundsMin = new Vector2(193.35f, 195.25f);
        private static readonly Vector2 InteriorBoundsMax = new Vector2(206.65f, 203.35f);
        private const int InteriorPlayerSortingOrder = 9500;

        private GameObject interiorRoot;
        private Transform bedTransform;
        private Transform doorTransform;
        private Vector3 lastOutdoorPosition;
        private bool inside;
        private string status = "Cabin interior ready.";

        public bool IsInside => inside;
        public Transform BedTransform => bedTransform;
        public Transform DoorTransform => doorTransform;
        public string Status => status;

        public void EnsureBuilt()
        {
            if (interiorRoot != null) return;

            interiorRoot = new GameObject("Cabin Interior");
            interiorRoot.transform.position = InteriorOrigin;
            interiorRoot.SetActive(false);

            CreateInteriorSprite("Interior Floor - Bedroom Living Kitchen", PrototypePixelArtFactory.CabinInteriorFloor(), new Vector3(0f, -0.45f, 0f), -9300, new Vector3(1.25f, 1.25f, 1f));
            CreateInteriorSprite("Interior Back Wall - Shelves and Windows", PrototypePixelArtFactory.CabinInteriorWall(), new Vector3(0f, 3.55f, 0f), -9200, new Vector3(1.25f, 1f, 1f));
            CreateInteriorSprite("Bedroom Partition Wall", PrototypePixelArtFactory.CabinPartitionWall(), new Vector3(-2.75f, -0.35f, 0f), -9100, Vector3.one);
            CreateInteriorSprite("Kitchen Partition Wall", PrototypePixelArtFactory.CabinPartitionWall(), new Vector3(2.75f, -0.35f, 0f), -9100, Vector3.one);

            bedTransform = CreateInteriorSprite("Bed - Sleeping Nook", PrototypePixelArtFactory.CabinBed(), new Vector3(-4.55f, -1.95f, 0f), -8900, Vector3.one).transform;
            CreateInteriorSprite("Bench - Living Room", PrototypePixelArtFactory.CabinBench(), new Vector3(-0.65f, 0.85f, 0f), -8950, Vector3.one);
            CreateInteriorSprite("Table - Living Room", PrototypePixelArtFactory.CabinTable(), new Vector3(0.65f, -0.35f, 0f), -8945, Vector3.one);
            CreateInteriorSprite("Kitchen Counter", PrototypePixelArtFactory.CabinKitchenCounter(), new Vector3(4.25f, 0.60f, 0f), -8950, Vector3.one);
            CreateInteriorSprite("Hearth - Kitchen", PrototypePixelArtFactory.CabinHearth(), new Vector3(4.50f, 2.05f, 0f), -8940, Vector3.one);
            doorTransform = CreateInteriorSprite("Cabin Door", PrototypePixelArtFactory.CabinDoorMarker(), new Vector3(0f, -4.25f, 0f), -8850, Vector3.one).transform;
        }

        public void Enter(PlayerMovement player, Vector3 outdoorCabinPosition)
        {
            if (player == null) return;
            EnsureBuilt();

            lastOutdoorPosition = outdoorCabinPosition + new Vector3(0f, -2.2f, 0f);
            inside = true;
            interiorRoot.SetActive(true);
            player.transform.position = InteriorOrigin + new Vector3(0f, -3.15f, 0f);
            SetPlayerInteriorRenderMode(player.gameObject, true);
            ConfigureInteriorCamera(player.transform);
            status = "Entered cabin. Stand by the bed and press F to sleep.";
        }

        public void Exit(PlayerMovement player)
        {
            if (player == null) return;

            inside = false;
            if (interiorRoot != null) interiorRoot.SetActive(false);
            SetPlayerInteriorRenderMode(player.gameObject, false);
            player.transform.position = lastOutdoorPosition;
            ConfigureWorldCamera(player.transform);
            status = "Exited cabin.";
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
            return inside && actor != null && bedTransform != null && Vector2.Distance(actor.position, bedTransform.position) <= 1.35f;
        }

        public void ClampActorInside(Transform actor)
        {
            if (!inside || actor == null) return;

            Vector3 position = actor.position;
            position.x = Mathf.Clamp(position.x, InteriorBoundsMin.x, InteriorBoundsMax.x);
            position.y = Mathf.Clamp(position.y, InteriorBoundsMin.y, InteriorBoundsMax.y);
            actor.position = position;
        }

        public void ConstrainActorInside(Transform actor, ref Vector3 previousValidPosition)
        {
            if (!inside || actor == null) return;

            Vector3 position = actor.position;
            position.x = Mathf.Clamp(position.x, InteriorBoundsMin.x, InteriorBoundsMax.x);
            position.y = Mathf.Clamp(position.y, InteriorBoundsMin.y, InteriorBoundsMax.y);

            Vector3 local = position - InteriorOrigin;
            Vector3 previousLocal = previousValidPosition - InteriorOrigin;
            bool blockedPartition =
                CrossedBlockedPartition(previousLocal, local, -2.75f)
                || CrossedBlockedPartition(previousLocal, local, 2.75f);

            if (blockedPartition)
            {
                position.x = previousValidPosition.x;
            }

            actor.position = position;
            previousValidPosition = actor.position;
        }

        private static bool CrossedBlockedPartition(Vector3 previousLocal, Vector3 currentLocal, float partitionX)
        {
            bool crossed = (previousLocal.x < partitionX && currentLocal.x >= partitionX)
                || (previousLocal.x > partitionX && currentLocal.x <= partitionX);
            if (!crossed) return false;

            bool inDoorGap = currentLocal.y > -2.55f && currentLocal.y < -0.05f;
            return !inDoorGap;
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

        private static void ConfigureInteriorCamera(Transform target)
        {
            Camera camera = Camera.main;
            if (camera == null) return;

            camera.orthographicSize = 5.65f;
            camera.transform.position = new Vector3(InteriorCameraCenter.x, InteriorCameraCenter.y, -10f);

            CameraFollow2D follow = camera.GetComponent<CameraFollow2D>();
            if (follow != null)
            {
                follow.Configure(target, InteriorCameraCenter, InteriorCameraCenter, 0f);
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
