using System;
using DG.Tweening;
using NTC.Global.Cache;
using UnityEditor.Rendering;
using UnityEngine;


public class RoomDoor : MonoCache
{
    [SerializeField] private bool trackEnemies = true;
    [SerializeField] private float height = 250;
    [SerializeField] private float openSpeed = 20f;
    [SerializeField] private float closeSpeed = 300f;

    private int _enemiesCount;
    
    private bool _closed = true;

    private float _speed = 1f;
    
    private Vector3 desiredPosition;

    private void Awake()
    {
        desiredPosition = transform.position;
    }

    protected override void OnEnabled()
    {
        if (!trackEnemies)
            return;

        var enemiesOnScene = FindObjectsOfType<BaseAiController>();
        _enemiesCount = enemiesOnScene.Length;

        for (var i = 0; i < enemiesOnScene.Length; i++)
        {
            if (enemiesOnScene[i].TryGetComponent<IHealthHandler>(out var health))
            {
                health.OnDied += HandleEnemyDied;
            }
        }
    }

    protected override void Run()
    {
        transform.position = Vector3.MoveTowards(transform.position, desiredPosition, _speed * Time.deltaTime);
    }

    public void Open()
    {
        desiredPosition += Vector3.up * height;
        _speed = openSpeed;
        _closed = false;
    }

    public void Close()
    {
        if (_closed)
            return;
        _speed = closeSpeed;
        desiredPosition -= Vector3.up * height;
        _closed = true;
    }

    private void HandleEnemyDied()
    {
        _enemiesCount--;
        if (_enemiesCount <= 0)
            Open();
    }
}
