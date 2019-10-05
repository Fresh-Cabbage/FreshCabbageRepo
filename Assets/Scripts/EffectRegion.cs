using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectRegion : MonoBehaviour
{
    public LineRenderer lr;
    public float circleRadius;

    private void Start() {
        DrawCircle(circleRadius);
    }



    void DrawCircle(float radius) {
        Vector3[] points = new Vector3[36];
        
        for (int p = 0; p < points.Length; p++) {
            points[p] = Quaternion.Euler(0, 0, 360f * p / points.Length) * Vector3.right * radius;
        }

        lr.positionCount = 36;
        lr.SetPositions(points);
        lr.gameObject.SetActive(true);
    }
}
