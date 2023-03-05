using System;
using System.Collections.Generic;
using NTC.Global.Cache;
using NTC.Global.Pool;
using UnityEngine;

public class LaserMakerVisual : MonoCache
{
    [SerializeField] private LaserTrace laserTrace;

    private float _timer;
    private IMakeLaser _laserMaker;

    private void Awake()
    {
        _laserMaker = Get<IMakeLaser>();
    }

    protected override void OnEnabled()
    {
        _laserMaker.OnHitToHitTaker += HandleHitTakerHit;
        _laserMaker.OnEnvironmentHit += HandleEnvironmentHit;
        _laserMaker.OnMissHit += HandleMiss;
    }

    protected override void OnDisabled()
    {
        _laserMaker.OnHitToHitTaker -= HandleHitTakerHit;
        _laserMaker.OnEnvironmentHit -= HandleEnvironmentHit;
        _laserMaker.OnMissHit -= HandleMiss;
    }

    private void HandleHitTakerHit(RaycastHit hit)
    {
        var trace = NightPool.Spawn(laserTrace, hit.point);
        trace.SetPosition(0, _laserMaker.GetStartPoint());
        trace.SetPosition(1, hit.point);
        Debug.Log("To hit taker");
    }

    private void HandleEnvironmentHit(RaycastHit hit)
    {
        var trace = NightPool.Spawn(laserTrace, hit.point);
        trace.SetPosition(0, _laserMaker.GetStartPoint());
        trace.SetPosition(1, hit.point);
        //place for some particles
    }

    private void HandleMiss()
    {
        var trace = NightPool.Spawn(laserTrace, _laserMaker.GetStartPoint());
        trace.SetPosition(0, _laserMaker.GetStartPoint());
        trace.SetPosition(1, _laserMaker.GetStartPoint() + _laserMaker.GetPerformDirection() * 100);
    }
    
}