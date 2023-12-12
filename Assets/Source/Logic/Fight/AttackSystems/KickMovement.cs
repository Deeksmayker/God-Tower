using NTC.Global.Cache;
using UnityEngine;

public class KickMovement : MonoCache
{
    [SerializeField] private float forwardDashForce;
    [SerializeField] private float hitPayoffForce;

    private IMover _mover;
    private NewKick _kick;

    private void Awake()
    {
        _kick = GetComponentInParent<NewKick>();
        _mover = GetComponentInParent<IMover>();
    }

    protected override void OnEnabled()
    {
        _kick.OnStartAttack += HandleKick;
        _kick.OnHit += HandleHit;
    }

    protected override void OnDisabled()
    {
        _kick.OnStartAttack -= HandleKick;
        _kick.OnHit -= HandleHit;
    }

    private void HandleKick()
    {
        _mover.AddVelocity(_kick.GetAttackDirection() * forwardDashForce);
    }

    private void HandleHit()
    {
        _mover.AddVelocity(_kick.GetAttackDirection() * hitPayoffForce);
    }
}
