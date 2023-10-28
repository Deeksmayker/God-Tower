using NTC.Global.Cache;
using NTC.Global.Pool;
using UnityEngine;

public class PlayerBallShooter : MonoCache
{
    [SerializeField] private Transform shootPoint;
    [SerializeField] private Transform camDirection;

    [SerializeField] private float bigBallCooldown = 1f;

    private PlayerBigBall _bigBallPrefab;
    private IMover _playerMover;

    private float _bigBallCooldownTimer;

    private bool _input;
    private bool _bigBallInput;

    private void Awake()
    {
        _bigBallPrefab = (Resources.Load(ResPath.Projectiles + "PlayerBigBall") as GameObject).GetComponent<PlayerBigBall>();
        _playerMover = Get<IMover>();
    }

    protected override void Run()
    {
        if (_bigBallCooldownTimer > 0)
            _bigBallCooldownTimer -= Time.deltaTime;

        if (_bigBallInput && _bigBallCooldownTimer <= 0)
        {
            ShootBigBall();
        }

        _bigBallInput = false;
    }

    private void ShootBigBall()
    {
        var ball = Instantiate(_bigBallPrefab, shootPoint.position, camDirection.rotation);
        ball.SetVelocity(_playerMover.GetVelocity());

        _bigBallCooldownTimer = bigBallCooldown;
    }

    public void SetInput(bool input)
    {
        Log("Input here " + input);
        if (_input && !input)
        {
            _bigBallInput = true;
        }

        _input = input;
    }
}