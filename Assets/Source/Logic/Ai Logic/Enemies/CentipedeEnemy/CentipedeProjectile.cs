using UnityEngine;
using NTC.Global.Cache;

public class CentipedeProjectile : MonoCache
{
	private void OnCollisionEnter(Collision col)
	{
		Destroy(gameObject);
	}
}
