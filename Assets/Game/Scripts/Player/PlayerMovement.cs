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
        public Vector2 LastMoveDirection { get; private set; }
        public bool IsMoving { get; private set; }

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
            if (inputSource == null)
            {
                LastMoveDirection = Vector2.zero;
                IsMoving = false;
                return;
            }

            Vector2 direction = Vector2.ClampMagnitude(inputSource.Move, 1f);
            LastMoveDirection = direction;
            IsMoving = direction.sqrMagnitude > 0.001f;
            transform.position += new Vector3(direction.x, direction.y, 0f) * (moveSpeed * UnityEngine.Time.deltaTime);
        }
    }
}
