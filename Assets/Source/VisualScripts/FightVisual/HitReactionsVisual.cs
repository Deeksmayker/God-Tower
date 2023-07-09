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
    
    private bool _stun;
    
    private Color _originalMeshColor;
    private Color _currentAuraColor;

    private MaterialPropertyBlock _propertyBlock;
    
    private MeshRenderer[] _meshRenderers;
    private SkinnedMeshRenderer _skinnedMeshRenderer;
    private ITakeHit[] _hitTakers;
    private IHealthHandler _healthHandler;
    private IAiController _aiController;

    private void Awake()
    {
        _meshRenderers = GetComponentsInChildren<MeshRenderer>();
        _hitTakers = GetComponentsInChildren<ITakeHit>();
        _healthHandler = GetComponentInParent<IHealthHandler>();
        _skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        _aiController = GetComponentInParent<IAiController>();

        _propertyBlock = new MaterialPropertyBlock();
    }

    private void Start()
    {
        SetAuraColor(Color.black);

        if (auraEffect != null)
            _currentAuraColor = auraEffect.GetVector4("Color");

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
        _healthHandler.OnStun += HandleStun;
        _healthHandler.OnDied += HandleDie;
        _healthHandler.OnRevive += RestoreColor;

        for (var i = 0; i < _hitTakers.Length; i++)
        {
            _hitTakers[i].OnTakeHitWithPosition += HandleHitBuffer;
        }
    }

    protected override void OnDisabled()
    {
        _healthHandler.OnStun -= HandleStun;
        _healthHandler.OnDied -= HandleDie;
        _healthHandler.OnRevive -= RestoreColor;

        for (var i = 0; i < _hitTakers.Length; i++)
        {
            _hitTakers[i].OnTakeHitWithPosition -= HandleHitBuffer;
        }
    }

    protected override void Run()
    {
        if (_aiController != null)
        {
            _currentAuraColor = Color.Lerp(Color.black, Color.red * 4, Mathf.Pow(_aiController.GetTimeDifficulty01(), 5));
            _currentAuraColor *= 2;

            auraEffect.SetFloat("NoisePower", Mathf.Lerp(0.1f, 1, Mathf.Pow(_aiController.GetTimeDifficulty01(), 3)));

            if (!_stun)
                SetAuraColor(_currentAuraColor);
        }
    }

    public void RestoreColor()
    {
        _stun = false;
        SetAuraColor(_currentAuraColor);
        
        if (changeMeshesColor)
            ChangeMeshesColor(Color.black);

        if (_skinnedMeshRenderer)
        {
            ChangeSkinnedMeshesColor(Color.black);
        }
    }

    private void HandleStun()
    {
        _stun = true;
        SetAuraColor(Color.white);
        
        if (changeMeshesColor)
            ChangeMeshesColor(Color.white * 3);

        if (_skinnedMeshRenderer)
        {
            ChangeSkinnedMeshesColor(Color.white * 3);
        }

        //ChangeColorCoverageOvertime();
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
        
        if (_stun)
            return;

        SetAuraColor(Color.white);

        if (changeMeshesColor)
            ChangeMeshesColor(Color.white * 3);

        if (_skinnedMeshRenderer)
        {
            ChangeSkinnedMeshesColor(Color.white * 3);
        }

        await UniTask.Delay(TimeSpan.FromSeconds(hitChangeColorTime));

        if (_stun)
            return;
        
        SetAuraColor(_currentAuraColor);

        if (changeMeshesColor)
            ChangeMeshesColor(Color.black);
        
        if (_skinnedMeshRenderer)
        {
            ChangeSkinnedMeshesColor(Color.black);
        }
    }

    /*private async UniTask ChangeColorCoverageOvertime()
    {
        var t = 1f;

        while (t > 0 && _stun)
        {
            if (changeMeshesColor && !_meshRenderers[0])
                break;

            if (changeMeshesColor)
                for (var i = 0; i < _meshRenderers.Length; i++)
                {
                    if (!_meshRenderers[i])
                        break;
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

            var auraColor = Color.Lerp(Color.white * 2, _currentAuraColor, 1 - t);
            SetAuraColor(auraColor);

            t = _healthHandler.GetCurrentReviveTimer() / _healthHandler.GetReviveTime();
            await UniTask.NextFrame();
        }
        SetAuraColor(_currentAuraColor);
    }*/

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
