using NTC.Global.Cache;
using System;
using UnityEditor.SceneManagement;
using UnityEngine;

public class JumpEnemyHealthSystem : MonoCache, IHealthHandler, IInStun
{
    [SerializeField] private float health;
    [SerializeField] private float stunDuration;

    private float _maxHealth;

    private float _stunTimer;

	private bool _dead;

    public event Action OnHit;
    public event Action<float> OnHealthChanged;
    public event Action OnHealthAdd;
    public event Action OnDied;
    public event Action OnStun;
    public event Action OnRecover;

    private void Awake()
    {
        _maxHealth = health;
    }

    protected override void OnEnabled()
    {
        var hitTaker = GetComponentInChildren<ITakeHit>();
        hitTaker.OnTakeHit += ChangeHealth;
    }

    protected override void OnDisabled()
    {
        GetComponentInChildren<ITakeHit>().OnTakeHit -= ChangeHealth;   
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

    public void ChangeHealth(float changeValue)
    {
        health = Mathf.Clamp(health + changeValue, 0, _maxHealth);

        if (health <= 0)
        {
            Die();
        }

        if (changeValue < 0)
            OnHit?.Invoke();
        else
            OnHealthAdd?.Invoke();
        OnHealthChanged?.Invoke(health);
    }

    public void Die(bool order = false)
    {
		if (_dead) return;
        gameObject.AddComponent<Death>();
        OnDied?.Invoke();
		_dead = true;
    }

    public float GetHealth01()
    {
        return health / _maxHealth;
    }

    public void SetHealth(float value)
    {
        throw new NotImplementedException();
    }

    public void StartStun()
    {
        Log("momma i'm in stun");
        _stunTimer = stunDuration;
        OnStun?.Invoke();
    }
}
