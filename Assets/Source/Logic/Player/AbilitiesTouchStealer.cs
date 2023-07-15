using NTC.Global.Cache;
using UnityEngine;

public class AbilitiesTouchStealer : MonoCache
{
    [SerializeField] private float addedVerticalSpeed;

    private PlayerMovementController _mover;
    private AbilitiesHandler _abilitiesHandler;

    private void Awake()
    {
        _mover = GetComponentInParent<PlayerMovementController>();
        _abilitiesHandler = GetComponentInParent<AbilitiesHandler>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<IGiveAbility>(out var giver) && giver.CanGiveAbility())
        {
            _abilitiesHandler.SetNewLeftAbility(giver);
        }

        if (other.TryGetComponent<BaseHealthHandler>(out var health) && health.InStun())
        {
            health.Die();

            _mover.StopDash();
            _mover.SetVerticalVelocity(addedVerticalSpeed);
        }
    }
}