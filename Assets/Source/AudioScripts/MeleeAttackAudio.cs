using System;
using NTC.Global.Cache;
using NTC.Global.Pool;
using UnityEngine;


public class MeleeAttackAudio : MonoCache
{
    [SerializeField] private AudioClip[] attackPerformClips;
    [SerializeField] private AudioClip[] attackHitClips;
    [SerializeField] private AudioSource audioSourceToSpawn;

    [SerializeField] private float volumeVariation = 0.3f;
    [SerializeField] private float pitchVariation = 0.1f;

    private IMeleeAttacker _meleeAttacker;

    private void Awake()
    {
        _meleeAttacker = GetComponentInParent<IMeleeAttacker>();
    }

    protected override void OnEnabled()
    {
        _meleeAttacker.OnStartPreparingAttack += HandleAttackPerform;
        _meleeAttacker.OnHit += HandleAttackHit;
        _meleeAttacker.OnParry += HandleAttackHit;
    }

    protected override void OnDisabled()
    {
        _meleeAttacker.OnStartPreparingAttack -= HandleAttackPerform;
        _meleeAttacker.OnHit -= HandleAttackHit;
        _meleeAttacker.OnParry -= HandleAttackHit;
    }

    private void HandleAttackPerform()
    {
        if (attackPerformClips.Length == 0)
            return;

        var source = NightPool.Spawn(audioSourceToSpawn, transform.position);
        source.clip = AudioUtils.GetRandomClip(attackPerformClips);
        AudioUtils.RandomiseAudioSourceParams(ref source, true, true, volumeVariation, pitchVariation);

        source.Play();
    }

    private void HandleAttackHit()
    {
        if (attackHitClips.Length == 0)
            return;

        var source = NightPool.Spawn(audioSourceToSpawn, transform.position);
        source.clip = AudioUtils.GetRandomClip(attackHitClips);
        AudioUtils.RandomiseAudioSourceParams(ref source, true, true, volumeVariation, pitchVariation);

        source.Play();
    }
}
