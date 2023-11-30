using System;
using NTC.Global.Cache;
using UnityEngine;
using Zenject;

namespace Source.VisualScripts.Postprocessing
{
    public class PostProcessingChanger : MonoCache
    {
        [Inject] private PostProcessingController _postProcessingController;

        [SerializeField] private bool _isChangedOnEnter = true;
        [SerializeField] private bool _changeBackOnExit;

        [Header("God Bloom")]
        [SerializeField] private bool _changeGodBloom;
        [SerializeField] private float godBloomTimeToChange = 1;
        [SerializeField] private float _godBloomThreshold;
        [SerializeField] private float _godBloomIntensity;
        [SerializeField] private int _godBloomTextureDensity;
        [SerializeField] private float _godBloomTextureCutoff;
        [SerializeField] private Vector2 _godBloomScrollDirection;
        [SerializeField] private float _godBloomDistortionAmount;
        [SerializeField] private Vector2 _godBloomDistortionRange;
        
        private float _oldGodBloomThreshold;
        private float _oldGodBloomIntensity;
        private int _oldGodBloomTextureDensity;
        private float _oldGodBloomTextureCutoff;
        private Vector2 _oldGodBloomScrollDirection;
        private float _oldGodBloomDistortionAmount;
        private Vector2 _oldGodBloomDistortionRange;

        [Header("Pixelize")] 
        [SerializeField] private bool _changePixelize;
        [SerializeField] private float pixelizeChangeTime = 1;
        [SerializeField] private int _pixelizeScreenHeight;

        private int _oldPixelizeScreenHeight;

		[Header("Gradient")]
		[SerializeField] private bool changeGradient;
        [SerializeField] private bool gradientEnabled;
		[SerializeField] private float gradientChangeTime = 1;
        [SerializeField] private Texture gradientTexture;
        [SerializeField] private float gradientIntensity;
		[SerializeField] private float gradientOpacity;
        
        private bool _oldGradientEnabled;
        private Texture _oldGradientTexture;
        private float _oldGradientIntensity;
		private float _oldGradientOpacity;

        [Header("Chromatic aberration")] 
        [SerializeField] private bool _changeAberration;
        [SerializeField] private float _changeAberrationSmooth = 1;
        [SerializeField] private float _aberrationIntensity;
        
        private float _oldAberrationIntensity;

        private event Action playerExited;

        private void OnTriggerEnter(Collider other)
        {
            if (!other.GetComponent<PlayerUnit>() || !_isChangedOnEnter) return;

            if (!_changeBackOnExit)
            {
				ChangePostProcessing();
				return;
            }
			Debug.Log("i'm here");

			playerExited += BackPostProcessing;
			
			if (_changeGodBloom)
			{
				var oldGodBloom = _postProcessingController.GetGodBloomEffectComponent();

				_oldGodBloomIntensity = oldGodBloom.intensity.value;
				_oldGodBloomThreshold = oldGodBloom.threshold.value;
				_oldGodBloomDistortionRange = oldGodBloom.distortionRange.value;
				_oldGodBloomDistortionAmount = oldGodBloom.distortionAmount.value;
				_oldGodBloomScrollDirection = oldGodBloom.scrollDirection.value;
				_oldGodBloomTextureCutoff = oldGodBloom.textureCutoff.value;
				_oldGodBloomTextureDensity = oldGodBloom.textureDensity.value;
			}

			if (_changePixelize)
			{
				Debug.Log("remembering pixelize");
				var oldPixelize = _postProcessingController.GetPixelizeEffectComponent();
				_oldPixelizeScreenHeight = oldPixelize.screenHeight.value;
			}

			if (changeGradient){
				Debug.Log("remembering gradient");
				var oldGradient = _postProcessingController.GetPixelizeEffectComponent();
				_oldGradientEnabled = oldGradient.Enabled.value;
				_oldGradientIntensity = oldGradient.intensity.value;
				_oldGradientTexture = oldGradient.gradientTexture.value;
				_oldGradientOpacity = oldGradient.opacity.value;
			}

			if (_changeAberration)
			{
				var oldAberration = _postProcessingController.GetChromaticAberration();

				_oldAberrationIntensity = oldAberration.intensity.value;
			}

			ChangePostProcessing();
        }

        private void OnTriggerExit(Collider other)
        {
            playerExited?.Invoke();
            
            if (!other.GetComponent<PlayerUnit>() || _isChangedOnEnter) return;

            ChangePostProcessing();
        }

        private void ChangePostProcessing()
        {
            if (_changeGodBloom)
            {
                _postProcessingController.SetThresholdGodBloom(_godBloomThreshold, godBloomTimeToChange);
                _postProcessingController.SetIntensityGodBloom(_godBloomIntensity, godBloomTimeToChange);
                _postProcessingController.SetTextureDensityGodBloom(_godBloomTextureDensity, godBloomTimeToChange);
                _postProcessingController.SetTextureCutoffGodBloom(_godBloomTextureCutoff, godBloomTimeToChange);
                _postProcessingController.SetScrollDirectionGodBloom(_godBloomScrollDirection);
                _postProcessingController.SetDistortionAmountGodBloom(_godBloomDistortionAmount, godBloomTimeToChange);
                _postProcessingController.SetDistortionRangeGodBloom(_godBloomDistortionRange);
            }

            if (_changePixelize)
            {
                _postProcessingController.SetScreenHeightPixelize(_pixelizeScreenHeight, pixelizeChangeTime);
            }

			if (changeGradient){
                _postProcessingController.TurnGradient(gradientEnabled);
                _postProcessingController.SetGradientTexture(gradientTexture);
                _postProcessingController.SetGradientIntensity(gradientIntensity, gradientChangeTime);
                _postProcessingController.SetGradientOpacity(gradientOpacity, gradientChangeTime);
			}

            if (_changeAberration)
            {
                _postProcessingController.SetChromaticAberrationIntensity(_aberrationIntensity,
                    _changeAberrationSmooth);
            }
        }

        private void BackPostProcessing()
        {
            playerExited -= BackPostProcessing;
            
            if (_changeGodBloom)
            {
                _postProcessingController.SetThresholdGodBloom(_oldGodBloomThreshold, godBloomTimeToChange);
                _postProcessingController.SetIntensityGodBloom(_oldGodBloomIntensity, godBloomTimeToChange);
                _postProcessingController.SetTextureDensityGodBloom(_oldGodBloomTextureDensity, godBloomTimeToChange);
                _postProcessingController.SetTextureCutoffGodBloom(_oldGodBloomTextureCutoff, godBloomTimeToChange);
                _postProcessingController.SetScrollDirectionGodBloom(_oldGodBloomScrollDirection);
                _postProcessingController.SetDistortionAmountGodBloom(_oldGodBloomDistortionAmount, godBloomTimeToChange);
                _postProcessingController.SetDistortionRangeGodBloom(_oldGodBloomDistortionRange);
            }

            if (_changePixelize)
            {
                _postProcessingController.SetScreenHeightPixelize(_oldPixelizeScreenHeight, pixelizeChangeTime);
            }

			if (changeGradient){
                _postProcessingController.TurnGradient(_oldGradientEnabled);
                _postProcessingController.SetGradientTexture(_oldGradientTexture);
                _postProcessingController.SetGradientIntensity(_oldGradientIntensity, gradientChangeTime);
                _postProcessingController.SetGradientOpacity(_oldGradientOpacity, gradientChangeTime);
			}

            if (_changeAberration)
            {
                _postProcessingController.SetChromaticAberrationIntensity(_oldAberrationIntensity,
                    _changeAberrationSmooth);
            }
        }
    }
}
