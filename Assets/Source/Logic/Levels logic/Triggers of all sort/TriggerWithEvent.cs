using System;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class TriggerWithEvent : MonoBehaviour
{
    public UnityEvent OnEnter;
    public UnityEvent<Transform> OnEnterWithTransform;
    public UnityEvent OnExit;

    private void OnTriggerEnter(Collider other)
    {
        OnEnter.Invoke();
        OnEnterWithTransform.Invoke(transform);
    }

    private void OnTriggerExit(Collider other)
    {
        OnExit.Invoke();
    }
}
