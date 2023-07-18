using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SavesManager
{
    [System.Serializable]
    public class SettingsData
    {
        public float Sensitivity = 0.1f;
        public float SFXVolume = 0.5f;
        public float MusicVolume = 0.5f;
        public string Language = "EN";
    }

    private const string LEVELS_SUB = "/levels";
    private const string SETTINGS_SUB = "/settings";

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

        var settingsData = new SettingsData();
        settingsData.Sensitivity = SettingsController.Sensitivity;
        settingsData.SFXVolume = SettingsController.SFXVolume;
        settingsData.MusicVolume = SettingsController.AmbientVolume;
        settingsData.Language = LanguageManager.CurrentLanguage;

        FileStream stream1 = new FileStream(Application.persistentDataPath + SETTINGS_SUB, FileMode.Create);
        formatter.Serialize(stream1 , settingsData);
        stream1.Close();
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

        var stream1 = new FileStream(Application.persistentDataPath + SETTINGS_SUB, FileMode.Open);
        var settingsData = formatter.Deserialize(stream1) as SettingsData;

        SettingsController.SFXVolume = settingsData.SFXVolume;
        SettingsController.AmbientVolume = settingsData.MusicVolume;
        SettingsController.Sensitivity = settingsData.Sensitivity;

        LanguageManager.SetLanguage(settingsData.Language);

        stream1.Close();
    }
}