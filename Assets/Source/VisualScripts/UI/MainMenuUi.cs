using NTC.Global.Cache;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUi : MonoCache
{
    public void LoadTutorial()
    {
        Application.targetFrameRate = 300;
        SceneManager.LoadScene(1);
    }

    public void LoadGame()
    {
        Application.targetFrameRate = 300;
        SceneManager.LoadScene(2);
    }

    public void SetNormalDifficulty()
    {
        DifficultyData.CurrentDifficulty = DifficultyData.Difficulties.Normal;
    }

    public void SetEasyDifficulty()
    {
        DifficultyData.CurrentDifficulty = DifficultyData.Difficulties.Easy;
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
