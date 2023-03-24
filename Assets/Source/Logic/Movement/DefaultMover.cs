using System;
using System.Collections;
using System.Collections.Generic;
using NTC.Global.Cache;
using UnityEngine;
using UnityEngine.AI;

public class DefaultMover : MonoCache, IMover
{
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckRadius;
    
    [SerializeField] private bool alignMovementWithRotation;
    [SerializeField] private bool rotateByVelocityVector;
    
    [SerializeField] private float targetHorizontalSpeed = 15;
    [SerializeField] private float groundHorizontalAcceleration = 75;
    [SerializeField] private float airHorizontalAcceleration = 25;
    
    private CharacterController _ch;
    private NavMeshAgent _aiAgent;

    private Vector2 _horizontalInput;
    private Vector3 _velocity;

    private bool _isResponseToInput = true;
    
    private void Awake()
    {
        _ch = Get<CharacterController>();
        _aiAgent = Get<NavMeshAgent>();
    }

    protected override void Run()
    {
        PerformMove();
    }

    public void PerformMove()
    {
        if (_isResponseToInput)
        {
            var horizontalInputDirection = new Vector3(_horizontalInput.x, 0, _horizontalInput.y).normalized;

            if (alignMovementWithRotation)
            {
                horizontalInputDirection = horizontalInputDirection.x * transform.right +
                                           horizontalInputDirection.z * transform.forward;
            }

            var acceleration = IsGrounded() ? groundHorizontalAcceleration : airHorizontalAcceleration;
            if (!horizontalInputDirection.x.Equals(0) && !Mathf.Sign(horizontalInputDirection.x).Equals(Mathf.Sign(_velocity.x)) ||
                !horizontalInputDirection.z.Equals(0) && !Mathf.Sign(horizontalInputDirection.z).Equals(Mathf.Sign(_velocity.z)))
                acceleration *= 5;
            acceleration *= Time.deltaTime;

            var desiredVelocity = horizontalInputDirection * targetHorizontalSpeed;

            var horizontalVelocity = Vector3.MoveTowards(_velocity, desiredVelocity, acceleration);

            if (NeedToChangeHorizontalSpeed(desiredVelocity.x, _velocity.x))
                _velocity.x = horizontalVelocity.x;
            if (NeedToChangeHorizontalSpeed(desiredVelocity.z, _velocity.z))
                _velocity.z = horizontalVelocity.z;
            
        }
        
        if (_ch != null)
            _ch.Move(_velocity * Time.deltaTime);
        else if (_aiAgent != null)
            _aiAgent.Move(_velocity * Time.deltaTime);
        
        if (rotateByVelocityVector)
        {
            if (_aiAgent.velocity == Vector3.zero)
                return;
            transform.rotation = Quaternion.LookRotation(_aiAgent.velocity);
        }
    }

    private bool NeedToChangeHorizontalSpeed(float desiredSpeed, float actualSpeed)
    {
        return IsGrounded() || !desiredSpeed.Equals(targetHorizontalSpeed) || actualSpeed < desiredSpeed;
    }

    public void SetHorizontalInput(Vector2 input)
    {
        _horizontalInput = input;
    }

    public void SetVerticalVelocity(float velocity)
    {
        _velocity.y = velocity;
    }

    public void SetMaxSpeed(float value)
    {
        targetHorizontalSpeed = value;
    }

    public void AddVerticalVelocity(float addedVelocity)
    {
        _velocity.y += addedVelocity;
    }

    public void SetVelocity(Vector3 newVelocity)
    {
        _velocity = newVelocity;
    }

    public void AddVelocity(Vector3 addedVelocityVector)
    {
        _velocity += addedVelocityVector;
    }

    public void SetInputResponse(bool value)
    {
        _isResponseToInput = value;
    }

    public float GetVelocityMagnitude()
    {
        return _ch != null ? _ch.velocity.magnitude : _aiAgent.velocity.magnitude;
    }

    public Vector3 GetVelocity()
    {
        return _velocity;
    }

    public float GetHorizontalSpeed()
    {
        return new Vector3(_velocity.x, 0, _velocity.z).magnitude;
    }

    public bool IsGrounded()
    {
        return Physics.CheckSphere(groundCheckPoint.position, groundCheckRadius, groundLayer);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheckPoint.position, groundCheckRadius);
    }
}
