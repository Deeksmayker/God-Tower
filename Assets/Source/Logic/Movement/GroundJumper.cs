using System;
using NTC.Global.Cache;
using UnityEngine;

[RequireComponent(typeof(IMover))]
public class GroundJumper : MonoCache, IJumper
{
    [SerializeField] private float jumpHeight = 10;

    private float _defaultJumpHeight;
    
    private bool _jumpInput;
    
    private IMover _mover;
    private GravityMaker _gravityMaker;
    
    public event Action OnJump;

    private void Awake()
    {
        _mover = Get<IMover>();
        _gravityMaker = Get<GravityMaker>();
        _defaultJumpHeight = jumpHeight;
    }

    protected override void Run()
    {
        if (NeedToJump())
        {
            Jump(Vector3.up);
        }
    }


    public void Jump(Vector3 direction)
    {
        OnJump?.Invoke();
        var gravityValue = _gravityMaker != null ? _gravityMaker.RiseGravity : -10;
        _mover.SetVerticalVelocity(Mathf.Sqrt(jumpHeight * -2f * gravityValue));
    }

    public void SetJumpInput(bool value)
    {
        _jumpInput = value;
    }

    public void AddJumpHeight(float addedValue)
    {
        jumpHeight += addedValue;
    }

    public void RestoreJumpHeight()
    {
        jumpHeight = _defaultJumpHeight;
    }

    public float GetJumpHeight()
    {
        return jumpHeight;
    }

    public float GetDefaultJumpHeight()
    {
        return _defaultJumpHeight;
    }

    public bool NeedToJump()
    {
        return _jumpInput && _mover.IsGrounded();
    }
}
