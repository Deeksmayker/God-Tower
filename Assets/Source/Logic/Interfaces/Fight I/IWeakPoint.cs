using System;
using UnityEngine;

public interface IWeakPoint
{
    public event Action<float> OnWeakPointHit;
    public event Action<Vector3> OnWeakPointHitWithPosition;

    public void TakeWeakPointHit(float damage, Vector3 hitPosition);
}
