using System;

public interface IHealthHandler
{
    public event Action OnHit;
    public event Action<float> OnHealthChanged;
    public event Action OnHealthAdd;
    public event Action OnDying;
    public event Action OnDied;

    public void HandleHit(float damage);
    public void StartDying();
    public void Die(bool order = false);

    public void AddHealth(float addValue);
    public void RemoveHealth(float removeValue);
    public void SetHealth(float value);
    public float GetHealth();
    public bool IsDead();
}
