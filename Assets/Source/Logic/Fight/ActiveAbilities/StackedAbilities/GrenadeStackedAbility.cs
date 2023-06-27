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
        if (_stackedCount > 1)
            _explosive.MakeExplosiveSuper();

        _explosive.transform.position = position;
        _explosive.Explode();
    }
}