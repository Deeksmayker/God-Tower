using System;
using NTC.Global.Cache;
using UnityEngine;

public class PlayerUnit : MonoCache
{
    [SerializeField] private Transform cameraRoot;
    [SerializeField] private bool walkForwardOnStart = true;

    private static bool _levelStarted;
    private static bool _levelEnded;
    private Transform _targetRotationTransform;

    private float _timer;

    private IMover _mover;

    private void Awake()
    {
        _levelEnded = false;
        Application.targetFrameRate = 200;
        _targetRotationTransform = transform;
        _mover = Get<IMover>();

        if (walkForwardOnStart)
        {
            Get<PlayerInputHandler>().DisableInputResponse();
            var camera = GetComponentInChildren<CameraLook>();
            camera.DisableInputResponse();
        }
    }

    protected override void LateRun()
    {
        if (!_levelEnded && (_timer > 1 || !walkForwardOnStart))
            return;

        _mover.SetInput(new Vector2(0, 1));
        transform.rotation = Quaternion.Slerp(transform.rotation, _targetRotationTransform.rotation, 3 * Time.deltaTime);
        cameraRoot.rotation = Quaternion.Slerp(cameraRoot.rotation, _targetRotationTransform.rotation, 3 * Time.deltaTime);

        if (_levelEnded)
            return;

        _timer += Time.deltaTime;
        if (_timer > 1)
        {
            Get<PlayerInputHandler>().EnableInputResponse();
            var camera = GetComponentInChildren<CameraLook>();
            camera.EnableInputResponse();
        }
    }

    public void HandleLevelEnd(Transform targetTransform)
    {
        _levelEnded = true;
        Get<PlayerInputHandler>().DisableInputResponse();
        var camera = GetComponentInChildren<CameraLook>();
        camera.DisableInputResponse();
        _targetRotationTransform = targetTransform;
    }

    public void HandleLevelStarted()
    {
        _levelStarted = true;
    }

    public void TeleportPlayer(Vector3 pos)
    {
        Get<CharacterController>().enabled = false;
        transform.position = pos;
        Get<CharacterController>().enabled = true;
    }

    public static bool LevelEnded() => _levelEnded;
    public static bool LevelStarted() => _levelStarted;
}
