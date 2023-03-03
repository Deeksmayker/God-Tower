using System;
using UnityEngine;

public interface IWeakPoint
{
    public event Action<int> OnWeakPointHit;
    public event Action<Vector3> OnWeakPointHitWithPosition;

    public void TakeWeakPointHit(int damage, Vector3 hitPosition);
}
