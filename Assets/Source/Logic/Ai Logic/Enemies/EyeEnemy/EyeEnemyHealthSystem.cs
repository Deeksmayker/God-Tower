using NTC.Global.Cache;
using System;
using UnityEngine;

public class EyeEnemyHealthSystem : MonoCache, IHealthHandler
{
    [SerializeField] private float health;

    private bool _dead;

    private IMover _mover;

    public event Action OnHit;
    public event Action<float> OnHealthChanged;
    public event Action OnHealthAdd;
    public event Action OnDied;

    private void Awake()
    {
        _mover = Get<IMover>();
    }

    protected override void OnEnabled()
    {
        var damagebles = GetComponentsInChildren<ITakeHit>();
        for (var i = 0; i < damagebles.Length; i++)
        {
            damagebles[i].OnTakeHitWithDescription += HandleHit;
        }
    }

    protected override void OnDisabled()
    {
        var damagebles = GetComponentsInChildren<ITakeHit>();
        for (var i = 0; i < damagebles.Length; i++)
        {
            damagebles[i].OnTakeHitWithDescription -= HandleHit;
        }
    }


    private void HandleHit(float damage, Vector3 attackerPos, string attackName)
    {
        if (_dead)
            return;

        Log("ah hit " + damage);
        ChangeHealth(damage);
        _mover.SetInputResponse(false);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent<ITakeHit>(out var hitable))
        {
            hitable.TakeHit(1, transform.position, "Angry eye is dying");
        }
    }

    public void ChangeHealth(float changeValue)
    {
        health += changeValue;


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
        _dead = true;
        gameObject.AddComponent<Death>();
    }

    public float GetHealth01()
    {
        throw new NotImplementedException();
    }

    public void SetHealth(float value)
    {
        throw new NotImplementedException();
    }
}
