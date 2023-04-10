using System;

public interface IMakeExplosion
{
    public event Action<float> OnBigExplosionWithRadius;
    public event Action<float> OnExplosionWithRadius;
}