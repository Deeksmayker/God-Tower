using System;
using NTC.Global.Cache;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class DefaultAiGroundPlayerChaser : MonoCache, IAiMovementController
{
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private bool rotateByMoveVector = true;

    private Transform _playerTransform;
    
    private NavMeshAgent _agent;
    private IMover _mover;

    private void Awake()
    {
        _agent = Get<NavMeshAgent>();
        _mover = Get<IMover>();
        _playerTransform = Physics.OverlapSphere(transform.position, 1000, playerLayer)[0].transform;
    }

    protected override void OnEnabled()
    {
        if (TryGetComponent<IHealthHandler>(out var health))
        {
           // health.OnStun += Stop;
        }

        _agent.updateRotation = false;
    }

    protected override void OnDisabled()
    {
        if (TryGetComponent<IHealthHandler>(out var health))
        {
          //  health.OnStun -= Stop;
        }
    }
    
    protected override void Run()
    {
        if (!_agent.isStopped)
        {
            SetMoveTarget(_playerTransform.position);
        }
        
        if (rotateByMoveVector && !_agent.velocity.Equals(Vector3.zero))
        {
            transform.rotation = Quaternion.LookRotation(_agent.velocity, Vector3.up);
        }
    }

    public void SetMoveTarget(Vector3 target)
    {
        _agent.destination = target;
    }

    public void Stop()
    {
        _agent.isStopped = true;
    }

    public void ResumeMoving()
    {
        _agent.isStopped = false;
    }

    public void SetRotationToVelocityVector(bool value)
    {
        rotateByMoveVector = value;
    }

    private void FindPlayer()
    {
        
    }
}
