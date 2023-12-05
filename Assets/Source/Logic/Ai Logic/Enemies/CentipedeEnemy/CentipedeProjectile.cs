using UnityEngine;
using NTC.Global.Cache;

public class CentipedeProjectile : MonoCache
{
	private void OnCollisionEnter(Collision col)
	{
		var particles = GetComponentsInChildren<ParticleSystem>();
		for (var i = 0; i < particles.Length; i++){
			particles[i].gameObject.AddComponent<ParticlesDeath>();
		}
		Destroy(gameObject);
	}
}
