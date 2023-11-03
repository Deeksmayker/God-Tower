using DG.Tweening;
using NTC.Global.Cache;
using UnityEngine;

public class GroundSlider : MonoCache
{
    [SerializeField] private ParticleSystem slidingParticles;
    [SerializeField] private float baseSlideSpeed;
    [SerializeField] private float slideDuration = 0.5f;
 //   [SerializeField] private float slideJumpAddedSpeed;
    [SerializeField] private float speedIncreaseRate = 10;
    [SerializeField] private float colliderHeight;

    [SerializeField] private ShakePreset slidingShake;

    private float _defaultHeight;
    private float _timer;
    
    private bool _input;
    private bool _canSlide = true;
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
        _timer -= Time.deltaTime;

        if (_mover.IsGrounded() && _canSlide && _input && !_sliding && _timer <= 0)
        {
            StartSlide();
            _canSlide = false;
        }

        if (!_input && _sliding)
        {
            StopSlide();
        }

        if (_sliding)
        {
            //var multiplier = _mover.GetHorizontalSpeed() > baseSlideSpeed ? speedIncreaseRate / 10 : speedIncreaseRate;
            //_mover.SetHorizontalVelocity(Vector3.MoveTowards(_mover.GetVelocity(), _slideDirection * baseSlideSpeed, speedIncreaseRate * Time.deltaTime));

            if (_timer <= 0)
            {
                StopSlide();
            }
        }
    }

    private void StartSlide()
    {
        slidingParticles.Play();
        _sliding = true;
        
        _ch.height = colliderHeight;
       // _mover.RecalculateGroundCheckerPosition();
        
        //_slideDirection = _mover.GetHorizontalInput().x * transform.right +
          //                _mover.GetHorizontalInput().y * transform.forward;
        if (_slideDirection.Equals(Vector3.zero))
            _slideDirection = transform.forward;
        //_mover.SetHorizontalVelocity(_slideDirection * (_mover.GetVelocity().magnitude / 2 > baseSlideSpeed ? _mover.GetVelocity().magnitude / 2 : baseSlideSpeed));
        _mover.SetInputResponse(false);
        _mover.SetVerticalVelocity(-30);

        _timer = slideDuration;
        
        ShakeCameraOnSliding();
    }

    private void ShakeCameraOnSliding()
    {
        /*CameraService.Instance.ShakeCameraPosition(slidingShake).OnComplete(() =>
        {
            if (_sliding)
                ShakeCameraOnSliding();
        });*/
    }

    private void StopSlide()
    {
        slidingParticles.Stop();
        _ch.height = _defaultHeight;
        _sliding = false;
        //_mover.RecalculateGroundCheckerPosition();
        _mover.SetInputResponse(true);
    }
    
    public void SetInput(bool value)
    {
        if (!value && _input)
            _canSlide = true;
        _input = value;
    }
}
