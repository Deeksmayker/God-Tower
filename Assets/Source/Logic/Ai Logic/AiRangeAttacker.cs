using System;
using NTC.Global.Cache;
using UnityEngine;


public class AiRangeAttacker : MonoCache, IAiRangeAttackController
{
    [SerializeField] private LayerMask layersToShoot;
    [SerializeField] private bool tryAttackOnLineOfSight;
    [SerializeField] private float attackRange;

    [SerializeField] private float chargingTime;

    [SerializeField] private Animation attackPreparingAnimation;

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
        if (_aiController.CanAttack() && _rangeAbility.CanPerform())
        {
            _rangeAbility.PerformWithDelay(chargingTime);

            if (attackPreparingAnimation)
                attackPreparingAnimation.Play();
        }
    }

    public float GetChargingTime()
    {
        return chargingTime;
    }

    public bool NeedToAttack()
    {
        return _aiController.CanAttack();
    }
}
