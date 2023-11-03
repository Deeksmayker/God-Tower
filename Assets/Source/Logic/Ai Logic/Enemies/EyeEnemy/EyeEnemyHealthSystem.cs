using NTC.Global.Cache;
using System;
using UnityEngine;

public class EyeEnemyHealthSystem : MonoCache, IHealthHandler
{
    [SerializeField] private float health;

    public event Action OnHit;
    public event Action<float> OnHealthChanged;
    public event Action OnHealthAdd;
    public event Action OnDied;

    public void ChangeHealth(float changeValue)
    {
        throw new NotImplementedException();
    }

    public void Die(bool order = false)
    {
        throw new NotImplementedException();
    }

    public float GetHealth01()
    {
        throw new NotImplementedException();
    }

    public void HandleHit(float damage)
    {
        throw new NotImplementedException();
    }

    public void SetHealth(float value)
    {
        throw new NotImplementedException();
    }
}