using Code.Global.Animations;
using DG.Tweening;
using NTC.Global.Cache;
using NTC.Global.Pool;
using UnityEngine;

public class Hook : MonoCache
{
    [SerializeField] private LayerMask hookTargetLayers;
    [SerializeField] private Transform camTransform;

    [SerializeField] private float minHookPower, maxHookPower;
    [SerializeField] private float hookUpPower;
    [SerializeField] private float cooldown;
    [SerializeField] private float hookRadius, hookDistance;

    [Header("Visual")]
    [SerializeField] private VfxTargetFly vfx;
    [SerializeField] private ShakePreset hookShake;
    [SerializeField] private Animator handAnimator;

    private float _timer;
    private float _currentHookPower;

    private bool _input;

    private IMover _mover;
    private PlayerStyleController _playerStyleController;

    private void Awake()
    {
        _mover = Get<IMover>();
        _playerStyleController = Get<PlayerStyleController>();
    }

    protected override void Run()
    {
        if (_timer > 0)
        {
            _timer -= Time.deltaTime;
            return;
        }

        _currentHookPower = Mathf.Lerp(minHookPower, maxHookPower, _playerStyleController.GetCurrentStyle01());

        if (_input)
        {
            var target = CheckForHookTarget();
            if (target.Equals(Vector3.zero))
                return;

            HookToTarget(target);
            MakeHookVisual(target);
            _timer = cooldown;
        }
    }

    public Vector3 CheckForHookTarget()
    {
        if (Physics.SphereCast(camTransform.position, hookRadius, camTransform.forward, out var hitInfo, hookDistance, hookTargetLayers))
        {
            return hitInfo.point;
        }
        return Vector3.zero;
    }

    public void HookToTarget(Vector3 targetPos)
    {
        var direction = Vector3.Normalize(targetPos - transform.position);
        _mover.SetVelocity(direction * _currentHookPower);
        _mover.AddVerticalVelocity(hookUpPower);
    }

    public void MakeHookVisual(Vector3 targetPos)
    {
        var effect = NightPool.Spawn(vfx, transform.position);
        effect.SetTarget(targetPos);

        CameraService.Instance.ShakeCamera(hookShake);

        handAnimator.SetTrigger("StealAbility");
    }

    public void SetInput(bool value)
    {
        _input = value;
    }
}