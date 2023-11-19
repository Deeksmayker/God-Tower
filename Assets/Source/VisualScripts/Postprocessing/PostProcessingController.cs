using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using NTC.Global.Cache;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;

public class PostProcessingController : MonoCache
{
    [SerializeField] private Volume _volume;
    [SerializeField] private float _smooth = 0.1f;
    [Space] 
    [SerializeField] private float _maxVignetteIntensity = 0.8f;
    [SerializeField] private float _maxChromaticAberrationIntensity = 0.8f;

    private Vignette _vignette;
    private MotionBlur _motionBlur;
    private ChromaticAberration _chromaticAberration;
    private GodBloomEffectComponent _bloomEffectComponent;
    private PixelizeEffectComponent _pixelizeEffectComponent;

    private float _defaultVignetteIntensity;
    private float _defaultChromaticAberrationIntensity;

    protected override void OnEnabled()
    {
        _volume.profile.TryGet<Vignette>(out _vignette);
        _volume.profile.TryGet<MotionBlur>(out _motionBlur);
        _volume.profile.TryGet<ChromaticAberration>(out _chromaticAberration);
        _volume.profile.TryGet<GodBloomEffectComponent>(out _bloomEffectComponent);
        _volume.profile.TryGet<PixelizeEffectComponent>(out _pixelizeEffectComponent);

        _defaultVignetteIntensity = _vignette.intensity.value;
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
        if (intensity > _maxVignetteIntensity)
            intensity = _maxVignetteIntensity;

        if (smooth.Equals(-1))
            smooth = this._smooth;

        ChangeIntensityWithSmooth(_vignette.intensity, intensity, smooth);
    }

    /// <summary>
    /// Возвращает ширину виньетки в исходное состояние. Если задать time, то ширина плавно измениться на исходное значение.
    /// </summary>
    /// <param name="time"> Время в секундах, за которое будет достигнуто исходное значение. </param>
    public void ResetVignetteIntensity(float smooth = -1)
    {
        if (smooth.Equals(-1))
            smooth = this._smooth;

        ChangeIntensityWithSmooth(_vignette.intensity, _defaultVignetteIntensity, smooth);
    }

    /// <summary>
    /// Позволяет задать значение интенсивности по-умолчанию для Vignette.
    /// </summary>
    /// <param name="intensity"> Новое значение по-умолчанию</param>
    public void SetDefaultVignetteIntensity(float intensity)
    {
        if (intensity > _maxVignetteIntensity)
            intensity = _maxVignetteIntensity;

        _defaultVignetteIntensity = intensity;
    }

    /// <summary>
    /// Позволяет задать максимальное значение интенсивности Vignette.
    /// </summary>
    /// <param name="intensity"> Новое максимальное значение интенсивности. </param>
    public void SetMaxVignetteIntensity(float intensity)
    {
        _maxVignetteIntensity = intensity;
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

    public void SetMotionBlurIntensity(float value)
    {
        _motionBlur.intensity.value = value;
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
        if (intensity > _maxChromaticAberrationIntensity)
            intensity = _maxChromaticAberrationIntensity;

        if (smooth.Equals(-1))
            smooth = this._smooth;

        ChangeIntensityWithSmooth(_chromaticAberration.intensity, intensity, smooth);
    }

    /// <summary>
    /// Возвращает интенсивность Chromatic Aberration в исходное состояние. Если задать time, то интенсивность плавно измениться на исходное значение.
    /// </summary>
    /// <param name="time"> Время в секундах, за которое будет достигнуто исходное значение. </param>
    public void ResetChromaticAberrationIntensity(float smooth = -1)
    {
        if (smooth.Equals(-1))
            smooth = this._smooth;

        ChangeIntensityWithSmooth(_chromaticAberration.intensity, _defaultChromaticAberrationIntensity, smooth);
    }

    /// <summary>
    /// Позволяет задать значение интенсивности по-умолчанию для Chromatic Aberration.
    /// </summary>
    /// <param name="intensity"> Новое значение по-умолчанию</param>
    public void SetDefaultChromaticAberrationIntensity(float intensity)
    {
        if (intensity > _maxChromaticAberrationIntensity)
            intensity = _maxChromaticAberrationIntensity;

        _defaultChromaticAberrationIntensity = intensity;
    }

    /// <summary>
    /// Позволяет задать максимальное значение интенсивности Chromatic Aberration.
    /// </summary>
    /// <param name="intensity"> Новое максимальное значение интенсивности. </param>
    public void SetMaxChromaticAberrationIntensity(float intensity)
    {
        _maxChromaticAberrationIntensity = intensity;
    }

    #endregion

    #region GodBloomEffectComponentControll

    public void SetThresholdBloom(float threshold, float smooth = -1)
    {
        if (smooth > 0)
        {
            ChangeFloatParameterWithSmooth(_bloomEffectComponent.threshold, threshold, smooth);
        }
        else
        {
            _bloomEffectComponent.threshold.value = threshold;
        }
    }

    public void SetIntensityBloom(float intensity, float smooth = -1)
    {
        if (smooth > 0)
        {
            ChangeFloatParameterWithSmooth(_bloomEffectComponent.intensity, intensity, smooth);
        }
        else
        {
            _bloomEffectComponent.intensity.value = intensity;
        }
    }

    public void SetScatterBloom(float scatter, float smooth = -1)
    {
        if (smooth > 0)
        {
            ChangeFloatParameterWithSmooth(_bloomEffectComponent.scatter, Mathf.Clamp01(scatter), smooth);
        }
        else
        {
            _bloomEffectComponent.scatter.value = Mathf.Clamp01(scatter);
        }
    }

    public void SetTextureDensityBloom(int density, float smooth = -1)
    {
        if (smooth > 0)
        {
            ChangeIntParameterWithSmooth(_bloomEffectComponent.textureDensity, density, smooth);
        }
        else
        {
            _bloomEffectComponent.textureDensity.value = density;
        }
    }

    public void SetTextureCutoffBloom(float cutoff, float smooth = -1)
    {
        if (smooth > 0)
        {
            ChangeFloatParameterWithSmooth(_bloomEffectComponent.textureCutoff, Mathf.Clamp01(cutoff), smooth);
        }
        else
        {
            _bloomEffectComponent.textureCutoff.value = Mathf.Clamp01(cutoff);
        }
    }

    public void SetScrollDirectionBloom(Vector2 direction)
    {
        _bloomEffectComponent.scrollDirection.value = direction;
    }

    public void SetDistortionAmountBloom(float distortionAmount, float smooth = -1)
    {
        if (smooth > 0)
        {
            ChangeFloatParameterWithSmooth(_bloomEffectComponent.distortionAmount, distortionAmount, smooth);
        }
        else
        {
            _bloomEffectComponent.distortionAmount.value = distortionAmount;
        }
    }

    public void SetDistortionRangeBloom(Vector2 range)
    {
        _bloomEffectComponent.distortionRange.value = range;
    }

    #endregion

    #region PixelizeEffectComponentControll

    public void SetScreenHeightPixelize(int screenHeight, float smooth = -1)
    {
        if (smooth > 0)
        {
            ChangeIntParameterWithSmooth(_pixelizeEffectComponent.screenHeight, screenHeight, smooth);
        }
        else
        {
            _pixelizeEffectComponent.screenHeight.value = screenHeight;
        }
    }

    public void TurnPixelize(bool value)
    {
        _pixelizeEffectComponent.Enabled.value = value;
    }

    public void SetGradientTexturePixelize(Texture2D texture)
    {
        _pixelizeEffectComponent.gradientTexture.value = texture;
    }

    public void SetIntensityPixelize(float intensity, float smooth = -1)
    {
        if (smooth > 0)
        {
            ChangeFloatParameterWithSmooth(_pixelizeEffectComponent.intensity, intensity, smooth);
        }
        else
        {
            _pixelizeEffectComponent.intensity.value = intensity;
        }
    }

    #endregion

    #region Other

    private void ChangeColorByPercentage(ColorParameter targetColor, Color from, Color to, float percent, float time)
    {
        var deltaColor = new Color(Mathf.Abs(from.r - to.r), Mathf.Abs(from.g - to.g), Mathf.Abs(from.b - to.b));
        ChangeColorWithTime(targetColor, new Color(
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

    private async Task ChangeFloatParameterWithSmooth(FloatParameter floatParameter, float value, float smooth)
    {
        var step = value * smooth;

        if (floatParameter.value < value)
        {
            while (floatParameter.value < value)
            {
                await Task.Delay(10);
                floatParameter.value += step;
            }
        }
        else
        {
            while (floatParameter.value > value)
            {
                await Task.Delay(10);
                floatParameter.value -= step;
            }
        }

        floatParameter.value = value;
    }

    private async Task ChangeIntParameterWithSmooth(IntParameter floatParameter, int value, float smooth)
    {
        var step = (int)Mathf.Round(value * smooth);

        if (floatParameter.value < value)
        {
            while (floatParameter.value < value)
            {
                await Task.Delay(10);
                floatParameter.value += step;
            }
        }
        else
        {
            while (floatParameter.value > value)
            {
                await Task.Delay(10);
                floatParameter.value -= step;
            }
        }

        floatParameter.value = value;
    }

    #endregion
}