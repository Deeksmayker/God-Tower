using System;
using System.Collections;
using System.Collections.Generic;
using NTC.Global.Cache;
using UnityEngine;
using UnityEngine.Serialization;

public class Wave : MonoCache
{
    [SerializeField] private List<EnemyController> _enemyControllers;

    public event Action Ended;

    private int _enemyControllersUnKilledCount;

    public void StartWave()
    {
        _enemyControllersUnKilledCount = _enemyControllers.Count;
        
        for (int i = 0; i < _enemyControllers.Count; i++)
        {
            _enemyControllers[i]?.Spawn();
            
            _enemyControllers[i].AllEnemyKilled += () =>
            {
                _enemyControllersUnKilledCount -= 1;

                if (_enemyControllersUnKilledCount <= 0)
                {
                    _enemyControllersUnKilledCount = _enemyControllers.Count;
                    Ended?.Invoke();
                }
            };
        }
    }
}
