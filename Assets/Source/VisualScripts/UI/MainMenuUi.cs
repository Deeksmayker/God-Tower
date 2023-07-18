using NTC.Global.Cache;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUi : MonoCache
{
    [SerializeField] private GameObject settingsPanel;

    private void Awake()
    {
        SavesManager.LoadAllData();

        settingsPanel.SetActive(true);
        settingsPanel.SetActive(false);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

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

    public void LoadScene(int index)
    {
        Application.targetFrameRate = 300;
        SceneManager.LoadScene(index);
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
