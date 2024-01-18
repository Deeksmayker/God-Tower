using UnityEngine;

public class Birth : MonoBehaviour{
    private float _appearDuration = 1f;

    private float _timer;

    private Color _color = new Color(0.2f, 0.4f, 0.7f) * 3;

    private MeshRenderer[] _meshRenderers;
    private Material[] _originalMaterials;
    private MaterialPropertyBlock _materialPropertyBlock;

    private void OnEnable()
    {
        _meshRenderers = GetComponentsInChildren<MeshRenderer>();
        _originalMaterials = new Material[_meshRenderers.Length];
        _materialPropertyBlock = new MaterialPropertyBlock();
        var material = Resources.Load(ResPath.Materials + "DeathMaterial") as Material;
        for (var i = 0; i < _meshRenderers.Length; i++)
        {
            _originalMaterials[i] = _meshRenderers[i].material;
            _meshRenderers[i].material = material;
        }

        _materialPropertyBlock.SetColor("_Color", _color);
        for (var i = 0; i < _meshRenderers.Length; i++)
        {
            _meshRenderers[i].SetPropertyBlock(_materialPropertyBlock);
        }
    }

    public void SetDissolveDuration(float value)
    {
        _appearDuration = value;
    }

    private void Update()
    {
        _timer += Time.deltaTime / _appearDuration;
        if (_timer >= 1)
        {
            var newPropertyBlock = new MaterialPropertyBlock();
            for (int i = 0; i < _meshRenderers.Length; i++){
                _meshRenderers[i].SetPropertyBlock(newPropertyBlock);
                _meshRenderers[i].material = _originalMaterials[i];
            }
            Destroy(this);
            return;
        }

        _materialPropertyBlock.SetFloat("_AlphaClip", 1f-_timer);
        for (var i = 0; i < _meshRenderers.Length; i++)
        {
            _meshRenderers[i].SetPropertyBlock(_materialPropertyBlock);
        }
    }
}
