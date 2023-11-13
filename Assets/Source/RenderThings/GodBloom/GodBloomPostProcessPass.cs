using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class GodBloomPostProcessPass : ScriptableRenderPass
{
    private Material _bloomMaterial;
    private Material _compositeMaterial;

    private GodBloomEffectComponent _bloomEffect;

    private RenderTextureDescriptor _descriptor;

    private RTHandle _cameraColorTarget;
    private RTHandle _cameraDepthTarget;

    const int k_MaxPyramidSize = 16;
    private int[] _bloomMipUp;
    private int[] _bloomMipDown;
    private RTHandle[] m_BloomMipUp;
    private RTHandle[] m_BloomMipDown;
    private GraphicsFormat hdrFormat;


    public GodBloomPostProcessPass(Material bloomMaterial, Material compositeMaterial)
    {
        _bloomMaterial = bloomMaterial;
        _compositeMaterial = compositeMaterial;

        renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;

        _bloomMipUp = new int[k_MaxPyramidSize];
        _bloomMipDown = new int[k_MaxPyramidSize];
        m_BloomMipUp = new RTHandle[k_MaxPyramidSize];
        m_BloomMipDown = new RTHandle[k_MaxPyramidSize];

        for (int i = 0;i < k_MaxPyramidSize; i++)
        {
            _bloomMipUp[i] = Shader.PropertyToID("_BloomMipUp" + i);
            _bloomMipDown[i] = Shader.PropertyToID("_BloomMipDown" + i);
            //Get name, will get Allocated with descriptor later
            m_BloomMipUp[i] = RTHandles.Alloc(m_BloomMipUp[i], name: "_BloomMipUp" + i);
            m_BloomMipDown[i] = RTHandles.Alloc(m_BloomMipDown[i], name: "_BloomMipDown" + i);

            const FormatUsage usage = FormatUsage.Linear | FormatUsage.Render;
            if (SystemInfo.IsFormatSupported(GraphicsFormat.B10G11R11_UFloatPack32, usage))
            {
                hdrFormat = GraphicsFormat.B10G11R11_UFloatPack32;
            }
            else
            {
                hdrFormat = QualitySettings.activeColorSpace == ColorSpace.Linear ?
                    GraphicsFormat.R8G8B8A8_SRGB
                    : GraphicsFormat.R8G8B8A8_UNorm;
            }
        }
    }

    public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
    {
        _descriptor = renderingData.cameraData.cameraTargetDescriptor;
    }

    public void SetTarget(RTHandle cameraColorTargetHandle, RTHandle cameraDepthTargetHandle)
    {
        _cameraColorTarget = cameraColorTargetHandle;
        _cameraDepthTarget = cameraDepthTargetHandle;
    }


    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        VolumeStack stack = VolumeManager.instance.stack;
        _bloomEffect = stack.GetComponent<GodBloomEffectComponent>();
        CommandBuffer cmd = CommandBufferPool.Get();

        using (new ProfilingScope(cmd, new ProfilingSampler("Custom post process effect")))
        {
            SetupBloom(cmd, _cameraColorTarget);

            _compositeMaterial.SetFloat("_Cutoff", _bloomEffect.textureCutoff.value);
            _compositeMaterial.SetFloat("_Density", _bloomEffect.textureDensity.value);
            _compositeMaterial.SetVector("_Direction", _bloomEffect.scrollDirection.value);
            //Blitter.BlitCameraTexture(cmd, _cameraColorTarget, _cameraColorTarget, _compositeMaterial, 0);
        }



        context.ExecuteCommandBuffer(cmd);
        cmd.Clear();

        CommandBufferPool.Release(cmd);
    }

    private void SetupBloom(CommandBuffer cmd, RTHandle source)
    {
        int downres = 1;
        
        int tw = _descriptor.width >> downres;
        int th = _descriptor.height >> downres;

        // Determine the iteration count
        int maxSize = Mathf.Max(tw, th);
        int iterations = Mathf.FloorToInt(Mathf.Log(maxSize, 2f) - 1);
        int mipCount = Mathf.Clamp(iterations, 1, _bloomEffect.maxIterations.value);

        // Pre-filtering parameters
        float clamp = _bloomEffect.clamp.value;
        float threshold = Mathf.GammaToLinearSpace(_bloomEffect.threshold.value);
        float thresholdKnee = threshold * 0.5f; // Hardcoded soft knee

        // Material setup
        float scatter = Mathf.Lerp(0.05f, 0.95f, _bloomEffect.scatter.value);
        var bloomMaterial = _bloomMaterial;

        bloomMaterial.SetVector("_Params", new Vector4(scatter, clamp, threshold, thresholdKnee));

        // Prefilter
        var desc = GetCompatibleDescriptor(tw, th, hdrFormat);
        for (int i = 0; i < mipCount; i++)
        {
            RenderingUtils.ReAllocateIfNeeded(ref m_BloomMipUp[i], desc, FilterMode.Bilinear, TextureWrapMode.Clamp, name: m_BloomMipUp[i].name);
            RenderingUtils.ReAllocateIfNeeded(ref m_BloomMipDown[i], desc, FilterMode.Bilinear, TextureWrapMode.Clamp, name: m_BloomMipDown[i].name);
            desc.width = Mathf.Max(1, desc.width >> 1);
            desc.height = Mathf.Max(1, desc.height >> 1);
        }

        Blitter.BlitCameraTexture(cmd, source, m_BloomMipDown[0], RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store, bloomMaterial, 0);


        // Downsample - gaussian pyramid
        var lastDown = m_BloomMipDown[0];
        for (int i = 1; i < mipCount; i++)
        {
            // Classic two pass gaussian blur - use mipUp as a temporary target
            //   First pass does 2x downsampling + 9-tap gaussian
            //   Second pass does 9-tap gaussian using a 5-tap filter + bilinear filtering
            Blitter.BlitCameraTexture(cmd, lastDown, m_BloomMipUp[i], RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store, bloomMaterial, 1);
            Blitter.BlitCameraTexture(cmd, m_BloomMipUp[i], m_BloomMipDown[i], RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store, bloomMaterial, 2);

            lastDown = m_BloomMipDown[i];
        }

        // Upsample (bilinear by default, HQ filtering does bicubic instead
        for (int i = mipCount - 2; i >= 0; i--)
        {
            var lowMip = (i == mipCount - 2) ? m_BloomMipDown[i + 1] : m_BloomMipUp[i + 1];
            var highMip = m_BloomMipDown[i];
            var dst = m_BloomMipUp[i];

            cmd.SetGlobalTexture("_SourceTexLowMip", lowMip);
            Blitter.BlitCameraTexture(cmd, highMip, dst, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store, bloomMaterial, 3);
        }
        
        cmd.SetGlobalTexture("_Bloom_Texture", m_BloomMipUp[0]);
        cmd.SetGlobalFloat("_BloomIntensity", _bloomEffect.intensity.value);
    }


    internal static RenderTextureDescriptor GetCompatibleDescriptor(RenderTextureDescriptor desc, int width, int height, GraphicsFormat format, DepthBits depthBufferBits = DepthBits.None)
    {
        desc.depthBufferBits = (int)depthBufferBits;
        desc.msaaSamples = 1;
        desc.width = width;
        desc.height = height;
        desc.graphicsFormat = format;
        return desc;
    }


        RenderTextureDescriptor GetCompatibleDescriptor()
            => GetCompatibleDescriptor(_descriptor.width, _descriptor.height, _descriptor.graphicsFormat);

    RenderTextureDescriptor GetCompatibleDescriptor(int width, int height, GraphicsFormat format, DepthBits depthBufferBits = DepthBits.None)
        => GetCompatibleDescriptor(_descriptor, width, height, format, depthBufferBits);
}