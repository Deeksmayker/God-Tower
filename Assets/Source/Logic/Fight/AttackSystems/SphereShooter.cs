using System;
using NTC.Global.Cache;
using NTC.Global.Pool;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject.SpaceFighter;

public class SphereShooter : MonoCache
{
    [SerializeField] private Transform _shootStartPoint;
    [SerializeField] private Transform _camDirection;
    
    [Header("Sphere's prefab")]
    [SerializeField] private GameObject _smallSpherePrefab;
    [SerializeField] private GameObject _bigSpherePrefab;
    
    [Header("Big sphere data")]
    [SerializeField] private float _bigSphereCooldown = 1f;
    
    [Header("Small sphere data")]
    [SerializeField] private float _timeBeforeAutoShoot = 1f;
    [SerializeField] private int _spherePerSecond = 3;

    private IMover _playerMover;
    
    private float _bigSphereCooldownTimer;
    
    private float _sphereShootTimer;
    private float _spawnTimer;
    
    private void Start()
    {
        _playerMover = Get<IMover>();
    }

    public void SetInput(bool value)
    {
        if (value)
        {
            _sphereShootTimer += Time.deltaTime;
            
            if (_sphereShootTimer >= _timeBeforeAutoShoot)
            {
                SpawnSmallSphere();
            }
        }
        else
        {
            if (_bigSphereCooldownTimer > 0)
            {
                _bigSphereCooldownTimer -= Time.deltaTime;
            }
            if (_sphereShootTimer > 0 && _sphereShootTimer <= _timeBeforeAutoShoot && _bigSphereCooldownTimer <= 0)
            {
                SpawnBigSphere();
            
                _bigSphereCooldownTimer = _bigSphereCooldown;
            }

            _sphereShootTimer = 0;
        }
    }

    private void SpawnSmallSphere()
    {
        var timeForSpawn = 1f / _spherePerSecond;
                
        _spawnTimer += Time.deltaTime;
        if (_spawnTimer >= timeForSpawn)
        {
            var spawnedSphere = Instantiate(_smallSpherePrefab, _shootStartPoint.position, _camDirection.rotation)
                .GetComponent<PlayerSmallSphere>();
            spawnedSphere.SetVelocity(new Vector3(_playerMover.GetVelocity().x, 0, _playerMover.GetVelocity().z));
            _spawnTimer -= timeForSpawn;

            //CameraService.Instance.ShakeCamera(.05f);
        }
    }

    private void SpawnBigSphere()
    {
        var spawnedSphere = Instantiate(_bigSpherePrefab, _shootStartPoint.position, _camDirection.rotation)
            .GetComponent<PlayerBigBall>();
        spawnedSphere.SetVelocity(_playerMover.GetVelocity());
    }
}