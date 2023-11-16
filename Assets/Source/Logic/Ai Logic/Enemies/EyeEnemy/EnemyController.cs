using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public enum GizmoType { Never, SelectedOnly, Always }

public class EnemyController : MonoBehaviour 
{
    [Header("Spawner data")]
    [SerializeField] private GameObject prefab;
    [SerializeField] private float spawnRadius = 10;
    [SerializeField] private int spawnCount = 10;
    
    [Header("Gizmo data")]
    [SerializeField] private Color colour;
    [SerializeField] private GizmoType showSpawnRegion;

    public event Action AllEnemyKilled;

    private int _enemyCount;
    
    public void Spawn()
    {
        _enemyCount = spawnCount;

        for (int i = 0; i < spawnCount; i++) 
        {
            var position = transform.position + Random.insideUnitSphere * spawnRadius;
            
            var enemyTransform = Instantiate(prefab).transform;
            enemyTransform.position = position;
            enemyTransform.forward = Random.insideUnitSphere;

            enemyTransform.GetComponent<IEnemy>().Killed += OnEnemyKilled;
        }
    }

    private void OnEnemyKilled()
    {
        _enemyCount -= 1;
        
        if (_enemyCount <= 0)
        {
            _enemyCount = spawnCount;
            AllEnemyKilled?.Invoke();
        }
    }

    private void OnDrawGizmos() 
    {
        if (showSpawnRegion == GizmoType.Always) 
        {
            DrawGizmos ();
        }
    }

    private void OnDrawGizmosSelected() 
    {
        if (showSpawnRegion == GizmoType.SelectedOnly) 
        {
            DrawGizmos ();
        }
    }

    private void DrawGizmos() 
    {
        Gizmos.color = new Color (colour.r, colour.g, colour.b, 0.3f);
        Gizmos.DrawSphere (transform.position, spawnRadius);
    }
}