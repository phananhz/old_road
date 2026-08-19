using UnityEngine;
using TheOldRoad.World;

namespace TheOldRoad.Fishing
{
    /// <summary>Visual floating bobber in river water with ripples and bite animations.</summary>
    public sealed class FishingBobber : MonoBehaviour
    {
        private SpriteRenderer bobberRenderer;
        private SpriteRenderer splashRenderer;
        private Vector3 basePosition;
        private float spawnTime;
        private bool isBiting;

        public bool IsBiting => isBiting;

        public static FishingBobber Spawn(Vector3 targetPosition)
        {
            GameObject go = new GameObject("FishingBobber");
            go.transform.position = targetPosition;
            FishingBobber bobber = go.AddComponent<FishingBobber>();
            return bobber;
        }

        private void Awake()
        {
            bobberRenderer = gameObject.AddComponent<SpriteRenderer>();
            bobberRenderer.sprite = PrototypePixelArtFactory.FishingBobberSprite;
            bobberRenderer.sortingOrder = 25;

            // Child splash ripple
            GameObject splashObj = new GameObject("SplashRipple");
            splashObj.transform.SetParent(transform, false);
            splashObj.transform.localPosition = new Vector3(0f, -0.15f, 0f);
            splashRenderer = splashObj.AddComponent<SpriteRenderer>();
            splashRenderer.sprite = PrototypePixelArtFactory.WaterSplashSprite;
            splashRenderer.sortingOrder = 24;
            splashRenderer.color = new Color(1f, 1f, 1f, 0f);

            basePosition = transform.position;
            spawnTime = UnityEngine.Time.time;
        }

        private void Update()
        {
            float elapsed = UnityEngine.Time.time - spawnTime;
            float bobY = Mathf.Sin(elapsed * 4f) * 0.06f;

            if (isBiting)
            {
                // Rapid shaking and dipping
                bobY = -0.15f + Mathf.Sin(elapsed * 18f) * 0.08f;
                splashRenderer.color = new Color(1f, 1f, 1f, 0.6f + Mathf.Sin(elapsed * 12f) * 0.3f);
                splashRenderer.transform.localScale = Vector3.one * (0.8f + Mathf.Sin(elapsed * 8f) * 0.2f);
            }
            else
            {
                splashRenderer.color = new Color(1f, 1f, 1f, 0.15f + Mathf.Sin(elapsed * 3f) * 0.1f);
            }

            transform.position = basePosition + new Vector3(0f, bobY, 0f);
        }

        public void TriggerBite()
        {
            isBiting = true;
            TheOldRoad.Audio.AudioManager.PlayWaterSplash();
        }

        public void Dismiss()
        {
            Destroy(gameObject);
        }
    }
}
