using System;
using Cysharp.Threading.Tasks;
using NTC.Global.Cache;
using NTC.Global.Pool;
using UnityEngine;
using UnityEngine.VFX;


public class HitReactionsVisual : MonoCache
{
    [SerializeField] private float colorChangeDuration;
    [SerializeField] private VisualEffect hitVisualEffect;
    [SerializeField] private VisualEffect dieVisualEffect;
    
    [SerializeField] private VisualEffect auraEffect;
    [SerializeField] private float hitChangeColorTime = 0.4f;

    private bool _dying;
    
    private Color _originalMeshColor;
    private Color _originalAuraColor;

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

    private void Start()
    {
        if (auraEffect != null)
            _originalAuraColor = auraEffect.GetVector4("Color");
        
        if (_meshRenderers.Length > 0)
        {
            _meshRenderers[0].GetPropertyBlock(_propertyBlock);
            _originalMeshColor = _propertyBlock.GetColor("Color");
        }
    }

    protected override void OnEnabled()
    {
        _healthHandler.OnDying += HandleDying;
        _healthHandler.OnDied += HandleDie;

        for (var i = 0; i < _hitTakers.Length; i++)
        {
            _hitTakers[i].OnTakeHitWithPosition += HandleHitBuffer;
        }
    }

    protected override void OnDisabled()
    {
        _healthHandler.OnDying -= HandleDying;
        _healthHandler.OnDied -= HandleDie;

        for (var i = 0; i < _hitTakers.Length; i++)
        {
            _hitTakers[i].OnTakeHitWithPosition -= HandleHitBuffer;
        }
    }

    private void HandleDying()
    {
        _dying = true;
        SetAuraColor(Color.white);
        
        for (var i = 0; i < _meshRenderers.Length; i++)
        {
            _meshRenderers[i].GetPropertyBlock(_propertyBlock);
            _propertyBlock.SetColor("_BaseColor", Color.white);

            _propertyBlock.SetColor("_EmissionColor", Color.white * 3);
            _meshRenderers[i].SetPropertyBlock(_propertyBlock);
        }
    }

    private void HandleDie()
    {
        NightPool.Spawn(dieVisualEffect, transform.position, Quaternion.identity);
    }

    private void HandleHitBuffer(Vector3 pos)
    {
        HandleHit(pos);
    }

    private async UniTask HandleHit(Vector3 pos)
    {
        if (hitVisualEffect != null)
            NightPool.Spawn(hitVisualEffect, pos, Quaternion.identity);
        
        if (_dying)
            return;

        SetAuraColor(Color.white);
        
        for (var i = 0; i < _meshRenderers.Length; i++)
        {
            _meshRenderers[i].GetPropertyBlock(_propertyBlock);
            _propertyBlock.SetColor("_BaseColor", Color.white);

            _propertyBlock.SetColor("_EmissionColor", Color.white * 3);
            _meshRenderers[i].SetPropertyBlock(_propertyBlock);
        }

        await UniTask.Delay(TimeSpan.FromSeconds(hitChangeColorTime));

        if (_dying)
            return;
        
        SetAuraColor(_originalAuraColor);
        
        for (var i = 0; i < _meshRenderers.Length; i++)
        {
            _meshRenderers[i].GetPropertyBlock(_propertyBlock);
            _propertyBlock.SetColor("_BaseColor", Color.black);

            _propertyBlock.SetColor("_EmissionColor", Color.black);
            _meshRenderers[i].SetPropertyBlock(_propertyBlock);
        }
    }

    private void SetAuraColor(Color color)
    {
        if (auraEffect == null)
            return;
        
        auraEffect.SetVector4("Color", color);
    }

    private void HandleWeakPointHit(float baseDamage)
    {
        for (var i = 0; i < _meshRenderers.Length; i++)
        {
            _meshRenderers[i].GetPropertyBlock(_propertyBlock);
            _propertyBlock.SetColor("_BaseColor", Color.red);
            _propertyBlock.SetColor("_EmissionColor", Color.red * 3);
            //propertyBlock.SetColor("_Emission", Color.red);
            _meshRenderers[i].SetPropertyBlock(_propertyBlock);
        }
    }
}
