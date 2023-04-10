using System;
using NTC.Global.Cache;
using NTC.Global.Pool;
using UnityEngine;

public class DefaultActiveAbility : MonoCache, IActiveAbility
{
    [SerializeField] private bool infinite;
    [SerializeField] protected Transform directionTarget;
    [SerializeField] protected Transform shootStartPoint;

    [SerializeField] protected float damage;
    
    [SerializeField] private int dumpShootChargesCount;

    [SerializeField] private float holdTimeToDump;
    [SerializeField] private float dumpingShootsDelay;
    [SerializeField] private float abilityLifetime;
    [SerializeField] private float cooldown;

    private int _remainingChargesToShootOnDump;

    private float _remainingLifetime;
    private float _cooldownTimer;
    private float _chargingTimer;
    private float _dumpingTimer;

    private bool _input;
    private bool _needToPerform;
    private bool _dumping;
    private bool _dumpLoaded;
    
    public event Action OnPerform;
    public event Action OnStartHolding;
    public event Action OnDumpLoaded;
    public event Action OnEmpty;
    public event Action OnDump;

    private void Awake()
    {
        _remainingChargesToShootOnDump = dumpShootChargesCount;
        _remainingLifetime = abilityLifetime;
    }

    protected override void Run()
    {
        if (_dumping)
        {
            DumpAbility();
            return;
        }
        
        if (_cooldownTimer > 0)
        {
            _cooldownTimer -= Time.deltaTime;
        }

        if (_needToPerform && _cooldownTimer <= 0)
        {
            PerformAbility();
            _cooldownTimer = cooldown;
        }
        
        if (_input)
        {
            ChargeAbility();
        }
        
        _needToPerform = false;

        if (infinite)
            return;
        _remainingLifetime -= Time.deltaTime;
        if (_remainingLifetime <= 0)
        {
            if (_chargingTimer >= holdTimeToDump)
            {
                _dumping = true;
                return;
            }
            
            OnEmpty?.Invoke();
            RemoveAbility();
        }
    }
    
    public void ChargeAbility()
    {
        if (_chargingTimer.Equals(0))
        {
            OnStartHolding?.Invoke();
        }

        _chargingTimer += Time.deltaTime;

        if (!_dumpLoaded && _chargingTimer >= holdTimeToDump)
        {
            OnDumpLoaded?.Invoke();
            _dumpLoaded = true;
        }
    }

    public virtual void PerformAbility()
    {
        _chargingTimer = 0;
        OnPerform?.Invoke();
    }

    public void DumpAbility()
    {
        _dumpingTimer -= Time.deltaTime;
        if (_dumpingTimer > 0)
            return;

        if (OnDump != null)
            OnDump.Invoke();
        else
            PerformAbility();

        _dumpingTimer = dumpingShootsDelay;
        _remainingChargesToShootOnDump--;

        if (_remainingChargesToShootOnDump < 0)
        {
            if (infinite)
            {
                _remainingChargesToShootOnDump = dumpShootChargesCount;
                _dumping = false;
                _dumpLoaded = false;
                return;
            }
            
            OnEmpty?.Invoke();
            RemoveAbility();
        }
    }

    public void PerformWithDelay(float delay)
    {
        if (_cooldownTimer > 0)
            return;
        OnStartHolding?.Invoke();
        Invoke(nameof(PerformAbility), delay);
        _cooldownTimer = cooldown;
    }

    public void SetInput(bool value)
    {
        if (_input && !value)
        {
            if (_chargingTimer >= holdTimeToDump)
            {
                _dumping = true;
                _dumpingTimer = dumpingShootsDelay;
            }
            else
            {
                _needToPerform = true;
            }
        }
        
        _input = value;
    }

    public void RemoveAbility()
    {
        Destroy(gameObject);
    }

    public void SetRotationTarget(Transform rotationTarget)
    {
        directionTarget = rotationTarget;
    }

    public void SetShootPoint(Transform shootPoint)
    {
        shootStartPoint = shootPoint;
    }

    public float GetRemainingLifetime()
    {
        return _remainingLifetime;
    }

    public float GetMaxLifetime()
    {
        return abilityLifetime;
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
