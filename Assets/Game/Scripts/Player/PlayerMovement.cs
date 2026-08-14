using UnityEngine;
using TheOldRoad.Input;

namespace TheOldRoad.Player
{
    /// <summary>Moves a player in 8 directions without knowing the input device.</summary>
    public sealed class PlayerMovement : MonoBehaviour
    {
        [SerializeField, Min(0f)] private float moveSpeed = 3f;
        [SerializeField] private MonoBehaviour inputSourceComponent;

        private IPlayerInputSource inputSource;

        public float MoveSpeed => moveSpeed;

        private void Awake()
        {
            ResolveInputSource();
        }

        public void Configure(MonoBehaviour inputSourceComponent, float moveSpeed)
        {
            this.inputSourceComponent = inputSourceComponent;
            this.moveSpeed = Mathf.Max(0f, moveSpeed);
            ResolveInputSource();
            enabled = inputSource != null;
        }

        private void ResolveInputSource()
        {
            inputSource = inputSourceComponent as IPlayerInputSource;
            if (inputSource != null) return;

            MonoBehaviour[] components = GetComponents<MonoBehaviour>();
            foreach (MonoBehaviour component in components)
            {
                if (component is IPlayerInputSource source)
                {
                    inputSourceComponent = component;
                    inputSource = source;
                    return;
                }
            }
        }

        private void Update()
        {
            if (inputSource == null) ResolveInputSource();
            if (inputSource == null) return;

            Vector2 direction = Vector2.ClampMagnitude(inputSource.Move, 1f);
            transform.position += new Vector3(direction.x, direction.y, 0f) * (moveSpeed * UnityEngine.Time.deltaTime);
        }
    }
}
