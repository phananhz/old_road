using UnityEngine;
using TheOldRoad.Player;
using TheOldRoad.Time;

namespace TheOldRoad.World
{
    /// <summary>Runtime prototype cabin interior built away from the outdoor map.</summary>
    public sealed class CabinInteriorController : MonoBehaviour
    {
        private static readonly Vector3 InteriorOrigin = new Vector3(200f, 200f, 0f);
        private static readonly Vector2 InteriorCameraCenter = new Vector2(200f, 200f);

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

            CreateInteriorSprite("Interior Floor", PrototypePixelArtFactory.CabinInteriorFloor(), new Vector3(0f, -0.6f, 0f), -9300, new Vector3(1.45f, 1.55f, 1f));
            CreateInteriorSprite("Interior Back Wall", PrototypePixelArtFactory.CabinInteriorWall(), new Vector3(0f, 3.45f, 0f), -9200, new Vector3(1.45f, 1f, 1f));
            CreateInteriorSprite("Hearth", PrototypePixelArtFactory.CabinHearth(), new Vector3(-3.7f, 2.1f, 0f), -9000, Vector3.one);
            CreateInteriorSprite("Table", PrototypePixelArtFactory.CabinTable(), new Vector3(2.7f, 0.8f, 0f), -8950, Vector3.one);
            bedTransform = CreateInteriorSprite("Bed", PrototypePixelArtFactory.CabinBed(), new Vector3(-3.2f, -1.8f, 0f), -8900, Vector3.one).transform;
            doorTransform = CreateInteriorSprite("Cabin Door", PrototypePixelArtFactory.CabinDoorMarker(), new Vector3(0f, -4.0f, 0f), -8850, Vector3.one).transform;
        }

        public void Enter(PlayerMovement player, Vector3 outdoorCabinPosition)
        {
            if (player == null) return;
            EnsureBuilt();

            lastOutdoorPosition = outdoorCabinPosition + new Vector3(0f, -2.2f, 0f);
            inside = true;
            interiorRoot.SetActive(true);
            player.transform.position = InteriorOrigin + new Vector3(0f, -2.4f, 0f);
            ConfigureInteriorCamera(player.transform);
            status = "Entered cabin. Stand by the bed and press F to sleep.";
        }

        public void Exit(PlayerMovement player)
        {
            if (player == null) return;

            inside = false;
            if (interiorRoot != null) interiorRoot.SetActive(false);
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

            camera.orthographicSize = 5.2f;
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
    }
}
