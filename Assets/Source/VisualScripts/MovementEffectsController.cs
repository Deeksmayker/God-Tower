using NTC.Global.Cache;
using NTC.Global.Pool;
using UnityEngine;
using UnityEngine.VFX;
using Zenject;

public class MovementEffectsController : MonoCache
{
    [SerializeField] private VisualEffect slidingSparksEffect;
    [SerializeField] private VisualEffect bounceEffect;
    [SerializeField] private VisualEffect dashEffect;

    private bool _dash;

    [Inject] private PostProcessingController _postProcessingController;
    private PlayerMovementController _mover;

    private void Awake()
    {
        if (!slidingSparksEffect)
            Destroy(gameObject);

        _mover = GetComponentInParent<PlayerMovementController>();
        slidingSparksEffect.Stop();
        dashEffect.Stop();
    }

    protected override void OnEnabled()
    {
        _mover.OnBounce += HandleBounce;
        _mover.OnStartDash += HandleStartDash;
        _mover.OnStopDash += HandleEndDash;
    }

    protected override void OnDisabled()
    {
        _mover.OnBounce -= HandleBounce;
        _mover.OnStartDash -= HandleStartDash;
        _mover.OnStopDash -= HandleEndDash;
    }

    protected override void Run()
    {
        if (_mover.IsGrounded() && _mover.GetHorizontalSpeed() > 25) 
        {
            slidingSparksEffect.Play();
        }
        else
        {
            slidingSparksEffect.Stop();
        }
    }

    protected override void LateRun()
    {
        if (_dash)
        {
            _postProcessingController.SetMotionBlurIntensity(1);
        }
    }

    private void HandleBounce(Vector3 normal)
    {
        var effect = NightPool.Spawn(bounceEffect, transform.position);
        effect.transform.rotation = Quaternion.FromToRotation(Vector3.up, normal);
    }

    private void HandleStartDash()
    {
        _dash = true;
        dashEffect.Play();
    }

    private void HandleEndDash()
    {
        _dash = false;
        dashEffect.Stop();
    }

    public bool Sliding() => _mover.IsGrounded() && _mover.GetHorizontalSpeed() > 25;
}