using System;
using UnityEngine;

public interface ITakeHit
{
    public event Action<float> OnTakeHit;
    public event Action<Vector3> OnTakeHitWithPosition;

    public void TakeHit(float damage, Vector3 hitPosition);
}
