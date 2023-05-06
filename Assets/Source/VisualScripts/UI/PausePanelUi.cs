using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PausePanelUi : MonoBehaviour
{
    [Header("Windows")] [SerializeField] private GameObject pauseWindow;
    [SerializeField] private GameObject settingsWindow;

    [Header("Buttons in pause window")] [SerializeField]
    private Button resumeButton;

    [SerializeField] private Button settingsButton;
    [SerializeField] private Button exitButton;

    [Header("Settings")] [SerializeField] private Button closeSettingsWindowButton;

    [SerializeField] private Slider senseSlider;
    [SerializeField] private TextMeshProUGUI senseValueText;

    [SerializeField] private Slider soundSlider;
    [SerializeField] private TextMeshProUGUI soundValueText;

    private int _senseValue;
    private int _soundValue;

    private bool _isWindowOpen;

    private void Start()
    {
        senseSlider.onValueChanged.AddListener(SetSliderSense);
        soundSlider.onValueChanged.AddListener(SetSliderSound);

        resumeButton.onClick.AddListener(ResumeGame);
        settingsButton.onClick.AddListener(OpenSettings);
        exitButton.onClick.AddListener(Exit);
        closeSettingsWindowButton.onClick.AddListener(CloseSettingsWindow);
    }

    private void OnDestroy()
    {
        senseSlider.onValueChanged.RemoveListener(SetSliderSense);
        soundSlider.onValueChanged.RemoveListener(SetSliderSound);

        resumeButton.onClick.RemoveListener(ResumeGame);
        settingsButton.onClick.RemoveListener(OpenSettings);
        exitButton.onClick.RemoveListener(Exit);
        closeSettingsWindowButton.onClick.RemoveListener(CloseSettingsWindow);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !_isWindowOpen)
        {
            // stop time
            TimeController.Instance.SetPause(true);

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            OpenPauseWindow();
            _isWindowOpen = true;
        }
    }

    private void SetSliderSound(float value)
    {
        soundValueText.text = value.ToString();
    }

    private void SetSliderSense(float value)
    {
        senseValueText.text = value.ToString();
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
        ClosePauseWindow();
    }

    private void OpenSettings()
    {
        ClosePauseWindow();
        settingsWindow.SetActive(true);
        SetSliderSense(senseSlider.value);
        SetSliderSound(soundSlider.value);
    }

    private void Exit()
    {
        Application.Quit();
    }
}