using System;
using UnityEngine;

public interface IMeleeAttacker
{
    public enum MeleeAttackStates
    {
        Resting,
        Preparing,
        Attacking,
        Cooldown
    }
    
    public event Action OnStartPreparingAttack;
    public event Action OnStartAttack;
    public event Action OnEndAttack;
    public event Action OnHit;
    
    public void PerformAttack();
    public void SetInput(bool input);
    public void SetAttackStateToNext();
    public void AllowAttack();
    public void DisallowAttack();
    public MeleeAttackStates GetCurrentAttackState();
    public float GetAttackPrepareTime();
    public float GetAttackDuration();
    public float GetAttackCooldown();
    public Vector3 GetAttackDirection();
    public bool NeedToAttack();
}
