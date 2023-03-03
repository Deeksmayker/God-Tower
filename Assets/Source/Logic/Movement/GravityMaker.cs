using NTC.Global.Cache;
using UnityEngine;


public class GravityMaker : MonoCache
{
    public float Gravity = -30;
    //public Vector3 GravityDirection = Vector3.down;

    private IMover _anyMover;

    private void Awake()
    {
        _anyMover = Get<IMover>();
    }

    protected override void Run()
    {
        if (_anyMover.IsGrounded() && Gravity < 0)
        {
            if (_anyMover.GetVelocity().y < 0)
            {
                _anyMover.SetVerticalVelocity(-1);
            }

            return;
        }

        _anyMover.AddVerticalVelocity(Gravity * Time.deltaTime);
    }
}
