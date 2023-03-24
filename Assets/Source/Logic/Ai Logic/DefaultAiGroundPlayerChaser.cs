using System;
using NTC.Global.Cache;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class DefaultAiGroundPlayerChaser : MonoCache, IAiMovementController
{
    [SerializeField] private LayerMask playerLayer;

    private Transform _playerTransform;
    
    private NavMeshAgent _agent;

    private void Awake()
    {
        _agent = Get<NavMeshAgent>();
        _playerTransform = Physics.OverlapSphere(transform.position, 1000, playerLayer)[0].transform;
    }

    protected override void OnEnabled()
    {
        if (TryGetComponent<IHealthHandler>(out var health))
        {
            health.OnDying += Stop;
        }
        
    }

    protected override void OnDisabled()
    {
        if (TryGetComponent<IHealthHandler>(out var health))
        {
            health.OnDying -= Stop;
        }
    }
    
    protected override void Run()
    {
        if (!_agent.isStopped)
        {
            SetMoveTarget(_playerTransform.position);
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

    private void FindPlayer()
    {
        
    }
}
