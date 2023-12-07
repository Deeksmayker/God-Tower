using UnityEngine;

public static class Layers
{
    public static LayerMask PlayerBase { get; } = LayerMask.GetMask("Player");
    public static LayerMask PlayerHurtBox { get; } = LayerMask.GetMask("PlayerHurtBox");
    public static LayerMask Environment { get; } = LayerMask.GetMask("Environment"); 
    public static LayerMask EnemyHurtBox { get; } = LayerMask.GetMask("EnemyHurtBox", "EnemyProjectile");

	public static LayerMask Hitable { get; } = LayerMask.GetMask("EnemyHurtBox", "EnemyProjectile", "PlayerProjectile");
}
