using NTC.Global.Cache;
using NTC.Global.Pool;
using UnityEngine;

public class EnemyAudioController : MonoCache
{
    [SerializeField] private AudioClip[] takeHitClips;
    [SerializeField] private AudioClip[] dyingClips;
    [SerializeField] private AudioClip[] diedClips;
    [SerializeField] private AudioSource audioSourceToSpawn;
    [SerializeField] private float volumeVariation = 0.3f;
    [SerializeField] private float pitchVariation = 0.1f;

    private IHealthHandler _healthHandler;

    private void Awake()
    {
        _healthHandler = GetComponentInParent<IHealthHandler>();
    }

    protected override void OnEnabled()
    {
        if (_healthHandler != null)
        {
            _healthHandler.OnHit += HandleTakeHit;
            _healthHandler.OnDying += HandleStartDying;
            _healthHandler.OnDied += HandleDie;
        }
    }
    
    protected override void OnDisabled()
    {
        if (_healthHandler != null)
        {
            _healthHandler.OnHit -= HandleTakeHit;
            _healthHandler.OnDying -= HandleStartDying;
            _healthHandler.OnDied -= HandleDie;
        }
    }

    private void HandleTakeHit()
    {
        PlayRandomClipOnPlace(takeHitClips);
    }

    private void HandleStartDying()
    {
        PlayRandomClipOnPlace(dyingClips);
    }

    private void HandleDie()
    {
        PlayRandomClipOnPlace(diedClips);
    }

    private void PlayRandomClipOnPlace(AudioClip[] clips)
    {
        if (clips.Length == 0)
            return;
        
        var source = NightPool.Spawn(audioSourceToSpawn, transform.position);
        source.clip = AudioUtils.GetRandomClip(clips);

        AudioUtils.RandomiseAudioSourceParams(ref source, true, true, volumeVariation, pitchVariation);
        source.Play();
    }
}
