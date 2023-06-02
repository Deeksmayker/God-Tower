using NTC.Global.Cache;
using UnityEngine;

public class MovementAudioController : MonoCache
{
    [SerializeField] private MovementEffectsController effectsController;
    [Header("Sliding")]
    [SerializeField] private AudioSource slidingAudioSource;

    [Header("Jump")]
    [SerializeField] private AudioClip[] jumpClips;

    [Header("Landing")]
    [SerializeField] private AudioClip[] landingClips;

    private float _stepTimer;

    private IMover _mover;
    private IJumper _jumper;
    private AudioSource _audioSource;

    private void Awake()
    {
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
        MakeSlidingSound();
    }

    private void HandleLanding()
    {
        var clip = AudioUtils.GetRandomClip(landingClips);
        AudioUtils.RandomiseAudioSourceParams(ref _audioSource, true, true, 0.2f, 0.1f, 0.3f);
        _audioSource.PlayOneShot(clip);
    }

    private void HandleJump()
    {
        var clip = AudioUtils.GetRandomClip(jumpClips);
        AudioUtils.RandomiseAudioSourceParams(ref _audioSource, true, true, 0.2f, 0.1f, 0.3f);
        _audioSource.PlayOneShot(clip);
    }

    private void HandleBounce()
    {

    }

    private void MakeSlidingSound()
    {
        if (!slidingAudioSource.isPlaying && effectsController.Sliding())
        {
            slidingAudioSource.volume = 0.5f;
            slidingAudioSource.Play();
        }

        if (slidingAudioSource.isPlaying && !effectsController.Sliding())
        {
            slidingAudioSource.volume = Mathf.Lerp(slidingAudioSource.volume, 0, Time.deltaTime * 5);
            if (MathUtils.CompareNumsApproximately(slidingAudioSource.volume, 0, 0.01f))
                slidingAudioSource.Stop();
        }
    }

    /*private void MakeFootstepsSound()
    {
        if (_mover.IsGrounded() && _mover.GetHorizontalSpeed() > horizontalSpeedThreshold) 
            _stepTimer -= Time.deltaTime;

        if (_stepTimer <= 0)
        {
            var clip = AudioUtils.GetRandomClip(stepClips);
            
            AudioUtils.RandomiseAudioSourceParams(ref _audioSource, true, true, 0.2f, 0.1f, 0.5f);
            _audioSource.PlayOneShot(clip);
            _stepTimer = stepSoundInterval;
        }
    }*/
}
