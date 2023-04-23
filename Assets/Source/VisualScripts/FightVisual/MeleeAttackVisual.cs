using System;
using NTC.Global.Cache;
using UnityEngine;


public class MeleeAttackVisual : MonoCache
{
    [SerializeField] private Animation animationTarget;
    [SerializeField] private bool shakeCamera;
    [SerializeField] private ShakePreset kickShakePreset;
    [SerializeField] private ShakePreset hitShakePreset;

    private Animator _animator;
    private IMeleeAttacker _meleeAttacker;

    private void Awake()
    {
        _meleeAttacker = GetComponentInParent<IMeleeAttacker>(); ;
        _animator = GetComponentInParent<Animator>();

        if (_meleeAttacker is null)
        {
            Debug.LogError("No melee attacker on prefab");  
        }

    }

    protected override void OnEnabled()
    {
        _meleeAttacker.OnStartPreparingAttack += HandleAttack;
        _meleeAttacker.OnHit += HandleHit;
    }
    
    private void HandleAttack()
    {
        if (animationTarget != null)
            animationTarget.Play();
        if (_animator != null)
        {
            _animator.SetTrigger("Melee");
        }

        if (shakeCamera)
        {
            CameraService.Instance.ShakeCamera(kickShakePreset);
        }
    }

    private void HandleHit()
    {
        if (shakeCamera)
        {
            CameraService.Instance.ShakeCamera(hitShakePreset);
        }
    }
}
