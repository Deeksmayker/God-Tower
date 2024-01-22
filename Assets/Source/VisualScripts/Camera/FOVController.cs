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
    [SerializeField] private float speedThreshold = 25;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float addedFovToMax = 30;
    [SerializeField] private float fovChangeRate = 10;

    private float _baseFov;
    private float _maxFov;
    private float _desiredFov;
    private float _currentFov;

    [Inject] private PlayerUnit _player;

    private Camera _camera;
    private IMover _mover;

    private void Awake()
    {
        _camera = _player.GetComponentInChildren<Camera>();
        _mover = _player.GetComponentInParent<IMover>();

        _baseFov = _camera.fieldOfView;
        _maxFov = _baseFov + addedFovToMax;
        _currentFov = _baseFov;
    }

/*
    protected override void LateRun()
    {
        if (_mover.GetVelocityMagnitude() < speedThreshold)
            _desiredFov = _baseFov;
        else
        {
            var fovProgress = Mathf.Clamp01(Mathf.InverseLerp(speedThreshold, maxSpeed, _mover.GetVelocityMagnitude()));
            _desiredFov = Mathf.Lerp(_baseFov, _maxFov, fovProgress * fovProgress);
        }

        _currentFov = Mathf.Lerp(_currentFov, _desiredFov, fovChangeRate * Time.deltaTime);
        _camera.fieldOfView = _currentFov;
    }
*/
}