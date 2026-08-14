using UnityEngine;

namespace TheOldRoad.Core
{
    /// <summary>Minimal composition root for the bootstrap scene.</summary>
    public sealed class GameBootstrap : MonoBehaviour
    {
        [SerializeField] private string gameVersion = "0.1.0-sprint1";
        [SerializeField] private bool createVerticalSlice = true;

        public string GameVersion => gameVersion;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void EnsureBootstrapExists()
        {
            if (FindAnyObjectByType<GameBootstrap>() != null) return;

            GameObject bootstrap = new GameObject("GameBootstrap");
            bootstrap.AddComponent<GameBootstrap>();
        }

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            if (createVerticalSlice && FindAnyObjectByType<VerticalSliceController>() == null)
            {
                GameObject slice = new GameObject("Valen Outskirts Vertical Slice");
                slice.AddComponent<VerticalSliceController>();
            }
        }
    }
}
