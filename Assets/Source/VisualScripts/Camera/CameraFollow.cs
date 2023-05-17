using NTC.Global.Cache;
using UnityEngine;

public class CameraFollow : MonoCache
{
    [SerializeField] private Transform camRootTransform;

    [SerializeField] private float smoothness = 1;

    protected override void LateRun()
    {
        var desiredPosition = camRootTransform.position;
        var smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothness * Time.deltaTime);
        transform.position = desiredPosition;
        transform.rotation = camRootTransform.rotation;
    }
}