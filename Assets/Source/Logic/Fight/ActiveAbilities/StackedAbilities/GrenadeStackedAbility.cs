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
        if (_stackedCount > 3)
            _explosive.MakeExplosiveSuper();

        var randomDisplacement = Random.insideUnitSphere * spreadPerStack * _stackedCount;
        randomDisplacement.y = Mathf.Clamp(randomDisplacement.y, -1, 50);
        randomDisplacement.y /= 2;

        _explosive.transform.position = position + randomDisplacement;
        _explosive.Explode();

        Destroy(gameObject);
    }

    public override AbilityTypes GetStackedAbilityType()
    {
        return AbilityTypes.Homing;
    }
}