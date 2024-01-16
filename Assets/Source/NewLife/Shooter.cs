using UnityEngine;

public class Shooter : MonoBehaviour{
    [SerializeField] private Projectile projectilePrefab;
    [SerializeField] private float shootCooldown;
    [SerializeField] private float shootSpeed;
    
    private float _cooldownTimer;
    
    private void Awake(){
        _cooldownTimer = shootCooldown;
    }
    
    private void Update(){
        _cooldownTimer -= Time.deltaTime;
        if (_cooldownTimer > 0) return;
        
        _cooldownTimer = shootCooldown;
        
        var projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        projectile.SetVelocity((PlayerLocator.Instance.GetPlayerPos() - transform.position).normalized * shootSpeed);
    }
}
