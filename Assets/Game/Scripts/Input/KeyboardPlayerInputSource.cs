using UnityEngine;

namespace TheOldRoad.Input
{
    /// <summary>Development adapter for keyboard input. Replaceable by mobile/gamepad adapters.</summary>
    public sealed class KeyboardPlayerInputSource : MonoBehaviour, IPlayerInputSource
    {
        private Vector2 lastMove;

        public Vector2 LastMove => lastMove;

        public Vector2 Move
        {
            get
            {
                float x = 0f;
                float y = 0f;

                if (PrototypeInput.GetKey(KeyCode.A) || PrototypeInput.GetKey(KeyCode.LeftArrow)) x -= 1f;
                if (PrototypeInput.GetKey(KeyCode.D) || PrototypeInput.GetKey(KeyCode.RightArrow)) x += 1f;
                if (PrototypeInput.GetKey(KeyCode.W) || PrototypeInput.GetKey(KeyCode.UpArrow)) y += 1f;
                if (PrototypeInput.GetKey(KeyCode.S) || PrototypeInput.GetKey(KeyCode.DownArrow)) y -= 1f;

                Vector2 move = new Vector2(x, y);
                if (move.sqrMagnitude <= 0.001f && PrototypeInput.GetMouseButton(1) && Camera.main != null)
                {
                    Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(PrototypeInput.MousePosition);
                    move = new Vector2(mouseWorld.x - transform.position.x, mouseWorld.y - transform.position.y);
                }

                lastMove = Vector2.ClampMagnitude(move, 1f);
                return lastMove;
            }
        }
    }
}
