using UnityEngine;

public class PlayerLeftHand : PlayerHand
{
    protected override void OnEnabled()
    {
        if (_abilitiesHandler != null)
        {
            _abilitiesHandler.OnNewLeftAbility += () => HandleNewAbility(_abilitiesHandler.GetLeftAbility());
        }
    }

    public override IActiveAbility GetHandAbility()
    {
        return _abilitiesHandler.GetLeftAbility();
    }
}
