using NTC.Global.Cache;
using UnityEngine.VFX;
using UnityEngine;

public class VfxTargetFly : MonoCache
{
    [SerializeField] private float speed = 5f;

    private float _timer;

    private Vector3 _targetPos;
    private VisualEffect _vfx;

    private void Awake()
    {
        _vfx = Get<VisualEffect>();
    }

    protected override void OnEnabled()
    {
        _timer = 0;
    }

    protected override void Run()
    {
        if (_timer >= 1)
            return;

        _vfx.SetVector3("Target", transform.position);

        transform.position = Vector3.Lerp(transform.position, _targetPos, speed * Time.deltaTime);

        _timer += Time.deltaTime;
    }

    public void SetTarget(Vector3 targetPos)
    {
        _targetPos = targetPos;
    }
}