using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[VolumeComponentMenuForRenderPipeline("Custom/Pixelize", typeof(UniversalRenderPipeline))]
public class PixelizeEffectComponent : VolumeComponent, IPostProcessComponent
{
    [Header("Pizelize")]
    public RenderPassEvent passEvent = RenderPassEvent.BeforeRenderingPostProcessing;
    public IntParameter screenHeight = new IntParameter(300, true);

    [Header("Gradient")]
    public BoolParameter Enabled = new BoolParameter(false, true);
    public Texture2DParameter gradientTexture = new Texture2DParameter(null, true);
    public ClampedFloatParameter intensity = new ClampedFloatParameter(1, 0, 10, true);

    public bool IsActive()
    {
        return true;
    }

    public bool IsTileCompatible()
    {
        return false;
    }
}