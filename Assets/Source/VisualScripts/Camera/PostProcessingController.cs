using Cysharp.Threading.Tasks;
using NTC.Global.Cache;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PostProcessingController : MonoCache
{
    [SerializeField] private Volume volume;
    [SerializeField] private float smooth = 0.1f;
    [Space] 
    [SerializeField] private float maxVignetteIntensity = 0.8f;
    [SerializeField] private float maxBloomIntensity = 8f;
    [SerializeField] private float maxChromaticAberrationIntensity = 0.8f;

    private Vignette _vignette;
    private Bloom _bloom;
    private ChromaticAberration _chromaticAberration;

    private float _defaultVignetteIntensity;
    private float _defaultBloomIntensity;
    private float _defaultChromaticAberrationIntensity;

    protected override void OnEnabled()
    {
        volume.profile.TryGet<Vignette>(out _vignette);
        volume.profile.TryGet<Bloom>(out _bloom);
        volume.profile.TryGet<ChromaticAberration>(out _chromaticAberration);
        
        _defaultVignetteIntensity = _vignette.intensity.value;
        _defaultBloomIntensity = _bloom.intensity.value;
        _defaultChromaticAberrationIntensity = _chromaticAberration.intensity.value;
    }

    #region VingnetteControll
    
    /// <summary>
    /// Позволяет задать новое значение ширины виньетки. Если задать time, то ширина плавно измениться на нужное значение.
    /// </summary>
    /// <param name="intensity"> Новое значение ширины виньетки. </param>
    /// <param name="time"> Время в секундах, за которое будет достигнуто новое значение. </param>
    public void SetVignetteIntensity(float intensity, float smooth = -1)
    {
        if (intensity > maxVignetteIntensity)
            intensity = maxVignetteIntensity;
        
        if (smooth.Equals(-1))
            smooth = this.smooth;
        
        ChangeIntensityWithSmooth(_vignette.intensity, intensity, smooth);
    }

    /// <summary>
    /// Возвращает ширину виньетки в исходное состояние. Если задать time, то ширина плавно измениться на исходное значение.
    /// </summary>
    /// <param name="time"> Время в секундах, за которое будет достигнуто исходное значение. </param>
    public void ResetVignetteIntensity(float smooth = -1)
    {
        if (smooth.Equals(-1))
            smooth = this.smooth;
        
        ChangeIntensityWithSmooth(_vignette.intensity, _defaultVignetteIntensity, smooth);
    }
    
    /// <summary>
    /// Позволяет задать значение интенсивности по-умолчанию для Vignette.
    /// </summary>
    /// <param name="intensity"> Новое значение по-умолчанию</param>
    public void SetDefaultVignetteIntensity(float intensity)
    {
        if (intensity > maxVignetteIntensity)
            intensity = maxVignetteIntensity;

        _defaultVignetteIntensity = intensity;
    }

    /// <summary>
    /// Позволяет задать максимальное значение интенсивности Vignette.
    /// </summary>
    /// <param name="intensity"> Новое максимальное значение интенсивности. </param>
    public void SetMaxVignetteIntensity(float intensity)
    {
        maxVignetteIntensity = intensity;
    }

    /// <summary>
    /// Позволяет изменить цвет виньетки за время <paramref name="time"/>. 
    /// </summary>
    public void SetVignetteColor(Color color, float time)
    {
        ChangeColorWithTime(_vignette.color, color, time);
    }

    #endregion

    #region BloomControll
    
    /// <summary>
    /// Позволяет задать новое значение интенсивности Bloom. Если задать time, то интенсивность плавно измениться на нужное значение.
    /// </summary>
    /// <param name="intensity"> Новое значение интенсивности Bloom. </param>
    /// <param name="time"> Время в секундах, за которое будет достигнуто новое значение. </param>
    public void SetBloomIntensity(float intensity, float smooth = -1)
    {
        if (intensity > maxBloomIntensity)
            intensity = maxBloomIntensity;
        
        if (smooth.Equals(-1))
            smooth = this.smooth;
        
        ChangeIntensityWithSmooth(_bloom.intensity, intensity, smooth);
    }

    /// <summary>
    /// Возвращает интенсивность Bloom в исходное состояние. Если задать time, то интенсивность плавно измениться на исходное значение.
    /// </summary>
    /// <param name="time"> Время в секундах, за которое будет достигнуто исходное значение. </param>
    public void ResetBloomIntensity(float smooth = -1)
    {
        if (smooth.Equals(-1))
            smooth = this.smooth;
        
        ChangeIntensityWithSmooth(_bloom.intensity, _defaultBloomIntensity, smooth);
    }
    
    /// <summary>
    /// Позволяет задать значение интенсивности по-умолчанию для Bloom.
    /// </summary>
    /// <param name="intensity"> Новое значение по-умолчанию</param>
    public void SetDefaultBloomIntensity(float intensity)
    {
        if (intensity > maxBloomIntensity)
            intensity = maxBloomIntensity;
        
        _defaultBloomIntensity = intensity;
    }
    
    /// <summary>
    /// Позволяет задать максимальное значение интенсивности Bloom.
    /// </summary>
    /// <param name="intensity"> Новое максимальное значение интенсивности. </param>
    public void SetMaxBloomIntensity(float intensity)
    {
        maxBloomIntensity = intensity;
    }

    /// <summary>
    /// Позволяет изменить цвет Bloom на время <paramref name="time"/>. 
    /// </summary>
    public void SetBloomColor(Color color, float time)
    {
        ChangeColorWithTime(_bloom.tint, color, time);
    }

    /// <summary>
    /// Позволяет плавно менять цвет Bloom из цвета <paramref name="from"/> в цвет <paramref name="to"/> по процентам.
    /// </summary>
    /// <param name="from"> Начальный цвет. Эквивалент <paramref name="percent"/> = 1. </param>
    /// <param name="to"> Конечный цвет. Эквивалент <paramref name="percent"/> = 0. </param>
    /// <param name="percent"> Проценты, в соответствии с которыми будет происходить переход. Чем больше <paramref name="percent"/>,
    /// тем ближе цвет будет к <paramref name="from"/>. </param>
    /// <param name="time"> Время, за которое будет происходить плавный переход. </param>
    public void ChangeBloomColorByPercentage(Color from, Color to, float percent, float time = 0.1f)
    {
        ChangeColorByPercentage(_bloom.tint, from, to, percent, time);
    }

    public void SetBloomTintColor(Color color, float time)
    {
        ChangeColorWithTime(_bloom.tint, color, time);
    }

    #endregion
    
    #region ChromaticAberrationControll

    /// <summary>
    /// Позволяет задать новое значение интенсивности Chromatic Aberration. Если задать time, то интенсивность плавно измениться на нужное значение.
    /// </summary>
    /// <param name="intensity"> Новое значение интенсивности Chromatic Aberration. </param>
    /// <param name="time"> Время в секундах, за которое будет достигнуто новое значение. </param>
    public void SetChromaticAberrationIntensity(float intensity, float smooth = -1)
    {
        if (intensity > maxChromaticAberrationIntensity)
            intensity = maxChromaticAberrationIntensity;

        if (smooth.Equals(-1))
            smooth = this.smooth;
        
        ChangeIntensityWithSmooth(_chromaticAberration.intensity, intensity, smooth);
    }

    /// <summary>
    /// Возвращает интенсивность Chromatic Aberration в исходное состояние. Если задать time, то интенсивность плавно измениться на исходное значение.
    /// </summary>
    /// <param name="time"> Время в секундах, за которое будет достигнуто исходное значение. </param>
    public void ResetChromaticAberrationIntensity(float smooth = -1)
    {
        if (smooth.Equals(-1))
            smooth = this.smooth;
        
        ChangeIntensityWithSmooth(_chromaticAberration.intensity, _defaultChromaticAberrationIntensity, smooth);
    }
    
    /// <summary>
    /// Позволяет задать значение интенсивности по-умолчанию для Chromatic Aberration.
    /// </summary>
    /// <param name="intensity"> Новое значение по-умолчанию</param>
    public void SetDefaultChromaticAberrationIntensity(float intensity)
    {
        if (intensity > maxChromaticAberrationIntensity)
            intensity = maxChromaticAberrationIntensity;
        
        _defaultChromaticAberrationIntensity = intensity;
    }
    
    /// <summary>
    /// Позволяет задать максимальное значение интенсивности Chromatic Aberration.
    /// </summary>
    /// <param name="intensity"> Новое максимальное значение интенсивности. </param>
    public void SetMaxChromaticAberrationIntensity(float intensity)
    {
        maxChromaticAberrationIntensity = intensity;
    }

    #endregion

    #region Other

    private void ChangeColorByPercentage(ColorParameter targetColor, Color from, Color to, float percent, float time)
    {
        var deltaColor = new Color(Mathf.Abs(from.r - to.r), Mathf.Abs(from.g - to.g), Mathf.Abs(from.b - to.b));
        ChangeColorWithTime(targetColor ,new Color(
            Mathf.Abs(targetColor.value.r - deltaColor.r * percent), 
            Mathf.Abs(targetColor.value.g - deltaColor.g * percent),
            Mathf.Abs(targetColor.value.b - deltaColor.b * percent)), time);
    }
    
    private async UniTask ChangeIntensityWithSmooth(FloatParameter currentIntensity, float intensity, float smooth)
    {
        var step = intensity * smooth;
        
        if (currentIntensity.value < intensity)
        {
            while (currentIntensity.value < intensity)
            {
                await UniTask.Delay(10);
                currentIntensity.value += step;
            }
        }
        else
        {
            while (currentIntensity.value > intensity)
            {
                await UniTask.Delay(10);
                currentIntensity.value -= step;
            }
        }

        currentIntensity.value = intensity;
    }

    private async UniTask ChangeColorWithTime(ColorParameter from, Color to, float time)
    {
        while (time > 0)
        {
            from.value = Color.Lerp(from.value, to, 0.1f / time);
            time -= 0.1f;
            await UniTask.Delay(2);
        }
    }

    #endregion
}