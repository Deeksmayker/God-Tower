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

    public void TakeHit(float damage, Vector3 hitPosition, HitTypes hitType)
    {
        Explode();
    }
    
    private void OnTriggerEnter(Collider collider)
    {
        Explode();
    }

    /// <summary>
    /// Вызов взрыва гранаты.
    /// </summary>
    private void Explode()
    {
        NightPool.Spawn(ExplodeParticle, transform.position);
        Destroy(gameObject);

        DamageEveryoneInExplodeRadius();
    }

    /// <summary>
    /// Выдает урон всем, кто может получить урон и находится в радиусе взрыва.
    /// </summary>
    private void DamageEveryoneInExplodeRadius()
    {
        var colliders = Physics.OverlapSphere(transform.position, ExplodeRadius);
        
        foreach (var collider in colliders)
            if (collider.TryGetComponent<ITakeHit>(out var hitTaker) && collider.gameObject.layer != 11) // 11 = Grenade
                hitTaker.TakeHit(Damage, collider.transform.position, HitTypes.NormalPoint);
    }
}
