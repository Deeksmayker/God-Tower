using System;
using NTC.Global.Cache;
using UnityEngine;


public class LaserAbility : MonoCache, IActiveAbility, IMakeLaser
{
    [SerializeField] private LayerMask hitTakerLayers;
    [SerializeField] private Transform directionTarget;
    [SerializeField] private Transform startPoint;
    [SerializeField] private Vector3 range;
    [SerializeField] private int damage;
    [SerializeField] private float cooldown;

    private float _timer;

    private bool _input;
    
    public event Action OnPerform;
    public event Action OnSurge;
    public event Action<RaycastHit> OnEnvironmentHit;
    public event Action<RaycastHit> OnHitToHitTaker;
    public event Action OnMissHit;

    protected override void Run()
    {
        if (_timer > 0)
        {
            _timer -= Time.deltaTime;
        }
        
        if (_timer <= 0 && _input)
        {
            PerformAbility();
            _timer = cooldown;
        }
    }
    
    public void ChargeAbility()
    {
        Debug.LogError("No charge for laser ability");
    }

    public void PerformAbility()
    {
        OnPerform?.Invoke();

        var hitBoxCenter = directionTarget.position + directionTarget.forward * (range.z / 2.0f);

        if (Physics.BoxCast(hitBoxCenter, range / 2, GetPerformDirection(), out var hitInfo,
                directionTarget.rotation, 100, hitTakerLayers))
        {
            if (hitInfo.transform.TryGetComponent<IWeakPoint>(out var weakPoint))
            {
                OnHitToHitTaker?.Invoke(hitInfo);
                weakPoint.TakeWeakPointHit(damage, hitInfo.point);
            }

            else if (hitInfo.transform.TryGetComponent<ITakeHit>(out var hitTaker))
            {
                OnHitToHitTaker?.Invoke(hitInfo);
                hitTaker.TakeHit(damage, hitInfo.point);
            }

            else
            {
                OnEnvironmentHit?.Invoke(hitInfo);
            }
            
            return;
        }
        
        OnMissHit?.Invoke();
    }

    public void SurgeAbility()
    {
        throw new NotImplementedException();
    }

    public void SetInput(bool value)
    {
        _input = value;
    }

    public Vector3 GetPerformDirection()
    {
        return directionTarget.forward;
    }

    public Vector3 GetStartPoint()
    {
        return startPoint.position;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.matrix = directionTarget.localToWorldMatrix;
        Gizmos.DrawWireCube(Vector3.forward * (range.z / 2.0f), range);
    }
}
