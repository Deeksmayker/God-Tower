using NTC.Global.Cache;
using UnityEngine;

public class WaterTrigger : MonoCache
{
    [SerializeField] private bool _damagePlayer;
    [SerializeField] private float playerPushForce;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PlayerUnit>(out var player) && _damagePlayer)
        {
            player.GetComponentInChildren<ITakeHit>().TakeHit(1, player.transform.position - Vector3.down, "Toxic sludge");
        }
    }

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