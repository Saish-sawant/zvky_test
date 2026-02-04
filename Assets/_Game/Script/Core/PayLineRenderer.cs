using UnityEngine;
using System.Collections.Generic;

namespace SpinWheel
{
    /// <summary>
    /// Handles visual rendering of a single payline using LineRenderer.
    /// This class is purely visual and contains no game logic.
    /// </summary>
    public class PaylineRenderer : MonoBehaviour
    {
        /// <summary>
        /// Unity LineRenderer used to draw the payline
        /// </summary>
        [SerializeField] private LineRenderer lineRenderer;

        private void Awake()
        {
            // Ensure line is not visible at start
            lineRenderer.positionCount = 0;
        }

        /// <summary>
        /// Draws a payline through the provided world-space positions.
        /// </summary>
        /// <param name="worldPositions">
        /// List of positions in WORLD SPACE where the line should pass through.
        /// Order matters.
        /// </param>
        public void DrawLine(List<Vector3> worldPositions)
        {
            if (worldPositions == null || worldPositions.Count == 0)
                return;

            lineRenderer.positionCount = worldPositions.Count;

            for (int i = 0; i < worldPositions.Count; i++)
            {
                lineRenderer.SetPosition(i, worldPositions[i]);
            }
        }

        /// <summary>
        /// Clears the currently drawn payline.
        /// </summary>
        public void ClearLine()
        {
            lineRenderer.positionCount = 0;
        }
    }
}
