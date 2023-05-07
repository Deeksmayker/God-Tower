using System;

public interface IAiController
{
    public abstract void SetTargetDetected(bool value);
    
    public abstract bool CanAttack();
}
