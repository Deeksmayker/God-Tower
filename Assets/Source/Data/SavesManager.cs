using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.InputSystem;

public static class SavesManager
{
    [System.Serializable]
    public class SettingsData
    {
        public float Sensitivity = 0.1f;
        public float SFXVolume = 0.5f;
        public float MusicVolume = 0.5f;
        public string Language = "EN";
        public InputAction[] KeyBindsActions;
    }

    public static SettingsData LoadedSettignsData;

    private const string LEVELS_SUB = "/levels1";
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

        LoadedSettignsData = new SettingsData();
        LoadedSettignsData.Sensitivity = SettingsController.Sensitivity;
        LoadedSettignsData.SFXVolume = SettingsController.SFXVolume;
        LoadedSettignsData.MusicVolume = SettingsController.AmbientVolume;
        LoadedSettignsData.Language = LanguageManager.CurrentLanguage;

        FileStream stream1 = new FileStream(Application.persistentDataPath + SETTINGS_SUB, FileMode.Create);
        formatter.Serialize(stream1 , LoadedSettignsData);
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
        LoadedSettignsData = formatter.Deserialize(stream1) as SettingsData;

        SettingsController.SFXVolume = LoadedSettignsData.SFXVolume;
        SettingsController.AmbientVolume = LoadedSettignsData.MusicVolume;
        SettingsController.Sensitivity = LoadedSettignsData.Sensitivity;

        LanguageManager.SetLanguage(LoadedSettignsData.Language);

        stream1.Close();
    }
}