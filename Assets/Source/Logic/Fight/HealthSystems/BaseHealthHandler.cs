using System;
using System.Linq;
using NTC.Global.Cache;
using NTC.Global.Pool;
using UnityEngine;
using Zenject;

public class BaseHealthHandler : MonoCache, IHealthHandler, ITrackingGiveAbilityOpportunity
{
    [SerializeField] private bool canDie = true;
    [SerializeField] private float health;
    [SerializeField] private float damageImmuneTime = 0.1f;
    [SerializeField] private float dyingDuration = 5;

    private float _maxHealth;

    private float _timer;
    private float _dyingTimer;

    private bool _dying;

    private ITakeHit[] _hitTakers;
    private IWeakPoint[] _weakPoints;

    public event Action<float> OnHealthChanged;
    public event Action OnDying;
    public event Action OnDied;
    public event Action OnCanGiveAbility;
    public event Action OnNotCanGiveAbility;

    private void Awake()
    {
        _maxHealth = health;
        
        _hitTakers = GetComponentsInChildren<ITakeHit>();
        _weakPoints = GetComponentsInChildren<IWeakPoint>();
    }

    protected override void OnEnabled()
    {
        for (var i = 0; i < _hitTakers.Length; i++)
        {
            _hitTakers[i].OnTakeHit += HandleHit;
        }
    }

    protected override void OnDisabled()
    {
        for (var i = 0; i < _hitTakers.Length; i++)
        {
            _hitTakers[i].OnTakeHit -= HandleHit;
        }
    }

    protected override void Run()
    {
        if (_timer > 0)
            _timer -= Time.deltaTime;

        if (_dyingTimer > 0 || _dying)
        {
            _dyingTimer -= Time.deltaTime;
            if (_dyingTimer <= 0)
            {
                Die();
            }
        }
    }
    
    public void HandleHit(float damage)
    {
        if (_timer > 0)
            return;
        
        _timer = damageImmuneTime;
        RemoveHealth(damage);
    }

    public void StartDying()
    {
        _dying = true;
        _dyingTimer = dyingDuration;
        OnDying?.Invoke();
        OnCanGiveAbility?.Invoke();
    }

    public void Die()
    {
        if (!canDie)
            return;
        
        OnDied?.Invoke();
        NightPool.Despawn(this);
    }

    public void AddHealth(float addValue)
    {
        SetHealth(health + addValue);
        health = Mathf.Clamp(health, 0, _maxHealth);
    }

    public void RemoveHealth(float removeValue)
    {
        SetHealth(health - removeValue);
        if (health <= 0 && !_dying)
        {
            StartDying();
        }

        if (health < -10)
        {
            Die();
        }
    }

    public void SetHealth(float value)
    {
        health = value;
        OnHealthChanged?.Invoke(health);
    }

    public float GetHealth()
    {
        return health;
    }


}
