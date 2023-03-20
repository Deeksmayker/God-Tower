using System;
using NTC.Global.Cache;
using UnityEngine;

public class PlayerHand : MonoCache
{
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

    protected void HandleNewAbility(IActiveAbility ability)
    {
        ability.OnPerform += HandleAbilityPerforming;
        ability.OnStartDumping += HandleAbilityCharging;
    }

    public ModelShaker GetShaker()
    {
        return _shaker;
    }
}
