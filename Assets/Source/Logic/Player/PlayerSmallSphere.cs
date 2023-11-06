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
        transform.localScale = Vector3.zero;
        transform.DOScale(_startScale, 0.5f).SetEase(Ease.InOutBounce);

        var randomNumberX = Random.Range(-_maxSpread / 2, _maxSpread / 2);
        var randomNumberY = Random.Range(-_maxSpread, _maxSpread);
        var randomNumberZ = Random.Range(-_maxSpread, _maxSpread);

        var spreadedDirection = Quaternion.Euler(randomNumberX, randomNumberY, randomNumberZ) * transform.forward;
        
        _rb.velocity += _startVelocity * spreadedDirection;

        _hitParticles = (Resources.Load(ResPath.Particles + "BallHitParticles") as GameObject).GetComponent<ParticleSystem>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        Log("Collided with " + collision.gameObject.name);

        _rb.velocity = Vector3.Reflect(_rb.velocity, collision.GetContact(0).normal);
        transform.rotation = Quaternion.LookRotation(_rb.velocity);

        var particles = NightPool.Spawn(_hitParticles, transform.position);
        particles.transform.rotation = Quaternion.LookRotation(collision.GetContact(0).normal);
        particles.Play();
        
        //TODO: написать логику дамага
    }

    public void SetVelocity(Vector3 newVelocity)
    {
        _rb.velocity = newVelocity;
    }
}