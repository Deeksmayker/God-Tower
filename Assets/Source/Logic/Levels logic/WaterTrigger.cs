using NTC.Global.Cache;
using UnityEngine;

public class WaterTrigger : MonoCache
{
    [SerializeField] private float playerPushForce;

    private void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent<PlayerUnit>(out var player))
        {
            player.Get<IMover>().AddVerticalVelocity(playerPushForce * Time.deltaTime);
        }

        else if (other.TryGetComponent<IHealthHandler>(out var health))
        {
            health.Die();
        }
    }
}