using DG.Tweening;
using NTC.Global.Cache;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class CameraRotating : MonoCache
{
    [Inject] private PlayerInput _playerInput;

    [SerializeField] private int angleOfRotationForZ = 5;
    [SerializeField] private int angleOfRotationForX = 5;

    [SerializeField] private float rotationSpeed = 5;

    protected override void Run()
    {
        RotateHead(_playerInput.actions["Move"].ReadValue<Vector2>());
    }

    private void RotateHead(Vector2 input)
    {
        var desiredZAngle = -input.x * angleOfRotationForZ;
        var desiredXAngle = input.y < 0 ? input.y * angleOfRotationForX : 0;
        
        var desiredAngles = new Vector3(desiredXAngle, transform.localRotation.eulerAngles.y, desiredZAngle);

        var needRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(desiredAngles), Time.deltaTime * rotationSpeed);

        transform.localRotation = needRotation;
    }
}
