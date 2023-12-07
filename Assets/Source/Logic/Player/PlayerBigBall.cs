using System;
using System.Collections.Generic;
using DG.Tweening;
using NTC.Global.Cache;
using NTC.Global.Pool;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerBigBall : MonoCache
{
    [SerializeField] private float _acceleration;
    [SerializeField] private float _damage;
    [SerializeField] private float kickPushForce = 50;
    
    public event Action Started;
    public event Action ImpulseHandled;
    public event Action Collided;
    
    private ParticleSystem _hitParticles;
    private ParticleSystem _hitEnemyParticles;

    private Rigidbody _rb;

    private Vector3 _lastVelocity;

	private Collider[] _enemiesAround = new Collider[30];

    private float _startTime;
    private Vector3 _startScale;

    protected override void OnEnabled()
    {
        _rb = GetComponent<Rigidbody>();
        _startTime = Time.time;
        _startScale = transform.localScale;
    }

    private void Start()
    {
        //transform.localScale = Vector3.zero;
        //transform.DOScale(_startScale, 0.5f).SetEase(Ease.InOutBounce);

        _hitParticles = (Resources.Load(ResPath.Particles + "BallHitParticles") as GameObject).GetComponent<ParticleSystem>();
        _hitEnemyParticles = (Resources.Load(ResPath.Particles + "BallHitEnemyParticles") as GameObject).GetComponent<ParticleSystem>();

        Started?.Invoke();
    }

    protected override void FixedRun()
    {
        //_rb.velocity += transform.forward * _acceleration * Mathf.Clamp01(Time.time - _startTime);
        _lastVelocity = _rb.velocity;
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Log("Collided with " + collision.gameObject.name);

		bool foundTarget = false;
		var vectorToCol = collision.transform.position - transform.position;

        if (true ){//collision.gameObject.TryGetComponent<EyeEnemy>(out var eye)){
			Physics.OverlapSphereNonAlloc(transform.position, 100, _enemiesAround, Layers.EnemyHurtBox);
			for (var i = 1; i < _enemiesAround.Length; i++){
				var col = _enemiesAround[i];
				if (!col || col.Equals(collision)) continue;
				var dir = col.transform.position - transform.position;
				if (col.GetComponent<EyeEnemy>() 
						&& !Physics.Raycast(transform.position, dir.normalized, dir.magnitude, Layers.Environment)){
					_rb.velocity = dir.normalized * _lastVelocity.magnitude;
					foundTarget = true;
					Log("Found eye and going to it - " + i);
					break;
				}
			}
		}

		if (!foundTarget)
			_rb.velocity = Vector3.Reflect(_lastVelocity, collision.GetContact(0).normal);

        transform.rotation = Quaternion.LookRotation(_rb.velocity);

        var hitEnemy = false;
        if (collision.gameObject.TryGetComponent<ITakeHit>(out var victim))
        {
            victim.TakeHit(_damage, transform.position, "Big ball");
			hitEnemy = true;
        }
		collision.gameObject.GetComponentInParent<IMover>()?.AddForce(vectorToCol.normalized * _rb.velocity.magnitude * 0.1f);
		collision.gameObject.GetComponentInParent<IInStun>()?.StartStun();

        Collided?.Invoke();
		
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
        transform.rotation = Quaternion.LookRotation(direction);
        _rb.velocity = direction * kickPushForce;
        
        ImpulseHandled?.Invoke();
    }
}
