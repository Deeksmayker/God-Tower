using System;

public interface IHealthHandler
{
    public event Action OnHit;
    public event Action<float> OnHealthChanged;
    public event Action OnHealthAdd;
    public event Action OnStun;
    public event Action OnDied;
    public event Action OnRevive;

    public void HandleHit(float damage);
    public void StartStun();
    public void Die(bool order = false);
    public void Revive();

    public void AddHealth(float addValue);
    public void RemoveHealth(float removeValue);
    public void SetHealth(float value);
    public float GetHealth();
    public float GetReviveTime();
    public float GetCurrentReviveTimer();
    public bool InStun();
}
