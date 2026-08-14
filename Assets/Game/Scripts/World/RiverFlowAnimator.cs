using UnityEngine;

namespace TheOldRoad.World
{
    /// <summary>Runtime river ripple layer for the prototype world.</summary>
    public sealed class RiverFlowAnimator : MonoBehaviour
    {
        private const int RippleCount = 42;
        private readonly SpriteRenderer[] ripples = new SpriteRenderer[RippleCount];
        private readonly float[] offsets = new float[RippleCount];

        public void Configure()
        {
            EnsureRipples();
        }

        private void Awake()
        {
            EnsureRipples();
        }

        private void Update()
        {
            EnsureRipples();

            float time = UnityEngine.Time.time;
            for (int i = 0; i < ripples.Length; i++)
            {
                SpriteRenderer ripple = ripples[i];
                if (ripple == null) continue;

                float t = Mathf.Repeat(offsets[i] + time * 0.045f, 1f);
                float x = Mathf.Lerp(-58f, 28f, t);
                float y = RiverY(x) + Mathf.Sin(time * 1.9f + i * 0.71f) * 0.12f;
                ripple.transform.position = new Vector3(x, y, 0f);
                ripple.transform.localScale = new Vector3(1.1f + Mathf.Sin(time + i) * 0.16f, 0.85f, 1f);
                ripple.color = new Color(0.55f, 0.86f, 1f, 0.20f + Mathf.Sin(time * 2.2f + i) * 0.06f);
            }
        }

        private static float RiverY(float worldX)
        {
            return -12.5f - Mathf.Sin(worldX * 0.16f) * 2.0f;
        }

        private void EnsureRipples()
        {
            for (int i = 0; i < ripples.Length; i++)
            {
                if (ripples[i] != null) continue;

                GameObject rippleObject = new GameObject("River Ripple " + i.ToString("00"));
                rippleObject.transform.SetParent(transform, false);
                offsets[i] = i / (float)ripples.Length;

                SpriteRenderer renderer = rippleObject.AddComponent<SpriteRenderer>();
                renderer.sprite = PrototypePixelArtFactory.WaterRipple();
                renderer.sortingOrder = -9900;
                renderer.color = new Color(0.55f, 0.86f, 1f, 0.24f);
                ripples[i] = renderer;
            }
        }
    }
}
