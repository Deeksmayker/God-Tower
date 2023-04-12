using System;
using System.Collections.Generic;
using NTC.Global.Cache;
using NTC.Global.Pool;
using Unity.VisualScripting;
using UnityEngine;

public class BaseExplosiveObject : MonoCache, IMakeExplosion
{
    [SerializeField] private LayerMask layersToExplode;

    [SerializeField] private bool explodeOnCollision = true;
    [SerializeField] private bool explodeOnTrigger = true;

    [SerializeField] private float explosionForce;
    [SerializeField] private float bigExplosionRadiusMultiplier = 2;
    [SerializeField, Min(0.0001f)] private float explosionDuration;
    [SerializeField] private float collisionImmuneDuration = 0.1f;

    private float _timer;

    private ITakeHit _hitTakerComponent;

    private Collider[] _attackHitsContainer = new Collider[10];
    private List<int> _objectsAlreadyTakeHit = new();
    
    public event Action<float> OnBigExplosionWithRadius;
    public event Action<float> OnExplosionWithRadius;
    
    public float ExplodeRadius = 1f;
    public float Damage = 10f;

    private void Awake()
    {
        _hitTakerComponent = Get<ITakeHit>();
    }

    protected override void OnEnabled()
    {
        _hitTakerComponent.OnTakeHit += HandleTakeHit;
    }
    
    protected override void OnDisabled()
    {
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
            if (_timer <= 0)
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
        ExplodeRadius *= bigExplosionRadiusMultiplier;
        OnBigExplosionWithRadius?.Invoke(ExplodeRadius);
        Explode(ExplodeRadius);
    }
    
    private void OnCollisionEnter(Collision col)
    {
        if (!explodeOnCollision || collisionImmuneDuration > 0)
            return;
        
        OnExplosionWithRadius?.Invoke(ExplodeRadius);
        Explode(ExplodeRadius);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!explodeOnTrigger || collisionImmuneDuration > 0)
            return;

        OnExplosionWithRadius?.Invoke(ExplodeRadius);
        Explode(ExplodeRadius);
    }

    private void Explode(float radius)
    {
        //Debug.Log("lifetime - " + _lifetime);
        _timer = explosionDuration;
        GetComponentInChildren<MeshRenderer>().enabled = false;
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
