using System;
using Cysharp.Threading.Tasks;
using NTC.Global.Cache;
using UnityEngine;

public class LightIntensityChanger : MonoCache
{
    [SerializeField] private float maxIntensity;
    [SerializeField] private float timeForMaxIntensity;
    [SerializeField] private float fadeOutTime;

    private Light _light;

    private void Awake()
    {
        _light = GetComponentInChildren<Light>();
    }

    protected override void OnEnabled()
    {
        _light.intensity = 0;

        ChangeLight();
    }

    private async UniTask ChangeLight()
    {
        var timer = 0f;
        var t = 0f;

        while (timer < timeForMaxIntensity)
        {
            timer += Time.deltaTime;
            t = timer / timeForMaxIntensity;
            _light.intensity = Mathf.Lerp(0, maxIntensity, t);
            await UniTask.NextFrame();
        }

        timer = 0f;
        t = 0f;

        while (timer < fadeOutTime)
        {
            timer += Time.deltaTime;
            t = timer / fadeOutTime;
            _light.intensity = Mathf.Lerp(maxIntensity, 0, t * t);
            await UniTask.NextFrame();
        }
    }
}
