using UnityEngine;
using NTC.Global.Cache;

public class CentipedeAi : MonoCache
{
    [SerializeField] private Transform headTarget;
    [SerializeField] private float attackCooldown;
    [SerializeField] private float headSpeed = 1f;
	[SerializeField] private float headAttackSpeed = .5f;
	[SerializeField] private float meleeAttackDuration = 5f;

	[SerializeField] private float projectileThrowPower;

    [SerializeField] private bool rangeAttack = true;
    [SerializeField] private bool meleeAttack;
	private bool _preparingAttack;
    private bool _rangeAttacking;
    private bool _meleeAttacking;

	private float _cooldownTimer;
	private float _meleeAttackTimer;

    private Vector3 _startHeadTargetPos;
    private Vector3 _previousPoint, _nextPoint;
    private float _movementRange;

    private float _t;

    private Quaternion[] _shootDirs = new Quaternion[]{Quaternion.identity, Quaternion.AngleAxis(30, Vector3.up), Quaternion.AngleAxis(10, Vector3.up), Quaternion.AngleAxis(20, Vector3.up), Quaternion.AngleAxis(-30, Vector3.up), Quaternion.AngleAxis(-10, Vector3.up), Quaternion.AngleAxis(-20, Vector3.up), Quaternion.AngleAxis(15, Vector3.right), Quaternion.AngleAxis(10, Vector3.right), Quaternion.AngleAxis(5, Vector3.right), Quaternion.AngleAxis(-15, Vector3.right), Quaternion.AngleAxis(-10, Vector3.right), Quaternion.AngleAxis(-5, Vector3.right), Quaternion.AngleAxis(-15, Vector3.up) * Quaternion.AngleAxis(-15, Vector3.right), Quaternion.AngleAxis(15, Vector3.up) * Quaternion.AngleAxis(-15, Vector3.right)}; 

    private CentipedeFragmentSpawner _fragments;
	private PlayerLocator _playerLocator;
	private CentipedeProjectile _projectile;

    private void Start()
    {
        _fragments = GetComponent<CentipedeFragmentSpawner>();
		_playerLocator = GetComponentInChildren<PlayerLocator>();
		_projectile = (Resources.Load(ResPath.Enemies + "CentipedeEnemy/CentipedeProjectile") as GameObject).GetComponent<CentipedeProjectile>();

        _movementRange = _fragments.GetFragmentsLength() * 0.2f;
        _startHeadTargetPos = headTarget.position;
        SetNextPoint();

		_cooldownTimer = attackCooldown;
    }

    protected override void Run()
    {
		if (!_fragments || !_fragments.IsCentipedeCapable()) return;

        if (_meleeAttacking){
            MakeMeleeAttack();
            return;
        }

		if (_cooldownTimer > 0){
			_cooldownTimer -= Time.deltaTime;
		}
		if (!_rangeAttacking && !_preparingAttack && _cooldownTimer <= 0 && (rangeAttack || meleeAttack))
            StartPreparingAttack();

        MoveTargetToPoint((_preparingAttack || _rangeAttacking) ? headAttackSpeed : headSpeed);
    }

	private void StartPreparingAttack()
	{
		Log("Centipede preparing");
		_preparingAttack = true;
		_t = 0;
		_previousPoint = headTarget.position;
		var dir = _playerLocator.GetDirectionToPlayerNorm();
		dir.y = 0;
		_nextPoint = _startHeadTargetPos - dir * _movementRange * 10;
	}

    private void MoveTargetToPoint(float speed)
    {
        _t += Time.deltaTime * speed; 
        headTarget.position = Vector3.Lerp(_previousPoint, _nextPoint, _t);

        if (_t >= 1)
        {
            SetNextPoint();
            _t = 0;
        }
    }

    private void SetNextPoint()
    {
		if (_preparingAttack){
            if (meleeAttack && CanMeleeAttack()){
                PrepareMeleeAttack();
                return;
            } else if (rangeAttack){
                StartPerformingRangeAttack();
                return;
            }
		}

        if (_meleeAttacking){
            _meleeAttacking = false;
        }

		if (_rangeAttacking){
			PerformRangeAttack();
			_previousPoint = headTarget.position;
			_nextPoint = headTarget.position;
			return;
		}

        _previousPoint = headTarget.position;
        _nextPoint = _startHeadTargetPos + Random.insideUnitSphere * _movementRange;
    }

	private void StartPerformingRangeAttack()
	{
		Log("Centipede attacking");
		_preparingAttack = false;
		_rangeAttacking = true;
		_t = 0;
		_previousPoint = headTarget.position;
		_nextPoint = _startHeadTargetPos + _playerLocator.GetDirectionToPlayerNorm() * _movementRange * 20;
	}

	private void PerformRangeAttack()
	{
		Log("Centipede range ttacking");

        for (var i = 0; i < _shootDirs.Length; i++){
            var proj = Instantiate(_projectile, _playerLocator.transform.position, Quaternion.identity);
            var dir = Quaternion.LookRotation(_playerLocator.GetDirectionToPlayerNorm()) * _shootDirs[i];
            proj.transform.rotation = dir;
            proj.GetComponent<Rigidbody>().velocity = projectileThrowPower * proj.transform.forward;
        }

		_rangeAttacking = false;
		_cooldownTimer = attackCooldown;
	}

    private void PrepareMeleeAttack(){
        _fragments.SetHeadHitBoxState(true);
        _preparingAttack = false;
        _meleeAttacking = true;
    }

    private void MakeMeleeAttack(){
        headTarget.position = _playerLocator.GetPlayerPos() + _playerLocator.GetDirectionToPlayerNorm();
        _meleeAttackTimer += Time.deltaTime;

        if (_meleeAttackTimer > meleeAttackDuration){
            StopMeleeAttack();
        }
    }

    private void StopMeleeAttack(){
        _fragments.SetHeadHitBoxState(false);
        SetNextPoint();
        _meleeAttackTimer = 0;
        _meleeAttacking = false;
    }

	public bool CanRangeAttack()
	{
		return _playerLocator.IsPlayerVisible();
	}

    public bool CanMeleeAttack(){
        return (_playerLocator.GetVectorFromPoint(_fragments.GetBaseFragmentPos()).magnitude < _fragments.GetHeight()) && _playerLocator.IsPlayerVisible();
    }
}
