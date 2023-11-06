using System;
using UnityEngine;


public interface IMover
{
    public event Action OnLanding;
    public event Action<Vector3> OnBounce;
    
    public void SetInput(Vector3 input);
    public void SetVerticalVelocity(float velocity);
    public void AddVerticalVelocity(float addedVelocity);
    public void SetVelocity(Vector3 newVelocity);
    public void AddForce(Vector3 force);
    public void AddVelocity(Vector3 addedVelocityVector);
    public void SetInputResponse(bool value);
    
    public Vector3 GetVelocity();
    public float GetVelocityMagnitude();
    public bool IsGrounded();
}
