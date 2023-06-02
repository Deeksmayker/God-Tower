using NTC.Global.Cache;
using NTC.Global.Pool;
using UnityEngine;
using UnityEngine.VFX;

public class MovementEffectsController : MonoCache
{
    [SerializeField] private VisualEffect slidingSparksEffect;
    [SerializeField] private VisualEffect bounceEffect;

    private IMover _mover;

    private void Awake()
    {
        if (!slidingSparksEffect)
            Destroy(gameObject);

        _mover = GetComponentInParent<IMover>();
        slidingSparksEffect.Stop();
    }

    protected override void OnEnabled()
    {
        _mover.OnBounce += HandleBounce;
    }

    protected override void OnDisabled()
    {
        _mover.OnBounce -= HandleBounce;
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

    private void HandleBounce(Vector3 normal)
    {
        var effect = NightPool.Spawn(bounceEffect, transform.position);
        effect.transform.rotation = Quaternion.FromToRotation(Vector3.up, normal);
    }

    public bool Sliding() => _mover.IsGrounded() && _mover.GetHorizontalSpeed() > 25;
}