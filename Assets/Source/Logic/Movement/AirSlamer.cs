using System;
using NTC.Global.Cache;
using UnityEngine;

public class AirSlamer : MonoCache
{
    [SerializeField] private float slamVelocity = 50;

    private bool _input;
    private bool _slaming;
    
    private IMover _mover;

    private void Awake()
    {
        _mover = Get<IMover>();
    }

    protected override void Run()
    {
        if (_mover.IsGrounded())
            _slaming = false;
        
        if (_input && !_slaming && !_mover.IsGrounded())
        {
            _slaming = true;
            _mover.SetVerticalVelocity(-slamVelocity);
        }
    }
    
    public void SetInput(bool value)
    {
        _input = value;
    }
}
