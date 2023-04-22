﻿using System;
using System.Collections.Generic;
using NTC.Global.Cache;
using NTC.Global.Pool;
using UnityEngine;
using UnityEngine.VFX;

public class LaserMakerVisual : MonoCache
{
    [SerializeField] private LaserTrace laserTrace;
    [SerializeField] private VisualEffect hitEffect;

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
        trace.SetPosition(1, _laserMaker.GetStartPoint() + _laserMaker.GetPerformDirection() * hit.distance);
    }

    private void HandleEnvironmentHit(RaycastHit hit)
    {
        var trace = NightPool.Spawn(laserTrace, hit.point);
        var hitPosition = _laserMaker.GetStartPoint() + _laserMaker.GetPerformDirection() * hit.distance;
        trace.SetPosition(0, _laserMaker.GetStartPoint());
        trace.SetPosition(1, hitPosition);

        var effect = NightPool.Spawn(hitEffect, hit.point, Quaternion.LookRotation(Vector3.Reflect(
            Vector3.Normalize(hitPosition - _laserMaker.GetStartPoint()),
            hit.normal)));
    }

    private void HandleMiss()
    {
        var trace = NightPool.Spawn(laserTrace, _laserMaker.GetStartPoint());
        trace.SetPosition(0, _laserMaker.GetStartPoint());
        trace.SetPosition(1, _laserMaker.GetStartPoint() + _laserMaker.GetPerformDirection() * 100);
    }
    
}