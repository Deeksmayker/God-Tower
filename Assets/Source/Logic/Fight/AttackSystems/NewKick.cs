using NTC.Global.Cache;
using System;
using System.Collections.Generic;
using UnityEngine;
using static IMeleeAttacker;

public class NewKick : MonoCache, IMeleeAttacker
{
    [SerializeField] private Transform directionTarget;
    [SerializeField] private Vector3 hitBoxSize;
    [SerializeField] private Vector3 envHitBoxSize;

    [Header("Impact")]
    [SerializeField] private int damage = 1;
    [SerializeField] private float kickPushForce = 50;

    [Header("Timers")]
    [SerializeField] private float preparingTime;
    [SerializeField] private float attackDuration;
    [SerializeField] private float cooldown;

    [Header("Parry")]
    [SerializeField] private bool allowParry = true;
    [SerializeField] private float parryDuration;

    private Collider[] _attackHitsContainer = new Collider[10];
    private List<int> _objectsAlreadyTakeHit = new();

    private bool _canAttack = true;
    private bool _attackInput;
    private bool _isHitAnything;

    private float _timer;
    private float _baseKickDashForce;

    private MeleeAttackStates _attackState = MeleeAttackStates.Resting;

    public event Action OnStartPreparingAttack;
    public event Action OnStartAttack;
    public event Action OnEndAttack;
    public event Action OnHit;
    public event Action OnParry;

    private void Awake()
    {
    }

    /*protected override void OnEnabled()
    {
        OnHit += HandleHit;
    }

    protected override void OnDisabled()
    {
        OnHit -= HandleHit;
    }*/

    protected override void Run()
    {
        if (!_canAttack)
            return;

        if (NeedToAttack())
            SetAttackStateToNext();

        if (GetCurrentAttackState() == MeleeAttackStates.Attacking)
        {
            PerformAttack(Layers.EnemyHurtBox, hitBoxSize);

            if (_timer >= attackDuration / 2)
                PerformAttack(Layers.Environment, envHitBoxSize);
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

            _attackHitsContainer[i].GetComponent<ITakeHit>()?.TakeHit(damage, hitPosition, "Player Kick");
            _attackHitsContainer[i].GetComponentInParent<IMover>()?.AddForce(GetAttackDirection() * kickPushForce);
            _attackHitsContainer[i].GetComponentInParent<IInStun>()?.StartStun();

            if (_attackHitsContainer[i].TryGetComponent<Rigidbody>(out var rb))
            {
                rb.AddForce(GetAttackDirection() * kickPushForce);
            }
        }

        if (_attackHitsContainer[0] && !_isHitAnything)
        {
            OnHit?.Invoke();
            HandleHit();
            _isHitAnything = true;
            _timer /= 2;
        }
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

    private void HandleHit()
    {
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
        return _isHitAnything ? cooldown / 4 : cooldown;
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

