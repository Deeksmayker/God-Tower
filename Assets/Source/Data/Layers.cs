using UnityEngine;

public static class Layers
{
    public static LayerMask PlayerBase { get; } = LayerMask.GetMask("Player");
    public static LayerMask PlayerHurtBox { get; } = LayerMask.GetMask("PlayerHurtBox");
    public static LayerMask Environment { get; } = LayerMask.GetMask("Environment"); 
    public static LayerMask EnemyHurtBox { get; } = LayerMask.GetMask("EnemyHurtBox");
    public static LayerMask EnemyBase { get; } = LayerMask.GetMask("EnemyBase");
    public static LayerMask Enemies { get; } = LayerMask.GetMask("EnemyBase", "EnemyHurtBox");
    public static LayerMask Hitable { get; } = LayerMask.GetMask("EnemyHurtBox", "EnemyProjectile", "PlayerProjectile", "EnemyBase", "Interactable");
    public static LayerMask BallHitable { get; } = LayerMask.GetMask("Environment", "EnemyHurtBox", "EnemyProjectile", "PlayerProjectile", "EnemyBase", "Interactable");
    public static LayerMask EnemyBallHitable { get; } = LayerMask.GetMask("Environment", "PlayerHurtBox");
    public static LayerMask Standable { get; } = LayerMask.GetMask("EnemyBase", "Environment");
    public static LayerMask Areas { get; } = LayerMask.GetMask("TriggerArea");
    public static LayerMask PlayerProjectile { get; } = LayerMask.GetMask("PlayerProjectile");
}
