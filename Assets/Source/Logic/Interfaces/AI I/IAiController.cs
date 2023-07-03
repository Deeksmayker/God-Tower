using System;

public interface IAiController
{
    public abstract void SetTargetDetected(bool value);
    public abstract void SetTimeDifficulty01(float value);
    public abstract float GetTimeDifficulty01();
    public abstract bool CanAttack();
}
