using ModestTree;
using System.Linq;
using UnityEngine;

public static class LevelsManager
{
    private static LevelData[] _levels;

    public static void SetupDefaultLevelsData()
    {
        _levels = new LevelData[]
        {
            new LevelData(LevelData.Levels.L0_1, true, 0, 30, 40, 45, 50),
            new LevelData(LevelData.Levels.L1_1, true, 0, 33, 40, 45, 50),
            new LevelData(LevelData.Levels.L1_2, false, 0, 45, 50, 55, 60)
        };
    }

    public static LevelData GetLevelData(LevelData.Levels level)
    {
        if (_levels == null || _levels.Length == 0)
            SetupDefaultLevelsData();

        return _levels.Where(l => l.Level == level).First();
    }

    public static void SetLevelRecord(LevelData.Levels level, float newRecord)
    {
        var levelData = _levels.Where(l => l.Level == level).First();

        if (levelData.Record != 0 && levelData.Record <= newRecord)
        {
            Debug.LogError("new record are worse than existed");
        }

        levelData.Record = newRecord;
    }

    public static void SetNextLevelToAvaliable(LevelData.Levels level)
    {
        if (_levels == null || _levels.Length == 0)
            SetupDefaultLevelsData();

        var nextLevelData = _levels.Where((l) => l.Level == level + 1).FirstOrDefault();
        if (nextLevelData != null)
        {
            nextLevelData.Avaliable = true;
        }
    }

    public static LevelData[] GetLevelsData() => _levels;
}