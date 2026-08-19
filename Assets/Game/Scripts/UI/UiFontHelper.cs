using System;
using UnityEngine;

namespace TheOldRoad.UI
{
    /// <summary>
    /// Global UI Font Helper that provides standard, crisp, non-pixel modern typography
    /// (Segoe UI, Arial, Roboto, Tahoma, Helvetica) with flawless Vietnamese Unicode support.
    /// </summary>
    public static class UiFontHelper
    {
        private static Font cachedCleanFont;
        private static bool fontQueried = false;

        public static Font CleanFont
        {
            get
            {
                if (!fontQueried)
                {
                    fontQueried = true;
                    try
                    {
                        // Priority list of standard, modern, clean system fonts
                        string[] fontNames = new[]
                        {
                            "Segoe UI",
                            "Arial",
                            "Roboto",
                            "Helvetica",
                            "Tahoma",
                            "Lucida Sans Unicode",
                            "Verdana"
                        };
                        cachedCleanFont = Font.CreateDynamicFontFromOSFont(fontNames, 13);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogWarning($"[UiFontHelper] Could not create dynamic OS font: {ex.Message}");
                    }
                }
                return cachedCleanFont;
            }
        }

        /// <summary>
        /// Applies the clean system font to the current GUI.skin and a target GUIStyle.
        /// </summary>
        public static void Apply(GUIStyle style)
        {
            if (CleanFont != null)
            {
                if (GUI.skin != null && GUI.skin.font != CleanFont)
                {
                    GUI.skin.font = CleanFont;
                }
                if (style != null)
                {
                    style.font = CleanFont;
                }
            }
        }

        /// <summary>
        /// Sets GUI.skin.font globally at the start of OnGUI.
        /// </summary>
        public static void EnsureGlobalSkinFont()
        {
            if (CleanFont != null && GUI.skin != null && GUI.skin.font != CleanFont)
            {
                GUI.skin.font = CleanFont;
            }
        }
    }
}
