﻿using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using NTC.Global.Cache;
using Unity.Mathematics;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

public class RunnerAiController : BaseAiController
{
    [SerializeField] private LayerMask layersToAttack;
    [SerializeField] private LayerMask environmentLayers;
    [SerializeField] private Transform rotationTarget;
    [SerializeField] private float timeBeforeShootToRotateHead = 0.3f;
    [SerializeField] private float moveSpeed = 30f;
    [SerializeField] private float timeChangeLocation = 3f;
    [SerializeField] private float cantAttackTimeToChangeLocation = 1f;

    private bool _canAttack;
    private bool _attacking;
    private bool _dead;
    private bool _moving;
    
    private float _timeOnPosition;
    private float _cantAttackTime;

    private Transform _target;
    private Vector3 _position;
    private Vector3 _targetPosition;
    
    private IAiRangeAttackController _rangeAttackController;
    private IActiveAbility _rangeAbility;
    private IMover _mover;
    
    private HomingEnemyMovePoint _currentPoint;
    [Inject] private List<HomingEnemyMovePoint> _movePoints;

    private void Awake()
    {
        _rangeAttackController = Get<IAiRangeAttackController>();
        _rangeAbility = GetComponentInChildren<IActiveAbility>();
        _mover = Get<IMover>();
        _mover.SetInputResponse(false);

        _target = Physics.OverlapSphere(transform.position, 1000, layersToAttack)[0].transform;
    }

    protected override void OnEnabled()
    {
        _rangeAbility.OnStartHolding += HandleStartChargingRangeAttack;
        _rangeAbility.OnPerform += HandlePerformingRangeAttack;
        
        if (TryGetComponent<IHealthHandler>(out var health))
        {
            health.OnDying += HandleDying;
        }
    }

    protected override void OnDisabled()
    {
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
        _targetPosition = _target.position;
        
        _canAttack = _targetDetected && !_attacking && !_dead && LineOfSightChecker.CanSeeTarget(_position, _targetPosition, environmentLayers);

        if (_dead)
            return;

        if (!_attacking)
            rotationTarget.LookAt(_target);

        if (_moving)
        {
            _timeOnPosition = 0;
            _cantAttackTime = 0;
            return;
        }

        _timeOnPosition += Time.deltaTime;

        if (!CanAttack())
        {
            _cantAttackTime += Time.deltaTime;
        }
        else
        {
            _cantAttackTime = 0;
        }

        if (_timeOnPosition > timeChangeLocation || _cantAttackTime > cantAttackTimeToChangeLocation)
        {
            TryFindPositionToMove();
        }
    }

    public override void SetTargetDetected(bool value)
    {
        base.SetTargetDetected(value);

        _moving = !value;
    }

    private async UniTask TryFindPositionToMove()
    {
        for (var i = 0; i < _movePoints.Count; i++)
        {
            var pointPosition = _movePoints[i].transform.position;
            var canAttack = LineOfSightChecker.CanSeeTarget(pointPosition, _targetPosition,
                environmentLayers);

            var direction = Vector3.Normalize(pointPosition - _position);
            direction.y = 0;
            
            if (IsWallInDirection(direction, pointPosition))
                continue;
            
            if (canAttack && !_movePoints[i].IsOccupied())
            {
                if (_currentPoint)
                    _currentPoint.LeavePoint();
                _currentPoint = _movePoints[i];
                _currentPoint.TakePoint();
                MoveToOtherPosition(pointPosition);
                return;
            }

            await UniTask.NextFrame();
        }

        var timer = 0f;

        while (timer < 1f)
        {
            timer += Time.deltaTime;
            var index = Random.Range(0, _movePoints.Count);
            var randomMovePoint = _movePoints[index].transform.position;

            var direction = Vector3.Normalize(randomMovePoint - _position);

            if (_movePoints[index].IsOccupied() || IsWallInDirection(direction, randomMovePoint))
            {
                await UniTask.NextFrame();
                continue;
            }
            
            if (_currentPoint)
                _currentPoint.LeavePoint();
            _currentPoint = _movePoints[index];
            _currentPoint.TakePoint();
            MoveToOtherPosition(randomMovePoint);
            return;
        }

        _moving = false;
    }

    private bool IsWallInDirection(Vector3 direction, Vector3 pointPosition)
    {
        return Physics.Raycast(_position, direction, out var envHit, Vector3.Distance(_position, pointPosition),
                   environmentLayers)
               && MathUtils.CompareNumsApproximately(envHit.normal.y, 0, 0.4f);
    }

    private async UniTask MoveToOtherPosition(Vector3 newPosition)
    {
        _moving = true;
        var distanceToNewPos = Vector3.Distance(_position, newPosition);
        var timer = 0f;

        while (distanceToNewPos > 1 && timer < 5f)
        {
            var direction = Vector3.Normalize(newPosition - _position);
            _mover.SetHorizontalVelocity(direction * moveSpeed);
            distanceToNewPos = Vector3.Distance(_position, newPosition);
            timer += Time.deltaTime;
            await UniTask.NextFrame();
        }
        
        _mover.SetHorizontalVelocity(Vector3.zero);
        _moving = false;
    }

    private void HandleStartChargingRangeAttack()
    {
        _attacking = true;
        RotateHeadBeforeShoot();
    }

    private async UniTask RotateHeadBeforeShoot()
    {
        rotationTarget.LookAt(_target);
        
        await UniTask.Delay(
            TimeSpan.FromSeconds(_rangeAttackController.GetChargingTime() - timeBeforeShootToRotateHead));
        
        rotationTarget.LookAt(_target);
    }

    private void HandlePerformingRangeAttack()
    {
        if (_dead)
            return;
        
        _attacking = false;
    }

    private void HandleDying()
    {
        _dead = true;
    }

    public override bool CanAttack()
    {
        return _canAttack;
    }
}
