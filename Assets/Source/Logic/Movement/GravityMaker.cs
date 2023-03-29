using NTC.Global.Cache;
using UnityEngine;


public class GravityMaker : MonoCache
{
    public float RiseGravity = -50;

    public float FallingGravity = -30;
    //public Vector3 GravityDirection = Vector3.down;

    private IMover _mover;

    private void Awake()
    {
        _mover = Get<IMover>();
    }

    protected override void Run()
    {
        if (_mover.IsGrounded() && RiseGravity < 0)
        {
            if (_mover.GetVelocity().y < 0)
            {
                _mover.SetVerticalVelocity(-10);
            }

            return;
        }

        var gravity = _mover.GetVelocity().y >= 0 ? RiseGravity : FallingGravity;
        
        _mover.AddVerticalVelocity(gravity * Time.deltaTime);
    }
}
