using System;
using Code.Global.Animations;
using DG.Tweening;
using NTC.Global.Cache;
using UnityEngine;

public class CameraService : MonoCache
{
    public static CameraService Instance;

    private bool _shaking;

    private void Awake()
    {
        Instance = this;
    }

    public void ShakeCamera(ShakePreset shakeSettings)
    {
        if (_shaking)
        {
            DOTween.Clear();
        }

        _shaking = true;

        var posShaking = shakeSettings.positionShake;
        if (posShaking.isOn)
        {
            AnimationShortCuts.ShakePositionAnimation(transform, shakeSettings).OnKill(() => _shaking = false);
        }

        var rotationShaking = shakeSettings.rotationShake;
        if (rotationShaking.isOn)
        {
            AnimationShortCuts.ShakeRotationAnimation(transform, shakeSettings).OnKill(() => _shaking = false);
        }
    }

    public Tween ShakeCameraPosition(ShakePreset shakePreset)
    {
        return AnimationShortCuts.ShakePositionAnimation(transform, shakePreset);
    }
    
    public Tween ShakeCameraRotation(ShakePreset shakePreset)
    {
        return AnimationShortCuts.ShakeRotationAnimation(transform, shakePreset);
    }
}
