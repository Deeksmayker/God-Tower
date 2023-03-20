using System;
using UnityEngine;

public class PlayerRightHand : PlayerHand
{
    protected override void OnEnabled()
    {
        if (_abilitiesHandler != null)
        {
            _abilitiesHandler.OnNewRightAbility += () => HandleNewAbility(_abilitiesHandler.GetRightAbility());
        }
    }
}
