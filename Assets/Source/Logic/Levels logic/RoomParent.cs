using NTC.Global.Cache;
using UnityEngine;

public class RoomParent : MonoCache
{
    private GrenadierJumpPoint[] _grenadierJumpPoints;
    private HomingEnemyMovePoint[] _homingEnemyMovePoints;
    private RunnerMovePoint[] _runnerMovePoints;

    private void Awake()
    {
        _grenadierJumpPoints = GetComponentsInChildren<GrenadierJumpPoint>();
        _homingEnemyMovePoints = GetComponentsInChildren<HomingEnemyMovePoint>();
        _runnerMovePoints = GetComponentsInChildren<RunnerMovePoint>();
    }

    public GrenadierJumpPoint[] GetGrenadierJumpPoints()
    {
        _grenadierJumpPoints ??= GetComponentsInChildren<GrenadierJumpPoint>();

        return _grenadierJumpPoints;
    }

    public HomingEnemyMovePoint[] GetHomingEnemyMovePoints()
    {
        _homingEnemyMovePoints ??= GetComponentsInChildren<HomingEnemyMovePoint>();

        return _homingEnemyMovePoints;
    }

    public RunnerMovePoint[] GetRunnerMovePoints() 
    {
        _runnerMovePoints ??= GetComponentsInChildren<RunnerMovePoint>();

        return _runnerMovePoints;
    }
}