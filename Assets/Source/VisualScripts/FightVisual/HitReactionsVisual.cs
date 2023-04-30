using System;
using Code.Global.Animations;
using Cysharp.Threading.Tasks;
using NTC.Global.Cache;
using NTC.Global.Pool;
using UnityEngine;
using UnityEngine.VFX;


public class HitReactionsVisual : MonoCache
{
    [SerializeField] private bool changeMeshesColor = true;
    
    [SerializeField] private float hitChangeColorTime = 0.4f;
    
    [Header("Visual Effects")]
    [SerializeField] private VisualEffect hitVisualEffect;
    [SerializeField] private VisualEffect dieVisualEffect;
    [SerializeField] private VisualEffect auraEffect;

    [SerializeField] private ShakePreset hitShake;
    
    private bool _dying;
    
    private Color _originalMeshColor;
    private Color _originalAuraColor;

    private MaterialPropertyBlock _propertyBlock;
    
    private MeshRenderer[] _meshRenderers;
    private SkinnedMeshRenderer _skinnedMeshRenderer;
    private ITakeHit[] _hitTakers;
    private IHealthHandler _healthHandler;

    private void Awake()
    {
        _meshRenderers = GetComponentsInChildren<MeshRenderer>();
        _hitTakers = GetComponentsInChildren<ITakeHit>();
        _healthHandler = GetComponentInParent<IHealthHandler>();
        _skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();

        _propertyBlock = new MaterialPropertyBlock();
    }

    private void Start()
    {
        if (auraEffect != null)
            _originalAuraColor = auraEffect.GetVector4("Color");
        
        if (changeMeshesColor && _meshRenderers.Length > 0)
        {
            _meshRenderers[0].GetPropertyBlock(_propertyBlock);
            _originalMeshColor = _propertyBlock.GetColor("Color");
        }

        if (_skinnedMeshRenderer)
        {
            _skinnedMeshRenderer.GetPropertyBlock(_propertyBlock);
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
        
        if (changeMeshesColor)
            for (var i = 0; i < _meshRenderers.Length; i++)
            {
                _meshRenderers[i].GetPropertyBlock(_propertyBlock);
                _propertyBlock.SetColor("_BaseColor", Color.white);

                _propertyBlock.SetColor("_EmissionColor", Color.white * 3);
                _meshRenderers[i].SetPropertyBlock(_propertyBlock);
            }

        if (_skinnedMeshRenderer)
        {
            _skinnedMeshRenderer.GetPropertyBlock(_propertyBlock);
            _propertyBlock.SetColor("_BaseColor", Color.white);

            _propertyBlock.SetColor("_EmissionColor", Color.white * 3);
            _skinnedMeshRenderer.SetPropertyBlock(_propertyBlock);
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

        AnimationShortCuts.ShakePositionAnimation(transform, hitShake);
        
        if (_dying)
            return;

        SetAuraColor(Color.white);
        
        if (changeMeshesColor)
            for (var i = 0; i < _meshRenderers.Length; i++)
            {
                _meshRenderers[i].GetPropertyBlock(_propertyBlock);
                _propertyBlock.SetColor("_BaseColor", Color.white);

                _propertyBlock.SetColor("_EmissionColor", Color.white * 3);
                _meshRenderers[i].SetPropertyBlock(_propertyBlock);
            }

        if (_skinnedMeshRenderer)
        {
            _skinnedMeshRenderer.GetPropertyBlock(_propertyBlock);
            _propertyBlock.SetColor("_BaseColor", Color.white);

            _propertyBlock.SetColor("_EmissionColor", Color.white * 3);
            _skinnedMeshRenderer.SetPropertyBlock(_propertyBlock);
        }

        await UniTask.Delay(TimeSpan.FromSeconds(hitChangeColorTime));

        if (_dying)
            return;
        
        SetAuraColor(_originalAuraColor);
        
        if (changeMeshesColor)
            for (var i = 0; i < _meshRenderers.Length; i++)
            {
                _meshRenderers[i].GetPropertyBlock(_propertyBlock);
                _propertyBlock.SetColor("_BaseColor", Color.black);

                _propertyBlock.SetColor("_EmissionColor", Color.black);
                _meshRenderers[i].SetPropertyBlock(_propertyBlock);
            }
        
        if (_skinnedMeshRenderer)
        {
            _skinnedMeshRenderer.GetPropertyBlock(_propertyBlock);
            _propertyBlock.SetColor("_BaseColor", Color.black);

            _propertyBlock.SetColor("_EmissionColor", Color.black);
            _skinnedMeshRenderer.SetPropertyBlock(_propertyBlock);
        }
    }

    private void SetAuraColor(Color color)
    {
        if (auraEffect == null)
            return;
        
        auraEffect.SetVector4("Color", color);
    }
}
