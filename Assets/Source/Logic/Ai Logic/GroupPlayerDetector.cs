using System;
using NTC.Global.Cache;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class GroupPlayerDetector : MonoBehaviour
{
    private IAiController[] _connectedAis;

    private bool _detected;

    private void Start()
    {
        _connectedAis = GetComponentsInChildren<IAiController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_detected)
            return;
        _detected = true;
        for (var i = 0; i < _connectedAis.Length; i++)
        {
            _connectedAis[i].SetTargetDetected(true);
        }
    }
}
