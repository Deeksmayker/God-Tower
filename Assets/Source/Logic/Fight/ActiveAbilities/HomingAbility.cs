using UnityEngine;

public class HomingAbility : DefaultActiveAbility, ISpawnImpacter
{
    [SerializeField] private BaseHomingObject HomingPrefab;
    [SerializeField] private float maxSpread = 30f;

    public event System.Action<IImpacter> OnImpacterSpawned;

    public override void PerformAbility(int count)
    {
        var direction = GetPerformDirection();

        for (var i = 0; i < count; i++)
        {
            var homing = Instantiate(HomingPrefab, GetStartPoint(), directionTarget.rotation);
            var homingRb = homing.Get<Rigidbody>();

            OnImpacterSpawned?.Invoke(homing);

            var randomNumberX = Random.Range(-maxSpread / 2, maxSpread / 2);
            var randomNumberY = Random.Range(-maxSpread, maxSpread);
            var randomNumberZ = Random.Range(-maxSpread, maxSpread);

            var spreadedDirection = Quaternion.Euler(randomNumberX, randomNumberY, randomNumberZ) * direction;

            homingRb.velocity = (spreadedDirection + 0.5f * homing.transform.up) * 10;
            homingRb.AddTorque(RandomUtils.GetRandomNormalizedVector() * 10);
        }

        base.PerformAbility(count);
    }

    public override AbilityTypes GetType()
    {
        return AbilityTypes.Homing;
    }
}
