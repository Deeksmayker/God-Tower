using NTC.Global.Cache;
using Unity.VisualScripting;
using UnityEngine;

public class LevelStatisticsManager : MonoCache
{
    private bool _levelStarted;
    private bool _levelEnded;
    private float _levelTime;

    protected override void OnEnabled()
    {
        PlayerUnit.OnLevelEnd += HandleLevelEnded;
        PlayerUnit.OnLevelStarted += HandleLevelStarted;
    }

    protected override void OnDisabled()
    {
        PlayerUnit.OnLevelEnd -= HandleLevelEnded;
        PlayerUnit.OnLevelStarted -= HandleLevelStarted;
    }

    protected override void Run()
    {
        if (_levelEnded || !_levelStarted)
            return;
        _levelTime += Time.deltaTime;
    }

    public void HandleLevelEnded()
    {
        _levelEnded = true;
    }

    public void HandleLevelStarted()
    {
        _levelStarted = true;
    }

    public float GetLevelTime()
    {
        return _levelTime;
    }
}

