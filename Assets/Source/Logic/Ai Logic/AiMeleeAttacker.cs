using System;
using NTC.Global.Cache;
using UnityEngine;

[RequireComponent(typeof(IMeleeAttacker))]
public class AiMeleeAttacker : MonoCache, IAiMeleeAttackController
{
    [SerializeField] private LayerMask layersToAttack;
    [SerializeField] private bool attackOnCloseEnough = true;
    [SerializeField] private float rangeToAttack;

    private bool _canAttack = true;

    private IAiController _aiController;
    private IMeleeAttacker _meleeAttacker;

    private void Awake()
    {
        _meleeAttacker = Get<IMeleeAttacker>();
        _aiController = Get<IAiController>();
    }

    protected override void Run()
    {
        if (NeedToAttack())
        {
            TryAttack();
        }
        else
        {
            _meleeAttacker.SetInput(false);
        }
    }

    protected override void OnEnabled()
    {
        if (TryGetComponent<IHealthHandler>(out var health))
        {
            health.OnStun += DisallowAttack;
        }
    }
    
    protected override void OnDisabled()
    {
        if (TryGetComponent<IHealthHandler>(out var health))
        {
            health.OnStun -= DisallowAttack;
        }
    }

    public void TryAttack()
    {
        if (_aiController.CanAttack())
            _meleeAttacker.SetInput(true);
    }

    public void AllowAttack()
    {
        _canAttack = true;
        Get<IMeleeAttacker>().AllowAttack();
    }

    public void DisallowAttack()
    {
        _canAttack = false;
        Get<IMeleeAttacker>().DisallowAttack();
    }

    public bool NeedToAttack()
    {
        return _canAttack && Physics.CheckSphere(transform.position, rangeToAttack, layersToAttack);
    }
}
