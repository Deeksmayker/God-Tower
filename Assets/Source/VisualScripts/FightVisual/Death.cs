using NTC.Global.Cache;
using System.Collections;
using UnityEngine;

public class Death : MonoCache
{
    [SerializeField] private float dissolveDuration = 0.5f;
    [SerializeField] private float flashDuration = 0.05f;

    private float _timer;

    private Color _deadColor = new Color(1f, 0, 0) * 2;
    private Color _flashColor = new Color(1f, 0.7f, 0.2f) * 2;

    private MeshRenderer[] _meshRenderers;
    private MaterialPropertyBlock _materialPropertyBlock;

    protected override void OnEnabled()
    {
        _meshRenderers = GetComponentsInChildren<MeshRenderer>();
        _materialPropertyBlock = new MaterialPropertyBlock();
        var material = Resources.Load(ResPath.Materials + "DeathMaterial") as Material;
        for (var i = 0; i < _meshRenderers.Length; i++)
        {
            _meshRenderers[i].material = material;
        }

        _materialPropertyBlock.SetColor("_Color", _flashColor);
        StartCoroutine(WaitAndChangeColor(flashDuration, _deadColor));
    }

    protected override void Run()
    {
        _timer += Time.deltaTime / dissolveDuration;
        if (_timer >= 1)
        {
            Destroy(gameObject);
            return;
        }

        _materialPropertyBlock.SetFloat("_AlphaClip", _timer);
        for (var i = 0; i < _meshRenderers.Length; i++)
        {
            _meshRenderers[i].SetPropertyBlock(_materialPropertyBlock);
        }
    }

    private IEnumerator WaitAndChangeColor(float time, Color newColor)
    {
        yield return new WaitForSeconds(time);
        _materialPropertyBlock.SetColor("_Color", newColor);
        for (var i = 0; i < _meshRenderers.Length; i++)
        {
            _meshRenderers[i].SetPropertyBlock(_materialPropertyBlock);
        }
    }
}