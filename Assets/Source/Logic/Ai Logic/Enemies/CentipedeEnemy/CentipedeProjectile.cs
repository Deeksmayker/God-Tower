using UnityEngine;
using NTC.Global.Cache;

public class CentipedeProjectile : MonoCache
{
    [SerializeField] private float damage = 5f;
    [SerializeField] private bool spawnDamageField = true;
    [SerializeField] private bool bounce;

    private int _bounceCount;

    private GameObject _damageField;

    private void Awake(){
        if (spawnDamageField)
            _damageField = Resources.Load(ResPath.Enemies + "DamageField") as GameObject;
    }

	private void OnCollisionEnter(Collision col)
	{
        var player = col.gameObject.GetComponentInParent<PlayerUnit>() ;
        if (player)
        {
            player.GetComponentInChildren<ITakeHit>()?.TakeHit(damage, transform.position, "Projectile");
        }

        if (bounce && _bounceCount == 0){
            var rb = GetComponent<Rigidbody>();
            rb.velocity = Vector3.Reflect(rb.velocity, col.GetContact(0).normal);
            _bounceCount++;
            return;
        }

		var particles = GetComponentsInChildren<ParticleSystem>();
		for (var i = 0; i < particles.Length; i++){
			var death = particles[i].gameObject.AddComponent<ParticlesDeath>();
            death.SetDeathTime(6);
		}

        if (spawnDamageField){
            _damageField = Instantiate(_damageField, transform.position, Quaternion.identity);
            _damageField.GetComponent<DeleteAfterTime>()?.SetTime(2f);
            _damageField.GetComponent<BaseHitBox>()?.SetDamage(damage);
            _damageField.GetComponent<SphereCollider>().radius = 5;
        }

		Destroy(gameObject);
	}
}
