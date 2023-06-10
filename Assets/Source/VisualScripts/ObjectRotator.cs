using NTC.Global.Cache;
using UnityEngine;

public class ObjectRotator : MonoCache
{
    [SerializeField] private bool rotateX, rotateY, rotateZ;
    [SerializeField] private float rotationSpeed;

    protected override void Run()
    {
        var dx = rotateX ? rotationSpeed * Time.deltaTime : 0;
        var dy = rotateY ? rotationSpeed * Time.deltaTime : 0;
        var dz = rotateZ ? rotationSpeed * Time.deltaTime : 0;

        transform.Rotate(dx, dy, dz);
    }
}