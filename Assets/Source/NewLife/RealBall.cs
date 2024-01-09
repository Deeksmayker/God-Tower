using UnityEngine;
using System.Collections.Generic;

public class RealBall : MonoBehaviour{
    [SerializeField] private float speed;
    [SerializeField] private float gravity;
    
    private Vector3 _lastPos;
    private Vector3 _velocity;
    
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
        _velocity = direction * speed;
        
        for (int i = 0; i < samples; i++){
            Step(step);
            result.Add(transform.position);
            if (_deadMan) break;
        }
        
        transform.position = startPosition;
        _velocity = Vector3.zero;
        
        return result.ToArray();
    }
    
    private void Step(float delta){
        var areas = Physics.OverlapSphere(transform.position, _collider.radius, Layers.Areas);
        for (int i = 0; i < areas.Length; i++){
            if (areas[i].TryGetComponent<WindArea>(out var wind)){
                _velocity += wind.GetDirection() * delta;
            }
        }
    
        var dir = transform.position - _lastPos;
        if (Physics.SphereCast(_lastPos, _collider.radius*2, dir.normalized, out var hit, dir.magnitude, Layers.BallHitable)){
            transform.position = hit.point + hit.normal * _collider.radius/2;
            _velocity = Vector3.Reflect(_velocity, hit.normal) * 0.7f;
            HandleHit(hit);
        }/* else if (Physics.Raycast(transform.position, _velocity.normalized, out var hit2, 2f, Layers.BallHitable) 
                && hit2.distance <= 2){
            _velocity = hit2.normal * gravity * delta;        
        }*/
        else if (Physics.SphereCast(transform.position, _collider.radius*2, Vector3.down, out var hit2, _collider.radius * 1.5f, Layers.BallHitable)){
            if (_velocity.magnitude <= 1){
                _velocity += hit2.normal * gravity * 10 * delta;
            }else{
                _velocity = Vector3.Reflect(_velocity, hit2.normal) * 0.9f;
            }
            HandleHit(hit2);
        }
        
        _velocity.y -= gravity * delta;
        
        _lastPos = transform.position;
        
        transform.Translate(_velocity * delta);
    }
    
    private void HandleHit(RaycastHit hit){
        if (!BallIsReal){
            _deadMan = true;
            return;
        }
        if (hit.transform.TryGetComponent<ITakeHit>(out var victim)){
            victim.TakeHit(1, hit.point, "Real ball");
            var particles = Instantiate(_hitEnemyParticles, hit.point, Quaternion.LookRotation(hit.normal));
            particles.Play();
        }
        
        var particles1 = Instantiate(_hitParticles, hit.point, Quaternion.identity);
        particles1.Play();
        Destroy(gameObject);
    }
    
    public void SetVelocity(Vector3 newVelocity){
        _velocity = newVelocity;
    }
    
    public void HandleKick(Vector3 dir){
        _velocity = dir * speed;
    }
}
