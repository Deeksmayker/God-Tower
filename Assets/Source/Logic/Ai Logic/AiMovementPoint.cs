using System;
using NTC.Global.Cache;
using UnityEngine;

public class AiMovementPoint : MonoCache
{
    private bool _occupied;

    public void TakePoint()
    {
        _occupied = true;
    }

    public void LeavePoint()
    {
        _occupied = false;
    }

    public bool IsOccupied() => _occupied;
}
