using System;
using NTC.Global.Cache;
using UnityEngine;


public class MeleeAttackVisual : MonoCache
{
    [SerializeField] private Animation animationTarget;

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

        _meleeAttacker.OnStartPreparingAttack += HandleAttack;
    }
    
    private void HandleAttack()
    {
        if (animationTarget != null)
            animationTarget.Play();
        if (_animator != null)
        {
            _animator.SetTrigger("Melee");
        }
    }
}
