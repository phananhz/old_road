using UnityEngine;
using TheOldRoad.Farming;
using TheOldRoad.Time;

namespace TheOldRoad.Building
{
    /// <summary>
    /// Irrigation aqueduct that automatically irrigates adjacent farm plots within 3.5m radius each morning.
    /// </summary>
    public sealed class WaterAqueductController : MonoBehaviour
    {
        [SerializeField] private float irrigationRadius = 3.5f;
        private GameTimeController gameTime;
        private int lastIrrigatedDay = -1;

        private void Update()
        {
            if (gameTime == null) gameTime = FindAnyObjectByType<GameTimeController>();
            if (gameTime == null) return;

            if (gameTime.Day != lastIrrigatedDay && gameTime.Hour >= 6)
            {
                lastIrrigatedDay = gameTime.Day;
                IrrigateSurroundingPlots();
            }
        }

        public void IrrigateSurroundingPlots()
        {
            Vector3 pos = transform.position;
            FarmPlotController[] plots = FindObjectsByType<FarmPlotController>(FindObjectsInactive.Exclude);

            for (int i = 0; i < plots.Length; i++)
            {
                FarmPlotController plot = plots[i];
                if (plot == null || !plot.IsTilled || plot.IsWatered) continue;

                float dist = Vector2.Distance(pos, plot.transform.position);
                if (dist <= irrigationRadius)
                {
                    plot.TryWaterSoil();
                }
            }
        }
    }
}
