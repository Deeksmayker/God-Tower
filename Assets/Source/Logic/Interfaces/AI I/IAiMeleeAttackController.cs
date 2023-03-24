using UnityEngine;

public interface IAiMeleeAttackController
{
    public void StartAttack();
    public void AllowAttack();
    public void DisallowAttack();
    public bool NeedToAttack();
}
