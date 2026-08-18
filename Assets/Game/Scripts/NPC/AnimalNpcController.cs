using UnityEngine;
using TheOldRoad.World;

namespace TheOldRoad.NPC
{
    /// <summary>Harmless wandering animal used to make prototype villages feel alive.</summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public sealed class AnimalNpcController : MonoBehaviour
    {
        [SerializeField] private string animalName = "Chicken";
        [SerializeField, Min(0.05f)] private float moveSpeed = 0.65f;
        [SerializeField] private Vector3[] wanderPoints;

        private SpriteRenderer spriteRenderer;
        private int targetIndex;
        private int frameIndex;
        private float frameTimer;
        private float waitUntil;

        public string AnimalName => animalName;

        public void Configure(string animalName, Vector3[] wanderPoints, float moveSpeed)
        {
            this.animalName = string.IsNullOrWhiteSpace(animalName) ? "Animal" : animalName;
            this.wanderPoints = wanderPoints ?? System.Array.Empty<Vector3>();
            this.moveSpeed = Mathf.Max(0.05f, moveSpeed);
            EnsureRenderer();
            ApplyFrame(0);
        }

        private void Awake()
        {
            EnsureRenderer();
        }

        private void Update()
        {
            EnsureRenderer();
            if (wanderPoints == null || wanderPoints.Length == 0) return;

            if (UnityEngine.Time.time < waitUntil)
            {
                ApplyFrame(0);
                return;
            }

            Vector3 target = wanderPoints[Mathf.Abs(targetIndex) % wanderPoints.Length];
            Vector3 delta = target - transform.position;
            delta.z = 0f;
            if (delta.sqrMagnitude <= 0.035f)
            {
                targetIndex = (targetIndex + 1) % wanderPoints.Length;
                waitUntil = UnityEngine.Time.time + 0.8f + (targetIndex % 3) * 0.35f;
                ApplyFrame(0);
                return;
            }

            Vector3 step = delta.normalized * (moveSpeed * UnityEngine.Time.deltaTime);
            if (step.sqrMagnitude > delta.sqrMagnitude) step = delta;
            transform.position += step;
            if (spriteRenderer != null && Mathf.Abs(step.x) > 0.01f) spriteRenderer.flipX = step.x < 0f;

            frameTimer += UnityEngine.Time.deltaTime;
            if (frameTimer >= 0.18f)
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

            Vector3 screen = camera.WorldToScreenPoint(transform.position + Vector3.up * 0.85f);
            if (screen.z < 0f) return;

            Rect rect = new Rect(screen.x - 42f, Screen.height - screen.y - 12f, 84f, 20f);
            string displayAnimal = TheOldRoad.UI.LocalizationRuntime.T("animal." + animalName.ToLowerInvariant()) != "animal." + animalName.ToLowerInvariant()
                ? TheOldRoad.UI.LocalizationRuntime.T("animal." + animalName.ToLowerInvariant())
                : animalName;
            GUI.Label(rect, displayAnimal, CreateLabelStyle());
        }

        private void EnsureRenderer()
        {
            if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void ApplyFrame(int frame)
        {
            if (spriteRenderer == null) return;
            int variant = Mathf.Abs(StableStringHash(animalName)) % 4;
            spriteRenderer.sprite = PrototypePixelArtFactory.Animal(variant, frame);
        }

        private static int StableStringHash(string value)
        {
            unchecked
            {
                int hash = 19;
                if (value == null) return hash;
                for (int i = 0; i < value.Length; i++) hash = hash * 31 + value[i];
                return hash;
            }
        }

        private static GUIStyle CreateLabelStyle()
        {
            return new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 9,
                fontStyle = FontStyle.Bold,
                normal = { textColor = new Color(0.92f, 0.88f, 0.70f, 1f) }
            };
        }
    }
}
