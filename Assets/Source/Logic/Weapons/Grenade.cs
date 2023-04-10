using System;
using NTC.Global.Cache;
using NTC.Global.Pool;
using Unity.VisualScripting;
using UnityEngine;

public class Grenade : MonoCache, ITakeHit, IMakeExplosion
{
    [SerializeField] private ParticleSystem ExplodeParticle;
    [SerializeField] private ParticleSystem BigExplodeParticle;
    
    public event Action<float> OnTakeHit;
    public event Action<Vector3> OnTakeHitWithPosition;
    public event Action<HitTypes> OnTakeHitWithType;

    public event Action OnBigExplosion;
    public event Action OnExplosion;
    
    public float ExplodeRadius = 1f;
    public float Damage = 10f;

    protected override void OnEnabled()
    {
        OnExplosion += StartExplosionParticle;
        OnBigExplosion += StartBigExplosionParticle;
    }
    
    protected override void OnDisabled()
    {
        OnExplosion -= StartExplosionParticle;
        OnBigExplosion -= StartBigExplosionParticle;
    }

    public void TakeHit(float damage, Vector3 hitPosition, HitTypes hitType)
    {
        OnBigExplosion?.Invoke();
        Explode(ExplodeRadius*2);
    }
    
    private void OnTriggerEnter(Collider collider)
    {
        OnExplosion?.Invoke();
        Explode(ExplodeRadius);
    }
    
    private void Explode(float radius)
    {
        Destroy(gameObject);
        DamageEveryoneInRadius(radius);
    }

    private void StartExplosionParticle()
    {
        NightPool.Spawn(ExplodeParticle, transform.position);
    }

    private void StartBigExplosionParticle()
    {
        NightPool.Spawn(BigExplodeParticle, transform.position);
    }

    private void DamageEveryoneInRadius(float radius)
    {
        var colliders = Physics.OverlapSphere(transform.position, radius);
        
        foreach (var collider in colliders)
            if (collider.TryGetComponent<ITakeHit>(out var hitTaker) && collider.gameObject.layer != 11) // 11 = Grenade
                hitTaker.TakeHit(Damage, collider.transform.position, HitTypes.NormalPoint);
    }
}
