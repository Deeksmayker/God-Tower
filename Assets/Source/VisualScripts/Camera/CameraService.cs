using System;
using Code.Global.Animations;
using DG.Tweening;
using NTC.Global.Cache;
using UnityEngine;

public class CameraService : MonoCache
{
    public static CameraService Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void ShakeCamera(ShakePreset shakeSettings)
    {
        var posShaking = shakeSettings.positionShake;
        if (posShaking.isOn)
        {
            AnimationShortCuts.ShakePositionAnimation(transform, shakeSettings);
        }

        var rotationShaking = shakeSettings.rotationShake;
        if (rotationShaking.isOn)
        {
            AnimationShortCuts.ShakeRotationAnimation(transform, shakeSettings);
        }
    }
}
