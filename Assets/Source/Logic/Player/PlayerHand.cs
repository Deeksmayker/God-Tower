using System;
using NTC.Global.Cache;
using NTC.Global.Pool;
using UnityEngine;

public class PlayerHand : MonoCache
{
    [SerializeField] private ParticleSystem dumpLoadedParticles;
    
    private ModelShaker _shaker;
    
    public Action<PlayerHand> OnAbilityPerformed;
    public Action<PlayerHand> OnAbilityCharging;
    
    protected AbilitiesHandler _abilitiesHandler;

    private void Awake()
    {
        _abilitiesHandler = GetComponentInParent<AbilitiesHandler>();
        _shaker = GetComponentInChildren<ModelShaker>();
    }
    
    public void HandleAbilityPerforming()
    {
        OnAbilityPerformed?.Invoke(this);
    }

    public void HandleAbilityCharging()
    {
        OnAbilityCharging?.Invoke(this);
    }

    public void HandleAbilityDumpLoaded()
    {
        NightPool.Spawn(dumpLoadedParticles, transform.position);
    }

    protected void HandleNewAbility(IActiveAbility ability)
    {
        ability.OnPerform += HandleAbilityPerforming;
        ability.OnStartHolding += HandleAbilityCharging;
        ability.OnDumpLoaded += HandleAbilityDumpLoaded;
    }

    public ModelShaker GetShaker()
    {
        return _shaker;
    }
}
