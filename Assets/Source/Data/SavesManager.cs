using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SavesManager
{
    private const string LEVELS_SUB = "/levels";

    public static void SaveAllData()
    {
        var levelsData = LevelsManager.GetLevelsData();

        BinaryFormatter formatter = new BinaryFormatter();
        var path = Application.persistentDataPath + LEVELS_SUB;

        for (var i = 0; i < levelsData.Length ; i++)
        {
            FileStream stream = new FileStream(path + i, FileMode.Create);
            
            formatter.Serialize(stream, levelsData[i]);
            stream.Close();
        }
    }

    public static void LoadAllData()
    {
        LevelsManager.SetupDefaultLevelsData();

        var levelsData = LevelsManager.GetLevelsData();

        BinaryFormatter formatter = new BinaryFormatter();
        var path = Application.persistentDataPath + LEVELS_SUB;

        for (var i = 0; i <  levelsData.Length ; i++)
        {
            if (!File.Exists(path + i))
            {
                Debug.LogError("No such path as " + path + i);
                continue;
            }

            var stream = new FileStream(path + i, FileMode.Open);
            levelsData[i] = formatter.Deserialize(stream) as LevelData;
            stream.Close();
        }
    }
}