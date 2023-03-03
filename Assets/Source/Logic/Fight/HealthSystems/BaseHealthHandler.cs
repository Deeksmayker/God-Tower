using System;
using NTC.Global.Cache;
using NTC.Global.Pool;
using UnityEngine;

public class BaseHealthHandler : MonoCache, IHealthHandler
{
    [SerializeField] private bool canDie = true;
    [SerializeField] private int health;
    [SerializeField] private int weakPointDamageMultiplier = 2;

    private int _maxHealth;
    
    private ITakeHit[] _hitTakers;
    private IWeakPoint[] _weakPoints;
    
    public event Action OnDie;

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
        
        for (var i = 0; i < _weakPoints.Length; i++)
        {
            _weakPoints[i].OnWeakPointHit += HandleWeakPointHit;
        }
    }

    protected override void OnDisabled()
    {
        for (var i = 0; i < _hitTakers.Length; i++)
        {
            _hitTakers[i].OnTakeHit -= HandleHit;
        }
        
        for (var i = 0; i < _weakPoints.Length; i++)
        {
            _weakPoints[i].OnWeakPointHit -= HandleWeakPointHit;
        }
    }
    
    public void HandleHit(int damage)
    {
        RemoveHealth(damage);
    }

    public void HandleWeakPointHit(int baseDamage)
    {
        RemoveHealth(baseDamage * weakPointDamageMultiplier);
    }

    public void Die()
    {
        OnDie?.Invoke();
        NightPool.Despawn(this);
    }

    public void AddHealth(int addValue)
    {
        health += addValue;
        health = Mathf.Clamp(health, 0, _maxHealth);
    }

    public void RemoveHealth(int removeValue)
    {
        health -= removeValue;
        if (health <= 0)
        {
            Die();
        }
    }

    public void SetHealth(int value)
    {
        health = value;
    }

    public int GetHealth()
    {
        return health;
    }
}
