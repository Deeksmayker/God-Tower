using DG.Tweening;
using NTC.Global.Cache;
using NTC.Global.Pool;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerSmallSphere : MonoCache
{
    [SerializeField] private float _startVelocity;
    [SerializeField] private float _maxSpread;

    private ParticleSystem _hitParticles;

    private Rigidbody _rb;

    private Vector3 _startScale;

    protected override void OnEnabled()
    {
        _rb = Get<Rigidbody>();
        _startScale = transform.localScale;
    }

    private void Start()
    {
        transform.localScale = Vector3.one / 10;
        transform.DOScale(_startScale * 4, 1.5f).SetEase(Ease.InCubic).SetLink(gameObject);

        var randomNumberX = Random.Range(-_maxSpread / 2, _maxSpread / 2);
        var randomNumberY = Random.Range(-_maxSpread, _maxSpread);
        var randomNumberZ = Random.Range(-_maxSpread, _maxSpread);

        var spreadedDirection = Quaternion.Euler(randomNumberX, randomNumberY, randomNumberZ) * transform.forward;
        
        _rb.velocity += _startVelocity * spreadedDirection;

        _hitParticles = (Resources.Load(ResPath.Particles + "SmallBallHitParticles") as GameObject).GetComponent<ParticleSystem>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        Log("Collided with " + collision.gameObject.name);

        _rb.velocity = Vector3.Reflect(_rb.velocity, collision.GetContact(0).normal);
        transform.rotation = Quaternion.LookRotation(_rb.velocity);

        var particles = NightPool.Spawn(_hitParticles, transform.position);
        particles.transform.rotation = Quaternion.LookRotation(collision.GetContact(0).normal);
        particles.Play();
        
        if (collision.gameObject.TryGetComponent<ITakeHit>(out var victim))
        {
            victim.TakeHit(1, transform.position, "Small ball");
        }

        gameObject.AddComponent<Death>();
    }

    public void SetVelocity(Vector3 newVelocity)
    {
        _rb.velocity = newVelocity;
    }
}