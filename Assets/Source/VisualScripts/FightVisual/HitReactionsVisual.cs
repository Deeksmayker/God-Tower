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
        SetAuraColor(Color.black);

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
        _healthHandler.OnStun += HandleDying;
        _healthHandler.OnDied += HandleDie;
        _healthHandler.OnRevive += RestoreColor;

        for (var i = 0; i < _hitTakers.Length; i++)
        {
            _hitTakers[i].OnTakeHitWithPosition += HandleHitBuffer;
        }
    }

    protected override void OnDisabled()
    {
        _healthHandler.OnStun -= HandleDying;
        _healthHandler.OnDied -= HandleDie;
        _healthHandler.OnRevive -= RestoreColor;

        for (var i = 0; i < _hitTakers.Length; i++)
        {
            _hitTakers[i].OnTakeHitWithPosition -= HandleHitBuffer;
        }
    }

    public void RestoreColor()
    {
        _dying = false;
        SetAuraColor(Color.black);
        
        if (changeMeshesColor)
            ChangeMeshesColor(Color.black);

        if (_skinnedMeshRenderer)
        {
            ChangeSkinnedMeshesColor(Color.black);
        }
    }

    private void HandleDying()
    {
        _dying = true;
        SetAuraColor(Color.white);
        
        if (changeMeshesColor)
            ChangeMeshesColor(Color.white * 3);

        if (_skinnedMeshRenderer)
        {
            ChangeSkinnedMeshesColor(Color.white * 3);
        }

        ChangeColorCoverageOvertime();
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
            ChangeMeshesColor(Color.white * 3);

        if (_skinnedMeshRenderer)
        {
            ChangeSkinnedMeshesColor(Color.white * 3);
        }

        await UniTask.Delay(TimeSpan.FromSeconds(hitChangeColorTime));

        if (_dying)
            return;
        
        SetAuraColor(_originalAuraColor);

        if (changeMeshesColor)
            ChangeMeshesColor(Color.black);
        
        if (_skinnedMeshRenderer)
        {
            ChangeSkinnedMeshesColor(Color.black);
        }
    }

    private async UniTask ChangeColorCoverageOvertime()
    {
        var t = 1f;

        while (t > 0 && _dying && gameObject)
        {
            if (changeMeshesColor)
                for (var i = 0; i < _meshRenderers.Length; i++)
                {
                    _meshRenderers[i].GetPropertyBlock(_propertyBlock);
                    _propertyBlock.SetFloat("_ColorCoverage", t);
                    _meshRenderers[i].SetPropertyBlock(_propertyBlock);
                    await UniTask.NextFrame();
                }
            
            if (_skinnedMeshRenderer)
            {
                _skinnedMeshRenderer.GetPropertyBlock(_propertyBlock);
                _propertyBlock.SetFloat("_ColorCoverage", t);
                _skinnedMeshRenderer.SetPropertyBlock(_propertyBlock);
            }

            var auraColor = Color.Lerp(Color.white * 2, Color.black, 1 - t);
            SetAuraColor(auraColor);

            t = _healthHandler.GetCurrentReviveTimer() / _healthHandler.GetReviveTime();
            await UniTask.NextFrame();
        }
        SetAuraColor(Color.black);
    }

    private void ChangeMeshesColor(Color color)
    {
        if (_meshRenderers == null)
            return;
        for (var i = 0; i < _meshRenderers.Length; i++)
        {
            _meshRenderers[i].GetPropertyBlock(_propertyBlock);
            _propertyBlock.SetColor("_Color", color);
            _meshRenderers[i].SetPropertyBlock(_propertyBlock);
        }
    }

    private void ChangeSkinnedMeshesColor(Color color)
    {
        if (_skinnedMeshRenderer == null)
            return;
        _skinnedMeshRenderer.GetPropertyBlock(_propertyBlock);
        _propertyBlock.SetColor("_Color", color);
        _skinnedMeshRenderer.SetPropertyBlock(_propertyBlock);
    }

    private void SetAuraColor(Color color)
    {
        if (auraEffect == null)
            return;
        
        auraEffect.SetVector4("Color", color);
    }
}
