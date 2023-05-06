using NTC.Global.Cache;
using NTC.Global.Pool;
using UnityEngine;

public class ExplosiveAudioController : MonoCache
{
    [SerializeField] private float volumeVariation = 0.2f;
    [SerializeField] private float pitchVariation = 0.1f;
    
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
        
        AudioUtils.RandomiseAudioSourceParams(ref source, true, true, volumeVariation, pitchVariation, 0.3f);

        source.Play();
    }

    private void HandleBigExplosion(float radius)
    {
        var source = NightPool.Spawn(audioSourceToSpawn, transform.position);
        source.clip = AudioUtils.GetRandomClip(bigExplosionClips);
        
        AudioUtils.RandomiseAudioSourceParams(ref source, true, true, volumeVariation, pitchVariation, 0.6f);

        source.Play();
    }
}
