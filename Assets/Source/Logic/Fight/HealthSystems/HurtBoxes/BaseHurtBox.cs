using System;
using NTC.Global.Cache;
using UnityEngine;

public class BaseHurtBox : MonoCache, ITakeHit
{
    [SerializeField] private float damageMultiplier = 1;
    
    public event Action<float> OnTakeHit;
    public event Action<Vector3> OnTakeHitWithAttackerPosition;
    public event Action<string> OnTakeHitWithAttackName;
    public event Action<float, Vector3, string> OnTakeHitWithDescription;

    public void TakeHit(float damage, Vector3 attackerPos, string attackName)
    {
        OnTakeHit?.Invoke(-damage * damageMultiplier);
        OnTakeHitWithAttackerPosition?.Invoke(attackerPos);
        OnTakeHitWithAttackName?.Invoke(attackName);
        OnTakeHitWithDescription?.Invoke(-damage * damageMultiplier, attackerPos, attackName);
    }
}
