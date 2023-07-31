[System.Serializable]
public class LevelData
{
    public enum Levels
    {
        L0_1,
        L1_1,
        L1_2,
        L2_1,
        L3_1
    }

    public Levels Level;
    public bool Avaliable;
    public float Record;

    public float RecordForSRank;
    public float RecordForARank;
    public float RecordForBRank;
    public float RecordForCRank;

    public bool SecretTutorialUnlockedAfterPassing;

    public DifficultyData.Difficulties MaxCompletedDifficulty = DifficultyData.Difficulties.Easy;

    public LevelData(Levels level, bool avaliable, float record, float recordForSRank, float recordForARank, float recordForBRank, float recordForCRank)
    {
        Level = level;
        Avaliable = avaliable;
        Record = record;
        RecordForSRank = recordForSRank;
        RecordForARank = recordForARank;
        RecordForBRank = recordForBRank;
        RecordForCRank = recordForCRank;
    }

    public string GetRankTextByRecord(bool inLevel)
    {
        return GetRankTextByTime(Record, inLevel);
    }

    public string GetRankTextByTime(float time, bool inLevel)
    {
        if (time == 0)
            return "-";

        if (time <= RecordForSRank && MaxCompletedDifficulty >= DifficultyData.Difficulties.Normal)
            return "S";
        if (time <= RecordForARank)
            return "A";
        if (time <= RecordForBRank)
            return "B";
        if (time <= RecordForCRank)
            return "C";

        return "D";
    }
}