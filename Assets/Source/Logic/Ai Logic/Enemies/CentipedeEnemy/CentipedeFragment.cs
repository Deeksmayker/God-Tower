using NTC.Global.Cache;
using UnityEngine;

public class CentipedeFragment : MonoCache
{
    private Rigidbody _rb;
    private Joint _joint;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _joint = GetComponent<Joint>();
    }

    public void SetParentJoint(Rigidbody parentRb)
    {
        _joint.connectedBody = parentRb;
    }

    public Rigidbody GetRb() => _rb;
}
