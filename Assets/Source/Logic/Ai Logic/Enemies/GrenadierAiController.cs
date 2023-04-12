using System;
using DG.Tweening;
using NTC.Global.Cache;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GrenadierAiController : MonoCache, IAiController
{
    [SerializeField] private LayerMask layersToAttack;
    [SerializeField] private LayerMask environmentLayers;
    [SerializeField] private Transform rotationTarget;
    [SerializeField] private float timeBeforeShootToRotateHead = 0.3f;
    
    private Transform _target;
    
    private Vector3 _position;
    
    private IAiMovementController _movementController;
    private IAiRangeAttackController _rangeAttackController;
    private GrenadeAbility _grenadeAbility;

    private void Awake()
    {
        _movementController = Get<IAiMovementController>();
        _rangeAttackController = Get<IAiRangeAttackController>();
        _grenadeAbility = GetComponentInChildren<GrenadeAbility>();
        
        _target = Physics.OverlapSphere(transform.position, 1000, layersToAttack)[0].transform;
    }
    
    protected override void OnEnabled()
    {
        _grenadeAbility.OnStartHolding += HandleStartChargingGrenadeAttack;
        _grenadeAbility.OnPerform += HandlePerformingGrenadeAttack;
        
        if (TryGetComponent<IHealthHandler>(out var health))
        {
            health.OnDying += HandleDying;
        }
    }

    protected override void OnDisabled()
    {
        _grenadeAbility.OnStartHolding -= HandleStartChargingGrenadeAttack;
        _grenadeAbility.OnPerform -= HandlePerformingGrenadeAttack;
        
        if (TryGetComponent<IHealthHandler>(out var health))
        {
            health.OnDying -= HandleDying;
        }
    }

    private void HandleStartChargingGrenadeAttack()
    {
        var pos = rotationTarget.position;
        var targetPos = _target.position;
        var distanceToTarget = Vector3.Distance(pos, targetPos);

        var launchAngle = CalculateLaunchAngle(distanceToTarget, _grenadeAbility.GetThrowPower(), pos.y - targetPos.y,
            Physics.gravity.y);

        var mover = _target.GetComponentInParent<IMover>();
        if (mover != null)
        {
            Debug.Log("mover");
            var moverVelocity = mover.GetVelocity();
            moverVelocity.y = 0;
            targetPos += moverVelocity * CalculateTimeOfFlight(distanceToTarget, launchAngle * Mathf.Deg2Rad,
                _grenadeAbility.GetThrowPower(), Physics.gravity.y);
            Debug.Log(mover.GetVelocity());
            Debug.Log(targetPos);
            
            distanceToTarget = Vector3.Distance(pos, targetPos);
            launchAngle = CalculateLaunchAngle(distanceToTarget, _grenadeAbility.GetThrowPower(), pos.y - targetPos.y,
                Physics.gravity.y);
        }
        
        
        //var fl
        //var launchDirection = Quaternion.Euler(launchAngle, 0, 0f) * Vector3.forward; // Step 3
        //Vector3 rotatedDirection = rotationTarget.transform.TransformDirection(launchDirection); // Step 4
        //Quaternion targetRotation = Quaternion.LookRotation(targetPos);
        //targetRotation.eulerAngles =
            //new Vector3(launchAngle, targetRotation.eulerAngles.y, targetRotation.eulerAngles.z);
       //Debug.Log(CalculateTimeOfFlight(distanceToTarget, launchAngle * Mathf.Deg2Rad, _grenadeAbility.GetThrowPower(), Physics.gravity.y));
        rotationTarget.LookAt(targetPos);
        rotationTarget.eulerAngles = new Vector3(launchAngle, rotationTarget.rotation.eulerAngles.y, 0);
        //Debug.Log(rotatedDirection);
        //rotationTarget.rotation = targetRotation;
        //rotationTarget.rotation 
    }
    
    float CalculateLaunchAngle(float distanceToTarget, float projectileVelocity, float heightDifference, float gravity)
    {
        float angleRad = Mathf.Atan((Mathf.Pow(projectileVelocity, 2f) - Mathf.Sqrt(Mathf.Pow(projectileVelocity, 4f) -
            gravity * (gravity * Mathf.Pow(distanceToTarget, 2f) +
                       2f * heightDifference * Mathf.Pow(projectileVelocity, 2f)))) / (gravity * distanceToTarget));
        return angleRad * Mathf.Rad2Deg; // Convert to degrees before returning
    }
    
    float CalculateTimeOfFlight(float distanceToTarget, float launchAngleRad, float projectileVelocity, float gravity)
    {
        gravity = Mathf.Abs(gravity);
        float height = Mathf.Abs(_target.position.y - rotationTarget.position.y);
        
        float time = (distanceToTarget / (projectileVelocity * Mathf.Cos(launchAngleRad))) +
                     Mathf.Sqrt(2f * height / gravity) - 1;

        return Mathf.Max(time, 0.1f);
    }

    private void HandlePerformingGrenadeAttack()
    {
        
    }

    private void HandleDying()
    {
        
    }

    public bool CanAttack()
    {
        return !Physics.Raycast(rotationTarget.position, _target.position - rotationTarget.position,
            Vector3.Distance(rotationTarget.position, _target.position), environmentLayers);
    }
}
