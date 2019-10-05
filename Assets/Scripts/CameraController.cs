using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject player; // camera will be connected to the player another way when scene flow starts being implemented
    
    public float lerpSpeed;

    Vector3 targetPosition;
    Vector3 anchorPosition;

    List<CameraShakeData> currentShakes;

    public CameraShakeData smallShake;
    public CameraShakeData normalShake;
    public CameraShakeData bigShake;


    private void OnEnable() {
        Totem.OnLand += StartNormalShake;
        PlayerController.OnDeath += StartBigShake;
    }
    private void OnDisable() {
        Totem.OnLand -= StartNormalShake;
        PlayerController.OnDeath -= StartBigShake;
    }

    private void Start() {
        anchorPosition = transform.position;
        currentShakes = new List<CameraShakeData>();
    }

    private void FixedUpdate() {
        if (player != null)
            targetPosition = player.transform.position.WithZ(anchorPosition.z);

        transform.position = Vector3.Lerp(anchorPosition, targetPosition, lerpSpeed);
        anchorPosition = transform.position;

        DoShakes();
    }

    void DoShakes() {
        // shakes
        transform.position = anchorPosition;
        transform.rotation = Quaternion.identity;
        
        for (int i = currentShakes.Count - 1; i >= 0; i--) {
            CameraShakeData shake = currentShakes[i];

            if (shake.IsComplete) {
                currentShakes.RemoveAt(i);
                continue;
            }
            
            transform.position += shake.GetShakePos().ToVector3(anchorPosition.z);
            transform.rotation = Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z + shake.GetShakeRot());

            shake.AdvanceShake();
        }
    }



    void StartSmallShake() => StartShake(smallShake);
    void StartNormalShake() => StartShake(normalShake);
    void StartBigShake() => StartShake(bigShake);

    void StartShake(CameraShakeData preset) {
        StartShake(preset.maxPosIntensity, preset.maxRotIntensity, preset.shakeTime, preset.roughness);
    }
    void StartShake(float posI, float rotI, float sTime, float rough) {
        currentShakes.Add(new CameraShakeData(posI, rotI, sTime, rough));
    }
}


[System.Serializable]
public class CameraShakeData {
    public float maxPosIntensity;
    public float maxRotIntensity;
    public float shakeTime;
    public float roughness;

    float shakeTimer;
    float randomIter;

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