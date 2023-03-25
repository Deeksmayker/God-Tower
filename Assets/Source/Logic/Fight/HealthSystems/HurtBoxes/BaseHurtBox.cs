using System;
using NTC.Global.Cache;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class BaseHurtBox : MonoCache, ITakeHit
{
    [SerializeField] private float damageMultiplier = 1;
    
    public event Action<float> OnTakeHit;
    public event Action<Vector3> OnTakeHitWithPosition;
    public event Action<HitTypes> OnTakeHitWithType;

    public void TakeHit(float damage, Vector3 hitPosition, HitTypes hitType)
    {
        OnTakeHit?.Invoke(damage * damageMultiplier);
        OnTakeHitWithPosition?.Invoke(hitPosition);
        OnTakeHitWithType?.Invoke(hitType);
    }
}
