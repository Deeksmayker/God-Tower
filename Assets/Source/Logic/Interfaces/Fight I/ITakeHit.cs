using System;
using UnityEngine;

public interface ITakeHit
{
    public event Action<int> OnTakeHit;
    public event Action<Vector3> OnTakeHitWithPosition;

    public void TakeHit(int damage, Vector3 hitPosition);
}
