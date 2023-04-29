using System;
using System.Collections.Generic;
using NTC.Global.Cache;
using UnityEngine;

public class BaseExplosiveObject : MonoCache, IMakeExplosion
{
    [SerializeField] private LayerMask layersToExplode;

    [SerializeField] private bool explodeOnCollision = true;
    [SerializeField] private bool explodeOnTrigger = true;
    [SerializeField] private bool destroyOnExplosion = true;
    [SerializeField] private bool disableMeshOnExplosion = true;

    [SerializeField] private float ExplodeRadius = 5;
    [SerializeField] private float Damage = 10;

    [SerializeField] private float explosionForce;
    [SerializeField] private float bigExplosionRadiusMultiplier = 2;
    [SerializeField, Min(0.0001f)] private float explosionDuration;
    [SerializeField] private float collisionImmuneDuration = 0.1f;

    private float _timer;

    private bool _isSuper;

    private ITakeHit _hitTakerComponent;

    private Collider[] _attackHitsContainer = new Collider[10];
    private List<int> _objectsAlreadyTakeHit = new();
    
    public event Action<float> OnBigExplosionWithRadius;
    public event Action<float> OnExplosionWithRadius;
    
    

    public Rigidbody Rb;

    private void Awake()
    {
        _hitTakerComponent = Get<ITakeHit>();
        Rb = Get<Rigidbody>();
    }

    protected override void OnEnabled()
    {
        if (_hitTakerComponent != null)
            _hitTakerComponent.OnTakeHit += HandleTakeHit;
    }   
    
    protected override void OnDisabled()
    {
        if (_hitTakerComponent != null)
            _hitTakerComponent.OnTakeHit -= HandleTakeHit;
    }

    //private float _lifetime = 0;
    protected override void Run()
    {
        //_lifetime += Time.deltaTime;
        if (_timer > 0)
        {
            _timer -= Time.deltaTime;
            DamageEveryoneInRadius(ExplodeRadius);
            if (_timer <= 0 && destroyOnExplosion)
            {
                Destroy(gameObject);
            }
        }

        collisionImmuneDuration -= Time.deltaTime;
    }

    public void HandleTakeHit(float dmg)
    {
        if (_timer > 0)
            return;
        
        MakeExplosiveSuper();
        Explode();
    }
    
    private void OnCollisionEnter(Collision col)
    {
        if (!explodeOnCollision || collisionImmuneDuration > 0)
            return;
        
        Explode();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!explodeOnTrigger)
            return;

        Explode();
    }

    public void Explode()
    {
        if (_isSuper)
            OnBigExplosionWithRadius?.Invoke(ExplodeRadius * bigExplosionRadiusMultiplier);
        else
            OnExplosionWithRadius?.Invoke(ExplodeRadius);
        _timer = explosionDuration;
        
        
        DamageEveryoneInRadius(_isSuper ? ExplodeRadius * bigExplosionRadiusMultiplier : ExplodeRadius);

        if (disableMeshOnExplosion)
        {
            GetComponentInChildren<MeshRenderer>().enabled = false;
        }
    }

    /*public void MakeBigExplosion()
    {
        ExplodeRadius *= 2;
        OnBigExplosionWithRadius?.Invoke(ExplodeRadius);
        _timer = explosionDuration;
        DamageEveryoneInRadius(ExplodeRadius);

        if (disableMeshOnExplosion)
        {
            GetComponentInChildren<MeshRenderer>().enabled = false;
        }
    }*/

    public void MakeExplosiveSuper()
    {
        _isSuper = true;
    }

    private void DamageEveryoneInRadius(float radius)
    {
        Array.Clear(_attackHitsContainer, 0, _attackHitsContainer.Length);

        Physics.OverlapSphereNonAlloc(transform.position, 
            radius, _attackHitsContainer, layersToExplode);
        
        for (var i = 0; i < _attackHitsContainer.Length; i++)
        {
            if (_attackHitsContainer[i] is null)
                break;
            
            var hitTransform = _attackHitsContainer[i].transform;
            var hitHash = hitTransform.parent == null
                ? hitTransform.GetHashCode()
                : hitTransform.parent.GetHashCode();

            if (_objectsAlreadyTakeHit.Contains(hitHash))
                continue;
            _objectsAlreadyTakeHit.Add(hitHash);
            
            var hitPosition = _attackHitsContainer[i].ClosestPoint(transform.position);

            var hitType = HitTypes.NormalPoint;

            if (_attackHitsContainer[i].GetComponent<IWeakPoint>() != null)
                hitType = HitTypes.WeakPoint;
            
            _attackHitsContainer[i].GetComponent<ITakeHit>()?.TakeHit(Damage, hitPosition, hitType);
            _attackHitsContainer[i].GetComponent<IMover>()?.AddVelocity((hitTransform.position - transform.position).normalized * explosionForce);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, ExplodeRadius);
    }
}
