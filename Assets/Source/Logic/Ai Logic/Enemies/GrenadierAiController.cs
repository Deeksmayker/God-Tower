using System;
using DG.Tweening;
using NTC.Global.Cache;
using UnityEngine;

public class GrenadierAiController : MonoCache, IAiController
{
    [SerializeField] private LayerMask layersToAttack;
    [SerializeField] private LayerMask environmentLayers;
    [SerializeField] private Transform rotationTarget;
    [SerializeField] private float timeBeforeShootToRotateHead = 0.3f;
    
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

    private void HandleStartChargingGrenadeAttack()
    {
        rotationTarget.DOLookAt(_target.position, timeBeforeShootToRotateHead);
    }

    private void HandlePerformingGrenadeAttack()
    {
        
    }

    private void HandleDying()
    {
        
    }

    public bool CanAttack()
    {
        return !Physics.Raycast(transform.position, _target.position - _position,
            Vector3.Distance(_position, _target.position), environmentLayers);
    }
}
