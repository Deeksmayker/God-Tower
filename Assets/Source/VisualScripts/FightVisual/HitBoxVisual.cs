using NTC.Global.Cache;
using UnityEngine;

public class HitBoxVisual : MonoCache
{
    private Color _flashColor = new Color(1f, 0.7f, 0.2f) * 2;
    private float _flashTime = 0.05f;

    private Color _baseColor;

    private ITakeHit _hitBox;

    private MeshRenderer _renderer;
    private MaterialPropertyBlock _materialPropertyBlock;


    private float _flashTimer;

    private void Awake()
    {
        _hitBox = Get<ITakeHit>();
        _renderer = GetComponentInChildren<MeshRenderer>();
        _baseColor = _renderer.material.GetColor("_Color");
        _materialPropertyBlock = new MaterialPropertyBlock();
        _renderer.SetPropertyBlock(_materialPropertyBlock);
    }

    protected override void OnEnabled()
    {
        _hitBox.OnTakeHit += MakeFlash;
    }

    protected override void Run()
    {
        if (_flashTimer <= 0)
        {
            return;
        }

        _flashTimer -= Time.deltaTime;

        if (_flashTimer <= 0)
        {
            _materialPropertyBlock.SetColor("_Color", _baseColor);
            _renderer.SetPropertyBlock(_materialPropertyBlock);
        }
    }

    private void MakeFlash(float dmg)
    {
        if (GetComponentInParent<Death>())
            return;

        _flashTimer = _flashTime;

        _materialPropertyBlock.SetColor("_Color", _flashColor);
        _renderer.SetPropertyBlock(_materialPropertyBlock);
    }
}