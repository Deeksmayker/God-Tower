using System;
using System.Linq;
using NTC.Global.Cache;
using NTC.Global.Pool;
using UnityEngine;
using Zenject;

public class BaseHealthHandler : MonoCache, IHealthHandler, ITrackingGiveAbilityOpportunity
{
    [SerializeField] private bool canDie = true;
    [SerializeField] private bool canStartDying = true;
    [SerializeField] private bool oneHealthDeathProtection;
    [SerializeField] private bool needToRecovery = true;
    [SerializeField] private float health;
    [SerializeField] private float hitsToDie;
    [SerializeField] private float damageImmuneTime = 0.1f;
    [SerializeField] private float stunDuration = 5;
    [SerializeField] private bool healOnRoomCompleted;

    private float _maxHealth;

    private float _timer;
    private float _stunTimer;

    private bool _stunned;
    private bool _dead;

    private ITakeHit[] _hitTakers;

    public event Action OnHit;
    public event Action<float> OnHealthChanged;
    public event Action OnHealthAdd;
    public event Action OnStun;
    public event Action OnDied;
    public event Action OnCanGiveAbility;
    public event Action OnNotCanGiveAbility;
    public event Action OnRevive;

    private void Awake()
    {
        _maxHealth = health;
        
        _hitTakers = GetComponentsInChildren<ITakeHit>();
    }

    protected override void OnEnabled()
    {
        for (var i = 0; i < _hitTakers.Length; i++)
        {
            _hitTakers[i].OnTakeHit += HandleHit;
        }

        if (healOnRoomCompleted)
        {
            RoomDoor.OnRoomCompleted += () => AddHealth(20);
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

        if (needToRecovery && (_stunTimer > 0 || _stunned))
        {
            _stunTimer -= Time.deltaTime;
            if (_stunTimer <= 0)
            {
                Revive();
            }
        }
    }
    
    public void HandleHit(float damage)
    {
        if (_timer > 0)
            return;
        
        OnHit?.Invoke();
        _timer = damageImmuneTime;
        RemoveHealth(damage);
    }

    public void StartStun()
    {
        if (!canStartDying)
            return;
        
        _stunned = true;
        _stunTimer = stunDuration;
        OnStun?.Invoke();
        OnCanGiveAbility?.Invoke();
    }

    public void Die(bool order = false)
    {
        if (!canDie && !order || _dead)
            return;

        _dead = true;
        OnDied?.Invoke();
        Destroy(gameObject);
    }

    public void Revive()
    {
        _stunned = false;
        health = _maxHealth;
        OnNotCanGiveAbility?.Invoke();
        OnRevive?.Invoke();
    }

    public void AddHealth(float addValue = 1)
    {
        SetHealth(health + _maxHealth / hitsToDie);
        health = Mathf.Clamp(health, 0, _maxHealth);
        OnHealthAdd?.Invoke();
    }

    public void RemoveHealth(float removeValue)
    {
        SetHealth(health - _maxHealth / hitsToDie);
        if (health <= 0)
        {
            StartStun();
        }

        if (health <= 0 && canDie && !canStartDying)
        {
            Die();
        }
    }

    public void SetHealth(float value)
    {
        if (oneHealthDeathProtection && health > 1 && value <= 0)
            value = 1;
        health = value;
        OnHealthChanged?.Invoke(health);
    }

    public void SetNeedRecovery(bool value)
    {
        needToRecovery = value;
    }

    public void SetCanStartDying(bool value)
    {
        canStartDying = value;
    }

    public float GetHealth()
    {
        return health;
    }

    public bool InStun() => _stunned;

    public float GetReviveTime()
    {
        return stunDuration;
    }

    public float GetCurrentReviveTimer()
    {
        return _stunTimer;
    }
}
