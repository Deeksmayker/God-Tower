using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuWindow : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button startButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button exitButton;
    [Header("Windows")]
    [SerializeField] private OptionsWindow optionsWindow;

    private void Start()
    {
        startButton.onClick.AddListener(StartGame);
        optionsButton.onClick.AddListener(OpenOptionsWindow);
        exitButton.onClick.AddListener(ExitGame);
    }

    private void OnDestroy()
    {
        startButton.onClick.RemoveListener(StartGame);
        optionsButton.onClick.RemoveListener(OpenOptionsWindow);
        exitButton.onClick.RemoveListener(ExitGame);
    }

    private void StartGame() => SceneManager.LoadScene(1);

    private void OpenOptionsWindow() => optionsWindow.gameObject.SetActive(true);

    private void ExitGame() => Application.Quit();
}
