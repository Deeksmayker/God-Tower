using NTC.Global.Cache;
using UnityEngine;

public class BaseHitBox : MonoCache
{
    [SerializeField] private float damage;
    [SerializeField] private string attackerName = "Unnamed hit box";

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent<ITakeHit>(out var victim))
        {
            victim.TakeHit(damage, transform.position, attackerName);
        }
    }
}