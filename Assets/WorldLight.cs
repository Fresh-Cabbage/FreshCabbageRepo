using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class WorldLight : MonoBehaviour
{
    public float defaultIntensity = 1;
    Coroutine currentLightChanger;

    new Light2D light;

    private void Start() {
        light = GetComponent<Light2D>();
    }

    public void Flash(float multiplier, float time) {
        if (currentLightChanger != null)
            StopCoroutine(currentLightChanger);

        currentLightChanger = StartCoroutine(DoFlash(multiplier, time));    
    }

    IEnumerator DoFlash(float multiplier, float time) {
        float timer = time;
        while (timer > 0) {
            light.intensity = Mathf.Lerp(defaultIntensity, defaultIntensity * multiplier, Mathf.Pow(timer / time, 3f));

            yield return 0;
            timer = Helpers.Timer(timer);
        }

        light.intensity = defaultIntensity;
    }
}
