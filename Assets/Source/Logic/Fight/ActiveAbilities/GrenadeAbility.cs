using System;
using NTC.Global.Pool;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.VFX;
using Random = UnityEngine.Random;

public class GrenadeAbility : DefaultActiveAbility, ISpawnImpacter
{
    [FormerlySerializedAs("GrenadePrefab")]
    [Header("Grenade Settings")]
    [SerializeField] private BaseExplosiveObject baseExplosiveObjectPrefab;

    [SerializeField] private float throwPower = 30;
    [SerializeField] private float rotationSpeed = 10;
    [SerializeField] private float maxSpread = 30;

    public event Action<IImpacter> OnImpacterSpawned;

    public override void PerformAbility(int count)
    {
        var direction = GetPerformDirection();

        for (var i = 0; i < count; i++)
        {
            var grenade = Instantiate(baseExplosiveObjectPrefab, GetStartPoint(), directionTarget.rotation);
            var grenadeRb = grenade.Get<Rigidbody>();
            
            OnImpacterSpawned?.Invoke(grenade);

            if (i == 0)
            {
                grenadeRb.velocity = direction * throwPower;
                grenadeRb.AddTorque(RandomUtils.GetRandomNormalizedVector() * rotationSpeed);
                continue;
            }

            var randomNumberX = Random.Range(-maxSpread / 2, maxSpread / 2);
            var randomNumberY = Random.Range(-maxSpread, maxSpread);
            var randomNumberZ = Random.Range(-maxSpread, maxSpread);

            var spreadedDirection = Quaternion.Euler(randomNumberX, randomNumberY, randomNumberZ) * direction;

            grenadeRb.velocity = spreadedDirection * throwPower;
            grenadeRb.AddTorque(RandomUtils.GetRandomNormalizedVector() * rotationSpeed);
        }

        base.PerformAbility(count);
    }

    public override AbilityTypes GetType()
    {
        return AbilityTypes.Grenade;
    }
    
    public float GetThrowPower() => throwPower;
}