public interface IAiRangeAttackController
{
    public void TryAttack();
    /*public void AllowAttack();
    public void DisallowAttack();*/
    public float GetCurrentCooldown();
    public float GetCooldownTimer();
    public bool NeedToAttack();
}
