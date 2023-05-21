using Cysharp.Threading.Tasks;
using NTC.Global.Cache;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Zenject;

public class TempPlayerTakeHitVisual : MonoCache
{
    [SerializeField] private AudioSource source;
    [SerializeField] private AudioClip takeDamageClip;
    [SerializeField] private ShakePreset camShake;
    [Inject] private PostProcessingController postProcessingController;

    protected override void OnEnabled()
    {
        var healthHandler = Get<IHealthHandler>();
        healthHandler.OnHit += HandleTakeHit;
        healthHandler.OnHealthChanged += SetDefaultValuePostProcessing;
        healthHandler.OnHealthAdd += RefreshPostProcessing;
    }

    private void SetDefaultValuePostProcessing(float value)
    {
        value = Mathf.Clamp(value, 0, 100);
        
        postProcessingController.SetDefaultVignetteIntensity(1 - value / 100);
        postProcessingController.SetDefaultBloomIntensity(5 - value / 20);
        postProcessingController.SetDefaultChromaticAberrationIntensity(1 - value / 100);
    }

    private void RefreshPostProcessing()
    {
        postProcessingController.ResetVignetteIntensity();
        postProcessingController.ResetBloomIntensity();
        postProcessingController.ResetChromaticAberrationIntensity();
    }

    private void HandleTakeHit()
    {
        CameraService.Instance.ShakeCamera(camShake);
        source.PlayOneShot(takeDamageClip);
        ChangePostProcess();
    }

    private async UniTask ChangePostProcess()
    {
        postProcessingController.SetVignetteColor(Color.red, 1);
        postProcessingController.SetBloomIntensity(10);
        postProcessingController.SetChromaticAberrationIntensity(1);
        postProcessingController.SetVignetteIntensity(1);
        
        await UniTask.Delay(150);
        
        postProcessingController.SetVignetteColor(Color.black, 5);
        postProcessingController.ResetBloomIntensity(0.02f);
        postProcessingController.ResetChromaticAberrationIntensity(0.02f);
        postProcessingController.ResetVignetteIntensity(0.02f);
    }
}
