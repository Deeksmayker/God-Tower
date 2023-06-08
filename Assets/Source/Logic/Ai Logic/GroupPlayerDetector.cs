using System;
using NTC.Global.Cache;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class GroupPlayerDetector : MonoBehaviour
{
    private IAiController[] _connectedAis;

    private bool _detected;

    public UnityEvent OnPlayerDetected = new();

    private void Start()
    {
        _connectedAis = GetComponentsInChildren<IAiController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_detected)
            return;
        OnPlayerDetected.Invoke();
        _detected = true;
        for (var i = 0; i < _connectedAis.Length; i++)
        {
            _connectedAis[i].SetTargetDetected(true);
        }
    }
}
