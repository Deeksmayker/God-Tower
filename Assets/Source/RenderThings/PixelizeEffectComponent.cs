using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[VolumeComponentMenuForRenderPipeline("Custom/Pixelize", typeof(UniversalRenderPipeline))]
public class PixelizeEffectComponent : VolumeComponent, IPostProcessComponent
{
    public RenderPassEvent passEvent = RenderPassEvent.AfterRenderingPostProcessing;
    public IntParameter screenHeight = new IntParameter(144, true);

    public bool IsActive()
    {
        return true;
    }

    public bool IsTileCompatible()
    {
        return false;
    }
}