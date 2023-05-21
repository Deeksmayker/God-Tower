using NTC.Global.Cache;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUi : MonoCache
{
    public void LoadGame()
    {
        Application.targetFrameRate = 300;
        SceneManager.LoadScene(SettingsController.LastScene);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
