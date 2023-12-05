using UnityEngine;
using System;
using NTC.Global.Cache;
using System.Threading.Tasks;

public class EyeEnemySpawner : MonoCache, IHealthHandler{
	[SerializeField] private int count;
	[SerializeField] private Vector3 startVelocity = Vector3.up;

    public event Action OnHit;
    public event Action<float> OnHealthChanged;
    public event Action OnHealthAdd;
    public event Action OnDied;

	private void Start(){
		BoidsBehaviour.Instance.SpawnBoids(transform.position, startVelocity, count);	
		CheckAliveCount();
	}

	private async void CheckAliveCount(){
		while(true){
			await Task.Delay(3000);
			var aliveCount = await BoidsBehaviour.Instance.GetAliveBoidsCount();
			if (aliveCount <= count*0.5f){
				Die();
				break;
			}
			await Task.Yield();
		}
	}

    public void Die(bool order = false){
		OnDied?.Invoke();
	}

    public void ChangeHealth(float changeValue){}
    public void SetHealth(float value){}
    public float GetHealth01() => 1;

}
