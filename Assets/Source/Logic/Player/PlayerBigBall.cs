using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using NTC.Global.Cache;
using NTC.Global.Pool;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerBigBall : MonoCache
{
    [SerializeField] private float _damage;
    [SerializeField] private float kickPushForce = 50;

	[SerializeField] private bool findEyesOnCollision = true;
    
	public event Action<Vector3> OnKick;

    private ParticleSystem _hitParticles;
    private ParticleSystem _hitEnemyParticles;

    private Rigidbody _rb;

    private Vector3 _lastVelocity;

	private Collider[] _enemiesAround = new Collider[30];

    private float _startTime;
    private Vector3 _startScale;

	private bool _activated = true;
	private bool _flying = true;

	private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _startTime = Time.time;
        _startScale = transform.localScale;
    }

    private void Start()
    {
        _hitParticles = (Resources.Load(ResPath.Particles + "BallHitParticles") as GameObject).GetComponent<ParticleSystem>();
        _hitEnemyParticles = (Resources.Load(ResPath.Particles + "BallHitEnemyParticles") as GameObject).GetComponent<ParticleSystem>();
    }

    protected override void FixedRun()
    {
        _lastVelocity = _rb.velocity;
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Log("Collided with " + collision.gameObject.name);
		if (!_activated) return;

		bool foundTarget = false;
		var vectorToCol = collision.transform.position - transform.position;

        if (findEyesOnCollision){//collision.gameObject.TryGetComponent<EyeEnemy>(out var eye)){
			Physics.OverlapSphereNonAlloc(transform.position, 100, _enemiesAround, Layers.EnemyHurtBox);
			for (var i = 1; i < _enemiesAround.Length; i++){
				var col = _enemiesAround[i];
				if (!col || col.Equals(collision)) continue;
				var dir = col.transform.position - transform.position;
				if (col.GetComponent<EyeEnemy>() 
						&& !Physics.Raycast(transform.position, dir.normalized, dir.magnitude, Layers.Environment)){
					_rb.velocity = dir.normalized * _lastVelocity.magnitude*1.01f;
					foundTarget = true;
					Log("Found eye and going to it - " + i);
					break;
				}
			}
		}

		if (!foundTarget)
			_rb.velocity = Vector3.Reflect(_lastVelocity, collision.GetContact(0).normal);

        //transform.rotation = Quaternion.LookRotation(_rb.velocity.normalized);

        var hitEnemy = false;
        if (collision.gameObject.TryGetComponent<ITakeHit>(out var victim))
        {
            victim.TakeHit(_damage, transform.position, "Big ball");
			hitEnemy = true;
        }
        // else{
        //     var enemiesNearby = Physics.OverlapSphere(transform.position, 50f, Layers.Hitable);
        //     for (var i = 0; i < enemiesNearby.Length; i++){
        //         if (enemiesNearby[i].gameObject.TryGetComponent<ITakeHit>(out var victim1)){
        //             victim1.TakeHit(_damage, transform.position, "Big ball");
        //         }
        //     }
        // }
		collision.gameObject.GetComponentInParent<IMover>()?.AddForce(vectorToCol.normalized * _rb.velocity.magnitude * 0.1f);
        if (collision.gameObject.name != "Shield")
		    collision.gameObject.GetComponentInParent<IInStun>()?.StartStun(0.2f);
		
		if (_rb.velocity.magnitude < 30) return;
        var particles = NightPool.Spawn(hitEnemy ? _hitEnemyParticles : _hitParticles, transform.position);
        particles.transform.rotation = Quaternion.LookRotation(collision.GetContact(0).normal);
        particles.Play();
        
    }

    public void SetVelocity(Vector3 newVelocity)
    {
        _rb.velocity = newVelocity;
    }

    public void HandleKick(Vector3 direction)
    {
		if (!_activated) return;
		_flying = true;

        transform.rotation = Quaternion.LookRotation(direction);
        _rb.velocity = direction * kickPushForce;
        
		OnKick?.Invoke(direction);
    }

	public void SetActivated(bool isIt){
		if (!_activated && isIt){
			transform.DOScale(_startScale, 0.3f).SetEase(Ease.OutQuad);
			GetComponentInChildren<TrailRenderer>().emitting = true;
			GetComponent<SphereCollider>().enabled = true;
			_rb.isKinematic = false;
            foreach (var item in GetComponentsInChildren<ParticleSystem>())
            {
                item.Play();
            }
		}
		else if (_activated && !isIt){
			transform.DOScale(_startScale * 0.02f, 0.3f).SetEase(Ease.OutQuint);
			GetComponentInChildren<TrailRenderer>().emitting = false;
			GetComponent<SphereCollider>().enabled = false;
			_rb.isKinematic = true;
            foreach (var item in GetComponentsInChildren<ParticleSystem>())
            {
                item.Stop();
            }

			_flying = false;
        }

		_activated = isIt;
	}

	public Rigidbody GetRb() => _rb;

	public bool IsActivated() => _activated;
	public bool IsFlying() => _flying;
}
