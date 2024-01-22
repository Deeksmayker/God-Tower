using UnityEngine;
using NTC.Global.Cache;
using System;

public class Enemy : MonoCache{
	public event Action OnSpawn;
	public event Action OnDie;

	protected override void OnEnabled(){
		var healthHandler = GetComponent<IHealthHandler>();
		if (healthHandler != null){
            healthHandler.OnDied += HandleDeath;
		}
		
		var dieOnHit = GetComponent<DieOnHit>();
		if (dieOnHit){
		  dieOnHit.OnDied += HandleDeath;
		}

        gameObject.AddComponent<Birth>();

		OnSpawn?.Invoke();
	}

	protected override void OnDisabled(){
		var healthHandler = GetComponent<IHealthHandler>();
		if (healthHandler != null){
            healthHandler.OnDied -= HandleDeath;
		}
		
		var dieOnHit = GetComponent<DieOnHit>();
		if (dieOnHit){
		  dieOnHit.OnDied -= HandleDeath;
		}
	}

	private void HandleDeath(){
		OnDie?.Invoke();
	}
}
