using System;
using System.Collections.Generic;
using NTC.Global.Cache;
using UnityEngine;
using static IMeleeAttacker;

public class Kicker : MonoCache, IMeleeAttacker
{
    [SerializeField] private Transform directionTarget;
    [SerializeField] private LayerMask environmentLayers;
    [SerializeField] private LayerMask layersToHit;
    [SerializeField] private Vector3 hitBoxSize;
    [SerializeField] private Vector3 environmentHitboxSize;

    [Header("Impact")]
    [SerializeField] private int damage = 1;
    [SerializeField] private float minDashPower;
    [SerializeField] private float maxDashPower;
    [SerializeField] private Vector3 minPayoffPowerVector;
    [SerializeField] private Vector3 maxPayoffPowerVector;

    [Header("Timers")]
    [SerializeField] private float preparingTime;
    [SerializeField] private float attackDuration;
    [SerializeField] private float cooldown;

    [Header("Parry")]
    [SerializeField] private bool allowParry = true;
    [SerializeField] private float parryProjectileBoostVelocity = 70;
    [SerializeField] private float parryHeal = 50f;

    private Collider[] _attackHitsContainer = new Collider[10];
    private List<int> _objectsAlreadyTakeHit = new();

    private bool _canAttack = true;
    private bool _attackInput;
    private bool _isHitAnything;

    private float _timer;
    private float _currentDashPower;

    private Vector3 _currentPayoffPowerVector;

    private MeleeAttackStates _attackState = MeleeAttackStates.Resting;
    
    private IMover _mover;
    private BaseHealthHandler _healthHandler;
    private PlayerStyleController _playerStyleController;
    
    public event Action OnStartPreparingAttack;
    public event Action OnStartAttack;
    public event Action OnEndAttack;
    public event Action OnHit;
    public event Action OnParry;

    private void Awake()
    {
        _mover = Get<IMover>();
        _healthHandler = Get<BaseHealthHandler>();
        _playerStyleController = Get<PlayerStyleController>();  
    }

    protected override void OnEnabled()
    {
        OnParry += HealOnParry;
    }

    protected override void OnDisabled()
    {
        OnParry -= HealOnParry;
    }

    protected override void Run()
    {
        if (!_canAttack)
            return;

        _currentDashPower = Mathf.Lerp(minDashPower, maxDashPower, _playerStyleController.GetCurrentStyle01());
        _currentPayoffPowerVector = Vector3.Lerp(minPayoffPowerVector, maxPayoffPowerVector, _playerStyleController.GetCurrentStyle01());
        
        if (NeedToAttack())
            SetAttackStateToNext();

        if (GetCurrentAttackState() == MeleeAttackStates.Attacking)
        {
            PerformAttack(layersToHit, hitBoxSize);
            PerformAttack(environmentLayers, environmentHitboxSize);
        }

        if (GetCurrentAttackState() != MeleeAttackStates.Resting)
        {
            _timer -= Time.deltaTime;
            if (_timer <= 0)
                SetAttackStateToNext();
        }
    }

    [ContextMenu("Imitate Kick")]
    public void PerformAttack(LayerMask layers, Vector3 hitbox)
    {
        Array.Clear(_attackHitsContainer, 0, _attackHitsContainer.Length);

        var hitBoxCenter = directionTarget.position + directionTarget.forward * (hitbox.z / 2.0f);
        
        Physics.OverlapBoxNonAlloc(hitBoxCenter, 
            hitbox / 2, _attackHitsContainer, directionTarget.rotation, layers);
        
        for (var i = 0; i < _attackHitsContainer.Length; i++)
        {
            if (!_attackHitsContainer[i])
                break;

            var hitTransform = _attackHitsContainer[i].transform;

            if (hitTransform.parent)
                hitTransform = hitTransform.parent;
            
            var hitParentHash = hitTransform.GetHashCode();

            if (_objectsAlreadyTakeHit.Contains(hitParentHash))
                continue;
            _objectsAlreadyTakeHit.Add(hitParentHash);

            var hitPosition = hitBoxCenter;

            var hitType = HitTypes.NormalPoint;

            if (allowParry && _attackHitsContainer[i].TryGetComponent<BaseExplosiveObject>(out var explosive))
            {
                explosive.MakeExplosiveSuper();
                explosive.RestoreCollisionImmune();
                var direction = GetAttackDirection();
                direction.y -= 0.1f;
                explosive.Rb.velocity = direction * parryProjectileBoostVelocity;
                OnParry?.Invoke();
                return;
            }

            if (allowParry && _attackHitsContainer[i].TryGetComponent<BaseHomingObject>(out var homing))
            {
                homing.BecomeSuperHoming();
                homing.ownRigidbody.velocity += (GetAttackDirection() * 100);
                OnParry?.Invoke();
                return;
            }
            
            _attackHitsContainer[i].GetComponent<ITakeHit>()?.TakeHit(damage, hitPosition, hitType);
            
            OnHit?.Invoke();
        }

        if (_attackHitsContainer[0] && !_isHitAnything)
        {
            _mover.SetVerticalVelocity(_currentPayoffPowerVector.y);
            _mover.AddVelocity(new Vector3(
                GetAttackDirection().x * _currentPayoffPowerVector.x,
                0,
                GetAttackDirection().z * _currentPayoffPowerVector.z));
            _isHitAnything = true;
            _timer /= 2;
        }
    }

    private void HandleStartPreparingAttack()
    {
        
    }

    private void HandleStartAttack()
    {
        
    }

    private void HealOnParry()
    {
        _healthHandler.AddHealth(parryHeal);
    }

    private void ReduceCooldown()
    {

    }

    public void SetInput(bool input)
    {
        _attackInput = input;
    }

    public void SetAttackStateToNext()
    {
        _attackState++;
        if (_attackState > MeleeAttackStates.Cooldown)
            _attackState = 0;
        switch (_attackState)
        {
            case MeleeAttackStates.Resting:
                _timer = 0;
                break;
            case MeleeAttackStates.Preparing:
                OnStartPreparingAttack?.Invoke();
                _timer = GetAttackPrepareTime();
                break;
            case MeleeAttackStates.Attacking:
                OnStartAttack?.Invoke();
                MakeForwardRecoil();
                _timer = GetAttackDuration();
                break;
            case MeleeAttackStates.Cooldown:
                OnEndAttack?.Invoke();
                _objectsAlreadyTakeHit.Clear();
                _timer = GetAttackCooldown();
                _isHitAnything = false;
                break;
        }
    }

    private void MakeForwardRecoil()
    {
        var resultVelocity = _mover.GetVelocity();
        resultVelocity += GetAttackDirection() * _currentDashPower;
        resultVelocity = resultVelocity.magnitude * GetAttackDirection();

        /*if (GetAttackDirection().y < -0.5f)
            resultVelocity *= 2f;*/

        resultVelocity.y = Mathf.Clamp(resultVelocity.y, -100, 20);
        _mover.SetVelocity(resultVelocity);
    }

    public void AllowAttack()
    {
        _canAttack = true;
    }

    public void DisallowAttack()
    {
        _canAttack = false;
    }

    public MeleeAttackStates GetCurrentAttackState()
    {
        return _attackState;
    }

    public float GetAttackPrepareTime()
    {
        return preparingTime;
    }

    public float GetAttackDuration()
    {
        return attackDuration;
    }

    public float GetAttackCooldown()
    {
        return _isHitAnything ? cooldown / 2 : cooldown;
    }

    public float GetTimerValueForCurrentAttackState()
    {
        switch (_attackState)
        {
            case MeleeAttackStates.Resting:
                return 0;
            case MeleeAttackStates.Preparing:
                return GetAttackPrepareTime();
            case MeleeAttackStates.Attacking:
                return GetAttackDuration();
            case MeleeAttackStates.Cooldown:
                return GetAttackCooldown();
            default:
                return -1;
        }
    }

    public Vector3 GetAttackDirection()
    {
        return directionTarget.forward;
    }

    public bool NeedToAttack()
    {
        return _attackInput && GetCurrentAttackState() == MeleeAttackStates.Resting;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.matrix = directionTarget.localToWorldMatrix;
        Gizmos.DrawWireCube(Vector3.forward * (hitBoxSize.z / 2.0f), hitBoxSize);
        
        /*Collider[] colliders = Physics.OverlapBox(directionTarget.position + directionTarget.forward * (hitBoxSize.z /  2.0f), 
            hitBoxSize / 2, directionTarget.rotation, layersToHit);

        // Draw a wireframe around each collider
        foreach (Collider collider in colliders)
        {
            if (collider is not null)
            {
                Debug.Log(collider.name);
            }
        }*/

    }
}
