using System;
using NTC.Global.Cache;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class BaseHurtBox : MonoCache, ITakeHit
{
    public event Action<int> OnTakeHit;
    public event Action<Vector3> OnTakeHitWithPosition;
    
    public void TakeHit(int damage, Vector3 hitPosition)
    {
        OnTakeHit?.Invoke(damage);
        OnTakeHitWithPosition?.Invoke(hitPosition);
    }
}
