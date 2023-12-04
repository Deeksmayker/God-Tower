using UnityEngine;
using NTC.Global.Cache;
using System;

public class Enemy : MonoCache{
	public event Action OnSpawn;
	public event Action OnDie;

	private IHealthHandler _health;

	private void Awake(){
		_health = GetComponent<IHealthHandler>();
	}

	protected override void OnEnabled(){
		_health.OnDied += HandleDeath;

		OnSpawn?.Invoke();
	}

	protected override void OnDisabled(){
		_health.OnDied -= HandleDeath;
	}

	private void HandleDeath(){
		OnDie?.Invoke();
	}
}
