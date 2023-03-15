using System;

public interface ITrackingGiveAbilityOpportunity
{
    public event Action OnCanGiveAbility;
    public event Action OnNotCanGiveAbility;
}
