using NTC.Global.Cache;
using UnityEngine;
using System;

public class CentipedeFragment : MonoCache
{
    public event Action<int> OnFragmentHit;

    private Rigidbody _rb;
    private Joint _joint;

    private int _index;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _joint = GetComponent<Joint>();
    }

    protected override void OnEnabled()
    {
        var hitTaker = GetComponent<ITakeHit>();
        if (hitTaker != null)
        {
            hitTaker.OnTakeHit += HandleFragmentHit;
        }
    }

    protected override void OnDisabled()
    {
        var hitTaker = GetComponent<ITakeHit>();
        if (hitTaker != null)
        {
            hitTaker.OnTakeHit -= HandleFragmentHit;
        }
    }

    public void SetParentJoint(Rigidbody parentRb)
    {
        _joint.connectedBody = parentRb;
    }

    public void SetIndex(int i) => _index = i;

    private void HandleFragmentHit(float dmg)
    {
        OnFragmentHit?.Invoke(_index);
    }

    public Rigidbody GetRb() => _rb;
}
