using UnityEngine;

public interface IGiveAbility
{
    public GameObject GetAbilityPrefab();
    public StackedAbility GetStackedAbilityPrefab();
    public bool CanGiveAbility();
}
