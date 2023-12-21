using UnityEngine;
using NTC.Global.Cache;

public class ShieldEnemyAi : MonoCache{
    [SerializeField] private Transform bodyTipPoint;
    [SerializeField] private Transform aimTarget;
    [SerializeField] private Transform handPivot;
    [SerializeField] private Transform tipSphere;

    [SerializeField] private GameObject shield;

    [SerializeField] private CentipedeProjectile projectile;
    [SerializeField] private float shootCount = 5;
    [SerializeField] private float shootDelay = 0.2f;
    [SerializeField] private float shootCooldown = 5;
    [SerializeField] private float shootSpeed = 50;

    [SerializeField] private float upForce, downForce;

    [SerializeField] private Rigidbody bodyBase;
    [SerializeField] private Rigidbody[] legTips;

    private int _shootsMade;

    private float _shootCooldownTimer;
    private float _shootDelayTimer;

    private bool _inStun;
    private bool _shooting;

    private MeshRenderer _shieldMesh;
    private MeshCollider _shieldCollider;

    private PlayerLocator _playerLocator;
    private IInStun _stunController;

    private void Awake(){
        _playerLocator = GetComponentInChildren<PlayerLocator>();
        _stunController = GetComponent<IInStun>();
        _shieldMesh = shield.GetComponent<MeshRenderer>();
        _shieldCollider = shield.GetComponent<MeshCollider>();
    }

    protected override void OnEnabled()
    {
        _stunController.OnStun += HandleStun;
        _stunController.OnRecover += HandleRecover;
    }

    protected override void OnDisabled()
    {
        _stunController.OnStun -= HandleStun;
        _stunController.OnRecover -= HandleRecover;
    }

    protected override void FixedRun(){
        if (!bodyTipPoint || !bodyBase) return;

        if (_inStun){
            if (shield.activeSelf){
                _shieldMesh.enabled = false;
                _shieldCollider.enabled = false;
            }
            return;
        }

        DoForces();

        var dirToPlayer = (_playerLocator.GetPlayerPos() - handPivot.position).normalized;
        aimTarget.position = handPivot.position + dirToPlayer * 10 * transform.localScale.x;

        _shootCooldownTimer += Time.fixedDeltaTime;

        if (_shootCooldownTimer < shootCooldown){
            if (shield.activeSelf){
                _shieldMesh.enabled = true;
                _shieldCollider.enabled = true;
            }
            return;
        }

        if (shield.activeSelf){
            _shieldMesh.enabled = false;
            _shieldCollider.enabled = false;
        }

        _shootDelayTimer += Time.fixedDeltaTime;

        if (_shootsMade == shootCount){
            if (_shootDelayTimer >= shootDelay * 3){
                _shootCooldownTimer = 0;
                _shootDelayTimer = 0;
                _shootsMade = 0;
            }

            return;
        }

        if (_shootDelayTimer >= shootDelay){
            ShootProjectile();
            _shootsMade++;
            _shootDelayTimer = 0;
        }
    }

    private void DoForces(){
        bodyBase.AddForceAtPosition(Vector3.up * upForce, bodyTipPoint.position);

        for (int i = 0; i < legTips.Length; i++){
            legTips[i].AddForce(Vector3.down * downForce);
        }
    }

    private void ShootProjectile(){
        var spawnedProjectile = Instantiate(projectile, tipSphere.position, Quaternion.identity);
        var dir = _playerLocator.GetDirectionToPlayerNorm();
        dir += Random.onUnitSphere * 0.1f;
        spawnedProjectile.GetComponent<Rigidbody>().velocity = dir * shootSpeed;//tipSphere.forward * shootSpeed;
    }

    private void HandleStun()
    {
        _inStun = true;
    }

    private void HandleRecover()
    {
        _inStun = false;
    }
}
