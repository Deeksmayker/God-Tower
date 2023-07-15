using System;
using Cysharp.Threading.Tasks;
using NTC.Global.Cache;
using NTC.Global.Pool;
using UnityEngine;

public class DefaultActiveAbility : MonoCache, IActiveAbility
{
    [SerializeField] private bool infinite;
    [SerializeField] protected Transform directionTarget;
    [SerializeField] protected Transform shootStartPoint;

    [SerializeField] protected float damage;
    
    [SerializeField] private int shootCount;

    [SerializeField] private float abilityLifetime;
    [SerializeField] private float cooldown;

    private float _remainingLifetime = 1;
    private float _cooldownTimer;

    private bool _input;
    private bool _needToPerform;
    private bool _dumping;
    private bool _dumpLoaded;

    private IHealthHandler _healthHandler;
    
    public event Action OnPerform;
    public event Action OnStartHolding;
    public event Action OnEmpty;

    private void Start()
    {
        _remainingLifetime = abilityLifetime;
        _healthHandler = GetComponentInParent<IHealthHandler>();
        _cooldownTimer = cooldown;
    }

    protected override void Run()
    {
        if (_cooldownTimer > 0)
        {
            _cooldownTimer -= Time.deltaTime;
        }

        if (infinite)
            return;
        _remainingLifetime -= Time.deltaTime;
        if (_remainingLifetime <= 0)
        {
            if (_input)
            {
                _dumping = true;
                return;
            }
            RemoveAbility();
        }
    }
    
    /*public void ChargeAbility()
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
    }*/

    public virtual void PerformAbility(int count)
    {
        _cooldownTimer = cooldown;
        OnPerform?.Invoke();
        if (!infinite)
            RemoveAbility();
    }

    /*public void DumpAbility()
    {
        _dumpingTimer -= Time.deltaTime;
        if (_dumpingTimer > 0)
            return;

        //OnDump?.Invoke();
        PerformAbility(true);

        _dumpingTimer = dumpingShootsDelay;
        _remainingChargesToShootOnDump--;

        if (_remainingChargesToShootOnDump < 0)
        {
            if (infinite)
            {
                _remainingChargesToShootOnDump = dumpShootChargesCount;
                _chargingTimer = 0;
                _dumping = false;
                _dumpLoaded = false;
                return;
            }
            
            RemoveAbility();
        }
    }*/

    public async UniTask PerformWithDelay(float delay, int count)
    {
        if (_cooldownTimer > 0)
            return;
        OnStartHolding?.Invoke();
        _cooldownTimer = cooldown;
        await UniTask.Delay(TimeSpan.FromSeconds(delay));

        if (_healthHandler != null)
        {
            if (_healthHandler.InStun())
                return;
        }

        PerformAbility(count);
        _cooldownTimer = cooldown;
    }
    
    public void SetInput(bool value)
    {
        if (_input && !value && _cooldownTimer <= 0)
        {
            PerformAbility(shootCount);
        }
        
        if (!_input && value)
        {
            OnStartHolding?.Invoke();
        }

        _input = value;
    }

    public void SetInfinity(bool value)
    {
        infinite = value;
    }

    public void SetCooldown(float newCooldown)
    {
        cooldown = newCooldown;
    }

    public bool IsInfinite() => infinite;

    public void RemoveAbility()
    {
        OnEmpty?.Invoke();
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

    public float GetCooldown() => cooldown;
    public float GetCooldownProgress() => 1f - _cooldownTimer / cooldown;
    public float GetCooldownTimer() => _cooldownTimer;
    
    public Vector3 GetPerformDirection()
    {
        return directionTarget.forward;
    }

    public Vector3 GetStartPoint()
    {
        return shootStartPoint.position;
    }

    public Transform GetRotationTargetTransform()
    {
        return directionTarget;
    }

    public virtual AbilityTypes GetType()
    {
        return AbilityTypes.None;
    }

    public bool CanPerform()
    {
        return _cooldownTimer <= 0 && !_dumping;
    }
}
