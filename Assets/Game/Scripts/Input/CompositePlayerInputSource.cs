using UnityEngine;

namespace TheOldRoad.Input
{
    /// <summary>Combines development keyboard/mouse input with mobile joystick input.</summary>
    public sealed class CompositePlayerInputSource : MonoBehaviour, IPlayerInputSource
    {
        [SerializeField] private MonoBehaviour keyboardSourceComponent;
        [SerializeField] private MonoBehaviour joystickSourceComponent;

        private IPlayerInputSource keyboardSource;
        private IPlayerInputSource joystickSource;

        public Vector2 Move
        {
            get
            {
                Vector2 joystick = joystickSource != null ? joystickSource.Move : Vector2.zero;
                if (joystick.sqrMagnitude > 0.0001f) return Vector2.ClampMagnitude(joystick, 1f);

                Vector2 keyboard = keyboardSource != null ? keyboardSource.Move : Vector2.zero;
                return Vector2.ClampMagnitude(keyboard, 1f);
            }
        }

        public void Configure(MonoBehaviour keyboardSourceComponent, MonoBehaviour joystickSourceComponent)
        {
            this.keyboardSourceComponent = keyboardSourceComponent;
            this.joystickSourceComponent = joystickSourceComponent;
            ResolveSources();
        }

        private void Awake()
        {
            ResolveSources();
        }

        private void ResolveSources()
        {
            keyboardSource = keyboardSourceComponent as IPlayerInputSource;
            joystickSource = joystickSourceComponent as IPlayerInputSource;
        }
    }
}
