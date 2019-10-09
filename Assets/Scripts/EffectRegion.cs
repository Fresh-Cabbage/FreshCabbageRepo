using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectRegion : MonoBehaviour
{
    public LineRenderer lr;
    public float baseRadius;
    public float maxSize;
    public float inflateSpeed;
    public float deflateTime;

    float deflateTimer;
    float currentRadius;
    bool isInflating;
    bool shouldInflate;

    private void Start() {
        transform.localScale = Vector3.zero.WithZ(1);

        DrawCircle(baseRadius);
    }

    private void FixedUpdate() {
        if (isInflating) {
            currentRadius = Mathf.Min(maxSize, currentRadius + inflateSpeed);

            // state switch
            if (currentRadius == maxSize && !shouldInflate) {
                deflateTimer = deflateTime;
                isInflating = false;
            }
        }
        else {
            deflateTimer = Helpers.Timer(deflateTimer);
            currentRadius = maxSize * (Mathf.Pow(deflateTimer / deflateTime, .333f));

            // state switch
            if (shouldInflate)
                isInflating = true;
        }

        transform.localScale = new Vector3(currentRadius, currentRadius, 1);
    }

    public void Inflate() {
        shouldInflate = true;
    }

    public void Deflate() {
        shouldInflate = false;
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
