using UnityEngine;
using TheOldRoad.Construction;
using TheOldRoad.World;

namespace TheOldRoad.Building
{
    /// <summary>Lightweight pixel smoke plume for prototype fires and chimneys.</summary>
    public sealed class SmokeVfxController : MonoBehaviour
    {
        [SerializeField] private ConstructionSite site;
        [SerializeField] private Vector3 sourceOffset = new Vector3(0f, 0.8f, 0f);
        [SerializeField, Min(1)] private int puffCount = 4;
        [SerializeField, Min(0.1f)] private float plumeHeight = 1.1f;
        [SerializeField, Min(0.1f)] private float plumeWidth = 0.34f;
        [SerializeField, Min(0.1f)] private float puffScale = 0.32f;
        [SerializeField, Min(0.1f)] private float speed = 0.45f;
        [SerializeField] private int sortingOrder = 9320;

        private SpriteRenderer[] puffs;

        public void Configure(
            ConstructionSite site,
            Vector3 sourceOffset,
            int puffCount,
            float plumeHeight,
            float plumeWidth,
            float puffScale,
            float speed,
            int sortingOrder)
        {
            this.site = site;
            this.sourceOffset = sourceOffset;
            this.puffCount = Mathf.Max(1, puffCount);
            this.plumeHeight = Mathf.Max(0.1f, plumeHeight);
            this.plumeWidth = Mathf.Max(0.1f, plumeWidth);
            this.puffScale = Mathf.Max(0.1f, puffScale);
            this.speed = Mathf.Max(0.1f, speed);
            this.sortingOrder = sortingOrder;
            EnsurePuffs();
        }

        private void Update()
        {
            if (site == null) site = GetComponent<ConstructionSite>();
            EnsurePuffs();

            bool active = site == null || site.IsCompleted;
            for (int i = 0; i < puffs.Length; i++)
            {
                SpriteRenderer puff = puffs[i];
                if (puff == null) continue;
                puff.enabled = active;
                if (!active) continue;

                float phase = Mathf.Repeat(UnityEngine.Time.time * speed + i / (float)puffs.Length, 1f);
                float drift = Mathf.Sin(UnityEngine.Time.time * 1.7f + i * 1.83f) * plumeWidth * phase;
                float wobble = Mathf.Sin(UnityEngine.Time.time * 4.1f + i) * 0.035f;
                Vector3 offset = sourceOffset + new Vector3(drift + wobble, phase * plumeHeight, 0f);

                puff.transform.position = transform.position + offset;
                puff.transform.localScale = Vector3.one * Mathf.Lerp(puffScale * 0.58f, puffScale * 1.55f, phase);
                puff.color = new Color(0.58f, 0.61f, 0.63f, Mathf.Lerp(0.34f, 0.02f, phase));
                puff.sortingOrder = sortingOrder;
            }
        }

        private void EnsurePuffs()
        {
            if (puffs != null && puffs.Length == puffCount) return;

            if (puffs != null)
            {
                foreach (SpriteRenderer oldPuff in puffs)
                {
                    if (oldPuff != null) Destroy(oldPuff.gameObject);
                }
            }

            puffs = new SpriteRenderer[puffCount];
            for (int i = 0; i < puffCount; i++)
            {
                GameObject puffObject = new GameObject("Smoke Puff " + (i + 1));
                puffObject.transform.SetParent(transform, false);
                SpriteRenderer puff = puffObject.AddComponent<SpriteRenderer>();
                puff.sprite = PrototypePixelArtFactory.SolidPixel();
                puff.sortingOrder = sortingOrder;
                puff.enabled = false;
                puffs[i] = puff;
            }
        }
    }
}
