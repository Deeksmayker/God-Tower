using System;
using NTC.Global.Cache;
using UnityEngine;


public class MeleeAttackVisual : MonoCache
{
    [SerializeField] private Animation animationTarget;

    private IMeleeAttacker _meleeAttacker;

    private void Awake()
    {
        _meleeAttacker = GetComponentInParent<IMeleeAttacker>(); ;

        if (_meleeAttacker is null)
        {
            Debug.LogError("No melee attacker on prefab");
        }

        _meleeAttacker.OnStartPreparingAttack += HandleAttack;
    }

    private void HandleAttack()
    {
        animationTarget.Play();
    }
}
