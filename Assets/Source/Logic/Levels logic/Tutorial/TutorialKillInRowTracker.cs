using NTC.Global.Cache;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class TutorialKillInRowTracker : MonoBehaviour
{
    [SerializeField] private BaseAiController enemyPrefab;
    [SerializeField] private Transform[] enemyPositions;

    private List<BaseHealthHandler> _connectedEnemies;

    public UnityEvent OnFailrue = new();

    private void Start()
    {
        _connectedEnemies = GetComponentsInChildren<BaseHealthHandler>().ToList();
    }

    private void OnEnable()
    {
        for (var i = 0; i < _connectedEnemies.Count; i++)
        {
            _connectedEnemies[i].OnDied += HandleEnemyDied;
        }
    }

    private void OnDisable()
    {
        for (var i = 0; i < _connectedEnemies.Count; i++)
        {
            _connectedEnemies[i].OnDied -= HandleEnemyDied;
        }
    }

    public void HandlePlayerShoot()
    {
       
    }

    private void HandleEnemyDied()
    {
        _connectedEnemies.RemoveAt(0);
    }
}