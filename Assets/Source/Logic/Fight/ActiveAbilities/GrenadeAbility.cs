using System;
using NTC.Global.Pool;
using Unity.VisualScripting;
using UnityEngine;

public class GrenadeAbility : DefaultActiveAbility
{
    [Header("Grenade")]
    [SerializeField] private Grenade GrenadePrefab;

    [SerializeField] private float throwPower = 1;
        
    public override void PerformAbility()
    {
        base.PerformAbility();

        var spawnedGrenade = Instantiate(GrenadePrefab, GetStartPoint(), Quaternion.identity);
        spawnedGrenade.transform.position = GetStartPoint();
        spawnedGrenade.GetComponent<Rigidbody>().AddForce(GetPerformDirection() * throwPower, ForceMode.Impulse);
    }
}