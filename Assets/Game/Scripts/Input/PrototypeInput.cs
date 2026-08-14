using System;
using UnityEngine;

namespace TheOldRoad.Input
{
    /// <summary>
    /// Prototype keyboard/mouse input bridge.
    /// Uses Unity's legacy Input API directly and reports when the project setting blocks it.
    /// </summary>
    public static class PrototypeInput
    {
        private static string lastBackendStatus = "legacy=unknown";

        public static bool GetKey(KeyCode key)
        {
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
