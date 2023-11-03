using NTC.Global.Cache;
using System;
using UnityEngine;

public class PlayerHealthHandler : MonoCache, IHealthHandler
{
    [SerializeField] private float health;

    private float _maxHealth;

    public event Action OnHit;
    public event Action<float> OnHealthChanged;
    public event Action OnHealthAdd;
    public event Action OnDied;

    private void Awake()
    {
        _maxHealth = health;
    }

    protected override void OnEnabled()
    {
        var hitTaker = GetComponentInChildren<ITakeHit>();
        hitTaker.OnTakeHit += HandleTakeHit;
    }

    protected override void OnDisabled()
    {
        var hitTaker = GetComponentInChildren<ITakeHit>();
        hitTaker.OnTakeHit -= HandleTakeHit;
    }

    private void HandleTakeHit(float damage)
    {
        health = Mathf.Clamp(health - damage, 0, _maxHealth);

        OnHit?.Invoke();
        OnHealthChanged?.Invoke(health);

        if (health <= 0)
        {
            OnDied?.Invoke();
        }
    }

    public void Die(bool order = false)
    {
        throw new NotImplementedException();
    }

    public void ChangeHealth(float changeValue)
    {
        throw new NotImplementedException();
    }

    public void SetHealth(float value)
    {
        throw new NotImplementedException();
    }

    public float GetHealth01()
    {
        throw new NotImplementedException();
    }
}