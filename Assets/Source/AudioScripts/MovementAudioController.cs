using NTC.Global.Cache;
using UnityEngine;

public class MovementAudioController : MonoCache
{
    [SerializeField] private float volumeVariation = 0.4f;
    [SerializeField] private float pitchVariation = 0.2f;
    
    [Header("Footsteps")]
    [SerializeField] private AudioClip[] stepClips;
    [SerializeField] private float stepSoundInterval = 0.3f;
    [SerializeField] private float horizontalSpeedThreshold = 10;

    [Header("Jump")]
    [SerializeField] private AudioClip[] jumpClips;

    [Header("Langing")]
    [SerializeField] private AudioClip[] landingClips;

    private float _stepTimer;

    private IMover _mover;
    private IJumper _jumper;
    private AudioSource _audioSource;

    private void Awake()
    {
        _stepTimer = stepSoundInterval;
        _audioSource = Get<AudioSource>();
        _mover = GetComponentInParent<IMover>();
        _jumper = GetComponentInParent<IJumper>();
    }

    protected override void OnEnabled()
    {
        if (_jumper != null)
        {
            _jumper.OnJump += HandleJump;
        }

        if (_mover != null)
        {
            _mover.OnLanding += HandleLanding;
        }
    }

    protected override void OnDisabled()
    {
        if (_jumper != null)
        {
            _jumper.OnJump -= HandleJump;
        }
        
        if (_mover != null)
        {
            _mover.OnLanding -= HandleLanding;
        }
    }
    
    protected override void Run()
    {
        //MakeFootstepsSound();
    }

    private void HandleLanding()
    {
        var clip = AudioUtils.GetRandomClip(landingClips);
        AudioUtils.RandomiseAudioSourceParams(ref _audioSource, true, true, volumeVariation, pitchVariation, 0.3f);
        _audioSource.PlayOneShot(clip);
    }

    private void HandleJump()
    {
        var clip = AudioUtils.GetRandomClip(jumpClips);
        AudioUtils.RandomiseAudioSourceParams(ref _audioSource, true, true, volumeVariation, pitchVariation, 0.3f);
        _audioSource.PlayOneShot(clip);
    }

    private void MakeFootstepsSound()
    {
        if (_mover.IsGrounded() && _mover.GetHorizontalSpeed() > horizontalSpeedThreshold) 
            _stepTimer -= Time.deltaTime;

        if (_stepTimer <= 0)
        {
            var clip = AudioUtils.GetRandomClip(stepClips);
            
            AudioUtils.RandomiseAudioSourceParams(ref _audioSource, true, true, volumeVariation, pitchVariation, 0.5f);
            _audioSource.PlayOneShot(clip);
            _stepTimer = stepSoundInterval;
        }
    }
}
