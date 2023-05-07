using NTC.Global.Cache;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUi : MonoCache
{
    public void LoadGame()
    {
        SceneManager.LoadScene(SettingsController.LastScene);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
