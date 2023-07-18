using System;
using System.Linq;
using NTC.Global.Cache;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsPanelUi : MonoCache
{
    [SerializeField] private AudioMixer mixer;

    [SerializeField] private TMP_Dropdown graphicsDropdown;
    
    [SerializeField] private Slider senseSlider;
    [SerializeField] private Slider soundSlider;
    [SerializeField] private Slider musicSlider;

    [SerializeField] private TextMeshProUGUI senseValueText;
    [SerializeField] private TextMeshProUGUI soundValueText;
    [SerializeField] private TextMeshProUGUI musicValueText;

    protected override void OnEnabled()
    {
        senseSlider.onValueChanged.AddListener(SetSenseValue);
        soundSlider.onValueChanged.AddListener(SetSfxVolume);
        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        
        soundSlider.value = SettingsController.SFXVolume;
        SetSfxVolume(soundSlider.value);
        musicSlider.value = SettingsController.AmbientVolume;
        SetMusicVolume(musicSlider.value);
        senseSlider.value = SettingsController.Sensitivity;
        SetSenseValue(senseSlider.value);
        
        graphicsDropdown.ClearOptions();
        graphicsDropdown.AddOptions(QualitySettings.names.ToList());
        graphicsDropdown.value = QualitySettings.GetQualityLevel();
        graphicsDropdown.onValueChanged.AddListener(QualitySettings.SetQualityLevel);
    }

    protected override void OnDisabled()
    {
        var camLook = FindObjectOfType<CameraLook>();
        if (camLook)
        {
            camLook.SetSense(SettingsController.Sensitivity);
        }
        
        senseSlider.onValueChanged.RemoveListener(SetSenseValue);
        soundSlider.onValueChanged.RemoveListener(SetSfxVolume);
        musicSlider.onValueChanged.RemoveListener(SetMusicVolume);

        SavesManager.SaveAllData();
    }

    public void SetSfxVolume(float value)
    {
        mixer.SetFloat("SFXVolume", Mathf.Log10(value) * 20);
        soundValueText.text = Mathf.Round(value * 100).ToString();
        SettingsController.SFXVolume = value;
    }

    public void SetMusicVolume(float value)
    {
        mixer.SetFloat("AmbientVolume", Mathf.Log10(value) * 20);
        musicValueText.text = Mathf.Round(value * 100).ToString();
        SettingsController.AmbientVolume = value;
    }

    public void SetSenseValue(float value)
    {
        senseValueText.text = Math.Round(value, 2).ToString();
        SettingsController.Sensitivity = value;
    }

    public void SetLanguage(string value)
    {
        LanguageManager.SetLanguage(value);
    }
}
