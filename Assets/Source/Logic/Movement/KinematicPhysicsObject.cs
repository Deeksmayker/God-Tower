using NTC.Global.Cache;
using UnityEngine;

public class KinematicPhysicsObject : MonoCache
{
    protected Vector3 _veloctiy;

    protected Rigidbody _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    protected override void Run()
    {
        _rb.MovePosition(transform.position + _veloctiy * Time.deltaTime);
    }

    public virtual void HandleImpulse(Vector3 direction, float power)
    {
        Log("Impulse in direction " + direction + " and power " + power);
        _veloctiy = direction * power;
    }

    public void SetVelocity(Vector3 newVelocity)
    {
        _veloctiy = newVelocity;
    }

    public void AddVelocity(Vector3 addedVelocity)
    {
        _veloctiy += addedVelocity;
    }

    public Vector3 GetVelocity() => _veloctiy;
}