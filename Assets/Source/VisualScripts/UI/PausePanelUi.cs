using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

public class PausePanelUi : MonoBehaviour
{
    [Header("Windows")] [SerializeField] private GameObject pauseWindow;
    [SerializeField] private GameObject settingsWindow;

    [Header("Buttons in pause window")] [SerializeField]
    private Button resumeButton;

    [SerializeField] private Button settingsButton;
    [SerializeField] private Button exitButton;

    [SerializeField] private Button closeSettingsWindowButton;

    [Inject] private PlayerInput _input;

    private bool _isWindowOpen;

    private void Start()
    {
        resumeButton.onClick.AddListener(ResumeGame);
        settingsButton.onClick.AddListener(OpenSettings);
        exitButton.onClick.AddListener(Exit);
        closeSettingsWindowButton.onClick.AddListener(CloseSettingsWindow);

        settingsWindow.SetActive(true);
        settingsWindow.SetActive(false);
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
            return;
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

        if (_input.actions["Restart"].WasPressedThisFrame())
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