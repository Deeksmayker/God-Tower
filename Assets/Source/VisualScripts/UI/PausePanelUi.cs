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

    private void Start()
    {
        /*senseSlider.onValueChanged.AddListener(SetSliderSense);
        soundSlider.onValueChanged.AddListener(SetSliderSound);

        resumeButton.onClick.AddListener(ResumeGame);
        settingsButton.onClick.AddListener(OpenSettings);
        exitButton.onClick.AddListener(Exit);
        closeSettingsWindowButton.onClick.AddListener(CloseSettingsWindow);*/
    }

    private void OnDestroy()
    {
        /*senseSlider.onValueChanged.RemoveListener(SetSliderSense);
        soundSlider.onValueChanged.RemoveListener(SetSliderSound);

        resumeButton.onClick.RemoveListener(ResumeGame);
        settingsButton.onClick.RemoveListener(OpenSettings);
        exitButton.onClick.RemoveListener(Exit);
        closeSettingsWindowButton.onClick.RemoveListener(CloseSettingsWindow);*/
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            OpenPauseWindow();
            TimeController.Instance.SetPause(true);
        }
    }


    public void SetSliderSound(float value)
    {
        soundValueText.text = value.ToString();
    }

    public void SetSliderSense(float value)
    {
        senseValueText.text = _senseValue.ToString();
    }

    public void OpenPauseWindow()
    {
        pauseWindow.SetActive(true);
    }

    public void ClosePauseWindow()
    {
        Debug.Log("!!!");
        pauseWindow.SetActive(false);
    }

    public void CloseSettingsWindow()
    {
        settingsWindow.SetActive(false);
        OpenPauseWindow();
    }

    public void ResumeGame()
    {
        TimeController.Instance.SetPause(false);
        Cursor.visible = false;
        ClosePauseWindow();
        // resume time
    }

    public void OpenSettings()
    {
        ClosePauseWindow();
        settingsWindow.SetActive(true);
    }

    public void Exit()
    {
        Application.Quit();
    }
}