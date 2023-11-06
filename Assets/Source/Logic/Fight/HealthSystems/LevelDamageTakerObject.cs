using System;
using NTC.Global.Cache;
using UnityEngine;
using UnityEngine.Events;


public class LevelDamageTakerObject : MonoCache, IHealthHandler
{
    [SerializeField] private float health = 1;
    [SerializeField] private bool needToRecovery;
    [SerializeField] private float recoveryTime = 1f;

    private float _maxHealth;

    private bool _dying;

    private float _timer;
    
    public event Action OnHit;
    public event Action<float> OnHealthChanged;
    public event Action OnHealthAdd;
    public event Action OnStun;
    public event Action OnDied;
    public event Action OnRevive;

    public UnityEvent OnHurt = new();
    public UnityEvent OnRecovery = new();

    private void Awake()
    {
        _maxHealth = health;
    }

    protected override void OnEnabled()
    {
        Get<ITakeHit>().OnTakeHit += HandleHit;
    }
    
    protected override void OnDisabled()
    {
        Get<ITakeHit>().OnTakeHit -= HandleHit;
    }

    protected override void Run()
    {
        if (_timer > 0 && needToRecovery)
        {
            _timer -= Time.deltaTime;

            if (_timer <= 0)
            {
                health = _maxHealth;
                _dying = false;
                OnRecovery.Invoke();
            }
        }
    }
    
    public void HandleHit(float damage)
    {
        health -= damage;
        OnHit?.Invoke();

        if (health <= 0 && !_dying)
            StartStun();
    }

    public void StartStun()
    {
        if (needToRecovery)
            _timer = recoveryTime;
        
        _dying = true;
        OnStun?.Invoke();
        OnHurt.Invoke();
    }

    public void SetNeedToRecovery(bool value)
    {
        needToRecovery = value;
    }

    public void Die(bool order = false)
    {
        throw new NotImplementedException();
    }

    public void AddHealth(float addValue)
    {
        OnHealthAdd?.Invoke();
        throw new NotImplementedException();
    }

    public void RemoveHealth(float removeValue)
    {
        health -= removeValue;
    }

    public void SetHealth(float value)
    {
        health = value;
    }

    public float GetHealth01()
    {
        return health;
    }

    public bool InStun()
    {
        return _dying;
    }

    public void Revive()
    {
        throw new NotImplementedException();
    }

    public float GetReviveTime()
    {
        throw new NotImplementedException();
    }

    public float GetCurrentReviveTimer()
    {
        throw new NotImplementedException();
    }

    public void ChangeHealth(float changeValue)
    {
        throw new NotImplementedException();
    }
}
