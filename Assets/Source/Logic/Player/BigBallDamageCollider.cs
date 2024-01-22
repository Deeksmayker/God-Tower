using UnityEngine;
using NTC.Global.Cache;

public class BigBallDamageCollider : MonoCache{
	[SerializeField] private float damage;
	[SerializeField] private float sizeIncreasePerHit;

	private float _currentSizeMultiplier = 1;

	private ParticleSystem _hitParticles;

	private void OnTriggerEnter(Collider other){
		Debug.Log("TRIGGER");
		if (other.TryGetComponent<ITakeHit>(out var victim)){
			victim.TakeHit(damage, transform.position, "Player Big Ball");
			_currentSizeMultiplier += sizeIncreasePerHit;

			transform.localScale = Vector3.one * _currentSizeMultiplier;

			Debug.Log("I killed someone <3");
		}
	}
}
