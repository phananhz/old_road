using UnityEngine;
using TheOldRoad.World;

namespace TheOldRoad.NPC
{
    /// <summary>Harmless prototype villager with a simple daily work loop.</summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public sealed class VillagerNpcController : MonoBehaviour
    {
        [SerializeField] private string villagerName = "Villager";
        [SerializeField] private string jobTitle = "Worker";
        [SerializeField, Min(0.1f)] private float moveSpeed = 0.85f;
        [SerializeField] private Vector3[] workPoints;

        private SpriteRenderer spriteRenderer;
        private int targetIndex;
        private float waitUntil;
        private int frameIndex;
        private float frameTimer;

        public string VillagerName => villagerName;
        public string JobTitle => jobTitle;

        public void Configure(string villagerName, string jobTitle, Vector3[] workPoints, float moveSpeed)
        {
            this.villagerName = string.IsNullOrWhiteSpace(villagerName) ? "Villager" : villagerName;
            this.jobTitle = string.IsNullOrWhiteSpace(jobTitle) ? "Worker" : jobTitle;
            this.workPoints = workPoints ?? System.Array.Empty<Vector3>();
            this.moveSpeed = Mathf.Max(0.1f, moveSpeed);
            EnsureRenderer();
        }

        private void Awake()
        {
            EnsureRenderer();
        }

        private void Update()
        {
            EnsureRenderer();
            if (workPoints == null || workPoints.Length == 0) return;

            if (UnityEngine.Time.time < waitUntil)
            {
                ApplyFrame(0);
                return;
            }

            Vector3 target = workPoints[Mathf.Abs(targetIndex) % workPoints.Length];
            Vector3 delta = target - transform.position;
            delta.z = 0f;
            if (delta.sqrMagnitude <= 0.05f)
            {
                targetIndex = (targetIndex + 1) % workPoints.Length;
                waitUntil = UnityEngine.Time.time + 1.4f + (targetIndex % 3) * 0.55f;
                ApplyFrame(0);
                return;
            }

            Vector3 step = delta.normalized * (moveSpeed * UnityEngine.Time.deltaTime);
            if (step.sqrMagnitude > delta.sqrMagnitude) step = delta;
            transform.position += step;
            if (Mathf.Abs(step.x) > 0.01f) spriteRenderer.flipX = step.x < 0f;

            frameTimer += UnityEngine.Time.deltaTime;
            if (frameTimer >= 0.16f)
            {
                frameTimer = 0f;
                frameIndex = (frameIndex + 1) % 4;
                ApplyFrame(frameIndex);
            }
        }

        private void OnGUI()
        {
            Camera camera = Camera.main;
            if (camera == null) return;

            Vector3 screen = camera.WorldToScreenPoint(transform.position + Vector3.up * 1.25f);
            if (screen.z < 0f) return;

            Rect rect = new Rect(screen.x - 54f, Screen.height - screen.y - 15f, 108f, 30f);
            GUI.Label(rect, villagerName + "\n" + jobTitle, CreateNpcLabelStyle());
        }

        private void EnsureRenderer()
        {
            if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer != null && spriteRenderer.sprite == null) ApplyFrame(0);
        }

        private void ApplyFrame(int frame)
        {
            if (spriteRenderer == null) return;
            int variant = Mathf.Abs(StableStringHash(villagerName + jobTitle)) % 4;
            spriteRenderer.sprite = PrototypePixelArtFactory.Villager(variant, frame);
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

        private static GUIStyle CreateNpcLabelStyle()
        {
            return new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 10,
                fontStyle = FontStyle.Bold,
                normal = { textColor = new Color(0.92f, 0.84f, 0.62f, 1f) }
            };
        }
    }
}
