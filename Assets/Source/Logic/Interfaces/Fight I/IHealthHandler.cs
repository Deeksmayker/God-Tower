using System;

public interface IHealthHandler
{
    public event Action OnDying;
    public event Action OnDied;

    public void HandleHit(int damage);
    public void HandleWeakPointHit(int baseDamage);
    public void StartDying();
    public void Die();

    public void AddHealth(int addValue);
    public void RemoveHealth(int removeValue);
    public void SetHealth(int value);
    public int GetHealth();
}
