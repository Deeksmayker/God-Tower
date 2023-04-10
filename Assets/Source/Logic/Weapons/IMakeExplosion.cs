using System;

public interface IMakeExplosion
{
    public event Action OnBigExplosion;
    public event Action OnExplosion;
}