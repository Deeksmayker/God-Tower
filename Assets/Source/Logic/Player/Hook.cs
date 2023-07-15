using Code.Global.Animations;
using DG.Tweening;
using NTC.Global.Cache;
using NTC.Global.Pool;
using System;
using UnityEngine;

public class Hook : MonoCache
{
    [SerializeField] private LayerMask hookTargetLayers;
    [SerializeField] private Transform camTransform;

    [SerializeField] private float hookPower;
    [SerializeField] private float hookUpPower;
    [SerializeField] private float cooldown;
    [SerializeField] private float hookRadius, hookDistance;

    [Header("Visual")]
    [SerializeField] private VfxTargetFly vfx;
    [SerializeField] private ShakePreset hookShake;
    [SerializeField] private Animator handAnimator;

    private float _timer;

    private bool _input;

    private IMover _mover;
    //private PlayerStyleController _playerStyleController;

    public event Action OnHook;

    private void Awake()
    {
        _mover = Get<IMover>();
        //_playerStyleController = Get<PlayerStyleController>();
    }

    protected override void Run()
    {
        if (_timer > 0)
        {
            _timer -= Time.deltaTime;
            return;
        }

        //_currentHookPower = Mathf.Lerp(minHookPower, maxHookPower, _playerStyleController.GetCurrentStyle01());

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
        if (Physics.SphereCast(camTransform.position - camTransform.forward * hookRadius / 2, hookRadius, camTransform.forward, out var hitInfo, hookDistance, hookTargetLayers))
        {
            return hitInfo.point;
        }
        return Vector3.zero;
    }

    public void HookToTarget(Vector3 targetPos)
    {
        var direction = (targetPos - transform.position).normalized;
        _mover.SetVelocity(direction * hookPower);
        _mover.AddVerticalVelocity(hookUpPower);

        OnHook?.Invoke();
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