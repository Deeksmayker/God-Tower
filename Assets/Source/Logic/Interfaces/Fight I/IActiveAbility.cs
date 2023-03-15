using System;
using UnityEngine;

public interface IActiveAbility
{
    public event Action OnPerform;
    public event Action OnSurge;
    
    public void ChargeAbility();
    public void PerformAbility();
    public void SurgeAbility();
    public void SetInput(bool value);
    public void SetRotationTarget(Transform rotationTarget);
    public void SetShootPoint(Transform shootPoint);
    public Vector3 GetPerformDirection();
    public Vector3 GetStartPoint();
}