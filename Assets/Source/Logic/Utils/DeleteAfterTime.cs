using NTC.Global.Cache;
using UnityEngine;

public class DeleteAfterTime : MonoCache
{
    [SerializeField] private float time;

    private float _startTime;

    protected override void OnEnabled()
    {
        _startTime = Time.time;
    }

    protected override void Run()
    {
        if (Time.time - _startTime >= time)
        {
            Destroy(gameObject);
        }
    }
}