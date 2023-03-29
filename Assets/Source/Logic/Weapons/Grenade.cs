using System;
using NTC.Global.Cache;
using NTC.Global.Pool;
using UnityEngine;

public class Grenade : MonoCache, ITakeHit
{
    [SerializeField] private ParticleSystem ExplodeParticle;
    
    public event Action<float> OnTakeHit;
    public event Action<Vector3> OnTakeHitWithPosition;
    public event Action<HitTypes> OnTakeHitWithType;

    public float ExplodeRadius = 1f;
    public float Damage = 10f;

    private Coroutine explodeCoroutine;

    public void TakeHit(float damage, Vector3 hitPosition, HitTypes hitType)
    {
        Explode();
    }
    
    public void StartExplosionThroughTime(float seconds)
    {
        explodeCoroutine = StartCoroutine(Timer.TakeActionAfterTime(seconds, Explode));
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.layer == 8) //EnemyBase
            Explode();
    }

    private void Explode()
    {
        NightPool.Spawn(ExplodeParticle, transform.position);
        Destroy(gameObject);
        
        if (explodeCoroutine != null)
            StopCoroutine(explodeCoroutine);
        
        DamageEveryoneInExplodeRadius();
    }

    private void DamageEveryoneInExplodeRadius()
    {
        var colliders = Physics.OverlapSphere(transform.position, ExplodeRadius);
        
        foreach (var collider in colliders)
        { 
            if (collider.TryGetComponent<ITakeHit>(out var hitTaker) && collider.gameObject.layer != 11) // 11 = Grenade
            {
                hitTaker.TakeHit(Damage, collider.transform.position, HitTypes.NormalPoint);
            }
        }
    }
}
