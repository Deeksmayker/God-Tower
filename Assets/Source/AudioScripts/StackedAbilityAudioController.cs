using NTC.Global.Cache;
using NTC.Global.Pool;
using UnityEngine;

public class StackedAbilityAudioController : MonoCache
{
    [SerializeField] private AudioClip[] clips;
    [SerializeField] private AudioSource source;

    private StackedAbility _stackedAbility;

    private void Awake()
    {
        _stackedAbility = GetComponent<StackedAbility>();
    }

    protected override void OnEnabled()
    {
        _stackedAbility.OnPerform += HandleImpact;
    }

    protected override void OnDisabled()
    {
        _stackedAbility.OnPerform -= HandleImpact;
    }

    private void HandleImpact(Vector3 position)
    {
        var s = NightPool.Spawn(source, position);
        s.clip = AudioUtils.GetRandomClip(clips);

        AudioUtils.RandomiseAudioSourceParams(ref source, true, true, 0.2f, 0.1f, 0.3f, 1f);

        s.Play();
    }
}