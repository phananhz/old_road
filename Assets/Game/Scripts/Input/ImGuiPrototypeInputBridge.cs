using System.Collections.Generic;
using UnityEngine;

namespace TheOldRoad.Input
{
    /// <summary>
    /// Editor/prototype fallback input captured from Game view IMGUI events.
    /// This keeps the vertical slice testable even when Player Settings block the legacy Input API.
    /// </summary>
    public sealed class ImGuiPrototypeInputBridge : MonoBehaviour
    {
        private static readonly HashSet<KeyCode> HeldKeys = new HashSet<KeyCode>();
        private static readonly Dictionary<KeyCode, int> KeyDownFrames = new Dictionary<KeyCode, int>();
        private static readonly HashSet<int> HeldMouseButtons = new HashSet<int>();
        private static readonly Dictionary<int, int> MouseDownFrames = new Dictionary<int, int>();

        private static Vector3 mousePosition;
        private static int lastEventFrame = -1;

        public static bool HasRecentEvents => UnityEngine.Time.frameCount - lastEventFrame <= 30;
        public static Vector3 MousePosition => mousePosition;

        public static bool GetKey(KeyCode key) => HeldKeys.Contains(key);

        public static bool GetKeyDown(KeyCode key)
        {
            return KeyDownFrames.TryGetValue(key, out int frame) && frame == UnityEngine.Time.frameCount;
        }

        public static bool GetMouseButton(int button) => HeldMouseButtons.Contains(button);

        public static bool GetMouseButtonDown(int button)
        {
            return MouseDownFrames.TryGetValue(button, out int frame) && frame == UnityEngine.Time.frameCount;
        }

        private void OnGUI()
        {
            Event current = Event.current;
            if (current == null) return;

            lastEventFrame = UnityEngine.Time.frameCount;
            mousePosition = new Vector3(current.mousePosition.x, Screen.height - current.mousePosition.y, 0f);

            if (current.isKey)
            {
                if (current.type == EventType.KeyDown)
                {
                    HeldKeys.Add(current.keyCode);
                    KeyDownFrames[current.keyCode] = UnityEngine.Time.frameCount;
                }
                else if (current.type == EventType.KeyUp)
                {
                    HeldKeys.Remove(current.keyCode);
                }
            }

            if (current.isMouse)
            {
                if (current.type == EventType.MouseDown)
                {
                    HeldMouseButtons.Add(current.button);
                    MouseDownFrames[current.button] = UnityEngine.Time.frameCount;
                }
                else if (current.type == EventType.MouseUp)
                {
                    HeldMouseButtons.Remove(current.button);
                }
            }
        }
    }
}
