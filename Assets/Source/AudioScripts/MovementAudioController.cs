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

    [SerializeField] private AudioClip[] hookClips;

    private float _stepTimer;

    private IMover _mover;
    private IJumper _jumper;
    private Hook _hook;
    private AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = Get<AudioSource>();
        _mover = GetComponentInParent<IMover>();
        _jumper = GetComponentInParent<IJumper>();
        _hook = GetComponentInParent<Hook>();
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

        if (_hook)
        {
            _hook.OnHook += HandleHook;
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

        if (_hook)
        {
            _hook.OnHook -= HandleHook; 
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

    private void HandleHook()
    {
        PlayOneSound(hookClips);
    }

    private void HandleBounce()
    {

    }

    private void PlayOneSound(AudioClip[] clips, float volumeVariation = 0.2f, float pitchVariation = 0.1f, float baseVolume = 1f)
    {
        var clip = AudioUtils.GetRandomClip(clips);
        AudioUtils.RandomiseAudioSourceParams(ref _audioSource, true, true, volumeVariation, pitchVariation, baseVolume);
        _audioSource.PlayOneShot(clip);
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
