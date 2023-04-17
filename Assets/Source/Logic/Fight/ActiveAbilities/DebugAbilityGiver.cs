using System;
using NTC.Global.Cache;
using UnityEngine;


public class DebugAbilityGiver : MonoCache
{
    private enum AbilityTypes
    {
        None,
        Laser,
        Grenade
    }

    [SerializeField] private GameObject laserAbilityPrefab;
    [SerializeField] private GameObject grenadeAbilityPrefab;

    [SerializeField] private AbilityTypes leftAbility;
    [SerializeField] private AbilityTypes rightAbility;
    [SerializeField] private bool infiniteAbilities;

    private AbilitiesHandler _abilitiesHandler;

    private void Awake()
    {
        _abilitiesHandler = Get<AbilitiesHandler>();
    }

    private void Start()
    {
        CheckAndSetLeftAbility();
        CheckAndSetRightAbility();
    }

    [ContextMenu("Set left ability")]
    public void CheckAndSetLeftAbility()
    {
        switch (leftAbility)
        {
            case AbilityTypes.Laser:
                _abilitiesHandler.SetNewLeftAbility(laserAbilityPrefab, infiniteAbilities);
                return;
            case AbilityTypes.Grenade:
                _abilitiesHandler.SetNewLeftAbility(grenadeAbilityPrefab, infiniteAbilities);
                return;
            default:
                return;
        }
    }
    
    [ContextMenu("Set right ability")]
    public void CheckAndSetRightAbility()
    {
        switch (rightAbility)
        {
            case AbilityTypes.Laser:
                _abilitiesHandler.SetNewRightAbility(laserAbilityPrefab, infiniteAbilities);
                return;
            case AbilityTypes.Grenade:
                _abilitiesHandler.SetNewRightAbility(grenadeAbilityPrefab, infiniteAbilities);
                return; 
            default:
                return;
        }
    }
}
