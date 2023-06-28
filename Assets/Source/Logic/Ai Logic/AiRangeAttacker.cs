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
    private DefaultActiveAbility _rangeAbility;

    private void Awake()
    {
        _aiController = Get<IAiController>();
        _rangeAbility = GetComponentInChildren<DefaultActiveAbility>();
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
            _rangeAbility.PerformWithDelay(chargingTime, 1);

            if (attackPreparingAnimation)
            {
                attackPreparingAnimation.Play();
            }
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
