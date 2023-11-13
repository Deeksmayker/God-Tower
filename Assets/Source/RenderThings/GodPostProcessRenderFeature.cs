using UnityEngine.Rendering.Universal;
using UnityEngine;
using UnityEngine.Rendering;

[System.Serializable]
public class GodPostProcessRenderFeature : ScriptableRendererFeature
{
    [SerializeField] private Shader bloomShader;
    [SerializeField] private Shader compositeShader;

    private Material _bloomMaterial;
    [SerializeField] private Material _compositeMaterial;

    private GodBloomPostProcessPass m_godBloomPass;

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(m_godBloomPass);
    }

    public override void Create()
    {
        _bloomMaterial = CoreUtils.CreateEngineMaterial(bloomShader);
        //_compositeMaterial = CoreUtils.CreateEngineMaterial(compositeShader);

        m_godBloomPass = new GodBloomPostProcessPass(_bloomMaterial, _compositeMaterial);
    }

    public override void SetupRenderPasses(ScriptableRenderer renderer, in RenderingData renderingData)
    {
        m_godBloomPass.ConfigureInput(ScriptableRenderPassInput.Depth);
        m_godBloomPass.ConfigureInput(ScriptableRenderPassInput.Color);
        m_godBloomPass.SetTarget(renderer.cameraColorTargetHandle, renderer.cameraDepthTargetHandle);
    }

    protected override void Dispose(bool disposing)
    {
        CoreUtils.Destroy(_bloomMaterial);
        CoreUtils.Destroy(_compositeMaterial);
    }
}