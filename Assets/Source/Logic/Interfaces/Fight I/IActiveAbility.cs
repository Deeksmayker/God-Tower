using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public interface IActiveAbility
{
    public event Action OnPerform;
    public event Action OnStartHolding;
    public event Action OnEmpty;
    
    //public void ChargeAbility();
    public void PerformAbility(int count);
    public void SetInput(bool value);
    public void SetRotationTarget(Transform rotationTarget);
    public void SetShootPoint(Transform shootPoint);
    public void SetInfinity(bool value);
    public void SetCooldown(float newCooldown);
    public void RemoveAbility();
    public bool IsInfinite();
    public float GetRemainingLifetime();
    public float GetMaxLifetime();
    public Vector3 GetPerformDirection();
    public Vector3 GetStartPoint();
    public Transform GetRotationTargetTransform();
    public AbilityTypes GetType();
    public bool CanPerform();
}