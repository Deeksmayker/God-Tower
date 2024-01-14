using UnityEngine;
using System.Collections.Generic;

public class RealBall : MonoBehaviour{
    [SerializeField] private float speed;
    [SerializeField] private float gravity;
    [SerializeField] private float reflectVelocityMultiplier = 0.9f;
    
    private Vector3 _lastPos;
    private Vector3 _velocity;
    
    private float _lifeTime;
    
    private ParticleSystem _hitParticles;
    private ParticleSystem _hitEnemyParticles;
    
    public bool BallIsReal = true;
    private bool _deadMan;

    private SphereCollider _collider;
    
    private void Awake(){
        _collider = GetComponent<SphereCollider>();
    
        _lastPos = transform.position;
        
        _hitParticles = (Resources.Load(ResPath.Particles + "BallHitParticles") as GameObject).GetComponent<ParticleSystem>();
        _hitEnemyParticles = (Resources.Load(ResPath.Particles + "BallHitEnemyParticles") as GameObject).GetComponent<ParticleSystem>();

    }
    
    private void Update(){
        Step(Time.deltaTime);
    }
    
    public Vector3[] PredictPositions(Vector3 startPosition, Vector3 direction){
        float step = 0.05f;
        int samples = 50;
        
        var elapsedTime = 0f;
        
        var result = new List<Vector3>();
        transform.position = startPosition;
        _velocity = direction * speed * _powerMultiplier;
        
        for (int i = 0; i < samples; i++){
            result.Add(transform.position);
            Step(step);
            if (_deadMan) break;
        }
        
        transform.position = startPosition;
        _velocity = Vector3.zero;
        
        return result.ToArray();
    }
    
    private void Step(float delta){
        _lifeTime += delta;
    
        var areas = Physics.OverlapSphere(transform.position, _collider.radius, Layers.Areas);
        for (int i = 0; i < areas.Length; i++){
            if (areas[i].TryGetComponent<WindArea>(out var wind)){
                _velocity += wind.GetDirection() * delta;
            }
        }
    
        var dir = transform.position - _lastPos;
        if (Physics.SphereCast(_lastPos, _collider.radius*2, dir.normalized,
                               out var hit, dir.magnitude, Layers.BallHitable)){
            HandleHit(hit);
        } else if (Physics.SphereCast(transform.position, _collider.radius, Vector3.down,
                                      out var hit2, _collider.radius * 1.5f, Layers.BallHitable)){
            if (_velocity.magnitude <= 1){
                _velocity += hit2.normal * gravity * 10 * delta;
            }else{
                _velocity = Vector3.Reflect(_velocity, hit2.normal) * reflectVelocityMultiplier;
            }
        }
        
        _velocity.y -= gravity * delta;
        
        _lastPos = transform.position;
        
        transform.Translate(_velocity * delta);
    }
    
    private void HandleHit(RaycastHit hit){
        /*
        if (!BallIsReal){
            _deadMan = true;
            return;
        }
        */
        
        var isCapable = BallIsReal && _velocity.magnitude >= 10;
        
        transform.position = hit.point + hit.normal * _collider.radius/2;
        
        if (hit.transform.TryGetComponent<ITakeHit>(out var victim)){
            if (isCapable){
                victim.TakeHit(1, hit.point, "Real ball");
                var particles = Instantiate(_hitEnemyParticles, hit.point, Quaternion.LookRotation(hit.normal));
                particles.Play();
            }
            
            var targetSpeed = _velocity.magnitude * 0.5f;
            var homingTowardsPlayerPower = 100f;
            _velocity *= 0.5f;
            _velocity = Vector3.Reflect(_velocity, hit.normal);
            var playerPos = FindObjectOfType<PlayerUnit>().transform.position;
            _velocity += (playerPos - transform.position).normalized * homingTowardsPlayerPower;
            _velocity = Vector3.ClampMagnitude(_velocity, targetSpeed);
        } else{
            _velocity = Vector3.Reflect(_velocity, hit.normal) * reflectVelocityMultiplier;
            if (isCapable){
                var particles1 = Instantiate(_hitParticles, hit.point, Quaternion.identity);
                particles1.Play();
            }
        }
        
        //Destroy(gameObject);
    }
    
    private float _powerMultiplier = 1;
    public void SetPowerMultiplier(float multiplier){
        _powerMultiplier = multiplier;
    }
    
    public void SetVelocity(Vector3 newVelocity){
        _velocity = newVelocity;
    }
    
    public void HandleKick(Vector3 dir){
        _velocity = dir * speed * _powerMultiplier;
    }
    
    public bool CanBeCollected(){
        return _lifeTime > 1;
    }
}
