using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PausePanelUi : MonoBehaviour
{
    [Header("Windows")] [SerializeField] private GameObject pauseWindow;
    [SerializeField] private GameObject settingsWindow;

    [Header("Buttons in pause window")] [SerializeField]
    private Button resumeButton;

    [SerializeField] private Button settingsButton;
    [SerializeField] private Button exitButton;

    [SerializeField] private Button closeSettingsWindowButton;

    private bool _isWindowOpen;

    private void Start()
    {
        resumeButton.onClick.AddListener(ResumeGame);
        settingsButton.onClick.AddListener(OpenSettings);
        exitButton.onClick.AddListener(Exit);
        closeSettingsWindowButton.onClick.AddListener(CloseSettingsWindow);
    }

    private void OnDestroy()
    {
        resumeButton.onClick.RemoveListener(ResumeGame);
        settingsButton.onClick.RemoveListener(OpenSettings);
        exitButton.onClick.RemoveListener(Exit);
        closeSettingsWindowButton.onClick.RemoveListener(CloseSettingsWindow);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && _isWindowOpen)
        {
            ResumeGame();
        }
        
        if (Input.GetKeyDown(KeyCode.Escape) && !_isWindowOpen)
        {
            // stop time
            TimeController.Instance.SetPause(true);

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            OpenPauseWindow();
            _isWindowOpen = true;
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    private void OpenPauseWindow()
    {
        pauseWindow.SetActive(true);
    }

    private void ClosePauseWindow()
    {
        pauseWindow.SetActive(false);
    }

    private void CloseSettingsWindow()
    {
        settingsWindow.SetActive(false);
        OpenPauseWindow();
    }

    private void ResumeGame()
    {
        // resume time
        TimeController.Instance.SetPause(false);
        
        _isWindowOpen = false;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        CloseSettingsWindow();
        ClosePauseWindow();
    }

    private void OpenSettings()
    {
        ClosePauseWindow();
        settingsWindow.SetActive(true);
    }

    public void Exit()
    {
        TimeController.Instance.SetPause(false);
        SceneManager.LoadScene(0);
    }
}