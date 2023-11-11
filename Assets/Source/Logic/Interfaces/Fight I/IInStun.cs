using System;

public interface IInStun
{
    public event Action OnStun;
    public event Action OnRecover;

    public void StartStun();
}