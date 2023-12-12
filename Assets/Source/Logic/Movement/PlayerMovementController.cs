using ModestTree;
using NTC.Global.Cache;
using System;
using UnityEngine;

public class PlayerMovementController : MonoCache, IMover, IJumper
{
    [System.Serializable]
    public class MovementSettings
    {
        public float MaxSpeed;
        public float Acceleration;
        public float Deceleration;

        public MovementSettings(float maxSpeed, float accel, float decel)
        {
            MaxSpeed = maxSpeed;
            Acceleration = accel;
            Deceleration = decel;
        }
    }

    [Header("Movement")]
    [SerializeField] private float m_Friction = 6;
    [SerializeField] private float m_Gravity = 20;
    [SerializeField] private float m_JumpForce = 8;
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private float groundCheckRadius;
    [Tooltip("Automatically jump when holding jump button")]
    [SerializeField] private bool m_AutoBunnyHop = false;
    [Tooltip("How precise air control is")]
    [SerializeField] private float m_AirControl = 0.3f;
    [SerializeField] private MovementSettings m_GroundSettings = new MovementSettings(7, 14, 10);
    [SerializeField] private MovementSettings m_AirSettings = new MovementSettings(7, 2, 2);
    [SerializeField] private MovementSettings m_StrafeSettings = new MovementSettings(1, 50, 50);
    [SerializeField] private float weight = 5;

    [Header("Dash")]
    [SerializeField] private LayerMask environmentLayers;
    [SerializeField] private Transform camRootTransform;
    [SerializeField] private int dashCharges = 3;
    [SerializeField] private float dashAddedSpeed = 20f;
    [SerializeField] private float baseDashDuration = 0.5f;
    [SerializeField] private float minDashDuration = 0.1f;
    [SerializeField] private float baseDashCooldown = 2f;

    /// <summary>
    /// Returns player's current speed.
    /// </summary>
    public float Speed { get { return _ch.velocity.magnitude; } }

    private Vector3 m_MoveDirectionNorm = Vector3.zero;
    private Vector3 _velocity = Vector3.zero;
    private Vector3 _currentUpNormal;

    private float _surfaceAngle;

    private int _currentDashCharges;

    private const float c_verticalVelocityToBreakSlopeMovement = 10;

    private float _dashTimer;
    private float _dashCooldownTimer;
    private float _currentDashDuration;

    private float _jumpTimer;

    // Used to queue the next jump just before hitting the ground.
    private bool m_JumpQueued = false;

    private bool _isGrounded = true;
    private bool _contactedWithEnvThisFrame;
    private bool _previousContactedWithEnv;

    private bool _dashInput;
    private bool _dashHoldInput;
    private bool _dash = false;
    private bool _dashGroundInput = false;
    private bool _dashSliding = false;

    private Vector3 m_MoveInput;
    private Transform m_Tran;

    private CharacterController _ch;
    private AbilitiesHandler _abilitiesHandler;

    public event Action OnLanding;
    public event Action<Vector3> OnBounce;
    public event Action OnJump;
    public event Action OnStartDash;
    public event Action OnStopDash;

    private void Awake()
    {
        _currentDashCharges = dashCharges;
        _currentDashDuration = baseDashDuration;
        m_Tran = transform;
        _ch = GetComponent<CharacterController>();
        _abilitiesHandler = Get<AbilitiesHandler>();
    }

    protected override void OnEnabled()
    {
        _abilitiesHandler.OnKillEnemy += RefreshDashCharges;
    }

    protected override void OnDisabled()
    {
        _abilitiesHandler.OnKillEnemy -= RefreshDashCharges;
    }

    protected override void Run()
    {
        var newGrounded = Physics.CheckSphere(groundCheckPoint.position, groundCheckRadius, Layers.Environment | Layers.EnemyHurtBox);

        if (newGrounded && _surfaceAngle < 30 && !IsGrounded())
            OnLanding?.Invoke();

        _isGrounded = newGrounded && _surfaceAngle < 30;

        // if (!_dash && _currentDashCharges < dashCharges)
        // {
        //     _dashCooldownTimer -= Time.deltaTime;
        //
        //     if (_dashCooldownTimer <= 0)
        //     {
        //         _currentDashCharges++;
        //
        //         if (_currentDashCharges < dashCharges)
        //             _dashCooldownTimer = baseDashCooldown;
        //     }
        // }
        //
        // if (_dash)
        // {
        //     ContinueDash();
        // }

        // Set movement state.
        if (IsGrounded())
        {
            GroundMove();
        }
        else
        {
            AirMove();
        }

        if (_jumpTimer <= 0 && OnSlope())
        {
            _velocity = Vector3.ProjectOnPlane(_velocity, _currentUpNormal);
            _ch.Move(Vector3.down * Time.deltaTime);
        }

        _velocity.y = Mathf.Clamp(_velocity.y, -60, 100);
        _ch.Move(_velocity * Time.deltaTime);

        if (_contactedWithEnvThisFrame)
        {
            _contactedWithEnvThisFrame = false;
            _previousContactedWithEnv = true;
        }
        else if (_previousContactedWithEnv)
        {
            _previousContactedWithEnv = false;
        }

        if (_jumpTimer > 0)
            _jumpTimer -= Time.deltaTime;
    }

    // Handle air movement.
    private void AirMove()
    {
        float accel;

        var wishdir = new Vector3(m_MoveInput.x, 0, m_MoveInput.z);
        wishdir = m_Tran.TransformDirection(wishdir);

        float wishspeed = wishdir.magnitude;
        wishspeed *= m_AirSettings.MaxSpeed;

        wishdir.Normalize();
        m_MoveDirectionNorm = wishdir;

        // CPM Air control.
        float wishspeed2 = wishspeed;
        if (Vector3.Dot(_velocity, wishdir) < 0)
        {
            accel = m_AirSettings.Deceleration;
        }
        else
        {
            accel = m_AirSettings.Acceleration;
        }

        // If the player is ONLY strafing left or right
        if (m_MoveInput.x != 0)
        {
            if (wishspeed > m_StrafeSettings.MaxSpeed)
            {
                wishspeed = m_StrafeSettings.MaxSpeed;
            }

            accel = m_StrafeSettings.Acceleration;
        }

        Accelerate(wishdir, wishspeed, accel);
        if (m_AirControl > 0)
        {
            AirControl(wishdir, wishspeed2);
        }

        // Apply gravity
        _velocity.y -= m_Gravity * (_velocity.y > 0 ? 1 : 0.8f) * Time.deltaTime;
    }

    // Air control occurs when the player is in the air, it allows players to move side 
    // to side much faster rather than being 'sluggish' when it comes to cornering.
    private void AirControl(Vector3 targetDir, float targetSpeed)
    {
        // Only control air movement when moving forward or backward.
        if (Mathf.Abs(m_MoveInput.z) < 0.001)
        {
            return;
        }

        float zSpeed = _velocity.y;
        _velocity.y = 0;
        /* Next two lines are equivalent to idTech's VectorNormalize() */
        float speed = _velocity.magnitude;
        _velocity.Normalize();

        float dot = Vector3.Dot(_velocity, targetDir);
        float k = 32;
        k *= m_AirControl * dot * dot * Time.deltaTime;

        // Change direction while slowing down.
        if (dot > 0)
        {
            _velocity.x *= speed + targetDir.x * k;
            _velocity.y *= speed + targetDir.y * k;
            _velocity.z *= speed + targetDir.z * k;

            _velocity.Normalize();
            m_MoveDirectionNorm = _velocity;
        }

        _velocity.x *= speed;
        _velocity.y = zSpeed; // Note this line
        _velocity.z *= speed;
    }

    // Handle ground movement.
    private void GroundMove()
    {
        // Do not apply friction if the player is queueing up the next jump
        if (!m_JumpQueued)
        {
            ApplyFriction(1.0f);
        }
        else
        {
            ApplyFriction(0);
        }

        var wishdir = new Vector3(m_MoveInput.x, 0, m_MoveInput.z);
        wishdir = m_Tran.TransformDirection(wishdir);
        wishdir.Normalize();
        m_MoveDirectionNorm = wishdir;

        var wishspeed = wishdir.magnitude;
        wishspeed *= m_GroundSettings.MaxSpeed;

        Accelerate(wishdir, wishspeed, m_GroundSettings.Acceleration);

        // Reset the gravity velocity
        // if (_velocity.y <= 0)
        //     _velocity.y = -.1f;
        if (_jumpTimer <= 0)
            _ch.Move(Vector3.down * 0.1f);

        if (m_JumpQueued)
        {
            Jump();
        }
    }

    private void ApplyFriction(float t)
    {
        // Equivalent to VectorCopy();
        Vector3 vec = _velocity;
        vec.y = 0;
        float speed = vec.magnitude;
        float drop = 0;

        // Only apply friction when grounded.
        if (IsGrounded())
        {
            float control = speed < m_GroundSettings.Deceleration ? m_GroundSettings.Deceleration : speed;
            drop = control * m_Friction * Time.deltaTime * t;
        }

        float newSpeed = speed - drop;
        if (newSpeed < 0)
        {
            newSpeed = 0;
        }

        if (speed > 0)
        {
            newSpeed /= speed;
        }

        _velocity.x *= newSpeed;
        // playerVelocity.y *= newSpeed;
        _velocity.z *= newSpeed;
    }

    // Calculates acceleration based on desired speed and direction.
    private void Accelerate(Vector3 targetDir, float targetSpeed, float accel)
    {
        float currentspeed = Vector3.Dot(_velocity.normalized, targetDir);
        currentspeed *= _velocity.magnitude;
        float addspeed = targetSpeed - currentspeed;
        if (addspeed <= 0)
        {
            return;
        }

        float accelspeed = accel * Time.deltaTime * targetSpeed;
        if (accelspeed > addspeed)
        {
            accelspeed = addspeed;
        }

        _velocity.x += accelspeed * targetDir.x;
        _velocity.z += accelspeed * targetDir.z;
    }

    private void Jump()
    {
        if (_jumpTimer > 0) return;
        _velocity.y = m_JumpForce;
        m_JumpQueued = false;

        OnJump?.Invoke();
        _jumpTimer = 0.2f;
    }

    private void StartDash()
    {
        _dash = true;
        var direction = camRootTransform.forward;
        if (!m_MoveInput.Equals(Vector3.zero) && direction.y > -0.5f && direction.y < 0.5f)
        {
            direction = m_Tran.TransformDirection(m_MoveInput);
            direction.y = camRootTransform.forward.y;
        }

        direction.y = Mathf.Clamp(direction.y, -1f, 0.1f);
        _velocity = direction * (_velocity.magnitude + dashAddedSpeed);
        OnStartDash?.Invoke();

        _currentDashCharges--;
        _dashCooldownTimer = baseDashCooldown;
    }

    private void ContinueDash()
    {
        _dashTimer += Time.deltaTime;

        if (!_dashSliding && IsGrounded())
        {
            _dashTimer = _currentDashDuration;
            _dashSliding = true;
        }

        if (_dashSliding)
            AirMove();

        if (IsGrounded() && m_JumpQueued && _dashTimer > _currentDashDuration / 2)
        {
            StopDash();
        }

        if (_dashTimer > _currentDashDuration || _dashTimer > _currentDashDuration / 2 && !_dashInput)
            StopDash();
    }

    public void StopDash(bool keepMomentum = false)
    {
        if (!_dash)
            return;

        _dash = false;
        OnStopDash?.Invoke();
        _dashTimer = 0;

        if (IsGrounded() && _dashInput)
        {
            _velocity *= 0.6f;
            Jump();
            _velocity.y = 40;
            return;
        }

        if (keepMomentum || _velocity.magnitude < dashAddedSpeed)
            return;
        
        _velocity *= 0.6f;
    }

    public void RefreshDashCharges()
    {
        _currentDashCharges = dashCharges;
    }

    private bool CanDash()
    {
        if (Physics.Raycast(transform.position, Vector3.down, 4f, environmentLayers) && _velocity.y < 0)
            return false;

        return !IsGrounded() && _currentDashCharges > 0 && !_dash;
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.normal.y > 0)
            _currentUpNormal = hit.normal;

        _surfaceAngle = Vector3.Angle(Vector3.up, hit.normal);

		if (hit.normal.y <= -0.99f && _velocity.y > 0){
			_velocity.y = 0;
		}

        if (Mathf.Pow(2, hit.gameObject.layer) == (int)Layers.Environment)
        {
            _previousContactedWithEnv = true;
            _contactedWithEnvThisFrame = true;
        }

        if (hit.normal.y > -0.99f){
            _velocity = Vector3.ProjectOnPlane(_velocity, hit.normal);
        }
    }

    public void SetInput(Vector3 input)
    {
        m_MoveInput = new Vector3(input.x, 0, input.y);
    }

    public void SetJumpInput(bool value)
    {
        m_JumpQueued = value;
    }

    public void SetDashInput(bool input)
    {
        _dashInput = input;

        if (!input)
            _dashHoldInput = false;

        if (!input && (_dashGroundInput || IsGrounded()))
            _dashGroundInput = false;

        if (input && IsGrounded())
            _dashGroundInput = true;

        if(input && _dashGroundInput || _dashHoldInput)
            return;

        if (input && CanDash())
        {
            StartDash();
            _dashHoldInput = true;
        }
    }

    public void SetDashDuration(float duration)
    {
        _currentDashDuration = duration;
    }

    public void SetVerticalVelocity(float velocity)
    {
        _velocity.y = velocity;
        if (_velocity.y > 10) _jumpTimer = 0.2f;
    }

    public void SetMaxSpeed(float value)
    {
        //targetHorizontalSpeed = value;
    }

    public void AddVerticalVelocity(float addedVelocity)
    {
        _velocity.y += addedVelocity;

        _velocity.y = Mathf.Clamp(_velocity.y, -200, 100);

        if (_velocity.y > 10) _jumpTimer = 0.2f;
    }

    public void SetVelocity(Vector3 newVelocity)
    {
        _velocity = newVelocity;
        if (_velocity.y > 10) _jumpTimer = 0.2f;
    }

    public void SetHorizontalVelocity(Vector3 newVelocity)
    {
        _velocity.x = newVelocity.x;
        _velocity.z = newVelocity.z;
    }

    public bool OnSlope(){
        return IsGrounded() && _surfaceAngle > 0 && _surfaceAngle < 30;
    }

    public void AddVelocity(Vector3 addedVelocityVector)
    {
        _velocity += addedVelocityVector;
        if (_velocity.y > 10) _jumpTimer = 0.2f;
    }

    public void AddOrSetVelocity(Vector3 addedVeloctiy)
    {
        _velocity = Vector3.RotateTowards(_velocity, addedVeloctiy, 100, 100) + addedVeloctiy;
        if (_velocity.y > 10) _jumpTimer = 0.2f;
    }

    public void SetInputResponse(bool value)
    {
        //_isResponseToInput = value;
    }

    [ContextMenu("Recalculate ground checker position")]
    public void RecalculateGroundCheckerPosition()
    {
        /*var localPos = groundCheckPoint.localPosition;
        groundCheckPoint.localPosition = new Vector3(localPos.x,
            _groundCheckerHeightRelatedPosition * _ch.height, localPos.z);*/
    }

    public float GetVelocityMagnitude()
    {
        return _ch.velocity.magnitude;
    }

    public Vector3 GetVelocity()
    {
        return _velocity;
    }

    public Vector2 GetHorizontalInput()
    {
        return m_MoveInput;
    }

    public float GetHorizontalSpeed()
    {
        return new Vector3(_velocity.x, 0, _velocity.z).magnitude;
    }

    public float GetBaseDashDuration()
    {
        return baseDashDuration;
    }

    public bool IsGrounded()
    {
        return _isGrounded;
    }

    public void AddForce(Vector3 force)
    {
        _velocity += force / weight;
        if (_velocity.y > 10) _jumpTimer = 0.2f;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheckPoint.position, groundCheckRadius);
    }
}
