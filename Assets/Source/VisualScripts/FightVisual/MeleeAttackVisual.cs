using System;
using NTC.Global.Cache;
using NTC.Global.Pool;
using UnityEngine;
using UnityEngine.VFX;


public class MeleeAttackVisual : MonoCache
{
    [SerializeField] private Animation animationTarget;
    private ParticleSystem parryEffect;
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

        parryEffect = (Resources.Load(ResPath.Particles + "PlayerHitParticles") as GameObject).GetComponent<ParticleSystem>();

    }

    protected override void OnEnabled()
    {
        _meleeAttacker.OnStartPreparingAttack += HandleAttack;
        _meleeAttacker.OnHit += HandleHit;
    }

    protected override void OnDisabled()
    {
        _meleeAttacker.OnStartPreparingAttack -= HandleAttack;
        _meleeAttacker.OnHit -= HandleHit;
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
            CameraService.Instance.ShakeCamera(0.05f);
        }
    }

    private void HandleHit()
    {
        var effect = NightPool.Spawn(parryEffect, transform.position + _meleeAttacker.GetAttackDirection(),
            Quaternion.LookRotation(_meleeAttacker.GetAttackDirection()));
        effect.Play();

        if (shakeCamera)
        {
            CameraService.Instance.ShakeCamera(0.1f);
        }
    }

    private void HandleParry()
    {
        var effect = NightPool.Spawn(parryEffect, transform.position + _meleeAttacker.GetAttackDirection(),
            Quaternion.LookRotation(_meleeAttacker.GetAttackDirection()));

        HandleHit();
    }
}
