using System;
using NTC.Global.Cache;
using NTC.Global.Pool;
using UnityEngine;


public class HitReactionsVisual : MonoCache
{
    [SerializeField] private float colorChangeDuration;
    [SerializeField] private ParticleSystem hitParticles;

    private float _timer;

    private MaterialPropertyBlock _propertyBlock;
    
    private MeshRenderer[] _meshRenderers;
    private ITakeHit[] _hitTakers;
    private IHealthHandler _healthHandler;

    private void Awake()
    {
        _meshRenderers = GetComponentsInChildren<MeshRenderer>();
        _hitTakers = GetComponentsInChildren<ITakeHit>();
        _healthHandler = GetComponentInParent<IHealthHandler>();

        _propertyBlock = new MaterialPropertyBlock();
    }

    protected override void Run()
    {
        if (_timer > 0)
        {
            
        }
    }
    
    protected override void OnEnabled()
    {
        _healthHandler.OnDying += HandleDying;
        
        for (var i = 0; i < _hitTakers.Length; i++)
        {
            _hitTakers[i].OnTakeHitWithPosition += HandleHit;
        }
    }

    protected override void OnDisabled()
    {
        _healthHandler.OnDying += HandleDying;

        for (var i = 0; i < _hitTakers.Length; i++)
        {
            _hitTakers[i].OnTakeHitWithPosition -= HandleHit;
        }
    }

    private void HandleDying()
    {
        for (var i = 0; i < _meshRenderers.Length; i++)
        {
            _meshRenderers[i].GetPropertyBlock(_propertyBlock);
            _propertyBlock.SetColor("_BaseColor", Color.red);

            _propertyBlock.SetColor("_EmissionColor", Color.red * 10);
            _meshRenderers[i].SetPropertyBlock(_propertyBlock);
        }
    }

    private void HandleHit(Vector3 pos)
    {
        NightPool.Spawn(hitParticles, pos, Quaternion.identity);
    }

    private void HandleWeakPointHit(float baseDamage)
    {
        for (var i = 0; i < _meshRenderers.Length; i++)
        {
            _meshRenderers[i].GetPropertyBlock(_propertyBlock);
            _propertyBlock.SetColor("_BaseColor", Color.red);
            _propertyBlock.SetColor("_EmissionColor", Color.red * 10);
            //propertyBlock.SetColor("_Emission", Color.red);
            _meshRenderers[i].SetPropertyBlock(_propertyBlock);
        }
    }
}
