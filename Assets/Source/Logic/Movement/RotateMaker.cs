using DG.Tweening;
using NTC.Global.Cache;
using UnityEngine;

public class RotateMaker : MonoCache
{
    private Vector3 _currentRotationToApply;

    private bool _rotatingByTorque;

    protected override void Run()
    {
        if (_currentRotationToApply.Equals(Vector3.zero))
        {
            return;
        }
        if (_rotatingByTorque )
        {
            transform.Rotate(_currentRotationToApply * Time.deltaTime);
        }
    }

    public void SetTorque(Vector3 addedTorque)
    {
        _currentRotationToApply = addedTorque;
        _rotatingByTorque = true;
    }

    public void StopTorque()
    {
        _currentRotationToApply = Vector3.zero;
    }

    public async void SmoothRotateToAngle()
    {
        //transform.DORo
    }
}