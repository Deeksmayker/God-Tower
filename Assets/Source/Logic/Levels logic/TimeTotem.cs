using NTC.Global.Cache;
using UnityEngine;

public class TimeTotem : MonoCache
{
    [SerializeField] private bool completedOnStart;
    [SerializeField] private float timeToCancel;

    private bool _roomCleared;
    private bool _playerInArea;

    private float _timer;

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
            var enemyGroup = _room.GetComponentInChildren<GroupPlayerDetector>();
            if (enemyGroup != null)
            {
                enemyGroup.OnPlayerDetected.AddListener(StartTimer);
            }
        }
    }

    protected override void Run()
    {
        if (!_playerInArea || _roomCleared || _timer >= timeToCancel)
            return;

        _timer += Time.deltaTime;;

        _propertyBlock.SetFloat("_ColorCoverage", _timer / timeToCancel);
        _meshRenderer.SetPropertyBlock(_propertyBlock);
    }

    private void OnTriggerEnter(Collider other)
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
    }

    public void StartTimer()
    {
        _playerInArea = true;
    }

    public void StopCanceling()
    {
        _roomCleared = true;
    }
}