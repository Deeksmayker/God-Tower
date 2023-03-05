using System;
using NTC.Global.Cache;
using NTC.Global.Pool;
using UnityEngine;

[RequireComponent(typeof(LineRenderer), typeof(NightPoolDespawner))]
public class LaserTrace : MonoCache
{
    private LineRenderer _lr;

    private void Awake()
    {
        _lr = Get<LineRenderer>();
    }

    protected override void OnEnabled()
    {
        _lr.positionCount = 0;
    }

    public void SetPosition(int linePointIndex, Vector3 position)
    {
        _lr.positionCount++;
        _lr.SetPosition(linePointIndex, position);
    }
}
