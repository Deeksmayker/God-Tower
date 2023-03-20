using System;
using NTC.Global.Cache;
using NTC.Global.Pool;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;


public class LaserAbility : DefaultActiveAbility, IMakeLaser
{
    [SerializeField] private LayerMask hitTakerLayers;

    [SerializeField] private float radius = 0.4f;
    [SerializeField] private float distance = 300f;
    
    public event Action<RaycastHit> OnEnvironmentHit;
    public event Action<RaycastHit> OnHitToHitTaker;
    public event Action OnMissHit;

    protected override void OnEnabled()
    {
        _remainingCharges = maxCharges;
    }

    public override void PerformAbility()
    {
        base.PerformAbility();

        var startPoint = directionTarget.position;

        if (Physics.SphereCast(startPoint, radius, GetPerformDirection(), out var hitInfo, distance, hitTakerLayers))
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
