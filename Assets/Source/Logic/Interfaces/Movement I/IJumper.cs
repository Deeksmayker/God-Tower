using System;
using UnityEngine;

public interface IJumper
{
    public event Action OnJump;
    
    public void SetJumpInput(bool value);
}
