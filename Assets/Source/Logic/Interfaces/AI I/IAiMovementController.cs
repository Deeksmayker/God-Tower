using UnityEngine;

public interface IAiMovementController
{
    public void SetMoveTarget(Vector3 target);
    public void Stop();
    public void ResumeMoving();
}    

