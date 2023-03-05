using System;
using NTC.Global.Cache;
using UnityEngine;


public class HitReactionsVisual : MonoCache
{
    [SerializeField] private float colorChangeDuration;

    private float _timer;

    private MaterialPropertyBlock _propertyBlock;
    
    private MeshRenderer[] _meshRenderers;
    private ITakeHit[] _hitTakers;
    private IWeakPoint[] _weakPoints;

    private void Awake()
    {
        _meshRenderers = GetComponentsInChildren<MeshRenderer>();
        _hitTakers = GetComponentsInChildren<ITakeHit>();
        _weakPoints = GetComponentsInChildren<IWeakPoint>();

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
        for (var i = 0; i < _hitTakers.Length; i++)
        {
            _hitTakers[i].OnTakeHit += HandleHit;
        }
        
        for (var i = 0; i < _weakPoints.Length; i++)
        {
            _weakPoints[i].OnWeakPointHit += HandleWeakPointHit;
        }
    }

    protected override void OnDisabled()
    {
        for (var i = 0; i < _hitTakers.Length; i++)
        {
            _hitTakers[i].OnTakeHit -= HandleHit;
        }
        
        for (var i = 0; i < _weakPoints.Length; i++)
        {
            _weakPoints[i].OnWeakPointHit -= HandleWeakPointHit;
        }
    }

    private void HandleHit(int damage)
    {
        for (var i = 0; i < _meshRenderers.Length; i++)
        {
            _meshRenderers[i].GetPropertyBlock(_propertyBlock);
            _propertyBlock.SetColor("_BaseColor", Color.red);

            _propertyBlock.SetColor("_EmissionColor", Color.red * 10);
            _meshRenderers[i].SetPropertyBlock(_propertyBlock);
        }
    }

    private void HandleWeakPointHit(int baseDamage)
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
