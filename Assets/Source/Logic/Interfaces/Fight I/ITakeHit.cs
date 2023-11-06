using System;
using UnityEngine;

public interface ITakeHit
{
    public event Action<float> OnTakeHit;
    public event Action<Vector3> OnTakeHitWithAttackerPosition;
    public event Action<string> OnTakeHitWithAttackName;
    public event Action<float, Vector3, string> OnTakeHitWithDescription;

    public void TakeHit(float damage, Vector3 attackerPos, string attackName);
}
