using UnityEngine;
using NTC.Global.Cache;

public class CentipedeAi : MonoCache
{
    [SerializeField] private Transform headTarget;
    [SerializeField] private float attackCooldown;
    [SerializeField] private float headSpeed = 1f;
	[SerializeField] private float headAttackSpeed = .5f;

	[SerializeField] private float projectileThrowPower;

    [SerializeField] private bool _rangeAttack = true;
    [SerializeField] private bool _meleeAttack;
	private bool _preparingAttack;
    private bool _attacking;

	private float _cooldownTimer;

    private Vector3 _startHeadTargetPos;
    private Vector3 _previousPoint, _nextPoint;
    private float _movementRange;

    private float _t;

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

		if (_cooldownTimer > 0){
			_cooldownTimer -= Time.deltaTime;
		}
		if (!_attacking && !_preparingAttack && _cooldownTimer <= 0 && _rangeAttack) StartPreparingRangeAttack();

        MoveTargetToPoint((_preparingAttack || _attacking) ? headAttackSpeed : headSpeed);
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
			StartPerformingRangeAttack();
			return;
		}

		if (_attacking){
			PerformRangeAttack();
			_previousPoint = headTarget.position;
			_nextPoint = headTarget.position;
			return;
		}

        _previousPoint = headTarget.position;
        _nextPoint = _startHeadTargetPos + Random.insideUnitSphere * _movementRange;
    }

	private void StartPreparingRangeAttack()
	{
		Log("Centipede preparing");
		_preparingAttack = true;
		_t = 0;
		_previousPoint = headTarget.position;
		var dir = _playerLocator.GetDirectionToPlayerNorm();
		dir.y = 0;
		_nextPoint = _startHeadTargetPos - dir * _movementRange * 10;
	}

	private void StartPerformingRangeAttack()
	{
		Log("Centipede attacking");
		_preparingAttack = false;
		_attacking = true;
		_t = 0;
		_previousPoint = headTarget.position;
		_nextPoint = _startHeadTargetPos + _playerLocator.GetDirectionToPlayerNorm() * _movementRange * 20;
	}

	private void PerformRangeAttack()
	{
		Log("Centipede range ttacking");

		var proj = Instantiate(_projectile, _playerLocator.transform.position, Quaternion.identity);
		var dir = _playerLocator.GetDirectionToPlayerNorm();
		dir.y += 0.05f;
		proj.GetComponent<Rigidbody>().AddForce(projectileThrowPower * dir, ForceMode.Force);

		_attacking = false;
		_cooldownTimer = attackCooldown;
	}

	public bool CanRangeAttack()
	{
		return _playerLocator.IsPlayerVisible();
	}
}
