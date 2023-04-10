using System;
using NTC.Global.Cache;
using NTC.Global.Pool;
using UnityEngine;

public class ExplosionMakerVisual : MonoCache
{
    [SerializeField] private ParticleSystem normalParticles;
    [SerializeField] private ParticleSystem bigParticles;

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
        var particle = NightPool.Spawn(normalParticles, transform.position);
        var shape = particle.shape;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = radius;
        particle.Play();
    }

    private void HandleBigExplosion(float radius)
    {
        var particle = NightPool.Spawn(bigParticles, transform.position);
        var shape = particle.shape;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = radius;
        particle.Play();
    }
}
