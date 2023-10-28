using Code.Global.Animations;
using DG.Tweening;
using NTC.Global.Cache;
using NTC.Global.Pool;
using System;
using UnityEngine;
using UnityEngine.VFX;

public class Hook : MonoCache
{
    [SerializeField] private LayerMask hookTargetLayers;
    [SerializeField] private Transform camTransform;

    [SerializeField] private float hookInitialPower;
    [SerializeField] private float hookPower;
    [SerializeField] private float hookUpPower;
    [SerializeField] private float cooldown;
    [SerializeField] private float hookRadius, hookDistance;

    [Header("Visual")]
    [SerializeField] private VfxTargetFly vfx;
    [SerializeField] private ShakePreset hookShake;
    [SerializeField] private Animator handAnimator;

    private Vector3 _hookPoint;

    private float _timer;

    private bool _input;
    private bool _hooking;

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

        if (_hooking)
        {
            HookToTarget(_hookPoint);
        }

        /*if (_input)
        {
            var target = CheckForHookTarget();
            if (target.Equals(Vector3.zero))
                return;

            HookToTarget(target);
            MakeHookVisual(target);
            _timer = cooldown;
        }*/
    }

    public Vector3 CheckForHookTarget()
    {
        if (Physics.SphereCast(camTransform.position - camTransform.forward * hookRadius / 4, hookRadius, camTransform.forward, out var hitInfo, hookDistance, hookTargetLayers))
        {
            return hitInfo.point;
        }
        return Vector3.zero;
    }

    public void HookToTarget(Vector3 targetPos)
    {
        var direction = targetPos - transform.position;

        var speedMultiplier = direction.magnitude / 50;
        speedMultiplier = Mathf.Clamp(speedMultiplier, 0.5f, 1);

        var addedSpeed = hookPower * speedMultiplier * Time.deltaTime * direction.normalized;

        var velocityDirectionDot = Vector3.Dot(direction.normalized, _mover.GetVelocity().normalized);

        if (speedMultiplier < 1 && velocityDirectionDot > 0.3f && (velocityDirectionDot * _mover.GetVelocity()).magnitude > 50)
        {
            addedSpeed = -addedSpeed * (1f - speedMultiplier);
        }

        _mover.AddVelocity(addedSpeed);
        _mover.AddVerticalVelocity(hookUpPower * 2 * Time.deltaTime);

        var effect = NightPool.Spawn(vfx, transform.position, Quaternion.identity);
        effect.SetTarget(targetPos);
        effect.SetStartPoint(transform.position);

        /*var direction = (targetPos - transform.position).normalized;
        _mover.SetVelocity(direction * hookPower);
        _mover.AddVerticalVelocity(hookUpPower);*/

        //OnHook?.Invoke();
    }

    public void MakeInitialHook(Vector3 targetPos)
    {
        var directionNorm = (_hookPoint - transform.position).normalized;

        var velocityDot = Vector3.Dot(directionNorm, _mover.GetVelocity().normalized);

        var addedVelocity = velocityDot > 0 ? _mover.GetVelocity().magnitude * velocityDot : 0;

        _mover.SetVelocity(directionNorm * (hookInitialPower + addedVelocity));
        _mover.AddVerticalVelocity(hookUpPower);
    }

    public void MakeHookVisual(Vector3 targetPos)
    {
        var effect = NightPool.Spawn(vfx, transform.position, Quaternion.identity);
        effect.SetTarget(targetPos);
        effect.SetStartPoint(transform.position);

        CameraService.Instance.ShakeCamera(0.2f);

        handAnimator.SetTrigger("StealAbility");
    }

    public void SetInput(bool value)
    {
        if (!_hooking && value && _timer <= 0)
        {
            _hookPoint = CheckForHookTarget();

            if (!_hookPoint.Equals(Vector3.zero))
            {
                _hooking = true;

                MakeInitialHook(_hookPoint);

                MakeHookVisual(_hookPoint);
                OnHook?.Invoke();
            }
        }

        if (_hooking && !value)
        {
            _hooking = false;
            _timer = cooldown;
        }

        _input = value;
    }
}