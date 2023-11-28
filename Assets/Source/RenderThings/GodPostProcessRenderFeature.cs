using UnityEngine.Rendering.Universal;
using UnityEngine;
using UnityEngine.Rendering;

[System.Serializable]
public class GodPostProcessRenderFeature : ScriptableRendererFeature
{
    [SerializeField] private Shader bloomShader;
    [SerializeField] private Shader compositeShader;
    [SerializeField] private Shader chromaticAberrationShader;

    private Material _bloomMaterial;
    [SerializeField] private Material _compositeMaterial;

    [SerializeField] private Material _chromaticAberrationMaterial;

    private GodBloomPostProcessPass m_godBloomPass;

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
#if UNITY_EDITOR
        if (renderingData.cameraData.isSceneViewCamera) return;
#endif
        renderer.EnqueuePass(m_godBloomPass);
    }

    public override void Create()
    {
        _bloomMaterial = CoreUtils.CreateEngineMaterial(bloomShader);
        //_compositeMaterial = CoreUtils.CreateEngineMaterial(compositeShader);
        //_chromaticAberrationMaterial = CoreUtils.CreateEngineMaterial(chromaticAberrationShader);

        m_godBloomPass = new GodBloomPostProcessPass(_bloomMaterial, _compositeMaterial, _chromaticAberrationMaterial);
    }

    public override void SetupRenderPasses(ScriptableRenderer renderer, in RenderingData renderingData)
    {
#if UNITY_EDITOR
        if (renderingData.cameraData.cameraType != CameraType.Game) return;
#endif
        m_godBloomPass.ConfigureInput(ScriptableRenderPassInput.Depth);
        m_godBloomPass.ConfigureInput(ScriptableRenderPassInput.Color);
        m_godBloomPass.SetTarget(renderer.cameraColorTargetHandle, renderer.cameraDepthTargetHandle);
    }

    protected override void Dispose(bool disposing)
    {
        CoreUtils.Destroy(_bloomMaterial);
        //CoreUtils.Destroy(_compositeMaterial);
        //CoreUtils.Destroy(_chromaticAberrationMaterial);
    }
}