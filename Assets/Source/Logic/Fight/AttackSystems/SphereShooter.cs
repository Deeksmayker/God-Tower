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
    [SerializeField] private PlayerBigBall _bigSpherePrefab;

	[SerializeField] private int ballCount;
	[SerializeField] private float radius;
    
    [Header("Big sphere data")]
    [SerializeField] private float _bigSphereCooldown = 1f;
    
    [Header("Small sphere data")]
    [SerializeField] private float _timeBeforeAutoShoot = 1f;
    [SerializeField] private int _spherePerSecond = 3;

    private IMover _playerMover;
    
    private float _bigSphereCooldownTimer;

	private PlayerBigBall[] _balls;
	private Vector3[] _orbits;
    
    private float _sphereShootTimer;
    private float _spawnTimer;

	private bool _input;
    
    private void Start()
    {
        _playerMover = Get<IMover>();

		_balls = new PlayerBigBall[ballCount];
		_orbits = new Vector3[ballCount];
		SetupBalls();

    }

	protected override void Run(){
		UpdateBalls();
	}

    public void SetInput(bool value)
    {
		if (value && !_input){
			for (var i = 0; i < _balls.Length; i++){
				if (!_balls[i].IsActivated()){
					_balls[i].SetActivated(true);
					break;
				}
			}
		}

		if (!value && _input){
			for (var i = 0; i < _balls.Length; i++){
				if (_balls[i].IsActivated() && !_balls[i].IsFlying()){
					_balls[i].SetActivated(false);
					break;
				}
			}
		}

		_input = value;
		/*
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
		*/
    }

	public void SetAttractInput(bool value){
		if (!value){
			return;
		}

		for (var i = 0; i < _balls.Length; i++){
			if (_balls[i].IsFlying()){
				_balls[i].SetVelocity((transform.position - _balls[i].transform.position).normalized * 200);
				if (Vector3.Distance(_balls[i].transform.position, transform.position) < 10){
					_balls[i].SetActivated(false);
				}
			}
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
        var spawnedSphere = Instantiate(_bigSpherePrefab, _shootStartPoint.position + _shootStartPoint.forward * 2, _camDirection.rotation);
        spawnedSphere.SetVelocity(_playerMover.GetVelocity());
    }

	private void SetupBalls(){
		for (var i = 0; i < ballCount; i++){
			_balls[i] = Instantiate(_bigSpherePrefab, _shootStartPoint.position, _camDirection.rotation);
			_balls[i].SetActivated(false);

			var randomPos = UnityEngine.Random.insideUnitCircle.normalized;
			_orbits[i] = randomPos * radius;
		}
	}

	private void UpdateBalls(){
		var damping = 100f;
		var rotSpeed = 200f;

		for (var i = 0; i < ballCount; i++){
			if (_input && _balls[i].IsActivated() && !_balls[i].IsFlying()){
				_balls[i].transform.position = Vector3.Lerp(_balls[i].transform.position, _shootStartPoint.transform.position + _camDirection.forward * 4 - _camDirection.up * 2, damping * Time.deltaTime);
				_balls[i].SetVelocity(Vector3.zero);
			}
			if (_balls[i].IsActivated()){
				continue;
			}
			
			_balls[i].SetVelocity(Vector3.zero);

			var startPoint = _shootStartPoint.position; 

			_orbits[i] = Quaternion.AngleAxis(rotSpeed * Time.deltaTime, Vector3.forward) * _orbits[i];

			_balls[i].transform.position = Vector3.Lerp(_balls[i].transform.position, startPoint - _shootStartPoint.forward * .3f + (_camDirection.right * _orbits[i].x + _camDirection.up * _orbits[i].y), damping * Time.deltaTime);
		}
	}
}
