using NTC.Global.Cache;
using System;
using Unity.VisualScripting;
using UnityEngine;

public class LaserShooter : MonoCache, IMakeLaser, IImpacter
{
    [SerializeField] private LayerMask hitTakerLayers;
    [SerializeField] private LayerMask environmentLayers;

    [SerializeField] private float damage = 20;
    [SerializeField] private float radius = 3;
    [SerializeField] private float distance = 300;

    public event Action<RaycastHit, Vector3> OnEnvironmentHit;
    public event Action<RaycastHit, Vector3> OnHitToHitTaker;
    public event Action<Vector3> OnMissHit;
    public event Action<Vector3> OnImpact;

    public Vector3 GetStartPoint()
    {
        return transform.position;
    }

    public void ShootLaser(Vector3 startPoint, Vector3 direction)
    {
        if (Physics.SphereCast(startPoint, radius, direction, out var hitInfo, distance, hitTakerLayers))
        {
            var hitTransform = hitInfo.transform;

            var hitType = HitTypes.NormalPoint;

            if (hitTransform.GetComponent<IWeakPoint>() != null)
            {
                hitType = HitTypes.WeakPoint;
            }

            if (hitInfo.transform.TryGetComponent<ITakeHit>(out var hitTaker))
            {
                OnHitToHitTaker?.Invoke(hitInfo, direction);
                hitTaker.TakeHit(damage, hitInfo.point, hitType);
                OnImpact?.Invoke(hitInfo.point);
                return;
            }
        }

        if (Physics.Raycast(startPoint, direction, out var envHitInfo, distance, environmentLayers))
        {
            OnEnvironmentHit?.Invoke(envHitInfo, direction);
            OnImpact?.Invoke(envHitInfo.point);
            return;
        }

        OnMissHit?.Invoke(direction);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;

        var firstPoint = transform.position;
        var endPoint = firstPoint + transform.forward * distance;

        var direction = (endPoint - firstPoint).normalized;

        Gizmos.DrawWireSphere(firstPoint, radius);
        Gizmos.DrawWireSphere(endPoint, radius);
        Gizmos.DrawLine(firstPoint + transform.up * radius, endPoint + transform.up * radius);
        Gizmos.DrawLine(firstPoint - transform.up * radius, endPoint - transform.up * radius);
        Gizmos.DrawLine(firstPoint + transform.right * radius, endPoint + transform.right * radius);
        Gizmos.DrawLine(firstPoint - transform.right * radius, endPoint - transform.right * radius);
        //Gizmos.DrawLine(firstPoint + directionTarget.up * radius, firstPoint - directionTarget.up * radius);
        //Gizmos.DrawLine(endPoint + directionTarget.up * radius, endPoint - directionTarget.up * radius);
    }
}