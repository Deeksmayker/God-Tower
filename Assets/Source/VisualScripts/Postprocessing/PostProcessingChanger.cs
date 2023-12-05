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
		[SerializeField] private float timeToChange = 1;

        [Header("God Bloom")]
        [SerializeField] private bool _changeGodBloom;
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
        [SerializeField] private int _pixelizeScreenHeight;

        private int _oldPixelizeScreenHeight;

		[Header("Gradient")]
		[SerializeField] private bool changeGradient;
        [SerializeField] private bool gradientEnabled;
        [SerializeField] private Texture gradientTexture;
        [SerializeField] private float gradientIntensity;
		[SerializeField] private float gradientOpacity;
        
        private bool _oldGradientEnabled;
        private Texture _oldGradientTexture;
        private float _oldGradientIntensity;
		private float _oldGradientOpacity;

        [Header("Chromatic aberration")] 
        [SerializeField] private bool _changeAberration;
        [SerializeField] private float _aberrationIntensity;
        
        private float _oldAberrationIntensity;

		private bool _inside;
		private bool _needChange;

		private bool _weOnWaveController;

        private event Action playerExited;

		private static float s_changingTimer;
		private static bool s_changedThisFrame;

		protected override void OnEnabled(){
			var wave = GetComponent<Wave>();
			if (wave){
				wave.OnEnded += StartThis;
			}

			var waveController = GetComponent<WaveController>();
			if (waveController){
				_weOnWaveController = true;
				waveController.WavesStarted.AddListener(StartThis);
				waveController.WavesEnded.AddListener(End);
			}
		}

		protected override void OnDisabled(){
			var wave = GetComponent<Wave>();
			if (wave){
				wave.OnEnded += StartThis;
			}
		}

		protected override void Run(){
			if (!s_changedThisFrame && s_changingTimer > 0){
				s_changedThisFrame = true;
				s_changingTimer -= Time.deltaTime;
			}

			if (s_changedThisFrame && s_changingTimer <= 0 && _needChange){
				if (_inside)
					HandleEnter();
				else
					HandleExit();
				_needChange = false;
			}
		}

		protected override void LateRun(){
			s_changedThisFrame = false;
		}

        private void OnTriggerEnter(Collider other)
        {
            if (_weOnWaveController || !other.GetComponent<PlayerUnit>() || !_isChangedOnEnter) return;
			StartThis();

        }

        private void OnTriggerExit(Collider other){
			if (_weOnWaveController || !other.GetComponent<PlayerUnit>())
				return;
			End();
        }

		private void StartThis(){
			_inside = true;
			if (s_changingTimer > 0){
				_needChange = !_needChange;
				return;
			}

			HandleEnter();
		}

		private void End(){
			_inside = false;
			if (s_changingTimer > 0){
				_needChange = !_needChange;
				return;
			}

			HandleExit();
            
            if (_isChangedOnEnter) return;

            ChangePostProcessing();
		}

		private void HandleEnter(){

			s_changingTimer = timeToChange + 0.5f;

            if (!_changeBackOnExit)
            {
				ChangePostProcessing();
				return;
            }

			playerExited += BackPostProcessing;
			
			RememberOldValues();

			ChangePostProcessing();
		}

		private void HandleExit(){
			if (_changeBackOnExit){
				s_changingTimer = timeToChange + 0.5f;
			}
            playerExited?.Invoke();
		}

        private void ChangePostProcessing()
        {
            if (_changeGodBloom)
            {
                _postProcessingController.SetThresholdGodBloom(_godBloomThreshold, timeToChange);
                _postProcessingController.SetIntensityGodBloom(_godBloomIntensity, timeToChange);
                _postProcessingController.SetTextureDensityGodBloom(_godBloomTextureDensity, timeToChange);
                _postProcessingController.SetTextureCutoffGodBloom(_godBloomTextureCutoff, timeToChange);
                _postProcessingController.SetScrollDirectionGodBloom(_godBloomScrollDirection);
                _postProcessingController.SetDistortionAmountGodBloom(_godBloomDistortionAmount, timeToChange);
                _postProcessingController.SetDistortionRangeGodBloom(_godBloomDistortionRange);
            }

            if (_changePixelize)
            {
                _postProcessingController.SetScreenHeightPixelize(_pixelizeScreenHeight, timeToChange);
            }

			if (changeGradient){
				if (gradientEnabled)
					_postProcessingController.TurnGradient(gradientEnabled);
                _postProcessingController.SetGradientTexture(gradientTexture, timeToChange);
                _postProcessingController.SetGradientIntensity(gradientIntensity, timeToChange);
				if (gradientTexture != null)
					_postProcessingController.SetGradientOpacity(gradientOpacity, timeToChange);
			}

            if (_changeAberration)
            {
                _postProcessingController.SetChromaticAberrationIntensity(_aberrationIntensity,
                    timeToChange);
            }
        }

        private void BackPostProcessing()
        {
            playerExited -= BackPostProcessing;
            
            if (_changeGodBloom)
            {
                _postProcessingController.SetThresholdGodBloom(_oldGodBloomThreshold, timeToChange);
                _postProcessingController.SetIntensityGodBloom(_oldGodBloomIntensity, timeToChange);
                _postProcessingController.SetTextureDensityGodBloom(_oldGodBloomTextureDensity, timeToChange);
                _postProcessingController.SetTextureCutoffGodBloom(_oldGodBloomTextureCutoff, timeToChange);
                _postProcessingController.SetScrollDirectionGodBloom(_oldGodBloomScrollDirection);
                _postProcessingController.SetDistortionAmountGodBloom(_oldGodBloomDistortionAmount, timeToChange);
                _postProcessingController.SetDistortionRangeGodBloom(_oldGodBloomDistortionRange);
            }

            if (_changePixelize)
            {
                _postProcessingController.SetScreenHeightPixelize(_oldPixelizeScreenHeight, timeToChange);
            }

			if (changeGradient){
				if (_oldGradientEnabled)
					_postProcessingController.TurnGradient(_oldGradientEnabled);
                _postProcessingController.SetGradientTexture(_oldGradientTexture, timeToChange);
                _postProcessingController.SetGradientIntensity(_oldGradientIntensity, timeToChange);
				if (_oldGradientTexture != null){
					_postProcessingController.SetGradientOpacity(_oldGradientOpacity, timeToChange);
				}
			}

            if (_changeAberration)
            {
                _postProcessingController.SetChromaticAberrationIntensity(_oldAberrationIntensity,
                    timeToChange);
            }
        }

		private void RememberOldValues(){
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
				var oldPixelize = _postProcessingController.GetPixelizeEffectComponent();
				_oldPixelizeScreenHeight = oldPixelize.screenHeight.value;
			}

			if (changeGradient){
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
		}
    }
}
