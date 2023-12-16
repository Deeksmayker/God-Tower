using UnityEngine;
using NTC.Global.Cache;

public class CentipedeProjectile : MonoCache
{
    [SerializeField] private float damage = 5f;

    private GameObject _damageField;

    private void Awake(){
        _damageField = Resources.Load(ResPath.Enemies + "DamageField") as GameObject;
    }

	private void OnCollisionEnter(Collision col)
	{
        if (col.gameObject.GetComponentInParent<PlayerUnit>() && col.gameObject.TryGetComponent<ITakeHit>(out var victim)){
            victim.TakeHit(damage, transform.position, "Centipede proj");
        }

		var particles = GetComponentsInChildren<ParticleSystem>();
		for (var i = 0; i < particles.Length; i++){
			var death = particles[i].gameObject.AddComponent<ParticlesDeath>();
            death.SetDeathTime(6);
		}
        _damageField = Instantiate(_damageField, transform.position, Quaternion.identity);
        _damageField.GetComponent<DeleteAfterTime>()?.SetTime(2f);
        _damageField.GetComponent<BaseHitBox>()?.SetDamage(damage);
        _damageField.GetComponent<SphereCollider>().radius = 5;

		Destroy(gameObject);
	}
}
