using NTC.Global.Cache;
using UnityEngine;

public class LevelStatisticsManager : MonoCache
{
    private bool _levelEnded;
    private float _levelTime;

    protected override void OnEnabled()
    {
        PlayerUnit.OnLevelEnd += HandleLevelEnded;
    }

    protected override void OnDisabled()
    {
        PlayerUnit.OnLevelEnd -= HandleLevelEnded;
    }

    protected override void Run()
    {
        if (_levelEnded)
            return;
        _levelTime += Time.deltaTime;
    }

    public void HandleLevelEnded()
    {
        _levelEnded = true;
    }

    public float GetLevelTime()
    {
        return _levelTime;
    }
}

