using System;
using System.Collections.Generic;
using DG.Tweening;
using NTC.Global.Cache;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class GrenadierAiController : MonoCache, IAiController
{
    [SerializeField] private LayerMask layersToAttack;
    [SerializeField] private LayerMask environmentLayers;
    [SerializeField] private Transform rotationTarget;
    
    [Header("Timers")]
    [SerializeField] private float timeBeforeShootToRotateHead = 0.3f;
    [SerializeField] private float timeChangeLocation = 5;
    [SerializeField] private float cantAttackTimeToChangeLocation = 1;

    [Inject] private List<GrenadierJumpPoint> _jumpPoints;

    private GrenadierJumpPoint _currentPoint;
    
    private float _timeOnLocation;
    private float _cantAttackTime;
    
    private bool _jumping;
    
    private Transform _target;
    
    private Vector3 _position;
    
    private IAiMovementController _movementController;
    private IAiRangeAttackController _rangeAttackController;
    private GrenadeAbility _grenadeAbility;

    private void Awake()
    {
        _movementController = Get<IAiMovementController>();
        _rangeAttackController = Get<IAiRangeAttackController>();
        _grenadeAbility = GetComponentInChildren<GrenadeAbility>();
        
        _target = Physics.OverlapSphere(transform.position, 1000, layersToAttack)[0].transform;
    }
    
    protected override void OnEnabled()
    {
        _grenadeAbility.OnStartHolding += HandleStartChargingGrenadeAttack;
        _grenadeAbility.OnPerform += HandlePerformingGrenadeAttack;
        
        if (TryGetComponent<IHealthHandler>(out var health))
        {
            health.OnDying += HandleDying;
        }
    }

    protected override void OnDisabled()
    {
        _grenadeAbility.OnStartHolding -= HandleStartChargingGrenadeAttack;
        _grenadeAbility.OnPerform -= HandlePerformingGrenadeAttack;
        
        if (TryGetComponent<IHealthHandler>(out var health))
        {
            health.OnDying -= HandleDying;
        }
    }

    protected override void Run()
    {
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
    }

    private void JumpOnOtherPosition(Vector3 positionToJump)
    {
        _jumping = true;
    }

    private void TryFindPositionToJump()
    {
        for (var i = 0; i < _jumpPoints.Count; i++)
        {
            if (CanAttackAtThatPosition(_jumpPoints[i].transform.position) && _currentPoint != null &&
                !_jumpPoints[i].Equals(_currentPoint))
            {
                JumpOnOtherPosition(_jumpPoints[i].transform.position);
            }
        }
    }

    private void HandleStartChargingGrenadeAttack()
    {
        var pos = rotationTarget.position;
        var targetPos = _target.position;
        var distanceToTarget = Vector3.Distance(pos, targetPos);

        var launchAngle = MathUtils.CalculateLaunchAngle(distanceToTarget, _grenadeAbility.GetThrowPower(), pos.y - targetPos.y,
            Physics.gravity.y);

        var mover = _target.GetComponentInParent<IMover>();
        if (mover != null)
        {
            
            var moverVelocity = mover.GetVelocity();
            moverVelocity.y = 0;
            targetPos += moverVelocity * MathUtils.CalculateFlightTime(pos, targetPos, launchAngle * Mathf.Deg2Rad,
                _grenadeAbility.GetThrowPower(), Physics.gravity.y);

            distanceToTarget = Vector3.Distance(pos, targetPos);
            launchAngle = MathUtils.CalculateLaunchAngle(distanceToTarget, _grenadeAbility.GetThrowPower(), pos.y - targetPos.y,
                Physics.gravity.y);
        }

        rotationTarget.LookAt(targetPos);
        rotationTarget.eulerAngles = new Vector3(launchAngle, rotationTarget.rotation.eulerAngles.y, 0);
    }

    private void HandlePerformingGrenadeAttack()
    {
        
    }

    private void HandleDying()
    {
        
    }

    public bool CanAttack()
    {
        return !Physics.Raycast(rotationTarget.position, _target.position - rotationTarget.position,
            Vector3.Distance(rotationTarget.position, _target.position), environmentLayers);
    }

    private bool CanAttackAtThatPosition(Vector3 position)
    {
        return !Physics.Raycast(position, _target.position - position,
            Vector3.Distance(position, _target.position), environmentLayers);
    }
}
