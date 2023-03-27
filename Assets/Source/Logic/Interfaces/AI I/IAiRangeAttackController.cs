public interface IAiRangeAttackController
{
    public void TryAttack();
    public void AllowAttack();
    public void DisallowAttack();
    public float GetChargingTime();
    public bool NeedToAttack();
}
