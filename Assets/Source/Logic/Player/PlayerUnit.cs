using System;
using NTC.Global.Cache;
using UnityEngine;

public class PlayerUnit : MonoCache
{
    [SerializeField] private Transform cameraRoot;

    private static bool _levelEnded;
    private Transform _targetRotationTransform;

    private IMover _mover;

    public static event Action OnLevelEnd;

    private void Awake()
    {
        _levelEnded = false;
        Application.targetFrameRate = 200;
    }

    protected override void Run()
    {
        if (!_levelEnded)
            return;

        _mover.SetHorizontalInput(new Vector2(0, 1));
        transform.rotation = Quaternion.Slerp(transform.rotation, _targetRotationTransform.rotation, 3 * Time.deltaTime);
        cameraRoot.rotation = Quaternion.Slerp(cameraRoot.rotation, _targetRotationTransform.rotation, 3 * Time.deltaTime);
    }

    public void HandleLevelEnd(Transform targetTransform)
    {
        _levelEnded = true;
        Get<PlayerInputHandler>().DisableInputResponse();
        var camera = GetComponentInChildren<CameraLook>();
        camera.DisableInputResponse();
        _targetRotationTransform = targetTransform;
        _mover = Get<IMover>();
        OnLevelEnd?.Invoke();
    }

    public static bool LevelEnded() => _levelEnded;
}
