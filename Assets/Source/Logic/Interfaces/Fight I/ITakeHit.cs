using System;
using UnityEngine;

public enum HitTypes
{
    NormalPoint,
    WeakPoint
}

public interface ITakeHit
{
    public event Action<float> OnTakeHit;
    public event Action<Vector3> OnTakeHitWithPosition;
    public event Action<HitTypes> OnTakeHitWithType;

    public void TakeHit(float damage, Vector3 hitPosition, HitTypes hitType);
}
