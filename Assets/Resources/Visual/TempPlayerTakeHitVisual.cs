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

    private IHealthHandler _healthHandler;

    protected override void OnEnabled()
    {
         _healthHandler = Get<IHealthHandler>();
        _healthHandler.OnHit += HandleTakeHit;
        _healthHandler.OnHealthChanged += SetDefaultValuePostProcessing;
        _healthHandler.OnHealthAdd += RefreshPostProcessing;
    }

    private void SetDefaultValuePostProcessing(float value)
    {
        value = Mathf.Clamp(value, 0, 100);

        //postProcessingController.SetBloomColor(Color.Lerp(Color.white, Color.red, 1 - value / 100), 0.2f);
        //postProcessingController.SetDefaultVignetteIntensity(1 - value / 100);
        //postProcessingController.SetDefaultBloomIntensity(5 - value / 20);
        //postProcessingController.SetDefaultChromaticAberrationIntensity(1 - value / 100);
    }

    private void RefreshPostProcessing()
    {
        postProcessingController.ResetVignetteIntensity();
        //postProcessingController.ResetBloomIntensity();
        postProcessingController.ResetChromaticAberrationIntensity();
    }

    private void HandleTakeHit()
    {
        CameraService.Instance.ShakeCamera(0.5f);
        source.PlayOneShot(takeDamageClip);
        ChangePostProcess();
    }

    private async UniTask ChangePostProcess()
    {
        ChangeVignette();
        ChangeChromatic();
    }

    private async UniTask ChangeVignette()
    {
        postProcessingController.SetVignetteColor(Color.red, 0.25f);
        postProcessingController.SetVignetteIntensity(0.5f, 0.25f);

        await UniTask.Delay(300);
        postProcessingController.SetVignetteColor(Color.black, 0.5f);
        postProcessingController.SetVignetteIntensity(Mathf.Max(0.05f, (1f - _healthHandler.GetHealth01()) * 0.35f), 0.4f);
    }

    private async UniTask ChangeChromatic()
    {
        postProcessingController.SetChromaticAberrationIntensity(1, 0.2f);
        
        await UniTask.Delay(200);
        
        //postProcessingController.ResetBloomIntensity(0.02f);
        postProcessingController.ResetChromaticAberrationIntensity(1f);

    }
}
