using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using NTC.Global.Cache;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.Serialization;
using Zenject;

public class FOVController : MonoCache
{
    [SerializeField] private float step = 0.5f;
    
    [Inject] private PlayerUnit player;
    
    private IMover _mover;
    private Camera _camera;
    private Coroutine _fovChangeCoroutine;

    private float _oldSpeed = 0;
    private float _maxSpeed = 120;
    private float _maxFov = 130;
    private float _minFov = 100;
    
    protected override void OnEnabled()
    {
        _mover = player.Get<IMover>();
        _camera = Get<Camera>();

        _oldSpeed = _mover.GetHorizontalSpeed();
    }

    protected override void LateRun()
    {
        if (_mover.GetVelocityMagnitude().Equals(_oldSpeed)) return;

        _oldSpeed = _mover.GetVelocityMagnitude();
        Debug.Log(_oldSpeed);
        ChangeFOV(Mathf.Clamp(_oldSpeed/_maxSpeed * (_maxFov - _minFov) + _minFov, _minFov, _maxFov));
    }

    private void ChangeFOV(float value)
    {
        if (_fovChangeCoroutine != null)
            StopCoroutine(_fovChangeCoroutine);
        
        _fovChangeCoroutine = StartCoroutine(ChangeFOVWithSmooth(_camera, value));
    }

    private IEnumerator ChangeFOVWithSmooth(Camera camera, float value)
    {
        if (camera.fieldOfView > value)
        {
            while (camera.fieldOfView > value)
            {
                yield return null;
                camera.fieldOfView -= step * Time.deltaTime;
            }
        }
        else
        {
            while (camera.fieldOfView < value)
            {
                yield return null;
                camera.fieldOfView += step * Time.deltaTime;
            }
        }
    }
}