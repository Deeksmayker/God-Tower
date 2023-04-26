using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using NTC.Global.Cache;
using UnityEngine;
using UnityEngine.Serialization;

public enum HomingState
{
    Calm,
    Searching,
    Hunting
}

public class BaseHomingObject : MonoCache, IMakeExplosion
{
    [SerializeField] private LayerMask layersToHoming;

    [Header ("General Homing Settings")]
    [SerializeField] private float Damage = 5f;
    [SerializeField] private float Speed = 1f;
    [SerializeField] private float SecondBeforeHoming = 0.5f;
    [SerializeField] private float Damping = 0.8f;
    [SerializeField] private float SearchRadius = 500f;
    
    [Header ("Super Homing Settings")]
    [SerializeField] private float SuperHomingDamage = 2f;
    [SerializeField] private float SuperHomingSpeed = 2f;
    [SerializeField] private float ExplosionForce = 10f;
    [SerializeField] private float ExplosionRadius = 5f;

    public event Action OnDestroy;
    public event Action OnChangeState;
    public event Action OnSuperHomingActivated;
    
    public event Action<float> OnBigExplosionWithRadius;
    public event Action<float> OnExplosionWithRadius;
    
    private List<Collider> attackHitsContainer = new();
    private List<int> objectsAlreadyTakeHit = new();

    private HomingState homingState;
    private bool isSuperHoming;
    
    private Rigidbody ownRigidbody;
    private Collider ownCollider;
    private ITakeHit hitTakerComponent;

    private GameObject target;
    
    private void Awake()
    {
        hitTakerComponent = Get<ITakeHit>();
        ownRigidbody = Get<Rigidbody>();
        ownCollider = Get<Collider>();
    }

    protected override void OnEnabled()
    {
        hitTakerComponent.OnTakeHit += HandleTakeHit;
    }
    
    protected override void OnDisabled()
    {
        hitTakerComponent.OnTakeHit -= HandleTakeHit;
    }

    private void Start()
    {
        ChangeStateAfterSeconds(HomingState.Searching, SecondBeforeHoming);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isSuperHoming)
        {
            OnExplosionWithRadius?.Invoke(ExplosionRadius);
            TakeDamageInRadius(ExplosionRadius, Damage);
        }
        else
        {
            TakeDamage(other, Damage);
        }
    }

    protected override void Run()
    {
        if (homingState == HomingState.Searching)
        {
            target = FindFavorableTarget();

            if (target)
            {
                ownCollider.isTrigger = true;
                homingState = HomingState.Hunting;
            }
            else
            {
                OnDestroy?.Invoke();
                Destroy(gameObject);
            }
        }

        if (homingState == HomingState.Hunting)
        {
            if (target)
                DirectHomingToTarget(target);
            else
            {
                homingState = HomingState.Searching;
                ownRigidbody.velocity *= 0f; // Обнуляем ускорение, чтобы не выходило на орбиту, а резко меняло направление.
            }
        }
    }

    private void HandleTakeHit(float damage)
    {
        Speed *= SuperHomingSpeed;
        Damage *= SuperHomingDamage;
        isSuperHoming = true;
        
        OnSuperHomingActivated?.Invoke();
    }

    private void DirectHomingToTarget(GameObject target)
    {
        var direction = target.transform.position - transform.position;
        ownRigidbody.velocity += direction * Speed * Time.fixedDeltaTime;
        ownRigidbody.velocity *= Mathf.Clamp01(1f - Damping * Time.fixedDeltaTime);
    }

    private void TakeDamage(Collider other, float damage)
    {
        var hitPosition = other.ClosestPoint(transform.position);

        var hitType = HitTypes.NormalPoint;

        if (other.GetComponent<IWeakPoint>() != null)
            hitType = HitTypes.WeakPoint;

        if (other.TryGetComponent<ITakeHit>(out var takeHit))
        {
            takeHit.TakeHit(damage, hitPosition, hitType);
            OnDestroy?.Invoke();
            Destroy(gameObject);            
        }
    }

    private void TakeDamageInRadius(float radius, float damage)
    {
        attackHitsContainer = Physics.OverlapSphere(transform.position, 
            SearchRadius, layersToHoming).ToList();
        
        for (var i = 0; i < attackHitsContainer.Count; i++)
        {
            if (attackHitsContainer[i] is null)
                break;
            
            var hitTransform = attackHitsContainer[i].transform;
            var hitHash = hitTransform.parent == null
                ? hitTransform.GetHashCode()
                : hitTransform.parent.GetHashCode();

            if (objectsAlreadyTakeHit.Contains(hitHash))
                continue;
            objectsAlreadyTakeHit.Add(hitHash);
            
            var hitPosition = attackHitsContainer[i].ClosestPoint(transform.position);

            var hitType = HitTypes.NormalPoint;

            if (attackHitsContainer[i].GetComponent<IWeakPoint>() != null)
                hitType = HitTypes.WeakPoint;
            
            attackHitsContainer[i].GetComponent<ITakeHit>()?.TakeHit(Damage, hitPosition, hitType);
            attackHitsContainer[i].GetComponent<IMover>()?.AddVelocity((hitTransform.position - transform.position).normalized * ExplosionForce);
        }
    }

    private async UniTask ChangeStateAfterSeconds(HomingState homingState, float seconds)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(seconds));
        
        this.homingState = homingState;
        OnChangeState?.Invoke();
    }

    private GameObject FindFavorableTarget()
    {
        attackHitsContainer = Physics.OverlapSphere(transform.position, 
            SearchRadius, layersToHoming).ToList();

        var targets = attackHitsContainer
            .Where(x => x)
            .Select(x => x.gameObject).ToList();

        for (int i = 0; i < targets.Count; i++)
        {
            if (targets[i].layer is 11) // PlayerProjectile
                return targets[i];
            
            if (targets[i].layer is 12) // EnemyProjectile
                return targets[i];
        }
        
        return FindClosestTarget(targets);
    }

    private GameObject FindClosestTarget(List<GameObject> points)
    {
        if (points == null) return null;

        var distance = Mathf.Infinity;
        GameObject closest = null;

        for (int i = 0; i < points.Count; i++)
        {
            var diff = points[i].transform.position - transform.position;
            var curDistance = diff.sqrMagnitude;

            if (curDistance < distance)
            {
                closest = points[i];
                distance = curDistance;
            }
        }
        
        return closest;
    }
}
