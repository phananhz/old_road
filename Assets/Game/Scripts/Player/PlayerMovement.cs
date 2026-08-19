using UnityEngine;
using TheOldRoad.Audio;
using TheOldRoad.Input;

namespace TheOldRoad.Player
{
    /// <summary>
    /// Moves a player in 8 directions with Sprint (Shift), river wading sounds,
    /// and footstep handling.
    /// </summary>
    public sealed class PlayerMovement : MonoBehaviour
    {
        [SerializeField, Min(0f)] private float moveSpeed = 3.2f;
        [SerializeField, Min(0f)] private float sprintMultiplier = 1.6f;
        [SerializeField] private MonoBehaviour inputSourceComponent;

        private IPlayerInputSource inputSource;
        private float footstepTimer;

        public float MoveSpeed => moveSpeed;
        public bool IsSprinting { get; private set; }
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
                IsSprinting = false;
                return;
            }

            Vector2 direction = Vector2.ClampMagnitude(inputSource.Move, 1f);
            LastMoveDirection = direction;
            IsMoving = direction.sqrMagnitude > 0.001f;

            bool shiftPressed = UnityEngine.Input.GetKey(KeyCode.LeftShift) || UnityEngine.Input.GetKey(KeyCode.RightShift);
            IsSprinting = IsMoving && shiftPressed;
            
            float pathBonus = 1.0f;
            Collider2D[] hits = Physics2D.OverlapPointAll(transform.position);
            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i] == null) continue;
                string hitName = hits[i].gameObject.name.ToLowerInvariant();
                if (hitName.Contains("stone-tile") || hitName.Contains("highway"))
                {
                    pathBonus = 1.35f;
                    break;
                }
                if (hitName.Contains("cobble") || hitName.Contains("wood") || hitName.Contains("bridge"))
                {
                    pathBonus = 1.25f;
                    break;
                }
                if (hitName.Contains("path") || hitName.Contains("dirt"))
                {
                    pathBonus = 1.15f;
                    break;
                }
            }

            float currentSpeed = moveSpeed * (IsSprinting ? sprintMultiplier : 1f) * pathBonus;

            if (IsMoving)
            {
                float stepInterval = IsSprinting ? 0.22f : 0.32f;
                if (UnityEngine.Time.unscaledTime >= footstepTimer)
                {
                    footstepTimer = UnityEngine.Time.unscaledTime + stepInterval;

                    // Check if player is wading through the river (approx Y between -5.5 and -3.0)
                    bool inRiver = transform.position.y >= -5.5f && transform.position.y <= -3.0f && transform.position.x < 150f;
                    if (inRiver)
                    {
                        AudioManager.PlayWaterSplash();
                    }
                    else
                    {
                        AudioManager.PlayFootstep();
                    }
                }
            }

            Vector3 targetPosition = transform.position + new Vector3(direction.x, direction.y, 0f) * (currentSpeed * UnityEngine.Time.deltaTime);
            if (targetPosition.x < 100f)
            {
                targetPosition.x = Mathf.Clamp(targetPosition.x, -58f, 58f);
                targetPosition.y = Mathf.Clamp(targetPosition.y, -34f, 34f);
            }
            transform.position = targetPosition;
        }
    }
}
