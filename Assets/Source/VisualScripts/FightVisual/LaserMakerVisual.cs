using System;
using System.Collections.Generic;
using NTC.Global.Cache;
using NTC.Global.Pool;
using UnityEngine;
using UnityEngine.VFX;

public class LaserMakerVisual : MonoCache
{
    [SerializeField] private VisualEffect traceEffect;
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

    private void HandleHitTakerHit(RaycastHit hit, Vector3 direction)
    {
        var trace = NightPool.Spawn(traceEffect);
        var hitPosition = _laserMaker.GetStartPoint() + direction * hit.distance;
        trace.SetVector3("Pos1", _laserMaker.GetStartPoint());
        trace.SetVector3("Pos2", hitPosition);
        
        var effect = NightPool.Spawn(hitEffect, hit.point, Quaternion.LookRotation(Vector3.Reflect(
            Vector3.Normalize(hitPosition - _laserMaker.GetStartPoint()),
            hit.normal)));
    }

    private void HandleEnvironmentHit(RaycastHit hit, Vector3 direction)
    {
        var trace = NightPool.Spawn(traceEffect);
        var hitPosition = _laserMaker.GetStartPoint() + direction * hit.distance;
        trace.SetVector3("Pos1", _laserMaker.GetStartPoint());
        trace.SetVector3("Pos2", hitPosition);

        var effect = NightPool.Spawn(hitEffect, hit.point, Quaternion.LookRotation(Vector3.Reflect(
            Vector3.Normalize(hitPosition - _laserMaker.GetStartPoint()),
            hit.normal)));
    }

    private void HandleMiss(Vector3 direction)
    {
        var trace = NightPool.Spawn(traceEffect);
        trace.SetVector3("Pos1", _laserMaker.GetStartPoint());
        trace.SetVector3("Pos2", _laserMaker.GetStartPoint() + direction * 100);
    }
    
}