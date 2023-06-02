using NTC.Global.Cache;
using NTC.Global.Pool;
using UnityEngine;

public class ExplosiveAudioController : MonoCache
{
    [SerializeField] private AudioClip[] normalExplosionClips;
    [SerializeField] private AudioClip[] bigExplosionClips;
    [SerializeField] private AudioSource audioSourceToSpawn;

    private IMakeExplosion _explosive;
    
    private void Awake()
    {
        _explosive = GetComponent<IMakeExplosion>();
    }

    protected override void OnEnabled()
    {
        _explosive.OnExplosionWithRadius += HandleExplosion;
        _explosive.OnBigExplosionWithRadius += HandleBigExplosion;
    }
    
    protected override void OnDisabled()
    {
        _explosive.OnExplosionWithRadius -= HandleExplosion;
        _explosive.OnBigExplosionWithRadius -= HandleBigExplosion;
    }
    
    private void HandleExplosion(float radius)
    {
        var source = NightPool.Spawn(audioSourceToSpawn, transform.position);
        source.clip = AudioUtils.GetRandomClip(normalExplosionClips);
        
        AudioUtils.RandomiseAudioSourceParams(ref source, true, true, 0.05f, 0.2f, 0.15f);

        source.Play();
    }

    private void HandleBigExplosion(float radius)
    {
        var source = NightPool.Spawn(audioSourceToSpawn, transform.position);
        source.clip = AudioUtils.GetRandomClip(bigExplosionClips);
        
        AudioUtils.RandomiseAudioSourceParams(ref source, true, true, 0.05f, 0.2f, 0.1f);

        source.Play();
    }
}
