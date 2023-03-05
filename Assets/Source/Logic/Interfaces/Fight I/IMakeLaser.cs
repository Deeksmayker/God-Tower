using System;
using UnityEngine;

public interface IMakeLaser
{
    public event Action<RaycastHit> OnEnvironmentHit;
    public event Action<RaycastHit> OnHitToHitTaker;
    public event Action OnMissHit;
    
    public Vector3 GetPerformDirection();
    public Vector3 GetStartPoint();
}
