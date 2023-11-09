using NTC.Global.Cache;
using System;
using UnityEditor.Rendering;
using UnityEngine;

public class JumpEnemyMover : MonoCache, IMover
{
    [SerializeField] private float jumpForce;
    [SerializeField] private float gravity;

    private float _distanceToCheckWalls = 100;

    private bool _onWall;
    private bool _lastFrameGrounded;

    private Quaternion _desiredAngle;

    private Vector3 _velocity;
    private Vector3 _lastNonZeroVelocity;
    private Vector3 _currentContactNormal;

    private CharacterController _ch;

    public event Action OnLanding;
    public event Action<Vector3> OnBounce;

    private void Awake()
    {
        _ch = GetComponent<CharacterController>();
    }


    protected override void Run()
    {
        if (!_ch.isGrounded && !_onWall)
        {
            PredictLandingNormal();
        }
        
        if (_ch.isGrounded)
        {
            var horizontalVelocity = new Vector3(_velocity.x, 0, _velocity.z);
            horizontalVelocity = Vector3.Lerp(horizontalVelocity, Vector3.zero, Time.deltaTime * 5);
            _velocity = new Vector3(horizontalVelocity.x, _velocity.y, horizontalVelocity.z);
        }

        transform.rotation = Quaternion.Slerp(transform.rotation, _desiredAngle, Time.deltaTime * 5);
        if (!_onWall)
            _velocity.y += gravity * Time.deltaTime;
        
        _ch.Move(_velocity * Time.deltaTime);

        _lastFrameGrounded = _ch.isGrounded;
        if (_velocity.x != 0 || _velocity.z != 0)
            _lastNonZeroVelocity = _velocity;
    }

    public void JumpToDirection(Vector3 direction)
    {
        _velocity = direction * jumpForce;
        _onWall = false;
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        _currentContactNormal = hit.normal;
        Log("We hit something guys");
        if (hit.normal.y < 0.4f)
        {
            Log("We on the wall guys");
            if (!_onWall)
                OnLanding?.Invoke();
            _onWall = true;
            _velocity = Vector3.zero;
        }
        else
        {
            Log("We on floor guys");
            _onWall = false;
            if (!_lastFrameGrounded)
                OnLanding?.Invoke();
        }
    }

    private void PredictLandingNormal()
    {
        if (Physics.Raycast(transform.position, _velocity.normalized, out var hit, 100, Layers.Environment))
        {
            if (hit.normal.y < 0.4f)
                _desiredAngle = Quaternion.LookRotation(hit.normal);
            else
                _desiredAngle = Quaternion.LookRotation(new Vector3(_lastNonZeroVelocity.x, 0, _lastNonZeroVelocity.z).normalized, hit.normal);
        }
    }

    public Vector3 GetCurrentNormal() => _currentContactNormal;
    public bool OnWall() => _onWall;


    public void AddForce(Vector3 force)
    {
        throw new NotImplementedException();
    }

    public void AddVelocity(Vector3 addedVelocityVector)
    {
        throw new NotImplementedException();
    }

    public void AddVerticalVelocity(float addedVelocity)
    {
        throw new NotImplementedException();
    }

    public Vector3 GetVelocity()
    {
        throw new NotImplementedException();
    }

    public float GetVelocityMagnitude()
    {
        throw new NotImplementedException();
    }

    public bool IsGrounded()
    {
        return _ch.isGrounded;
    }

    public void SetInput(Vector3 input)
    {
        throw new NotImplementedException();
    }

    public void SetInputResponse(bool value)
    {
        throw new NotImplementedException();
    }

    public void SetVelocity(Vector3 newVelocity)
    {
        throw new NotImplementedException();
    }

    public void SetVerticalVelocity(float velocity)
    {
        throw new NotImplementedException();
    }
}