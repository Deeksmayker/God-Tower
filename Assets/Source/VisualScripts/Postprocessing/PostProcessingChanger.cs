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
        [SerializeField] private float _changeGodBloomSmooth = 1;
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
        [SerializeField] private float _changePixelizeSmooth = 1;
        [SerializeField] private bool _pixelizeEnabled;
        [SerializeField] private Texture _pixelizeGradientTexture;
        [SerializeField] private float _pixelizeIntensity;
        [SerializeField] private int _pixelizeScreenHeight;
        
        private bool _oldPixelizeEnabled;
        private Texture _oldPixelizeGradientTexture;
        private float _oldPixelizeIntensity;
        private int _oldPixelizeScreenHeight;

        [Header("Chromatic aberration")] 
        [SerializeField] private bool _changeAberration;
        [SerializeField] private float _changeAberrationSmooth = 1;
        [SerializeField] private float _aberrationIntensity;
        
        private float _oldAberrationIntensity;

        private event Action playerExited;

        private void OnTriggerEnter(Collider other)
        {
            if (!other.GetComponent<PlayerUnit>() || !_isChangedOnEnter) return;

            if (_changeBackOnExit)
            {
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
                    var oldPixelize = _postProcessingController.GetPixelizeEffectComponent();

                    _oldPixelizeEnabled = oldPixelize.Enabled.value;
                    _oldPixelizeIntensity = oldPixelize.intensity.value;
                    _oldPixelizeGradientTexture = oldPixelize.gradientTexture.value;
                    _oldPixelizeScreenHeight = oldPixelize.screenHeight.value;
                }

                if (_changeAberration)
                {
                    var oldAberration = _postProcessingController.GetChromaticAberration();

                    _oldAberrationIntensity = oldAberration.intensity.value;
                }
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
                _postProcessingController.SetThresholdGodBloom(_godBloomThreshold, _changeGodBloomSmooth);
                _postProcessingController.SetIntensityGodBloom(_godBloomIntensity, _changeGodBloomSmooth);
                _postProcessingController.SetTextureDensityGodBloom(_godBloomTextureDensity, _changeGodBloomSmooth);
                _postProcessingController.SetTextureCutoffGodBloom(_godBloomTextureCutoff, _changeGodBloomSmooth);
                _postProcessingController.SetScrollDirectionGodBloom(_godBloomScrollDirection);
                _postProcessingController.SetDistortionAmountGodBloom(_godBloomDistortionAmount, _changeGodBloomSmooth);
                _postProcessingController.SetDistortionRangeGodBloom(_godBloomDistortionRange);
            }

            if (_changePixelize)
            {
                _postProcessingController.TurnPixelize(_pixelizeEnabled);
                _postProcessingController.SetGradientTexturePixelize(_pixelizeGradientTexture);
                _postProcessingController.SetIntensityPixelize(_pixelizeIntensity, _changePixelizeSmooth);
                _postProcessingController.SetScreenHeightPixelize(_pixelizeScreenHeight, _changePixelizeSmooth);
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
                _postProcessingController.SetThresholdGodBloom(_oldGodBloomThreshold, _changeGodBloomSmooth);
                _postProcessingController.SetIntensityGodBloom(_oldGodBloomIntensity, _changeGodBloomSmooth);
                _postProcessingController.SetTextureDensityGodBloom(_oldGodBloomTextureDensity, _changeGodBloomSmooth);
                _postProcessingController.SetTextureCutoffGodBloom(_oldGodBloomTextureCutoff, _changeGodBloomSmooth);
                _postProcessingController.SetScrollDirectionGodBloom(_oldGodBloomScrollDirection);
                _postProcessingController.SetDistortionAmountGodBloom(_oldGodBloomDistortionAmount, _changeGodBloomSmooth);
                _postProcessingController.SetDistortionRangeGodBloom(_oldGodBloomDistortionRange);
            }

            if (_changePixelize)
            {
                _postProcessingController.TurnPixelize(_oldPixelizeEnabled);
                _postProcessingController.SetGradientTexturePixelize(_oldPixelizeGradientTexture);
                _postProcessingController.SetIntensityPixelize(_oldPixelizeIntensity, _changePixelizeSmooth);
                _postProcessingController.SetScreenHeightPixelize(_oldPixelizeScreenHeight, _changePixelizeSmooth);
            }

            if (_changeAberration)
            {
                _postProcessingController.SetChromaticAberrationIntensity(_oldAberrationIntensity,
                    _changeAberrationSmooth);
            }
        }
    }
}