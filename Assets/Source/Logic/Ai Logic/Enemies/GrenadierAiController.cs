using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using Zenject;

public class GrenadierAiController : BaseAiController
{
    [SerializeField] private LayerMask layersToAttack;
    [SerializeField] private LayerMask environmentLayers;
    [SerializeField] private Transform rotationTarget;

    [SerializeField] private float jumpForce = 50;
    
    [Header("Timers")]
    [SerializeField] private float timeBeforeShootToRotateHead = 0.3f;
    [SerializeField] private float timeChangeLocation = 5;
    [SerializeField] private float cantAttackTimeToChangeLocation = 1;
    [SerializeField] private float prepareJumpTime = 1;

    [Inject] private List<GrenadierJumpPoint> _jumpPoints;

    private GrenadierJumpPoint _currentPoint;
    
    private float _timeOnLocation;
    private float _cantAttackTime;
    
    private bool _jumping;
    
    private Transform _target;
    
    private Vector3 _position;
    private Vector3 _rotationTargetPosition;

    private Animator _animator;
    private IMover _mover;
    private GravityMaker _gravityMaker;
    private IAiMovementController _movementController;
    private IAiRangeAttackController _rangeAttackController;
    private GrenadeAbility _grenadeAbility;

    public event Action OnStartPreparingJump;
    public event Action OnJump;

    private void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
        _movementController = Get<IAiMovementController>();
        _rangeAttackController = Get<IAiRangeAttackController>();
        _grenadeAbility = GetComponentInChildren<GrenadeAbility>();
        _mover = Get<IMover>();
        _gravityMaker = Get<GravityMaker>();
        
        _target = Physics.OverlapSphere(transform.position, 1000, layersToAttack)[0].transform;
    }
    
    protected override void OnEnabled()
    {
        base.OnEnabled();
        _grenadeAbility.OnStartHolding += HandleStartChargingGrenadeAttack;
        _grenadeAbility.OnPerform += HandlePerformingGrenadeAttack;
    }

    protected override void OnDisabled()
    {
        base.OnDisabled();
        _grenadeAbility.OnStartHolding -= HandleStartChargingGrenadeAttack;
        _grenadeAbility.OnPerform -= HandlePerformingGrenadeAttack;
    }

    protected override void Run()
    {
        _position = transform.position;
        _rotationTargetPosition = rotationTarget.position;

        if (_dead)
            return;
        
        if (_jumping)
        {
            _timeOnLocation = 0;
            _cantAttackTime = 0;
            return;
        }

        _timeOnLocation += Time.deltaTime;

        if (!CanAttack())
        {
            _cantAttackTime += Time.deltaTime;
        }

        if (_timeOnLocation > timeChangeLocation || _cantAttackTime > cantAttackTimeToChangeLocation)
        {
            TryFindPositionToJump();
        }
    }

    public override void SetTargetDetected(bool value)
    {
        base.SetTargetDetected(value);
        _jumping = !value;
    }

    private async UniTask TryFindPositionToJump()
    {
        for (var i = 0; i < _jumpPoints.Count; i++)
        {
            var canAttack = CanAttackAtThatPosition(_jumpPoints[i].transform.position);
            if (canAttack && !_jumpPoints[i].IsOccupied())
            {
                if (Physics.Raycast(_jumpPoints[i].transform.position, Vector3.down, out var hit, 10,
                        environmentLayers))
                {
                    if (_currentPoint != null) 
                        _currentPoint.LeavePoint();
                    _currentPoint = _jumpPoints[i];
                    _currentPoint.TakePoint();
                    JumpOnOtherPosition(hit.point);
                    return;
                }
            }
            await UniTask.NextFrame();
        }
    }

    private async UniTask JumpOnOtherPosition(Vector3 positionToJump)
    {
        _jumping = true;
        OnStartPreparingJump?.Invoke();
        transform.DOLookAt(positionToJump, prepareJumpTime / 2);
        _animator.SetTrigger("Jump");

        await UniTask.Delay(TimeSpan.FromSeconds(prepareJumpTime));
        
        var jumpAngle = Mathf.Abs(MathUtils.CalculateHighLaunchAngle(Vector3.Distance(_position, positionToJump), jumpForce,
            _position.y - positionToJump.y, _gravityMaker.FallingGravity));
        
        var directionToTarget = Vector3.Normalize(positionToJump - _position);
        directionToTarget.y = 0;

        var axis = Vector3.Cross(directionToTarget, Vector3.up);
        var rotation = Quaternion.AngleAxis(jumpAngle, axis);

        var jumpDirection = rotation * directionToTarget;
        _mover.SetVelocity(jumpDirection * jumpForce);
        CheckForGrounded();
    }

    private async UniTask CheckForGrounded()
    {
        await UniTask.Delay(200);

        while (!_mover.IsGrounded())
        {
            await UniTask.Delay(100);
        }

        _jumping = false;
    }


    private void HandleStartChargingGrenadeAttack()
    {
        RotateHeadToRightAngleBeforeShoot();
    }

    private async UniTask RotateHeadToRightAngleBeforeShoot()
    {
        rotationTarget.LookAt(_target);

        await UniTask.Delay(TimeSpan.FromSeconds(_rangeAttackController.GetChargingTime() - timeBeforeShootToRotateHead));

        /*if (_jumping)
        {
            rotationTarget.LookAt(_target);
            return;
        }

        var pos = rotationTarget.position;
        var targetPos = _target.position;
        var distanceToTarget = Vector3.Distance(pos, targetPos);

        var launchAngle = MathUtils.CalculateLowLaunchAngle(distanceToTarget, _grenadeAbility.GetThrowPower(), pos.y - targetPos.y,
            Physics.gravity.y);

        var mover = _target.GetComponentInParent<IMover>();
        if (mover != null)
        {

            var moverVelocity = mover.GetVelocity();
            moverVelocity.y = 0;
            targetPos += moverVelocity * MathUtils.CalculateFlightTime(pos, targetPos, launchAngle * Mathf.Deg2Rad,
                _grenadeAbility.GetThrowPower(), Physics.gravity.y);

            distanceToTarget = Vector3.Distance(pos, targetPos);
            launchAngle = MathUtils.CalculateLowLaunchAngle(distanceToTarget, _grenadeAbility.GetThrowPower(), pos.y - targetPos.y,
                Physics.gravity.y);
        }*/

        rotationTarget.LookAt(_target);
        //rotationTarget.eulerAngles = new Vector3(launchAngle, rotationTarget.rotation.eulerAngles.y, 0);
    }

    private void HandlePerformingGrenadeAttack()
    {
        
    }

    public override bool CanAttack()
    {
        return _targetDetected && !_dead && !Physics.Raycast(rotationTarget.position, _target.position - rotationTarget.position,
            Vector3.Distance(rotationTarget.position, _target.position), environmentLayers);
    }

    private bool CanAttackAtThatPosition(Vector3 position)
    {
        return _targetDetected && !_dead && !Physics.Raycast(position, _target.position - position,
            Vector3.Distance(position, _target.position), environmentLayers);
    }
}
