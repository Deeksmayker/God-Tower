using System;
using NTC.Global.Cache;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;


public class LaserAbility : MonoCache, IActiveAbility, IMakeLaser
{
    [SerializeField] private LayerMask hitTakerLayers;
    [SerializeField] private Transform directionTarget;
    [SerializeField] private Transform shootStartPoint;
    [SerializeField] private float radius = 0.4f;
    [SerializeField] private float distance = 300f;
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

        var startPoint = directionTarget.position;

        if (Physics.SphereCast(startPoint, radius, GetPerformDirection(), out var hitInfo, distance, hitTakerLayers))
        {
            if (hitInfo.transform.TryGetComponent<IWeakPoint>(out var weakPoint))
            {
                Debug.Log("weak point");
                OnHitToHitTaker?.Invoke(hitInfo);
                weakPoint.TakeWeakPointHit(damage, hitInfo.point);
            }

            else if (hitInfo.transform.TryGetComponent<ITakeHit>(out var hitTaker))
            {
                Debug.Log("hit point");
                OnHitToHitTaker?.Invoke(hitInfo);
                hitTaker.TakeHit(damage, hitInfo.point);
            }

            else
            {
                Debug.Log("environment hit " + hitInfo.collider.name);
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

    public void SetRotationTarget(Transform rotationTarget)
    {
        directionTarget = rotationTarget;
    }

    public void SetShootPoint(Transform shootPoint)
    {
        shootStartPoint = shootPoint;
    }

    public Vector3 GetPerformDirection()
    {
        return directionTarget.forward;
    }

    public Vector3 GetStartPoint()
    {
        return shootStartPoint.position;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        
        var firstPoint = directionTarget.position;
        var endPoint = firstPoint + directionTarget.forward * distance;

        var direction = (endPoint - firstPoint).normalized;
        
        Gizmos.DrawWireSphere(firstPoint, radius);
        Gizmos.DrawWireSphere(endPoint, radius);
        Gizmos.DrawLine(firstPoint + directionTarget.up * radius, endPoint + directionTarget.up * radius);
        Gizmos.DrawLine(firstPoint - directionTarget.up * radius, endPoint - directionTarget.up * radius);
        Gizmos.DrawLine(firstPoint + directionTarget.right * radius, endPoint + directionTarget.right * radius);
        Gizmos.DrawLine(firstPoint - directionTarget.right * radius, endPoint - directionTarget.right * radius);
        //Gizmos.DrawLine(firstPoint + directionTarget.up * radius, firstPoint - directionTarget.up * radius);
        //Gizmos.DrawLine(endPoint + directionTarget.up * radius, endPoint - directionTarget.up * radius);
    }
}
