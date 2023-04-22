using System;
using NTC.Global.Cache;
using NTC.Global.Pool;
using UnityEngine;
using UnityEngine.VFX;

public class ExplosionMakerVisual : MonoCache
{
    [SerializeField] private VisualEffect explosionEffect;
    [SerializeField] private Color normalColor, bigColor;

    private IMakeExplosion _explosionMaker;

    private void Awake()
    {
        _explosionMaker = Get<IMakeExplosion>();
    }
    
    protected override void OnEnabled()
    {
        _explosionMaker.OnExplosionWithRadius += HandleExplosion;
        _explosionMaker.OnBigExplosionWithRadius += HandleBigExplosion;
    }

    protected override void OnDisabled()
    {
        _explosionMaker.OnExplosionWithRadius -= HandleExplosion;
        _explosionMaker.OnBigExplosionWithRadius -= HandleBigExplosion;
    }

    private void HandleExplosion(float radius)
    {
        var particle = NightPool.Spawn(explosionEffect, transform.position);
        
        particle.SetFloat("Radius", radius);
        particle.SetVector4("Color", normalColor);
        particle.Play();
    }

    private void HandleBigExplosion(float radius)
    {
        var particle = NightPool.Spawn(explosionEffect, transform.position);
        
        particle.SetFloat("Radius", radius);
        particle.SetVector4("Color", bigColor);
        particle.Play();
    }
}
