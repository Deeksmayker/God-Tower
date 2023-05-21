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
    
    [SerializeField] private float targetHorizontalSpeed = 15;
    [SerializeField] private float groundHorizontalAcceleration = 75;
    [SerializeField] private float airHorizontalAcceleration = 25;
    
    private CharacterController _ch;
    private NavMeshAgent _aiAgent;

    private Vector2 _horizontalInput;
    private Vector3 _velocity;

    private float _groundCheckerHeightRelatedPosition;

    private bool _grounded = true;
    private bool _isResponseToInput = true;
    
    public event Action OnLanding;

    private void Awake()
    {
        _ch = Get<CharacterController>();
        _aiAgent = Get<NavMeshAgent>();
        
        if (_ch != null)
            _groundCheckerHeightRelatedPosition = groundCheckPoint.localPosition.y / _ch.height;
    }

    protected override void Run()
    {
        var newGroundedState = Physics.CheckSphere(groundCheckPoint.position, groundCheckRadius, groundLayer);
        
        if (newGroundedState && !_grounded)
            OnLanding?.Invoke();
        _grounded = newGroundedState;
        
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
    }

    private bool NeedToChangeHorizontalSpeed(float desiredSpeed, float actualSpeed)
    {
        if (IsGrounded())
            return true;

        if (!desiredSpeed.Equals(0) && !Mathf.Sign(desiredSpeed).Equals(Mathf.Sign(actualSpeed)))
            return true;
                                  
        return Mathf.Abs(desiredSpeed) > Mathf.Abs(actualSpeed);
    }

    /*private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.normal.y <= 0.5f && !IsGrounded())
        {
            SetVelocity(Vector3.Reflect(GetVelocity(), hit.normal) / 2);
        }
    }*/

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

    public void SetHorizontalVelocity(Vector3 newVelocity)
    {
        _velocity.x = newVelocity.x;
        _velocity.z = newVelocity.z;
    }

    public void AddVelocity(Vector3 addedVelocityVector)
    {
        _velocity += addedVelocityVector;
    }

    public void SetInputResponse(bool value)
    {
        _isResponseToInput = value;
    }

    [ContextMenu("Recalculate ground checker position")]
    public void RecalculateGroundCheckerPosition()
    {
        var localPos = groundCheckPoint.localPosition;
        groundCheckPoint.localPosition = new Vector3(localPos.x,
            _groundCheckerHeightRelatedPosition * _ch.height, localPos.z);
    }

    public float GetVelocityMagnitude()
    {
        return _ch != null ? _ch.velocity.magnitude : _aiAgent.velocity.magnitude;
    }

    public Vector3 GetVelocity()
    {
        return _velocity;
    }

    public Vector2 GetHorizontalInput()
    {
        return _horizontalInput;
    }

    public float GetHorizontalSpeed()
    {
        return new Vector3(_velocity.x, 0, _velocity.z).magnitude;
    }

    public bool IsGrounded()
    {
        return _grounded;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheckPoint.position, groundCheckRadius);
    }
}
