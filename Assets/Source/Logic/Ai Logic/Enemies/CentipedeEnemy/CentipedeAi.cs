using UnityEngine;
using NTC.Global.Cache;

public class CentipedeAi : MonoCache
{
    [SerializeField] private Transform headTarget;
    [SerializeField] private float attackCooldown;
    [SerializeField] private float headSpeed = 5f;

    private bool _rangeAttack;
    private bool _meleeAttack;
    private bool _attacking;

    private Vector3 _startHeadTargetPos;
    private Vector3 _previousPoint, _nextPoint;
    private float _movementRange;

    private float _t;

    private CentipedeFragmentSpawner _fragments;

    private void Start()
    {
        _fragments = GetComponent<CentipedeFragmentSpawner>();

        _movementRange = _fragments.GetFragmentsLength() * 0.2f;
        _startHeadTargetPos = headTarget.position;
        SetNextPoint();
    }

    protected override void Run()
    {
        if (!_attacking)
        {
            MoveTargetToPoint();
        }
    }

    private void MoveTargetToPoint()
    {
        _t += Time.deltaTime * headSpeed; 
        headTarget.position = Vector3.Lerp(_previousPoint, _nextPoint, _t);

        if (_t >= 1)
        {
            SetNextPoint();
            _t = 0;
        }
    }

    private void SetNextPoint()
    {
        _previousPoint = headTarget.position;
        _nextPoint = _startHeadTargetPos + Random.insideUnitSphere * _movementRange;
    }
}
