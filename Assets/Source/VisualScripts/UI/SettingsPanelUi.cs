using NTC.Global.Cache;
using UnityEngine;
using UnityEngine.Audio;

public class SettingsPanelUi : MonoCache
{
    [SerializeField] private AudioMixer mixer;

    protected override void OnDisabled()
    {
        var camLook = FindObjectOfType<CameraLook>();
        if (camLook)
        {
            camLook.SetSense(SettingsController.Sensitivity);
        }
    }
    
    public void SetSfxVolume(float value)
    {
        mixer.SetFloat("SFXVolume", Mathf.Log10(value) * 20);
    }

    public void SetMusicVolume(float value)
    {
        mixer.SetFloat("AmbientVolume", Mathf.Log10(value) * 20);
    }

    public void SetSenseValue(float value)
    {
        SettingsController.Sensitivity = value;
    }
}
