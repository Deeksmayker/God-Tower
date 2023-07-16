using NTC.Global.Cache;
using System;
using UnityEngine;

public class LevelStatisticsManager : MonoCache
{
    [SerializeField] private LevelData.Levels level;

    private bool _levelStarted;
    private bool _levelEnded;
    private float _levelTime;

    public static event Action OnLevelStarted;
    public static event Action OnLevelEnded;
    public static event Action OnNewRecord;
    public static event Action OnNewSecretTutorial;

    protected override void Run()
    {
        if (_levelEnded || !_levelStarted)
            return;
        _levelTime += Time.deltaTime;
    }

    public void HandleLevelEnded()
    {
        _levelEnded = true;
        LevelsManager.SetNextLevelToAvaliable(level);

        if (GetCurrentLevelData().MaxCompletedDifficulty < DifficultyData.CurrentDifficulty)
            GetCurrentLevelData().MaxCompletedDifficulty = DifficultyData.CurrentDifficulty;

        if (GetCurrentLevelData().Record == 0 && GetCurrentLevelData().SecretTutorialUnlockedAfterPassing)
            OnNewSecretTutorial?.Invoke();

        if (GetCurrentLevelData().Record > _levelTime || GetCurrentLevelData().Record == 0)
        {
            LevelsManager.SetLevelRecord(level, _levelTime);
            OnNewRecord?.Invoke();
        }

        SavesManager.SaveAllData();

        OnLevelEnded?.Invoke();
    }

    public void HandleLevelStarted()
    {
        _levelStarted = true;
        OnLevelStarted?.Invoke();
    }

    public float GetLevelTime()
    {
        return _levelTime;
    }

    public LevelData.Levels GetCurrentLevel() => level;

    public LevelData GetCurrentLevelData() => LevelsManager.GetLevelData(level);
}

