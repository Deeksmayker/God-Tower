using UnityEngine;

public class GrenadeStackedAbility : StackedAbility
{
    [SerializeField] private float spreadPerStack = 5;

    private BaseExplosiveObject _explosive;

    private void Awake()
    {
        _explosive = Get<BaseExplosiveObject>();
    }

    public override void PerformOnImpact(Vector3 position)
    {
        var stackedCount = transform.parent.GetComponentsInChildren<GrenadeStackedAbility>().Length;

        if (stackedCount > 3)
            _explosive.MakeExplosiveSuper();

        var randomDisplacement = stackedCount * spreadPerStack * Random.insideUnitSphere;
        randomDisplacement.y = Mathf.Clamp(randomDisplacement.y, -1, 50);
        randomDisplacement.y /= 2;

        _explosive.transform.position = position + randomDisplacement;
        _explosive.Explode();

        TimeController.Instance.AddTimeStopDuration(0.01f);
        base.PerformOnImpact(position);
        _performed = true;
    }

    public override AbilityTypes GetStackedAbilityType()
    {
        return AbilityTypes.Homing;
    }
}