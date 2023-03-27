using System;
using NTC.Global.Cache;
using UnityEngine;


public class AiRangeAttacker : MonoCache, IAiRangeAttackController
{
    [SerializeField] private LayerMask layersToShoot;
    [SerializeField] private bool tryAttackOnLineOfSight;
    [SerializeField] private float attackRange;

    [SerializeField] private float chargingTime;

    private bool _canAttack = true;
    
    private IAiController _aiController;
    private IActiveAbility _rangeAbility;

    private void Awake()
    {
        _aiController = Get<IAiController>();
        _rangeAbility = GetComponentInChildren<IActiveAbility>();
    }

    protected override void Run()
    {
        if (NeedToAttack())
        {
            TryAttack();
        }
    }

    public void TryAttack()
    {
        if (_aiController.CanAttack())
        {
            _rangeAbility.PerformWithDelay(chargingTime);
        }
    }

    public void AllowAttack()
    {
        _canAttack = true;
    }

    public void DisallowAttack()
    {
        _canAttack = false;
    }

    public float GetChargingTime()
    {
        return chargingTime;
    }

    public bool NeedToAttack()
    {
        return _canAttack && Physics.CheckSphere(transform.position, attackRange, layersToShoot);
    }
}
