using NTC.Global.Cache;
using System;
using UnityEngine;

public class PlayerHealthHandler : MonoCache
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
}