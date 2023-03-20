using System;
using NTC.Global.Cache;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class BaseWeakPointHurtBox : MonoCache, IWeakPoint
{
    public event Action<float> OnWeakPointHit;
    public event Action<Vector3> OnWeakPointHitWithPosition;
    
    public void TakeWeakPointHit(float damage, Vector3 hitPosition)
    {
        OnWeakPointHit?.Invoke(damage);
        OnWeakPointHitWithPosition?.Invoke(hitPosition);
    }
}
