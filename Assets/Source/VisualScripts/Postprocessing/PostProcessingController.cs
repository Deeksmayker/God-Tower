using System.Threading.Tasks;
using NTC.Global.Cache;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PostProcessingController : MonoCache
{
    private Volume _volume;
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

	private void Awake()
    {
		if (!_volume) _volume = FindObjectOfType<Volume>();

        _volume.profile.TryGet<Vignette>(out _vignette);
        _volume.profile.TryGet<MotionBlur>(out _motionBlur);
        _volume.profile.TryGet<ChromaticAberration>(out _chromaticAberration);
        _volume.profile.TryGet<GodBloomEffectComponent>(out _bloomEffectComponent);
        _volume.profile.TryGet<PixelizeEffectComponent>(out _pixelizeEffectComponent);

        _defaultVignetteIntensity = _vignette.intensity.value;
        _defaultChromaticAberrationIntensity = _chromaticAberration.intensity.value;
    }

    #region VingnetteControll

    public void SetVignetteIntensity(float intensity, float time = 0)
    {
        if (intensity > _maxVignetteIntensity)
            intensity = _maxVignetteIntensity;

        ChangeFloatParamInTime(_vignette.intensity, intensity, time);
    }

    public void ResetVignetteIntensity(float time = 0)
    {
        ChangeFloatParamInTime(_vignette.intensity, _defaultVignetteIntensity, time);
    }

    public void SetDefaultVignetteIntensity(float intensity)
    {
        if (intensity > _maxVignetteIntensity)
            intensity = _maxVignetteIntensity;

        _defaultVignetteIntensity = intensity;
    }

    public void SetMaxVignetteIntensity(float intensity)
    {
        _maxVignetteIntensity = intensity;
    }

    public void SetVignetteColor(Color color, float time = 0)
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

    public ChromaticAberration GetChromaticAberration()
    {
        return _chromaticAberration;
    }
    
    public void SetChromaticAberrationIntensity(float intensity, float time = 0)
    {
        if (intensity > _maxChromaticAberrationIntensity)
            intensity = _maxChromaticAberrationIntensity;

        ChangeFloatParamInTime(_chromaticAberration.intensity, intensity, time);
    }

    public void ResetChromaticAberrationIntensity(float time = 0)
    {
        ChangeFloatParamInTime(_chromaticAberration.intensity, _defaultChromaticAberrationIntensity, time);
    }

    public void SetDefaultChromaticAberrationIntensity(float intensity)
    {
        if (intensity > _maxChromaticAberrationIntensity)
            intensity = _maxChromaticAberrationIntensity;

        _defaultChromaticAberrationIntensity = intensity;
    }

    public void SetMaxChromaticAberrationIntensity(float intensity)
    {
        _maxChromaticAberrationIntensity = intensity;
    }

    #endregion

    #region GodBloomEffectComponentControll

    public GodBloomEffectComponent GetGodBloomEffectComponent()
    {
        return _bloomEffectComponent;
    }
    
    public void SetThresholdGodBloom(float threshold, float time = 0)
    {
        ChangeFloatParamInTime(_bloomEffectComponent.threshold, threshold, time);
    }

    public void SetIntensityGodBloom(float intensity, float time = 0)
    {
        ChangeFloatParamInTime(_bloomEffectComponent.intensity, intensity, time);
    }

    public void SetScatterGodBloom(float scatter, float time = 0)
    {
        ChangeFloatParamInTime(_bloomEffectComponent.scatter, Mathf.Clamp01(scatter), time);
    }

    public void SetTextureDensityGodBloom(int density, float time = 0)
    {
        ChangeIntParamInTime(_bloomEffectComponent.textureDensity, density, time);
    }

    public void SetTextureCutoffGodBloom(float cutoff, float time = 0)
    {
        ChangeFloatParamInTime(_bloomEffectComponent.textureCutoff, Mathf.Clamp01(cutoff), time);
    }

    public void SetScrollDirectionGodBloom(Vector2 direction)
    {
        _bloomEffectComponent.scrollDirection.value = direction;
    }

    public void SetDistortionAmountGodBloom(float distortionAmount, float time = 0)
    {
        ChangeFloatParamInTime(_bloomEffectComponent.distortionAmount, distortionAmount, time);
    }

    public void SetDistortionRangeGodBloom(Vector2 range)
    {
        _bloomEffectComponent.distortionRange.value = range;
    }

    #endregion

    #region PixelizeEffectComponentControll

    public PixelizeEffectComponent GetPixelizeEffectComponent()
    {
        return _pixelizeEffectComponent;
    }
    
    public void SetScreenHeightPixelize(int screenHeight, float time = 0)
    {
        ChangeIntParamInTime(_pixelizeEffectComponent.screenHeight, screenHeight, time);
    }

    public void TurnGradient(bool value)
    {
        _pixelizeEffectComponent.Enabled.value = value;
    }

    public async void SetGradientTexture(Texture texture, float time = 0)
    {
		if (texture == null){
			await SetGradientOpacity(0, time);
			TurnGradient(false);
			_pixelizeEffectComponent.gradientTexture.value = null;
			_pixelizeEffectComponent.secondGradientTexture.value = null;
			return;
		}

		_pixelizeEffectComponent.gradientTexture.value = _pixelizeEffectComponent.secondGradientTexture.value; 
		if (_pixelizeEffectComponent.gradientTexture == null){
			await SetGradientOpacity(0, 0);
			_pixelizeEffectComponent.gradientTexture.value = texture;
		}

        _pixelizeEffectComponent.secondGradientTexture.value = texture;

		var t = 0f;
		while (t < 1){
			t += Time.deltaTime;
			_pixelizeEffectComponent.gradientT.value = t;
			await Task.Yield();
		}

		_pixelizeEffectComponent.gradientTexture.value = _pixelizeEffectComponent.secondGradientTexture.value; 
    }

    public void SetGradientIntensity(float intensity, float time = 0)
    {
        ChangeFloatParamInTime(_pixelizeEffectComponent.intensity, intensity, time);
    }

	public async Task SetGradientOpacity(float opacity, float time = 0)
	{
		await ChangeFloatParamInTime(_pixelizeEffectComponent.opacity, opacity, time);
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

    private async Task ChangeFloatParamInTime(FloatParameter currentIntensity, float intensity, float time)
    {
		var startIntensity = currentIntensity.value;
		var t = 0f;

		while (t < 1 && time > 0){
			t += Time.deltaTime / time;
			currentIntensity.value = Mathf.Lerp(startIntensity, intensity, t);
			await Task.Yield();
		}

        currentIntensity.value = intensity;
    }

    private async void ChangeColorWithTime(ColorParameter from, Color to, float time)
    {
		var startColor = from.value;
		var t = 0f;

		while (t < 1 && time > 0){
			t += Time.deltaTime / time;
			from.value = Color.Lerp(startColor, to, t);
			await Task.Yield();
		}

		from.value = to;
    }

    private async void ChangeIntParamInTime(IntParameter intParam, int value, float time)
    {
		var startInt = intParam.value;
		var t = 0f;

		while (t < 1 && time > 0){
			t += Time.deltaTime / time;
			intParam.value = (int)Mathf.Round(Mathf.Lerp(startInt, value, t));
			await Task.Yield();
		}

		intParam.value = value;
    }

    #endregion
}
