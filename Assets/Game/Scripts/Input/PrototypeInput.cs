using System;
using System.Collections.Generic;
using UnityEngine;
using TheOldRoad.Core;

namespace TheOldRoad.Input
{
    /// <summary>
    /// Prototype keyboard/mouse input bridge.
    /// Uses Unity's legacy Input API directly and reports when the project setting blocks it.
    /// </summary>
    public static class PrototypeInput
    {
        private static readonly Dictionary<KeyCode, int> VirtualKeyDownFrames = new Dictionary<KeyCode, int>();
        private static string lastBackendStatus = "legacy=unknown";

        public static bool GetKey(KeyCode key)
        {
            if (PrototypeGameState.GameplayInputBlocked) return false;
            if (ImGuiPrototypeInputBridge.GetKey(key)) return true;

            try
            {
                bool value = UnityEngine.Input.GetKey(key);
                lastBackendStatus = "legacy=on";
                return value;
            }
            catch (InvalidOperationException exception)
            {
                lastBackendStatus = "legacy=blocked " + exception.GetType().Name;
                return false;
            }
        }

        public static bool GetKeyDown(KeyCode key)
        {
            if (PrototypeGameState.GameplayInputBlocked) return false;
            if (VirtualKeyDownFrames.TryGetValue(key, out int virtualFrame) && virtualFrame == UnityEngine.Time.frameCount) return true;
            if (ImGuiPrototypeInputBridge.GetKeyDown(key)) return true;

            try
            {
                bool value = UnityEngine.Input.GetKeyDown(key);
                lastBackendStatus = "legacy=on";
                return value;
            }
            catch (InvalidOperationException exception)
            {
                lastBackendStatus = "legacy=blocked " + exception.GetType().Name;
                return false;
            }
        }

        public static bool GetMouseButtonDown(int button)
        {
            if (PrototypeGameState.GameplayInputBlocked) return false;
            if (ImGuiPrototypeInputBridge.GetMouseButtonDown(button)) return true;

            try
            {
                bool value = UnityEngine.Input.GetMouseButtonDown(button);
                lastBackendStatus = "legacy=on";
                return value;
            }
            catch (InvalidOperationException exception)
            {
                lastBackendStatus = "legacy=blocked " + exception.GetType().Name;
                return false;
            }
        }

        public static bool GetMouseButton(int button)
        {
            if (PrototypeGameState.GameplayInputBlocked) return false;
            if (ImGuiPrototypeInputBridge.GetMouseButton(button)) return true;

            try
            {
                bool value = UnityEngine.Input.GetMouseButton(button);
                lastBackendStatus = "legacy=on";
                return value;
            }
            catch (InvalidOperationException exception)
            {
                lastBackendStatus = "legacy=blocked " + exception.GetType().Name;
                return false;
            }
        }

        public static Vector3 MousePosition
        {
            get
            {
                if (ImGuiPrototypeInputBridge.HasRecentEvents) return ImGuiPrototypeInputBridge.MousePosition;

                try
                {
                    Vector3 value = UnityEngine.Input.mousePosition;
                    lastBackendStatus = "legacy=on";
                    return value;
                }
                catch (InvalidOperationException exception)
                {
                    lastBackendStatus = "legacy=blocked " + exception.GetType().Name;
                    return Vector3.zero;
                }
            }
        }

        public static string Diagnostics
        {
            get
            {
                string guiStatus = ImGuiPrototypeInputBridge.HasRecentEvents ? " imgui=on" : " imgui=waiting";
                return "Input: " + lastBackendStatus + guiStatus + " anyKey=" + SafeAnyKey();
            }
        }

        public static void QueueKeyDown(KeyCode key)
        {
            VirtualKeyDownFrames[key] = UnityEngine.Time.frameCount + 1;
            lastBackendStatus = "virtual=on";
        }

        private static string SafeAnyKey()
        {
            try
            {
                return UnityEngine.Input.anyKey ? "true" : "false";
            }
            catch (InvalidOperationException)
            {
                return "blocked";
            }
        }
    }
}
