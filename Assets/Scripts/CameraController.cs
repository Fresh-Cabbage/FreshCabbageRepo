using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject player; // camera will be connected to the player another way when scene flow starts being implemented
    
    public float lerpSpeed;

    Vector3 targetPosition;
    Vector3 anchorPosition;

    public List<CameraShakeData> currentShakes;

    private void Start() {
        anchorPosition = transform.position;

        //currentShakes = new List<CameraShakeData>();
    }

    private void FixedUpdate() {
        if (player != null)
            targetPosition = /*player.*/anchorPosition.WithZ(anchorPosition.z);

        transform.position = Vector3.Lerp(anchorPosition, targetPosition, lerpSpeed);
        anchorPosition = transform.position;

        // shakes
        if (currentShakes.Count == 0) {
            // reset pos and rot
            transform.position = anchorPosition;
            transform.rotation = Quaternion.identity;
        }
        for (int i = currentShakes.Count - 1; i >= 0; i--) {
            CameraShakeData shake = currentShakes[i];

            if (shake.IsComplete) {
                currentShakes.RemoveAt(i);
                continue;
            }
            
            transform.position = anchorPosition + shake.GetShakePos().ToVector3(anchorPosition.z);
            transform.rotation = Quaternion.Euler(0, 0, shake.GetShakeRot());

            shake.AdvanceShake();
        }
    }
}


[System.Serializable]
public class CameraShakeData {
    public float maxPosIntensity;
    public float maxRotIntensity;
    public float shakeTime;
    public float roughness;

    public float shakeTimer;
    public float randomIter;

    public bool IsComplete { get { return shakeTimer == 0; }}

    public CameraShakeData(float posI, float rotI, float sTime, float rough) {
        maxPosIntensity = posI;
        maxRotIntensity = rotI;
        shakeTime = sTime;
        roughness = rough;

        shakeTimer = shakeTime;
        randomIter = Random.Range(-100, 100);
    }

    public Vector2 GetShakePos() {
        Vector2 randVector = new Vector2(Mathf.PerlinNoise(randomIter, 0) - 0.5f, Mathf.PerlinNoise(0, randomIter) - 0.5f);
        Debug.Log((randVector * maxPosIntensity * EaseOut(shakeTimer / shakeTime)).magnitude);
        return randVector * maxPosIntensity * EaseOut(shakeTimer / shakeTime);
    }
    public float GetShakeRot() {
        float randAngle = Mathf.PerlinNoise(randomIter, randomIter) - 0.5f;
        return randAngle * maxRotIntensity * EaseOut(shakeTimer / shakeTime);
    }

    public void AdvanceShake() {
        randomIter += Time.deltaTime * roughness;
        shakeTimer = Helpers.Timer(shakeTimer);
    }


    // note: will replace with full easings class later
    static public float EaseOut(float p)
	{
		return p * p * p;
	}

}