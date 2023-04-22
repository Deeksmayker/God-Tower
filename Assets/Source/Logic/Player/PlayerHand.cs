using System;
using NTC.Global.Cache;
using NTC.Global.Pool;
using UnityEngine;

public class PlayerHand : MonoCache
{
    [SerializeField] private ParticleSystem dumpLoadedParticles;
    
    private ModelShaker _shaker;
    
    public Action<PlayerHand> OnHandAbilityPerformed;
    public Action<PlayerHand> OnHandAbilityCharging;
    public Action<PlayerHand> OnHandEndCharging;
    public Action<PlayerHand> OnHandEmpty;
    public Action<PlayerHand, AbilityTypes> OnHandNewAbility;
    
    protected AbilitiesHandler _abilitiesHandler;

    private void Awake()
    {
        _abilitiesHandler = GetComponentInParent<AbilitiesHandler>();
        _shaker = GetComponentInChildren<ModelShaker>();
    }
    
    public void HandleAbilityPerforming()
    {
        OnHandAbilityPerformed?.Invoke(this);
    }

    public void HandleAbilityCharging()
    {
        OnHandAbilityCharging?.Invoke(this);
    }

    public void HandleAbilityEndCharging()
    {
        OnHandEndCharging?.Invoke(this);
    }

    public void HandleAbilityDumpLoaded()
    {
        NightPool.Spawn(dumpLoadedParticles, transform.position);
    }

    public void HandleAbilityEmpty()
    {
        OnHandEmpty?.Invoke(this);
    }

    protected void HandleNewAbility(IActiveAbility ability)
    {
        ability.OnPerform += HandleAbilityPerforming;
        ability.OnStartHolding += HandleAbilityCharging;
        ability.OnEndHolding += HandleAbilityEndCharging;
        ability.OnDumpLoaded += HandleAbilityDumpLoaded;
        ability.OnEmpty += HandleAbilityEmpty;
        
        OnHandNewAbility?.Invoke(this, ability.GetType());
    }

    public ModelShaker GetShaker()
    {
        return _shaker;
    }
}
