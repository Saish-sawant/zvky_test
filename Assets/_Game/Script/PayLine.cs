using UnityEngine;
using System.Collections.Generic;

public class Payline : MonoBehaviour
{
    [SerializeField] PaylineRenderer renderer;
    [SerializeField] Transform[] testPoints;

    public void LocalTest()
    {
        List<Vector3> points = new List<Vector3>();

        foreach (var t in testPoints)
            points.Add(t.position);

        renderer.DrawLine(points);
    }
}
