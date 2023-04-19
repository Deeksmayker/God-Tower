using DG.Tweening;
using NTC.Global.Cache;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class CameraRotating : MonoCache
{
    [Inject] private PlayerInput _playerInput;

    [SerializeField] private int angleOfRotationForZ = 20;
    //[SerializeField] private int angleOfRotationForX = 40;

    [SerializeField] private float rotationSpeed = 5;

    protected override void Run()
    {
        RotateHead(_playerInput.actions["Move"].ReadValue<Vector2>());
    }

    private void RotateHead(Vector2 input)
    {
        var desiredZAngle = -input.x * angleOfRotationForZ;
        
        var desiredAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, desiredZAngle);

        var needRotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(desiredAngles), Time.deltaTime * rotationSpeed);

        transform.rotation = needRotation;
    }
}
