using System;
using UnityEngine;

public interface IMakeLaser
{
    public event Action<RaycastHit, Vector3> OnEnvironmentHit;
    public event Action<RaycastHit, Vector3> OnHitToHitTaker;
    public event Action<Vector3> OnMissHit;

    public Vector3 GetStartPoint();
}
