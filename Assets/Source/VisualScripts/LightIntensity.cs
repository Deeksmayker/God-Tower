using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightIntensity : MonoBehaviour
{
    [SerializeField] private Light lightSource;
    
    [SerializeField] private float defaultIntensity = 10.0f;
    [SerializeField] private float shakeVolume = 1.0f;
    [SerializeField] private float shakeSharpness = 0.1f;
    [SerializeField] private float shakeIntensity = 0.0f;
    [SerializeField] private float shakeTimer = 0.0f;

    void Start()
    {
        //ShakeIntensity(10);
        lightSource.intensity = defaultIntensity;
    }

    void Update()
    {
        if (shakeTimer > 0.0f)
        {
            shakeTimer -= Time.deltaTime;
            shakeIntensity = Mathf.Lerp(shakeIntensity, 0.0f, shakeSharpness * Time.deltaTime);
            float shakeValue = Random.Range(-shakeIntensity, shakeIntensity) * shakeVolume;
            lightSource.intensity = defaultIntensity + shakeValue;
        }
        else
        {
            lightSource.intensity = defaultIntensity;
        }
    }

    public void ShakeIntensity(float duration)
    {
        shakeIntensity = 1.0f;
        shakeTimer = duration;
    }
}