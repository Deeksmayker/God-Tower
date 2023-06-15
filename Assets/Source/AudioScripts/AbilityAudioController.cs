using NTC.Global.Cache;
using NTC.Global.Pool;
using UnityEngine;

public class AbilityAudioController : MonoCache
{
    [SerializeField] private AudioClip[] performClips;
    [SerializeField] private AudioSource emptySource;

    [SerializeField] private float volumeVariation, pitchVariation;
    
    private IActiveAbility _ability;

    private void Awake()
    {
        _ability = GetComponentInParent<IActiveAbility>();
    }

    protected override void OnEnabled()
    {
        _ability.OnDump += HandlePerform;
    }
    
    protected override void OnDisabled()
    {
        _ability.OnDump -= HandlePerform;
    }
    
    private void HandlePerform()
    {
        var source = NightPool.Spawn(emptySource, transform.position);
        source.clip = AudioUtils.GetRandomClip(performClips);
        
        AudioUtils.RandomiseAudioSourceParams(ref source, true, true, volumeVariation, pitchVariation, 0.6f, 0.7f);

        source.Play();
    }
}
