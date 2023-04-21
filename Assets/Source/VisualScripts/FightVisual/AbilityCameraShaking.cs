using System;
using NTC.Global.Cache;
using UnityEngine;
using Zenject;

public class AbilityCameraShaking : MonoCache
{
    [SerializeField] private ShakePreset shakePreset;

    [SerializeField] private bool separateShakingOnDump;
    [SerializeField] private ShakePreset shakePresetOnDump;

    private IActiveAbility _ability;

    private void Awake()
    {
        _ability = Get<IActiveAbility>();
    }

    protected override void OnEnabled()
    {
        _ability.OnPerform += HandlePerformingAbility;

        if (separateShakingOnDump)
            _ability.OnDump += HandleDump;
    }
    
    protected override void OnDisabled()
    {
        _ability.OnPerform -= HandlePerformingAbility;
        
        if (separateShakingOnDump)
            _ability.OnDump -= HandleDump;
    }

    private void HandlePerformingAbility()
    {
        CameraService.Instance.ShakeCamera(shakePreset);
    }

    private void HandleDump()
    {
        CameraService.Instance.ShakeCamera(shakePresetOnDump);
    }
}
