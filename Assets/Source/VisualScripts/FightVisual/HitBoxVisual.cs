using NTC.Global.Cache;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Timeline;

public class HitBoxVisual : MonoCache
{
    private Color _flashColor = new Color(1f, 0.7f, 0.2f) * 2;
    private float _flashTime = 0.05f;

    private Color _baseColor;

    private ITakeHit _hitBox;

    private MeshRenderer _renderer;
    private MaterialPropertyBlock _materialPropertyBlock;

    private bool _inStun;

    private float _flashTimer;
    private float _stunFlashTimer;

    private void Awake()
    {
        _hitBox = Get<ITakeHit>();
        _renderer = GetComponentInChildren<MeshRenderer>();
        _baseColor = _renderer.material.GetColor("_EmissionColor");
        _materialPropertyBlock = new MaterialPropertyBlock();
        _renderer.SetPropertyBlock(_materialPropertyBlock);
    }

    protected override void OnEnabled()
    {
        _hitBox.OnTakeHit += MakeFlash;

        var stunHandler = GetComponentInParent<IInStun>();

        if (stunHandler != null)
        {
            stunHandler.OnStun += HandleStun;
            stunHandler.OnRecover += HandleStunRecover;
        }
    }

    protected override void OnDisabled()
    {
        _hitBox.OnTakeHit -= MakeFlash;

        var stunHandler = GetComponentInParent<IInStun>();

        if (stunHandler != null)
        {
            stunHandler.OnStun -= HandleStun;
            stunHandler.OnRecover -= HandleStunRecover;
        }
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
            _materialPropertyBlock.SetColor("_EmissionColor", _baseColor);
            _renderer.SetPropertyBlock(_materialPropertyBlock);
        }
    }

    private void MakeFlash(float dmg)
    {
        if (GetComponentInParent<Death>())
            return;

        _flashTimer = _flashTime;

        _materialPropertyBlock.SetColor("_EmissionColor", _flashColor);
        _renderer.SetPropertyBlock(_materialPropertyBlock);
    }

    public void HandleStun()
    {
        StartCoroutine(MakeStunFlash());
    }

    private IEnumerator MakeStunFlash()
    {
        _inStun = true;

        while (_inStun)
        {
            yield return new WaitForSeconds(_flashTime * 2);
            _materialPropertyBlock.SetColor("_EmissionColor", Color.white * 2);
            _renderer.SetPropertyBlock( _materialPropertyBlock);
            yield return new WaitForSeconds(_flashTime * 2);
            _materialPropertyBlock.SetColor("_EmissionColor", _baseColor);
            _renderer.SetPropertyBlock( _materialPropertyBlock);
        }
    }

    private void HandleStunRecover()
    {
        _inStun = false;
    }
}
