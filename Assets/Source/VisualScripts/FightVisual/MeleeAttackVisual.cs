using System;
using NTC.Global.Cache;
using NTC.Global.Pool;
using UnityEngine;
using UnityEngine.VFX;


public class MeleeAttackVisual : MonoCache
{
    [SerializeField] private Animation animationTarget;
    [SerializeField] private VisualEffect parryEffect;
    [SerializeField] private bool shakeCamera;
    [SerializeField] private ShakePreset kickShakePreset;
    [SerializeField] private ShakePreset hitShakePreset;

    private Animator _animator;
    private Kicker _meleeAttacker;

    private void Awake()
    {
        _meleeAttacker = GetComponentInParent<Kicker>(); ;
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
        _meleeAttacker.OnParry += HandleParry;
    }

    protected override void OnDisabled()
    {
        _meleeAttacker.OnStartPreparingAttack -= HandleAttack;
        _meleeAttacker.OnHit -= HandleHit;
        _meleeAttacker.OnParry -= HandleParry;
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
        var effect = NightPool.Spawn(parryEffect, transform.position + _meleeAttacker.GetAttackDirection(),
            Quaternion.LookRotation(_meleeAttacker.GetAttackDirection()));

        if (shakeCamera)
        {
            CameraService.Instance.ShakeCamera(hitShakePreset);
        }
    }

    private void HandleParry()
    {
        var effect = NightPool.Spawn(parryEffect, transform.position + _meleeAttacker.GetAttackDirection(),
            Quaternion.LookRotation(_meleeAttacker.GetAttackDirection()));
        
        HandleHit();
    }
}
