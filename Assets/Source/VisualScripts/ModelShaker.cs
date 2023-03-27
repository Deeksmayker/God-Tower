using System;
using NTC.Global.Cache;
using UnityEngine;
using Random = UnityEngine.Random;

public class ModelShaker : MonoCache
{
    private float _rapidTimer;
    
    private float _durableLerpTimer;
    private float _durableTimeToMaxIntensity;

    private float _rapidIntensity;
    private float _durableMaxIntensity;

    private float _rapidAmount;
    private float _durableAmount;

    private bool _durableShaking;

    private Vector3 _originalPosition;
    
    private void Awake()
    {
        _originalPosition = transform.localPosition;
    }

    protected override void Run()
    {
        if (_rapidTimer > 0)
        {
            var shakeDisplacement = Mathf.Sin(Time.time * _rapidIntensity) * _rapidAmount;
            transform.localPosition =
                _originalPosition + new Vector3(shakeDisplacement, shakeDisplacement, shakeDisplacement) + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)) * _rapidAmount;
            _rapidTimer -= Time.deltaTime;
            if (_rapidTimer <= 0)
            {
                transform.localPosition = _originalPosition;
            }
        }
        else if (_durableShaking)
        {
            var currentIntensity = Mathf.Lerp(0, _durableMaxIntensity, _durableLerpTimer);
            var shakeDisplacement = Mathf.Sin(Time.time * currentIntensity) * _durableAmount;
            transform.localPosition =
                _originalPosition + new Vector3(shakeDisplacement, shakeDisplacement, shakeDisplacement) + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)) * _durableAmount;
        }

        if (_durableShaking)
        {
            _durableLerpTimer += Time.deltaTime / _durableTimeToMaxIntensity;
            _durableLerpTimer = Mathf.Clamp01(_durableLerpTimer);
        }
    }


    public void StartRapidShaking(float duration, float intensity, float amount)
    {
        _rapidTimer = duration;
        _rapidIntensity = intensity;
        _rapidAmount = amount;
    }

    public void StartDurableShaking(float timeToMaxIntensity, float maxIntensity, float amount)
    {
        _durableTimeToMaxIntensity = timeToMaxIntensity;
        _durableMaxIntensity = maxIntensity;
        _durableAmount = amount;
        _durableLerpTimer = 0;
        _durableShaking = true;
    }

    public void StopDurableShaking()
    {
        _durableShaking = false;
        transform.localPosition = _originalPosition;
    }
}
