using System;
using NTC.Global.Cache;
using NTC.Global.Pool;
using UnityEngine;

public class BaseHealthHandler : MonoCache, IHealthHandler
{
    [SerializeField] private bool canDie = true;
    [SerializeField] private int health;
    [SerializeField] private int weakPointDamageMultiplier = 2;
    [SerializeField] private float damageImmuneTime = 0.1f;

    private int _maxHealth;

    private float _timer;
    
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

    protected override void Run()
    {
        if (_timer > 0)
            _timer -= Time.deltaTime;
    }
    
    public void HandleHit(int damage)
    {
        if (_timer > 0)
            return;
        Debug.Log("NormalHit");
        _timer = damageImmuneTime;
        RemoveHealth(damage);
    }

    public void HandleWeakPointHit(int baseDamage)
    {
        if (_timer > 0)
            return;
        Debug.Log("Weak point hit");
        _timer = damageImmuneTime;
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
