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

    private IMeleeAttacker _meleeAttacker;

    private void Awake()
    {
        _meleeAttacker = Get<IMeleeAttacker>();
    }

    protected override void Run()
    {
        if (NeedToAttack())
        {
            StartAttack();
        }
    }

    protected override void OnEnabled()
    {
        if (TryGetComponent<IHealthHandler>(out var health))
        {
            health.OnDying += DisallowAttack;
        }
    }
    
    protected override void OnDisabled()
    {
        if (TryGetComponent<IHealthHandler>(out var health))
        {
            health.OnDying -= DisallowAttack;
        }
    }

    public void StartAttack()
    {
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
