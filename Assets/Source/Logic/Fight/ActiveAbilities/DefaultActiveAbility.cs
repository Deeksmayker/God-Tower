using System;
using NTC.Global.Cache;
using NTC.Global.Pool;
using UnityEngine;

public class DefaultActiveAbility : MonoCache, IActiveAbility
{
    [SerializeField] protected Transform directionTarget;
    [SerializeField] protected Transform shootStartPoint;

    [SerializeField] protected float damage;
    [SerializeField] protected int maxCharges;

    [SerializeField] protected float defaultTimeToCharge;
    [SerializeField, Range(0.1f, 1f)] protected float chargeTimeDecreaseMultiplier;
    [SerializeField] protected float dumpingDelay;
    [SerializeField] protected float cooldown;

    protected float _reloadTimer;
    protected float _chargingTimer;
    protected float _dumpingDelayTimer;

    protected int _remainingCharges;
    protected int _chargesLoaded;

    protected bool _input;
    protected bool _needToPerform;
    
    public event Action OnPerform;
    public event Action OnStartDumping;
    public event Action OnEmpty;

    protected override void OnEnabled()
    {
        _remainingCharges = maxCharges;
        _chargingTimer = 0;
    }

    protected override void Run()
    {
        if (_needToPerform)
        {
            _dumpingDelayTimer = dumpingDelay;
        }
        
        if (_reloadTimer > 0)
        {
            _reloadTimer -= Time.deltaTime;
        }

        if (_reloadTimer <= 0 && _input)
        {
            ChargeAbility();
        }

        if (_dumpingDelayTimer > 0)
        {
            _dumpingDelayTimer -= Time.deltaTime;
            if (_dumpingDelayTimer <= 0)
            {
                PerformAbility();
                _chargesLoaded--;
                _remainingCharges--;
                if (_chargesLoaded > 0)
                    _dumpingDelayTimer = dumpingDelay;
                else
                {
                    _chargingTimer = 0;
                }

                if (GetRemainingChargesCount() <= 0)
                {
                    OnEmpty?.Invoke();
                    Destroy(gameObject);
                }
            }
        }

       

        _needToPerform = false;
    }
    
    public void ChargeAbility()
    {
        if (_chargesLoaded >= GetRemainingChargesCount())
            return;
        
        if (_chargingTimer.Equals(0))
        {
            _chargesLoaded = 1;
            OnStartDumping?.Invoke();
        }
            
        _chargingTimer += Time.deltaTime;

        var currentTimeToCharge = defaultTimeToCharge;

        for (var i = 0; i < _chargesLoaded - 1; i++)
        {
            currentTimeToCharge += defaultTimeToCharge * chargeTimeDecreaseMultiplier;
        }

        if (_chargingTimer >= currentTimeToCharge)
        {
            _chargesLoaded++;
        }
    }

    public virtual void PerformAbility()
    {
        OnPerform?.Invoke();
    }

    public void SetInput(bool value)
    {
        if (_input && !value)
        {
            _needToPerform = true;
        }
        
        _input = value;
    }

    public void SetRotationTarget(Transform rotationTarget)
    {
        directionTarget = rotationTarget;
    }

    public void SetShootPoint(Transform shootPoint)
    {
        shootStartPoint = shootPoint;
    }

    public int GetRemainingChargesCount()
    {
        return _remainingCharges;
    }

    public Vector3 GetPerformDirection()
    {
        return directionTarget.forward;
    }

    public Vector3 GetStartPoint()
    {
        return shootStartPoint.position;
    }
}
