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

    private Rigidbody _rb;

    private Vector3 _lastVelocity;

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
        transform.localScale = Vector3.zero;
        transform.DOScale(_startScale, 0.5f).SetEase(Ease.InOutBounce);

        _hitParticles = (Resources.Load(ResPath.Particles + "BallHitParticles") as GameObject).GetComponent<ParticleSystem>();

        Started?.Invoke();
    }

    protected override void FixedRun()
    {
        _rb.velocity += transform.forward * _acceleration * Mathf.Clamp01(Time.time - _startTime);
        _lastVelocity = _rb.velocity;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Log("Collided with " + collision.gameObject.name);

        _rb.velocity = Vector3.Reflect(_lastVelocity, collision.GetContact(0).normal);
        transform.rotation = Quaternion.LookRotation(_rb.velocity);

        var particles = NightPool.Spawn(_hitParticles, transform.position);
        particles.transform.rotation = Quaternion.LookRotation(collision.GetContact(0).normal);
        particles.Play();
        
        if (collision.gameObject.TryGetComponent<ITakeHit>(out var victim))
        {
            victim.TakeHit(_damage, transform.position, "Big ball");
        }
        
        Collided?.Invoke();
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
