using System;
using System.Collections;
using System.Collections.Generic;
using NTC.Global.Cache;
using NTC.Global.Pool;
using UnityEngine;

public class LightConeSpawner : MonoCache
{
    [SerializeField] private GameObject lightConePrefab;

    private IHealthHandler _healthHandler;

    private void Awake()
    {
        _healthHandler = GetComponentInParent<IHealthHandler>();
    }

    protected override void OnEnabled()
    {
        _healthHandler.OnDied += HandleDie;
    }

    protected override void OnDisabled()
    {
        _healthHandler.OnDied -= HandleDie;
    }

    private void HandleDie()
    {
        NightPool.Spawn(lightConePrefab, transform.position + Vector3.up * 500, lightConePrefab.transform.rotation);
    }
}
