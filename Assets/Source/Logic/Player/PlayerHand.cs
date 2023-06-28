using System;
using NTC.Global.Cache;
using NTC.Global.Pool;
using UnityEngine;

public class PlayerHand : MonoCache
{
    [HideInInspector] public Animator HandAnimator;
    [HideInInspector] public SkinnedMeshRenderer HandMeshRenderer;

    private AbilityTypes _abilityType = AbilityTypes.None;
    
    private ModelShaker _shaker;
    
    public Action<PlayerHand> OnHandAbilityPerformed;
    public Action<PlayerHand> OnHandAbilityCharging;
    public Action<PlayerHand> OnHandEndCharging;
    public Action<PlayerHand> OnHandEmpty;
    public Action<PlayerHand, AbilityTypes> OnHandNewAbility;
    public Action<PlayerHand, AbilityTypes> OnHandDumpLoaded;
    
    protected AbilitiesHandler _abilitiesHandler;

    private void Awake()
    {
        _abilitiesHandler = GetComponentInParent<AbilitiesHandler>();
        _shaker = GetComponentInParent<ModelShaker>();
        HandAnimator = GetComponentInChildren<Animator>();
        HandMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
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
        OnHandDumpLoaded?.Invoke(this, _abilityType);
    }

    public void HandleAbilityEmpty()
    {
        _abilityType = AbilityTypes.None;
        OnHandEmpty?.Invoke(this);
    }

    protected void HandleNewAbility(IActiveAbility ability)
    {
        _abilityType = ability.GetType();
        
        ability.OnPerform += HandleAbilityPerforming;
        ability.OnStartHolding += HandleAbilityCharging;
        ability.OnPerform += HandleAbilityEndCharging;
        ability.OnStartHolding += HandleAbilityDumpLoaded;
        ability.OnEmpty += HandleAbilityEmpty;
        
        OnHandNewAbility?.Invoke(this, ability.GetType());
    }

    public virtual IActiveAbility GetHandAbility() => null;

    public ModelShaker GetShaker()
    {
        return _shaker;
    }
}
