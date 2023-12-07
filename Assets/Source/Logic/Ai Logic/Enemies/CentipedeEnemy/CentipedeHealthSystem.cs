using UnityEngine;
using NTC.Global.Cache;
using System;

public class CentipedeHealthSystem : MonoCache, IHealthHandler, IInStun
{
    public event Action OnHit;
    public event Action<float> OnHealthChanged;
    public event Action OnHealthAdd;
    public event Action OnDied;
    public event Action OnStun;
    public event Action OnRecover;

    [SerializeField] private float health;
    [SerializeField] private float stunDuration;

    private float _maxHealth;
    private float _stunTimer;

	private bool _dead;

    private void Awake()
    {
        _maxHealth = health;
    }

    protected override void OnEnabled()
    {
    }

    protected override void OnDisabled()
    {
        var hitTakers = GetComponentsInChildren<ITakeHit>();
        for (var i = 0; i < hitTakers.Length; i++)
        {
            hitTakers[i].OnTakeHit -= ChangeHealth;
        }
    }

    public void SubscribeToHitTakers()
    {
        var hitTakers = GetComponentsInChildren<ITakeHit>();
        for (var i = 0; i < hitTakers.Length; i++)
        {
            hitTakers[i].OnTakeHit += ChangeHealth;
        }
    }

    protected override void Run()
    {
        if (_stunTimer > 0)
        {
            _stunTimer -= Time.deltaTime;
            if (_stunTimer <= 0)
                OnRecover?.Invoke();
        }
    }

    [ContextMenu("Stun this thing")]
    public void StartStun()
    {
        _stunTimer = stunDuration;
        OnStun?.Invoke();
    }


    public void ChangeHealth(float changeValue)
    {
        health += changeValue;

        if (health <= 0)
        {
            Die();
        }
    }

    public void Die(bool order = false)
    {
		if (_dead) return;

		_dead = true;
        OnDied?.Invoke();
    }

    public void SetHealth(float value)
    {
        health = value;
    }
    public float GetHealth01() => health / _maxHealth;

}
