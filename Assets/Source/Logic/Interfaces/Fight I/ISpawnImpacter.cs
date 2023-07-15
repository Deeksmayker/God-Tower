using System;

public interface ISpawnImpacter
{
    public event Action<IImpacter> OnImpacterSpawned;
}