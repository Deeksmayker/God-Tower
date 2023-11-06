using System;
using NTC.Global.Cache;
using UnityEngine;

public class PlayerCameraShaker : MonoCache
{
    [SerializeField] private ShakePreset jumpShakePreset;

    private IJumper _jumper;

    private void Awake()
    {
        _jumper = GetComponentInParent<IJumper>();
    }

    protected override void OnEnabled()
    {
        _jumper.OnJump += HandleJump;
    }
    
    protected override void OnDisabled()
    {
        _jumper.OnJump -= HandleJump;
    }

    private void HandleJump()
    {
        CameraService.Instance.ShakeCamera(0.15f);
    }
}
