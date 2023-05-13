using Cysharp.Threading.Tasks;
using NTC.Global.Cache;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class TempPlayerTakeHitVisual : MonoCache
{
    [SerializeField] private AudioSource source;
    [SerializeField] private AudioClip takeDamageClip;
    [SerializeField] private Volume volume;
    [SerializeField] private ShakePreset camShake;
    [Range(0.1f, 1f)]
    [SerializeField] private float maxVignetteIntensity = 0.8f;
    
    private float _vignetteIntensity = 0f;
    private float _minVignetteIntensity = 0f;

    protected override void OnEnabled()
    {
        var healthHandler = Get<IHealthHandler>();
        healthHandler.OnHit += HandleTakeHit;
        healthHandler.OnHealthChanged += SetVignetteIntensity;

        volume.profile.TryGet<Vignette>(out var vignette);
        _minVignetteIntensity = vignette.intensity.value;
    }

    private void SetVignetteIntensity(float value)
    {
        _vignetteIntensity = maxVignetteIntensity < (1 - (value / 100)) 
            ? maxVignetteIntensity 
            : _minVignetteIntensity > (1 - (value / 100)) 
                ? _minVignetteIntensity 
                : (1 - (value / 100));

        Vignette vignette;
        
        if (volume.profile.TryGet<Vignette>(out vignette))
            ChangeVignetteIntensity(vignette, _vignetteIntensity);
    }

    private void HandleTakeHit()
    {
        CameraService.Instance.ShakeCamera(camShake);
        source.PlayOneShot(takeDamageClip);
        ChangePostProcess();
    }

    private async UniTask ChangePostProcess()
    {
        Vignette vignette;
        
        if (volume.profile.TryGet<Vignette>(out vignette))
        {
            ChangeVignetteColorWithTime(vignette, Color.red, 1);
            ChangeVignetteIntensity(vignette,
                maxVignetteIntensity < _vignetteIntensity+0.3f ? maxVignetteIntensity : _vignetteIntensity+0.3f);
        }

        await UniTask.Delay(150);
        
        if (volume.profile.TryGet<Vignette>(out vignette))
        {
            ChangeVignetteColorWithTime(vignette, Color.black, 5);
            ChangeVignetteIntensity(vignette, _vignetteIntensity);
        }
    }

    private async UniTask ChangeVignetteIntensity(Vignette vignette, float intensity)
    {
        if (vignette.intensity.value < intensity)
        {
            while (vignette.intensity.value < intensity)
            {
                await UniTask.Delay(10);
                vignette.intensity.value += 0.1f;
            }
        }
        else
        {
            while (vignette.intensity.value > intensity)
            {
                await UniTask.Delay(10);
                vignette.intensity.value -= 0.1f;
            }
        }
    }

    private async UniTask ChangeVignetteColorWithTime(Vignette vignette, Color to, float time)
    {
        while (time > 0)
        {
            vignette.color.value = Color.Lerp(vignette.color.value, to, 0.1f / time);
            time -= 0.1f;
            await UniTask.Delay(2);
        }
    }
}
