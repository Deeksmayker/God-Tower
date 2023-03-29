using NTC.Global.Cache;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class CameraInputRotator : MonoCache
{
    [SerializeField] private bool hideCursor = true;
    [SerializeField] private float sensitivity;
    [SerializeField] private float topClamp, bottomClamp;
    
    public GameObject CinemachineCameraTarget;

    private float _cinemachineTargetPitch;
    private float _rotationVelocity;
    
    private PlayerInput _playerInput;

    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();

        if (hideCursor)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    protected override void LateRun()
    {
        RotateCamera();
    }


    private void RotateCamera()
    {
        var mouseDelta = _playerInput.actions["Look"].ReadValue<Vector2>();
        
        _cinemachineTargetPitch -= mouseDelta.y * sensitivity;
        _rotationVelocity = mouseDelta.x * sensitivity;

        // clamp our pitch rotation
        _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, bottomClamp, topClamp);

        // Update Cinemachine camera target pitch
        CinemachineCameraTarget.transform.localRotation = Quaternion.Euler(_cinemachineTargetPitch, 0.0f, 0.0f);

        // rotate the player left and right
        transform.Rotate(Vector3.up * _rotationVelocity);
    }    
    
    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }
}
