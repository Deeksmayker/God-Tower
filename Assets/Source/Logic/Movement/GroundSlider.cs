using DG.Tweening;
using NTC.Global.Cache;
using UnityEngine;

public class GroundSlider : MonoCache
{
    [SerializeField] private ParticleSystem slidingParticles;
    [SerializeField] private float baseSlideSpeed;
 //   [SerializeField] private float slideJumpAddedSpeed;
    [SerializeField] private float speedDecreasingRate;
    [SerializeField] private float colliderHeight;

    [SerializeField] private ShakePreset slidingShake;

    private float _defaultHeight;
    
    private bool _input;
    private bool _sliding;

    private Vector3 _slideDirection;
    
    private CharacterController _ch;
    private IMover _mover;
    private IJumper _jumper;

    private void Awake()
    {
        _ch = Get<CharacterController>();
        _mover = Get<IMover>();
        _jumper = Get<IJumper>();
        _defaultHeight = _ch.height;    
    }

    protected override void OnEnabled()
    {
        if (_jumper != null)
        {
            _jumper.OnJump += StopSlide;
        }
    }
    
    protected override void OnDisabled()
    {
        if (_jumper != null)
        {
            _jumper.OnJump -= StopSlide;
        }
    }
    
    protected override void Run()
    {
        if (_mover.IsGrounded() && _input && !_sliding)
        {
            StartSlide();
        }

        if (!_input && _sliding)
        {
            StopSlide();
        }

        if (_sliding)
        {
            _mover.SetHorizontalVelocity(Vector3.MoveTowards(_mover.GetVelocity(), _slideDirection * baseSlideSpeed, speedDecreasingRate * Time.deltaTime));
        }
    }

    private void StartSlide()
    {
        slidingParticles.Play();
        _ch.height = colliderHeight;
        _sliding = true;
        _mover.RecalculateGroundCheckerPosition();
        _slideDirection = _mover.GetHorizontalInput().x * transform.right +
                          _mover.GetHorizontalInput().y * transform.forward;
        if (_slideDirection.Equals(Vector3.zero))
            _slideDirection = transform.forward;
        _mover.SetHorizontalVelocity(_slideDirection * (_mover.GetVelocity().magnitude / 2 > baseSlideSpeed ? _mover.GetVelocity().magnitude / 2 : baseSlideSpeed));
        _mover.SetInputResponse(false);
        _mover.SetVerticalVelocity(-30);
        ShakeCameraOnSliding();
    }

    private void ShakeCameraOnSliding()
    {
        CameraService.Instance.ShakeCameraPosition(slidingShake).OnComplete(() =>
        {
            if (_sliding)
                ShakeCameraOnSliding();
        });
    }

    private void StopSlide()
    {
        slidingParticles.Stop();
        _ch.height = _defaultHeight;
        _sliding = false;
        _mover.RecalculateGroundCheckerPosition();
        _mover.SetInputResponse(true);
    }
    
    public void SetInput(bool value)
    {
        _input = value;
    }
}
