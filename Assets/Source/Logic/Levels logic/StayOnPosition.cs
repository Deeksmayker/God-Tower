using NTC.Global.Cache;
using UnityEngine;

public class StayOnPosition : MonoCache
{
    private Vector3 _originalPosition;

    private void Awake()
    {
        _originalPosition = transform.position;
    }

    protected override void Run()
    {
        transform.position = _originalPosition;
    }
}