using System;
using System.Collections.Generic;
using NTC.Global.Cache;
using UnityEngine;
using static IMeleeAttacker;

public class Kicker : MonoCache, IMeleeAttacker
{
    [SerializeField] private Transform directionTarget;
    [SerializeField] private LayerMask layersToHit;
    [SerializeField] private Vector3 hitBoxSize;

    [Header("Impact")]
    [SerializeField] private int damage = 1;
    [SerializeField] private float forwardRecoilBeforePunch;
    [SerializeField] private Vector3 payoffPowerVector;

    [Header("Timers")]
    [SerializeField] private float preparingTime;
    [SerializeField] private float attackDuration;
    [SerializeField] private float cooldown;

    private Collider[] _attackHitsContainer = new Collider[10];
    private List<int> _objectsAlreadyTakeHit = new();

    private bool _canAttack = true;
    private bool _attackInput;
    private bool _isHitAnything;

    private float _timer;

    private MeleeAttackStates _attackState = MeleeAttackStates.Resting;
    
    private IMover _mover;
    
    public event Action OnStartPreparingAttack;
    public event Action OnStartAttack;
    public event Action OnEndAttack;

    private void Awake()
    {
        _mover = Get<IMover>();
    }

    protected override void Run()
    {
        if (!_canAttack)
            return;
        
        if (NeedToAttack())
            SetAttackStateToNext();

        if (GetCurrentAttackState() == MeleeAttackStates.Attacking)
        {
            PerformAttack();
        }

        if (GetCurrentAttackState() != MeleeAttackStates.Resting)
        {
            _timer -= Time.deltaTime;
            if (_timer <= 0)
                SetAttackStateToNext();
        }
    }

    [ContextMenu("Imitate Kick")]
    public void PerformAttack()
    {
        
        Array.Clear(_attackHitsContainer, 0, _attackHitsContainer.Length);

        var hitBoxCenter = directionTarget.position + directionTarget.forward * (hitBoxSize.z / 2.0f);
        
        Physics.OverlapBoxNonAlloc(hitBoxCenter, 
            hitBoxSize / 2, _attackHitsContainer, directionTarget.rotation, layersToHit);
        
        for (var i = 0; i < _attackHitsContainer.Length; i++)
        {
            if (_attackHitsContainer[i] is null)
                break;

            var hitParentHash = _attackHitsContainer[i].transform.parent.GetHashCode();

            if (_objectsAlreadyTakeHit.Contains(hitParentHash))
                continue;
            _objectsAlreadyTakeHit.Add(hitParentHash);
            
            var hitPosition = _attackHitsContainer[i].ClosestPoint(hitBoxCenter);

            var hitType = HitTypes.NormalPoint;

            if (_attackHitsContainer[i].GetComponent<IWeakPoint>() != null)
                hitType = HitTypes.WeakPoint;
            
            _attackHitsContainer[i].GetComponent<ITakeHit>()?.TakeHit(damage, hitPosition, hitType);
        }

        if (_attackHitsContainer[0] is not null && !_isHitAnything)
        {
            _mover.SetVerticalVelocity(payoffPowerVector.y);
            _mover.AddVelocity(new Vector3(
                -GetAttackDirection().x * payoffPowerVector.x,
                0,
                -GetAttackDirection().z * payoffPowerVector.z));
            _isHitAnything = true;
        }
    }

    private void HandleStartPreparingAttack()
    {
        
    }

    private void HandleStartAttack()
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
                _isHitAnything = false;
                _timer = GetAttackCooldown();
                break;
        }
    }

    private void MakeForwardRecoil()
    {
        var resultVelocity = _mover.GetVelocity();
        resultVelocity += GetAttackDirection() * forwardRecoilBeforePunch;
        resultVelocity = resultVelocity.magnitude * GetAttackDirection();
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
        return cooldown;
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
