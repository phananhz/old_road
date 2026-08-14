using UnityEngine;

namespace TheOldRoad.World
{
    /// <summary>Simple fixed-orientation orthographic camera follow for the prototype world.</summary>
    [RequireComponent(typeof(Camera))]
    public sealed class CameraFollow2D : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField, Min(0f)] private float smoothTime = 0.12f;
        [SerializeField] private Vector2 minBounds = new Vector2(-4f, -2.5f);
        [SerializeField] private Vector2 maxBounds = new Vector2(4f, 2.5f);

        private Vector3 velocity;

        public void Configure(Transform target, Vector2 minBounds, Vector2 maxBounds, float smoothTime)
        {
            this.target = target;
            this.minBounds = minBounds;
            this.maxBounds = maxBounds;
            this.smoothTime = Mathf.Max(0f, smoothTime);
        }

        private void LateUpdate()
        {
            if (target == null) return;

            Vector3 desired = new Vector3(
                Mathf.Clamp(target.position.x, minBounds.x, maxBounds.x),
                Mathf.Clamp(target.position.y, minBounds.y, maxBounds.y),
                -10f);

            transform.position = smoothTime <= 0f
                ? desired
                : Vector3.SmoothDamp(transform.position, desired, ref velocity, smoothTime);
            transform.rotation = Quaternion.identity;
        }
    }
}
