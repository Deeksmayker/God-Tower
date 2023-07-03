using NTC.Global.Cache;
using UnityEngine;

public class RoomParent : MonoCache
{
    private GrenadierJumpPoint[] _grenadierJumpPoints;
    private HomingEnemyMovePoint[] _homingEnemyMovePoints;
    private RunnerMovePoint[] _runnerMovePoints;
    private EnemyGroup[] _enemyGroups;

    private void Awake()
    {
        _grenadierJumpPoints = GetComponentsInChildren<GrenadierJumpPoint>();
        _homingEnemyMovePoints = GetComponentsInChildren<HomingEnemyMovePoint>();
        _runnerMovePoints = GetComponentsInChildren<RunnerMovePoint>();
        _enemyGroups = GetComponentsInChildren<EnemyGroup>();
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

    public EnemyGroup[] GetEnemyGroups()
    {
        if (_enemyGroups == null || _enemyGroups.Length == 0)
            _enemyGroups = GetComponentsInChildren<EnemyGroup>();
        return _enemyGroups;
    }
}