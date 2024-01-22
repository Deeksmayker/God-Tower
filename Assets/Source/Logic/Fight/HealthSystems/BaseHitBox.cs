using NTC.Global.Cache;
using UnityEngine;

public class BaseHitBox : MonoCache
{
    [SerializeField] private float damage;
    [SerializeField] private string attackerName = "Unnamed hit box";

    private void OnCollisionEnter(Collision collision)
    {
        var player = collision.gameObject.GetComponentInParent<PlayerUnit>() ;
        if (player)
        {
            player.GetComponentInChildren<ITakeHit>()?.TakeHit(damage, transform.position, attackerName);
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.GetComponentInParent<PlayerUnit>() && collision.gameObject.TryGetComponent<ITakeHit>(out var victim))
        {
            victim.TakeHit(damage, transform.position, attackerName);
        }
    }

    public void SetDamage(float dmg){
        damage = dmg;
    }
}
