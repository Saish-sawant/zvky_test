using UnityEngine;
using System.Collections.Generic;

public class PaylineRenderer : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;

    private void Awake()
    {
        lineRenderer.positionCount = 0;
    }

    // Draws a line through given world positions
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

    public void ClearLine()
    {
        lineRenderer.positionCount = 0;
    }
}
