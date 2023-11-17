using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[VolumeComponentMenuForRenderPipeline("Custom/God Bloom", typeof(UniversalRenderPipeline))]
public class GodBloomEffectComponent : VolumeComponent, IPostProcessComponent
{
    [Header("Bloom Settings")]
    public FloatParameter threshold = new FloatParameter(0.9f, true);
    public FloatParameter intensity = new FloatParameter(1, true);
    public ClampedFloatParameter scatter = new ClampedFloatParameter(0.7f, 0, 1, true);
    public IntParameter clamp = new IntParameter(65472, true);
    public ClampedIntParameter maxIterations = new ClampedIntParameter(6, 0, 10);
    public NoInterpColorParameter tint = new NoInterpColorParameter(Color.white);

    [Header("God bloom")]
    public IntParameter textureDensity = new IntParameter(10, true);
    public ClampedFloatParameter textureCutoff = new ClampedFloatParameter(0.4f, 0, 1, true);
    public Vector2Parameter scrollDirection = new Vector2Parameter(new Vector2());
    public FloatParameter distortionAmount = new FloatParameter(100, true);
    public Vector2Parameter distortionRange = new Vector2Parameter(new Vector2(-1, 1));


    public bool IsActive()
    {
        return true;
    }

    public bool IsTileCompatible()
    {
        return true;
    }
}