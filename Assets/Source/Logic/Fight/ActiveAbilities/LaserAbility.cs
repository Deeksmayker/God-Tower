using System;
using NTC.Global.Cache;
using NTC.Global.Pool;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class LaserAbility : DefaultActiveAbility, IMakeLaser
{
    [SerializeField] private LayerMask hitTakerLayers;
    [SerializeField] private LayerMask environmentLayers;

    [SerializeField] private float radius = 0.4f;
    [SerializeField] private float distance = 300f;

    [Header("Dump")]
    [SerializeField] private float spreadAngles;

    public event Action<RaycastHit, Vector3> OnEnvironmentHit;
    public event Action<RaycastHit, Vector3> OnHitToHitTaker;
    public event Action<Vector3> OnMissHit;

    protected override void OnEnabled()
    {
        
    }

    public override void PerformAbility(bool isDumping = false)
    {
        base.PerformAbility(isDumping);

        var startPoint = directionTarget.position;

        var direction = GetPerformDirection();

        if (isDumping)
        {
            var randomNumberX = Random.Range(-spreadAngles, spreadAngles);
            var randomNumberY = Random.Range(-spreadAngles, spreadAngles);
            var randomNumberZ = Random.Range(-spreadAngles, spreadAngles);

            direction = Quaternion.Euler(randomNumberX, randomNumberY, randomNumberZ) * direction;
        }
       

        ShootLaser(startPoint, direction);
    }

    private void ShootLaser(Vector3 startPoint, Vector3 direction)
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
                return;
            }
        }

        if (Physics.Raycast(startPoint, direction, out var envHitInfo, distance, environmentLayers))
        {
            OnEnvironmentHit?.Invoke(envHitInfo, direction);
            return;
        }

        OnMissHit?.Invoke(direction);
    }

    public override AbilityTypes GetType()
    {
        return AbilityTypes.Laser;
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
