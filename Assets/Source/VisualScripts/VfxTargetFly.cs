using NTC.Global.Cache;
using UnityEngine.VFX;
using UnityEngine;

public class VfxTargetFly : MonoCache
{
    [SerializeField] private float speed = 5f;

    private float _timer;

    private Vector3 _targetPos;
    private Vector3 _startPoint;
    private VisualEffect _vfx;

    private void Awake()
    {
        _vfx = Get<VisualEffect>();
    }

    protected override void OnEnabled()
    {
        _timer = 0;

        if (_startPoint.Equals(Vector3.zero))
        {
            _startPoint = transform.position;
        }
    }

    protected override void Run()
    {
        if (_timer >= 1)
            return;

        _vfx.SetVector3("Target", transform.position);

        transform.position = Vector3.Lerp(_startPoint, _targetPos, _timer);

        _timer += Time.deltaTime * 2;
    }

    public void SetTarget(Vector3 targetPos)
    {
        _targetPos = targetPos;
    }

    public void SetStartPoint(Vector3 startPoint)
    {
        _startPoint = startPoint;
    }
}