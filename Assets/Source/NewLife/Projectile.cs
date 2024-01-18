using UnityEngine;

public class Projectile : MonoBehaviour{
    private float _lifeTime;
    private Vector3 _velocity;
    private Vector3 _lastPosition;

    private SphereCollider _collider;
    
    private ParticleSystem _hitParticles;
    private ParticleSystem _hitEnemyParticles;

    
    private void Awake(){
        _collider = GetComponent<SphereCollider>();
    
        _lastPosition = transform.position;
        
        _hitParticles = (Resources.Load(ResPath.Particles + "BallHitParticles") as GameObject).GetComponent<ParticleSystem>();
    }
    
    private void Update(){
        _lifeTime += Time.deltaTime;
        
        var dir = transform.position - _lastPosition;
    
        //Main hit
        if (Physics.SphereCast(_lastPosition, _collider.radius, dir.normalized,
                               out var hit, Mathf.Max(_collider.radius, dir.magnitude), Layers.EnemyBallHitable)){
            HandleHit(hit);
        } else if (Physics.Raycast(transform.position, Vector3.down, out var hit2, _collider.radius, Layers.EnemyBallHitable)){
         //Other shit
            HandleHit(hit2);
        }
        
        _lastPosition = transform.position;
        
        transform.Translate(_velocity * Time.deltaTime);

    }
    
    private void HandleHit(RaycastHit hit){
        if (hit.transform.GetComponentInParent<PlayerUnit>()){
            Debug.Log("HIT YA");
            Destroy(gameObject);
        } else if (hit.transform.GetComponent<RealBall>()){
            _velocity = Vector3.Reflect(_velocity, hit.normal);
        } else {
            var particles1 = Instantiate(_hitParticles, hit.point, Quaternion.identity);
            particles1.Play();
            Destroy(gameObject);
        }
    }
    
    public void SetVelocity(Vector3 velocity){
        _velocity = velocity;
    }
}
