using NTC.Global.Cache;
using UnityEngine;

public class TimeTotem : MonoCache
{
    [SerializeField] private bool completedOnStart;
    [SerializeField] private float timeToMaxDifficulty;

    private bool _roomCleared;
    private bool _playerInArea;

    private float _timer;
    private float _secondsTimer;

    private RoomParent _room;
    private MeshRenderer _meshRenderer;
    private MaterialPropertyBlock _propertyBlock;

    private void Awake()
    {
        _room = GetComponentInParent<RoomParent>();
        _meshRenderer = Get<MeshRenderer>();
        _propertyBlock = new MaterialPropertyBlock();
        _meshRenderer.GetPropertyBlock(_propertyBlock);

        if (completedOnStart)
        {
            _roomCleared = true;
            _propertyBlock.SetFloat("_ColorCoverage", 0.5f);
            _meshRenderer.SetPropertyBlock(_propertyBlock);
        }
    }   

    protected override void OnEnabled()
    {
        if (_room != null)
        {
            var enemyGroup = _room.GetEnemyGroups();
            if (enemyGroup != null)
            {
                for (var i = 0; i < enemyGroup.Length; i++)
                {
                    enemyGroup[i].OnPlayerDetected.AddListener(StartTimer);
                }
            }
        }
    }

    protected override void Run()
    {
        if (!_playerInArea || _roomCleared || _timer >= timeToMaxDifficulty)
            return;

        _timer += Time.deltaTime;;
        _secondsTimer += Time.deltaTime;

        if (_secondsTimer >= 1)
        {
            SetDifficultyToEnemies();
            _secondsTimer -= 1;
        }


        _propertyBlock.SetFloat("_ColorCoverage", _timer / timeToMaxDifficulty);
        _meshRenderer.SetPropertyBlock(_propertyBlock);
    }

    private void SetDifficultyToEnemies()
    {
        for (var i = 0; i < _room.GetEnemyGroups().Length; i++)
        {
            var group = _room.GetEnemyGroups()[i];
            for (var j = 0; j < group.GetConnectedEnemies().Length; j++)
            {
                var enemy = group.GetConnectedEnemies()[j];
                enemy.SetTimeDifficulty01(Mathf.Clamp01(Mathf.InverseLerp(0, timeToMaxDifficulty, _timer)));
            }
        }
    }

    public void SetDifficultyMultiplier(float multiplier)
    {
        timeToMaxDifficulty *= multiplier;
    }

    /*private void OnTriggerEnter(Collider other)
    {
        if (_roomCleared && _timer < timeToCancel && other.TryGetComponent<PlayerStyleController>(out var style))
        {
            style.SetStyleToMax();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (_roomCleared && _timer < timeToCancel && other.TryGetComponent<PlayerStyleController>(out var style))
        {
            style.SetStyleToMax();
        }
    }*/

    public void StartTimer()
    {
        _playerInArea = true;
    }

    public void StopCanceling()
    {
        _roomCleared = true;
    }
}