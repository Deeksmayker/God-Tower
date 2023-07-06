using System;
using NTC.Global.Cache;
using UnityEngine;


public class AiRangeAttacker : MonoCache, IAiRangeAttackController
{
    [SerializeField] private LayerMask layersToShoot;
    [SerializeField] private bool tryAttackOnLineOfSight;
    [SerializeField] private float attackRange;

    [SerializeField] private float baseReloadTime, maxReloadTime;

    [SerializeField] private Animator _animator;

    private float _currentCooldown;
    private float _cooldownTimer;

    private IAiController _aiController;
    private DefaultActiveAbility _rangeAbility;

    private void Awake()
    {
        _aiController = Get<IAiController>();

        _rangeAbility = GetComponentInChildren<DefaultActiveAbility>();

        _currentCooldown = baseReloadTime;
        _cooldownTimer = _currentCooldown;
    }

    protected override void Run()
    {
        _currentCooldown = Mathf.Lerp(baseReloadTime, maxReloadTime, Mathf.Pow(_aiController.GetTimeDifficulty01(), 1.5f));
        //Debug.Log(_cooldownTimer);
        if (NeedToAttack())
        {
            _cooldownTimer -= Time.deltaTime;
            TryAttack();
        }
    }

    public void TryAttack()
    {
        if (_aiController.CanAttack())
        {
            if (_cooldownTimer <= 0)
            {
                _rangeAbility.PerformAbility(1);
                _cooldownTimer = _currentCooldown;
                if (_animator)
                {
                    _animator.SetBool("Prepare", false);
                }
            }

            else if (_cooldownTimer / _currentCooldown <= 0.5f)
            {
                if (_animator)
                {
                    _animator.SetFloat("Speed", 1f / (_currentCooldown / 2f));
                    _animator.SetBool("Prepare", true);
                }
            }
        }
    }

    public float GetCurrentCooldown()
    {
        return _currentCooldown;
    }

    public float GetCooldownTimer() => _cooldownTimer;

    public bool NeedToAttack()
    {
        return _aiController.CanAttack();
    }
}
