using UnityEngine;
using NTC.Global.Cache;

public class ExplosiveObjectController : MonoCache
{
    [SerializeField] private bool explodeOnCollision, explodeOnTrigger = true;

    private BaseExplosiveObject connectedExplosiveObject;

    protected override void OnEnabled()
    {
        connectedExplosiveObject = GetComponentInChildren<BaseExplosiveObject>();    
        
        if (connectedExplosiveObject == null)
        {
            Debug.LogError("No Explosive object detected, script will destroy yourself");
            Destroy(this);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (!explodeOnCollision)
            return;
        connectedExplosiveObject.MakeBigExplosion();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!explodeOnTrigger)
            return;
        connectedExplosiveObject.MakeBigExplosion();
    }
}