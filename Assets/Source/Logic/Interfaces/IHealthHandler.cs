using System;

public interface IHealthHandler
{
    public event Action OnDie;

    public void HandleHit(int damage);
    public void HandleWeakPointHit(int baseDamage);
    public void Die();

    public void AddHealth(int addValue);
    public void RemoveHealth(int removeValue);
    public void SetHealth(int value);
    public int GetHealth();
}
