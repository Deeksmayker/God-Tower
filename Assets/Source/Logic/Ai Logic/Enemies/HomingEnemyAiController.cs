using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using NTC.Global.Cache;
using Zenject;

public class HomingEnemyAiController : BaseAiController
{
    [SerializeField] private LayerMask environmentLayers;
    [SerializeField] private LayerMask targetLayers;

    [Header("Default Movement")]
    [SerializeField] private Vector3 strength = Vector3.one;
    [SerializeField] private int vibrato = 10;

    [Header("Flying")]
    [SerializeField] private float flySpeed;
    
    [Header("Timers")]
    [SerializeField] private float timeChangeLocation = 5;
    [SerializeField] private float cantAttackTimeToChangeLocation = 1;

    private float _timeOnPosition;
    private float _cantAttackTime;

    private Vector3 _position;
    private Vector3 _targetPosition;
    
    private Transform _target;

    private bool _canAttack;
    private bool _moving;
    private bool _dead;

    private HomingEnemyMovePoint _currentPoint;
    [Inject] private List<HomingEnemyMovePoint> _movePoints;

    private IMover _mover;
    private GravityMaker _gravityMaker;
    private IHealthHandler _healthHandler;

    private void Awake()
    {
        _mover = Get<IMover>();
        _gravityMaker = Get<GravityMaker>();
        _gravityMaker.enabled = false;

        _healthHandler = Get<IHealthHandler>();
        MakeDefaultMovement();
    }

    protected override void Start()
    {
        base.Start();
        _mover.SetInputResponse(false);
        
        _target = Physics.OverlapSphere(transform.position, 1000, targetLayers)[0].transform;
    }

    protected override void OnEnabled()
    {
        _healthHandler.OnDying += HandleDying;
    }

    
    protected override void OnDisabled()
    {
        _healthHandler.OnDying -= HandleDying;
    }
    
    protected override void Run()
    {
        _position = transform.position;
        _targetPosition = _target.position;
        
        if (_dead)
            return;

        _canAttack = _targetDetected && !_dead && LineOfSightChecker.CanSeeTarget(_position, _targetPosition, environmentLayers);
        //Debug.Log(_canAttack);
        
        transform.LookAt(_targetPosition);
        
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

    private void TryFindPositionToMove()
    {
        for (var i = 0; i < _movePoints.Count; i++)
        {
            var pointPosition = _movePoints[i].transform.position;
            var canAttack = LineOfSightChecker.CanSeeTarget(pointPosition, _targetPosition,
                environmentLayers);
            if (canAttack && !_movePoints[i].IsOccupied())
            {
                if (_currentPoint != null)
                    _currentPoint.LeavePoint();
                _currentPoint = _movePoints[i];
                _currentPoint.TakePoint();
                MoveToOtherPosition(pointPosition);
                return;
            }
        }
    }

    private async UniTask MoveToOtherPosition(Vector3 newPosition)
    {
        _moving = true;
        var distanceToNewPos = Vector3.Distance(_position, newPosition);

        while (distanceToNewPos > 1)
        {
            var direction = Vector3.Normalize(newPosition - _position);
            _mover.SetVelocity(direction * flySpeed);
            distanceToNewPos = Vector3.Distance(_position, newPosition);
            await UniTask.NextFrame();
        }
        
        MakeDefaultMovement();
        _mover.SetVelocity(Vector3.zero);
        _moving = false;
    }

    private void MakeDefaultMovement()
    {
        transform.DOShakePosition(0.5f, strength, vibrato, 90, false, false, ShakeRandomnessMode.Harmonic)
            .OnComplete(() =>
            {
                if (!_dead && !_moving)
                    MakeDefaultMovement();
            });
    }

    private void HandleDying()
    {
        _dead = true;
        _canAttack = false;
        _gravityMaker.enabled = true;
    }

    public override bool CanAttack()
    {
        return _canAttack;
    }
}