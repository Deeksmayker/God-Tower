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
    public Vector3 GetPerformDirection();
    public Vector3 GetStartPoint();
}