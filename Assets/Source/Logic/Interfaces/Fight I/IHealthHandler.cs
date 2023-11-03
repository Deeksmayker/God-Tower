using System;

public interface IHealthHandler
{
    public event Action OnHit;
    public event Action<float> OnHealthChanged;
    public event Action OnHealthAdd;
    public event Action OnDied;

    public void Die(bool order = false);

    public void ChangeHealth(float changeValue);
    public void SetHealth(float value);
    public float GetHealth01();
}
