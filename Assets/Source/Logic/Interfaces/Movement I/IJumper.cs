using UnityEngine;

public interface IJumper
{
    public void Jump(Vector3 direction);
    public void SetJumpInput(bool value);
    public void AddJumpHeight(float addedValue);
    public void RestoreJumpHeight();
    public float GetJumpHeight();
    public float GetDefaultJumpHeight();
    public bool NeedToJump();
}
