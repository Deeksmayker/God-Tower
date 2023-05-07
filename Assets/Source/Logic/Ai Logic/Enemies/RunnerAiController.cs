﻿using System;
using DG.Tweening;
using NTC.Global.Cache;
using Unity.Mathematics;
using UnityEngine;

public class RunnerAiController : BaseAiController
{
    [SerializeField] private LayerMask layersToAttack;
    [SerializeField] private LayerMask environmentLayers;
    [SerializeField] private Transform rotationTarget;
    [SerializeField] private float timeBeforeShootToRotateHead = 0.3f;

    private bool _attacking;
    private bool _lookedOnTarget;
    private bool _dead;

    private float _rangeChargingTimer;

    private Transform _target;
    private Vector3 _position;
    
    private IAiMeleeAttackController _meleeAttackController;
    private IMeleeAttacker _meleeAttacker;
    private IAiMovementController _movementController;
    private IAiRangeAttackController _rangeAttackController;
    private IActiveAbility _rangeAbility;

    private void Awake()
    {
        _meleeAttacker = Get<IMeleeAttacker>();
        _meleeAttackController = Get<IAiMeleeAttackController>();
        _movementController = Get<IAiMovementController>();
        _rangeAttackController = Get<IAiRangeAttackController>();
        _rangeAbility = GetComponentInChildren<IActiveAbility>();

        _target = Physics.OverlapSphere(transform.position, 1000, layersToAttack)[0].transform;
    }

    protected override void OnEnabled()
    {
        _meleeAttacker.OnStartPreparingAttack += HandleStartMeleeAttack;
        _meleeAttacker.OnEndAttack += HandleEndMeleeAttack;
        _rangeAbility.OnStartHolding += HandleStartChargingRangeAttack;
        _rangeAbility.OnPerform += HandlePerformingRangeAttack;
        
        if (TryGetComponent<IHealthHandler>(out var health))
        {
            health.OnDying += HandleDying;
        }
    }

    protected override void OnDisabled()
    {
        _meleeAttacker.OnStartPreparingAttack -= HandleStartMeleeAttack;
        _meleeAttacker.OnEndAttack -= HandleEndMeleeAttack;
        _rangeAbility.OnStartHolding -= HandleStartChargingRangeAttack;
        _rangeAbility.OnPerform -= HandlePerformingRangeAttack;
        
        if (TryGetComponent<IHealthHandler>(out var health))
        {
            health.OnDying -= HandleDying;
        }
    }

    protected override void Run()
    {
        _position = transform.position;
        
        if (!_attacking)
            rotationTarget.LookAt(_target);
        
        if (_rangeChargingTimer > 0)
        {
            _rangeChargingTimer -= Time.deltaTime;
            if (!_lookedOnTarget && _rangeChargingTimer <= timeBeforeShootToRotateHead)
            {
                rotationTarget.transform.DOLookAt(_target.position, 0.1f);
                _lookedOnTarget = true;
            }
        }
    }

    public override void SetTargetDetected(bool value)
    {
        base.SetTargetDetected(value);

        if (value)
        {
            _movementController.ResumeMoving();
        }
        else
        {
            _movementController.Stop();
        }
        _movementController.SetRotationToVelocityVector(value);
    }

    private void HandleStartMeleeAttack()
    {
        _movementController.Stop();
        _movementController.SetRotationToVelocityVector(false);
        _attacking = true;
        //Physics.OverlapSphereNonAlloc(transform.position, 100, _target, layersToAttack);
        transform.DOLookAt(_target.position, 0.1f);
    }

    private void HandleEndMeleeAttack()
    {
        if (_dead)
            return;
        
        _movementController.ResumeMoving();
        _movementController.SetRotationToVelocityVector(true);
        _attacking = false;
    }
    
    private void HandleStartChargingRangeAttack()
    {
        _movementController.Stop();
        _movementController.SetRotationToVelocityVector(false);
        _attacking = true;
        _rangeChargingTimer = _rangeAttackController.GetChargingTime();
        //Physics.OverlapSphereNonAlloc(transform.position, 100, _target, layersToAttack);
        rotationTarget.transform.DOLookAt(_target.position, 0.05f);
    }

    private void HandlePerformingRangeAttack()
    {
        if (_dead)
            return;
        
        _movementController.ResumeMoving();
        _movementController.SetRotationToVelocityVector(true);
        rotationTarget.transform.DORotate(transform.forward, 0.1f);
        _attacking = false;
        _lookedOnTarget = false;
    }

    private void HandleDying()
    {
        _dead = true;
    }

    public override bool CanAttack()
    {
        return _targetDetected && !_dead && !_attacking && !Physics.Raycast(transform.position, _target.position - _position,
            Vector3.Distance(_position, _target.position), environmentLayers);
    }
}
