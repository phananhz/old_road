using UnityEngine;
using TheOldRoad.Audio;
using TheOldRoad.Combat;
using TheOldRoad.UI;
using TheOldRoad.World;

namespace TheOldRoad.Farming
{
    /// <summary>
    /// Interactive loyal farm dog guarding the animal pasture.
    /// Can be petted by player to cheer up and bark happily!
    /// </summary>
    public sealed class FarmDogController : MonoBehaviour
    {
        private SpriteRenderer dogRenderer;
        private Transform playerTransform;
        private GameObject heartObj;
        private float heartHideTime;
        private float nextPetTime;

        private void Start()
        {
            dogRenderer = GetComponent<SpriteRenderer>();
            if (dogRenderer == null)
            {
                dogRenderer = gameObject.AddComponent<SpriteRenderer>();
            }
            dogRenderer.sprite = PrototypePixelArtFactory.FarmDog();
            dogRenderer.sortingOrder = 9;

            BoxCollider2D col = GetComponent<BoxCollider2D>();
            if (col == null)
            {
                col = gameObject.AddComponent<BoxCollider2D>();
            }
            col.size = new Vector2(1.2f, 1.0f);
            col.offset = new Vector2(0f, 0.2f);
        }

        private void Update()
        {
            if (heartObj != null && UnityEngine.Time.time >= heartHideTime)
            {
                heartObj.SetActive(false);
            }

            if (playerTransform == null)
            {
                var player = GameObject.FindWithTag("Player");
                if (player != null) playerTransform = player.transform;
                return;
            }

            // Look towards the player if nearby
            if (dogRenderer != null && Vector2.Distance(playerTransform.position, transform.position) <= 4.0f)
            {
                dogRenderer.flipX = (playerTransform.position.x - transform.position.x) < 0;
            }

            if (Vector2.Distance(playerTransform.position, transform.position) <= 1.8f)
            {
                if (TheOldRoad.Input.PrototypeInput.GetKeyDown(KeyCode.F))
                {
                    TryPetDog();
                }
            }
        }

        private void TryPetDog()
        {
            if (UnityEngine.Time.time < nextPetTime) return;

            nextPetTime = UnityEngine.Time.time + 3.0f;
            AudioManager.PlayUiClick();

            if (heartObj == null)
            {
                heartObj = new GameObject("HeartEmote");
                heartObj.transform.SetParent(transform, false);
                heartObj.transform.localPosition = new Vector3(0f, 1.1f, 0f);
                var sr = heartObj.AddComponent<SpriteRenderer>();
                sr.sprite = PrototypePixelArtFactory.HeartEmote();
                sr.sortingOrder = 25;
            }
            heartObj.SetActive(true);
            heartHideTime = UnityEngine.Time.time + 2.5f;

            string barkText = LocalizationRuntime.IsVietnamese 
                ? "Gâu gâu! (Chú chó vẫy đuôi mừng rỡ! ❤️)" 
                : "Woof woof! (The loyal farm dog wags its tail! ❤️)";
            FloatingTextController.Spawn(barkText, transform.position + Vector3.up * 1.0f, new Color(1f, 0.85f, 0.4f, 1f));
        }
    }
}
