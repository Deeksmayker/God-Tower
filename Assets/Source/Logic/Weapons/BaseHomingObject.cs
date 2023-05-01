using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using NTC.Global.Cache;
using NTC.Global.Pool;
using UnityEngine;

public enum HomingState
{
    Calm,
    Searching,
    Hunting
}

public class BaseHomingObject : MonoCache
{
    [SerializeField] private LayerMask layersToHoming;
    [SerializeField] private LayerMask enemyLayers;

    [Header ("General Homing Settings")]
    [SerializeField] private float Damage = 5f;
    [SerializeField] private float Speed = 1f;
    [SerializeField] private float SecondBeforeHoming = 0.5f;
    [SerializeField] private float Damping = 0.8f;
    [SerializeField] private float SearchRadius = 500f;
    [SerializeField] private bool lookForProjectiles = true;
    
    [Header ("Super Homing Settings")]
    [SerializeField] private float SuperHomingDamage = 2f;
    [SerializeField] private float SuperHomingSpeed = 2f;
    [SerializeField] private float ExplosionForce = 10f;
    [SerializeField] private float ExplosionRadius = 5f;
    [SerializeField] private BaseExplosiveObject explosivePrefab;

    private Collider[] _targets = new Collider[10];
    
    private static Dictionary<BaseHomingObject, int> _sHomingTargetsHash = new();

    private bool _isSuperHoming;
    
    public event Action OnDestroy;
    public event Action OnChangeState;
    public event Action OnSuperHomingActivated;

    private HomingState homingState;

    private Rigidbody ownRigidbody;
    private ITakeHit hitTakerComponent;

    private Transform target;
    
    private void Awake()
    {
        hitTakerComponent = Get<ITakeHit>();
        ownRigidbody = Get<Rigidbody>();
    }

    protected override void OnEnabled()
    {
        hitTakerComponent.OnTakeHit += HandleTakeHit;

        ChangeStateAfterSeconds(HomingState.Searching, SecondBeforeHoming);
    }
    
    protected override void OnDisabled()
    {
        hitTakerComponent.OnTakeHit -= HandleTakeHit;

        _sHomingTargetsHash.Remove(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (homingState != HomingState.Hunting)
            return;
        DealDamage(other, Damage);
    }

    protected override void Run()
    {
        if (homingState == HomingState.Searching)
        {
            target = FindFavorableTarget();

            if (target)
            {
                if (_sHomingTargetsHash.ContainsKey(this))
                    _sHomingTargetsHash[this] = target.GetHashCode();
                else
                    _sHomingTargetsHash.Add(this, target.GetHashCode());
                //ownCollider.isTrigger = true;
                homingState = HomingState.Hunting;
            }
            else
            {
                /*OnDestroy?.Invoke();
                Destroy(gameObject);*/
            }
        }

        if (homingState == HomingState.Hunting)
        {
            if (target)
                DirectHomingToTarget(target);
            else
            {
                homingState = HomingState.Searching;
            }
        }
    }

    private void HandleTakeHit(float damage)
    {
        BecomeSuperHoming();
    }

    private void DirectHomingToTarget(Transform targetTransform)
    {
        var direction = targetTransform.position - transform.position;
        ownRigidbody.AddForce(Speed * Time.fixedDeltaTime * direction, ForceMode.Acceleration);
        ownRigidbody.velocity *= Mathf.Clamp01(1f - Damping * Time.fixedDeltaTime);

        ownRigidbody.rotation = Quaternion.LookRotation(ownRigidbody.velocity);
    }

    public void BecomeSuperHoming()
    {
        if (_isSuperHoming)
            return;
        
        Speed *= SuperHomingSpeed;
        //Damage *= SuperHomingDamage;
        NightPool.Spawn(explosivePrefab, transform);
        gameObject.AddComponent<ExplosiveObjectController>();
        _isSuperHoming = true;
        homingState = HomingState.Searching;
        layersToHoming = enemyLayers;

        OnSuperHomingActivated?.Invoke();
    }

    private async UniTask DealDamage(Collider other, float damage)
    {
        var hitPosition = transform.position;

        var hitType = HitTypes.NormalPoint;

        if (other.GetComponent<IWeakPoint>() != null)
            hitType = HitTypes.WeakPoint;

        if (other.TryGetComponent<BaseExplosiveObject>(out var explosive))
        {
            explosive.MakeExplosiveSuper();
            explosive.Explode();
            BecomeSuperHoming();
            return;
        }

        if (other.TryGetComponent<ITakeHit>(out var takeHit))
        {
            takeHit.TakeHit(damage, hitPosition, hitType);
            OnDestroy?.Invoke();
            await UniTask.NextFrame();
            Destroy(gameObject);            
        }
    }

    private async UniTask ChangeStateAfterSeconds(HomingState newState, float seconds)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(seconds));

        /*if (homingState >= newState)
            return;*/
        
        homingState = newState;
        OnChangeState?.Invoke();
    }

    private Transform FindFavorableTarget()
    {
        Physics.OverlapSphereNonAlloc(transform.position,
            SearchRadius, _targets, layersToHoming);

        if (!lookForProjectiles)
            return FindClosestTarget(_targets);
        
        for (int i = 0; i < _targets.Length; i++)
        {
            if (_targets[i] == null || _sHomingTargetsHash.ContainsValue(_targets[i].transform.GetHashCode()))
                continue;
            
            if (_targets[i].gameObject.layer is 11 && !_targets[i].TryGetComponent<BaseHomingObject>(out var homing)) // PlayerProjectile
                return _targets[i].transform;
            
            if (_targets[i].gameObject.layer is 12 && !_targets[i].TryGetComponent<BaseHomingObject>(out var hominge)) // EnemyProjectile
                return _targets[i].transform;
        }
        
        return FindClosestTarget(_targets);
    }

    private Transform FindClosestTarget(Collider[] points)
    {
        if (points == null) return null;

        var distance = Mathf.Infinity;
        Collider closest = null;

        for (int i = 0; i < points.Length; i++)
        {
            if (points[i] == null
                || _sHomingTargetsHash.ContainsValue(points[i].transform.GetHashCode())
                || points[i].TryGetComponent<BaseHomingObject>(out var homing))
                continue;

            var healthHandler = points[i].GetComponentInParent<IHealthHandler>();
            if (healthHandler.IsDead())
                continue;
            
            var diff = points[i].transform.position - transform.position;
            var curDistance = diff.sqrMagnitude;

            if (curDistance < distance)
            {
                closest = points[i];
                distance = curDistance;
            }
        }
        
        return closest == null ? null : closest.transform;
    }
}
