using UnityEngine;
using NTC.Global.Cache;

public class ParticlesDeath : MonoCache{
	private void Awake(){
		transform.parent = null;
		GetComponent<ParticleSystem>().emissionRate = 0;
		transform.localScale = Vector3.one;
		Destroy(gameObject, 3f);
	}
}
