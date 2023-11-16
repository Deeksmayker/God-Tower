using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ChromaticAberrationDistanceEffectComponent : VolumeComponent, IPostProcessComponent
{
    

    public bool IsActive()
    {
        return true;
    }

    public bool IsTileCompatible()
    {
        return false;
    }
}