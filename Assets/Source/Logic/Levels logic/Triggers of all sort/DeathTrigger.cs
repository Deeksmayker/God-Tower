using NTC.Global.Cache;
using UnityEngine;

public class DeathTrigger : MonoCache
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<BaseHealthHandler>(out var health))
        {
            health.Die();
        }
    }
}