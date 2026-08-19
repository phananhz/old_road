using UnityEngine;

namespace TheOldRoad.Input
{
    /// <summary>
    /// Runtime IMGUI virtual joystick for the mobile prototype.
    /// It intentionally plugs into IPlayerInputSource so movement remains device-independent.
    /// </summary>
    public sealed class MobileJoystickInputSource : MonoBehaviour, IPlayerInputSource
    {
        [SerializeField, Min(28f)] private float radius = 38f;
        [SerializeField, Range(0f, 1f)] private float deadZone = 0.12f;

        private Vector2 move;
        private bool dragging;
        private int activeButton = -1;
        private Texture2D pixel;

        public Vector2 Move => move;

        private Rect JoystickBounds
        {
            get
            {
                Vector2 center = JoystickCenter;
                return new Rect(center.x - radius, center.y - radius, radius * 2f, radius * 2f);
            }
        }

        private Vector2 JoystickCenter => new Vector2(58f, Screen.height - 62f);

        private void OnGUI()
        {
            EnsureTexture();
            HandlePointer(Event.current);
            DrawJoystick();
        }

        private void HandlePointer(Event current)
        {
            if (current == null || !current.isMouse) return;

            Vector2 pointer = current.mousePosition;
            Rect bounds = JoystickBounds;

            if (current.type == EventType.MouseDown && bounds.Contains(pointer))
            {
                dragging = true;
                activeButton = current.button;
                UpdateMove(pointer);
                current.Use();
            }
            else if (current.type == EventType.MouseDrag && dragging && current.button == activeButton)
            {
                UpdateMove(pointer);
                current.Use();
            }
            else if (current.type == EventType.MouseUp && dragging && current.button == activeButton)
            {
                dragging = false;
                activeButton = -1;
                move = Vector2.zero;
                current.Use();
            }
        }

        private void UpdateMove(Vector2 pointer)
        {
            Vector2 center = JoystickCenter;
            Vector2 guiDelta = Vector2.ClampMagnitude(pointer - center, radius);
            Vector2 worldDelta = new Vector2(guiDelta.x, -guiDelta.y) / radius;
            move = worldDelta.magnitude < deadZone ? Vector2.zero : Vector2.ClampMagnitude(worldDelta, 1f);
        }

        private void DrawJoystick()
        {
            Vector2 center = JoystickCenter;
            Vector2 knobOffset = new Vector2(move.x, -move.y) * (radius * 0.58f);

            DrawCircle(center, radius, new Color(0.02f, 0.018f, 0.015f, 0.54f));
            DrawCircle(center, radius * 0.78f, new Color(0.11f, 0.075f, 0.045f, 0.58f));
            DrawRing(center, radius, new Color(0.72f, 0.48f, 0.18f, 0.84f));
            DrawCircle(center + knobOffset, radius * 0.34f, dragging ? new Color(0.95f, 0.68f, 0.25f, 0.92f) : new Color(0.38f, 0.28f, 0.17f, 0.86f));
            DrawRing(center + knobOffset, radius * 0.34f, new Color(0.04f, 0.025f, 0.015f, 0.95f));
            DrawLabel(new Rect(center.x - 54f, center.y + radius + 6f, 108f, 20f), "MOVE");
        }

        private void DrawCircle(Vector2 center, float circleRadius, Color color)
        {
            Color previous = GUI.color;
            GUI.color = color;

            const int steps = 18;
            float stepHeight = (circleRadius * 2f) / steps;
            for (int i = 0; i < steps; i++)
            {
                float y = -circleRadius + i * stepHeight + stepHeight * 0.5f;
                float width = Mathf.Sqrt(Mathf.Max(0f, circleRadius * circleRadius - y * y)) * 2f;
                GUI.DrawTexture(new Rect(center.x - width * 0.5f, center.y + y - stepHeight * 0.5f, width, stepHeight + 1f), pixel);
            }

            GUI.color = previous;
        }

        private void DrawRing(Vector2 center, float circleRadius, Color color)
        {
            Color previous = GUI.color;
            GUI.color = color;

            const int steps = 22;
            const float thickness = 3f;
            float innerRadius = Mathf.Max(0f, circleRadius - thickness);
            float stepHeight = (circleRadius * 2f) / steps;

            for (int i = 0; i < steps; i++)
            {
                float y = -circleRadius + i * stepHeight + stepHeight * 0.5f;
                float outerWidth = Mathf.Sqrt(Mathf.Max(0f, circleRadius * circleRadius - y * y)) * 2f;
                float innerWidth = Mathf.Sqrt(Mathf.Max(0f, innerRadius * innerRadius - y * y)) * 2f;
                float sideWidth = Mathf.Max(0f, (outerWidth - innerWidth) * 0.5f);
                if (sideWidth <= 0f) continue;

                float rowY = center.y + y - stepHeight * 0.5f;
                GUI.DrawTexture(new Rect(center.x - outerWidth * 0.5f, rowY, sideWidth, stepHeight + 1f), pixel);
                GUI.DrawTexture(new Rect(center.x + innerWidth * 0.5f, rowY, sideWidth, stepHeight + 1f), pixel);
            }

            GUI.color = previous;
        }

        private void DrawLabel(Rect rect, string text)
        {
            TheOldRoad.UI.UiFontHelper.EnsureGlobalSkinFont();
            GUIStyle style = new GUIStyle(GUI.skin.label)
            {
                font = TheOldRoad.UI.UiFontHelper.CleanFont,
                alignment = TextAnchor.MiddleCenter,
                fontSize = 11,
                fontStyle = FontStyle.Bold,
                normal = { textColor = new Color(0.87f, 0.78f, 0.58f, 0.88f) }
            };
            GUI.Label(rect, text, style);
        }

        private void EnsureTexture()
        {
            if (pixel != null) return;

            pixel = new Texture2D(1, 1, TextureFormat.RGBA32, false)
            {
                filterMode = FilterMode.Point,
                hideFlags = HideFlags.DontSave
            };
            pixel.SetPixel(0, 0, Color.white);
            pixel.Apply(false, true);
        }
    }
}
