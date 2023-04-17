using System;
using NTC.Global.Pool;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.VFX;
using Random = UnityEngine.Random;

public class GrenadeAbility : DefaultActiveAbility
{
    [FormerlySerializedAs("GrenadePrefab")]
    [Header("Grenade Settings")]
    [SerializeField] private BaseExplosiveObject baseExplosiveObjectPrefab;

    [SerializeField] private float throwPower = 30;
    [SerializeField] private float rotationSpeed = 10;
    [SerializeField] private float maxSpread = 30;

    public override void PerformAbility(bool isDumping = false)
    {
        base.PerformAbility(isDumping);

        var grenade = Instantiate(baseExplosiveObjectPrefab, GetStartPoint(), directionTarget.rotation);

        if (isDumping)
        {
            var randomNumberX = Random.Range(-maxSpread/2, maxSpread/2);
            var randomNumberY = Random.Range(-maxSpread, maxSpread);
            var randomNumberZ = Random.Range(-maxSpread, maxSpread);

            grenade.transform.Rotate(randomNumberX, randomNumberY, randomNumberZ);
        }

        grenade.Get<Rigidbody>().velocity = grenade.transform.forward * throwPower;
        grenade.Get<Rigidbody>().AddTorque(RandomUtils.GetRandomNormalizedVector() * rotationSpeed);
    }

    public float GetThrowPower() => throwPower;
}