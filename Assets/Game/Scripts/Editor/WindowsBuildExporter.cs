using System.IO;
using UnityEditor;

using UnityEngine;
using UnityEditor.Build.Reporting;

namespace TheOldRoad.Editor
{
    public static class WindowsBuildExporter
    {
        private const string BuildDirectory = "Builds/Windows";
        private const string BuildExecutable = "TheOldRoad.exe";

        [MenuItem("The Old Road/Build Windows 64-bit")]
        public static void BuildWindows64()
        {
            Directory.CreateDirectory(BuildDirectory);

            BuildPlayerOptions options = new BuildPlayerOptions
            {
                scenes = new[] { "Assets/Game/Scenes/Bootstrap/Bootstrap.unity" },
                locationPathName = Path.Combine(BuildDirectory, BuildExecutable),
                target = BuildTarget.StandaloneWindows64,
                options = BuildOptions.None
            };

            BuildReport report = BuildPipeline.BuildPlayer(options);
            BuildSummary summary = report.summary;

            if (summary.result == BuildResult.Succeeded)
            {
                Debug.Log($"[Build Succeeded] Size: {summary.totalSize} bytes, Output: {options.locationPathName}");
            }
            else if (summary.result == BuildResult.Failed)
            {
                Debug.LogError($"[Build Failed] Total errors: {summary.totalErrors}");
            }
        }
    }
}
