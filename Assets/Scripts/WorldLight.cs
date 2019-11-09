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
        TryStartLightChanger(FadeIntensity(defaultIntensity * multiplier, defaultIntensity, time, 3f));    
    }


    public void FadeIn(float time) {
        if (light == null)
            light = GetComponent<Light2D>();

        TryStartLightChanger(FadeIntensity(0, defaultIntensity, time, 1.5f));
    }
    public void FadeOut(float time) {
        TryStartLightChanger(FadeIntensity(light.intensity, 0, time, 1.5f));
    }


    private void TryStartLightChanger(IEnumerator coroutine) {
        if (currentLightChanger != null)
            StopCoroutine(currentLightChanger);
        
        currentLightChanger = StartCoroutine(coroutine);
    }


    IEnumerator FadeIntensity(float startIntensity, float targetIntensity, float time, float lerpPower) {
        float timer = time;

        while (timer > 0) {
            light.intensity = Mathf.Lerp(targetIntensity, startIntensity, Mathf.Pow(timer / time, lerpPower));

            yield return 0;
            timer = Helpers.Timer(timer);
        }

        light.intensity = targetIntensity;
    }
}
