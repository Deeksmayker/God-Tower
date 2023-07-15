using NTC.Global.Cache;
using UnityEngine;

public class DifficultyLevelController : MonoCache
{
    private void Start()
    {
        var multiplier = 1;

        switch (DifficultyData.CurrentDifficulty)
        {
            case DifficultyData.Difficulties.Easy:
                multiplier = 2;
                break;
            case DifficultyData.Difficulties.Normal:
                multiplier = 1;
                break;
        }

        var rangeAttackers = Resources.FindObjectsOfTypeAll<AiRangeAttacker>();

        for (var i = 0; i <  rangeAttackers.Length; i++)
        {
            rangeAttackers[i].SetDifficultyMultiplier(multiplier);
        }

        var timeTotems = Resources.FindObjectsOfTypeAll<TimeTotem>();

        for (var i = 0; i < timeTotems.Length; i++)
        {
            timeTotems[i].SetDifficultyMultiplier(multiplier);
        }
    }
}