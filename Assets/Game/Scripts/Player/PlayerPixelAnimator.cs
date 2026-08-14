using UnityEngine;
using TheOldRoad.World;

namespace TheOldRoad.Player
{
    /// <summary>Prototype pixel walk animation for the runtime-generated player sprite.</summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public sealed class PlayerPixelAnimator : MonoBehaviour
    {
        [SerializeField, Min(0.04f)] private float frameSeconds = 0.14f;
        [SerializeField] private PlayerMovement movement;

        private SpriteRenderer spriteRenderer;
        private Sprite[] walkFrames;
        private float frameTimer;
        private int frameIndex;
        private int lastRenderedFrame = -1;
        private bool wasMoving;

        public void Configure(PlayerMovement movement, float frameSeconds = 0.14f)
        {
            this.movement = movement;
            this.frameSeconds = Mathf.Max(0.04f, frameSeconds);
            EnsureReferences();
            ApplyFrame(0);
        }

        private void Awake()
        {
            EnsureReferences();
            ApplyFrame(0);
        }

        private void LateUpdate()
        {
            EnsureReferences();
            if (spriteRenderer == null) return;

            bool moving = movement != null && movement.IsMoving;
            if (!moving)
            {
                frameTimer = 0f;
                frameIndex = 0;
                wasMoving = false;
                ApplyFrame(0);
                return;
            }

            Vector2 direction = movement.LastMoveDirection;
            if (Mathf.Abs(direction.x) > 0.05f)
            {
                spriteRenderer.flipX = direction.x < 0f;
            }

            if (!wasMoving)
            {
                frameTimer = 0f;
                frameIndex = 1;
                wasMoving = true;
            }

            frameTimer += UnityEngine.Time.deltaTime;
            if (frameTimer >= frameSeconds)
            {
                frameTimer -= frameSeconds;
                frameIndex = (frameIndex + 1) % walkFrames.Length;
            }

            ApplyFrame(frameIndex);
        }

        private void EnsureReferences()
        {
            if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
            if (movement == null) movement = GetComponent<PlayerMovement>();
            if (walkFrames != null && walkFrames.Length == 4) return;

            walkFrames = new[]
            {
                PrototypePixelArtFactory.PlayerWalk(0),
                PrototypePixelArtFactory.PlayerWalk(1),
                PrototypePixelArtFactory.PlayerWalk(2),
                PrototypePixelArtFactory.PlayerWalk(3)
            };
        }

        private void ApplyFrame(int index)
        {
            if (spriteRenderer == null || walkFrames == null || walkFrames.Length == 0) return;
            int safeIndex = Mathf.Abs(index) % walkFrames.Length;
            if (lastRenderedFrame == safeIndex && spriteRenderer.sprite == walkFrames[safeIndex]) return;

            spriteRenderer.sprite = walkFrames[safeIndex];
            lastRenderedFrame = safeIndex;
        }
    }
}
