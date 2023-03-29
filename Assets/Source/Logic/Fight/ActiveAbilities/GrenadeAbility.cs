using System;
using NTC.Global.Pool;
using Unity.VisualScripting;
using UnityEngine;

public class GrenadeAbility : DefaultActiveAbility
{
    [Header("Other Settings")]
    [SerializeField] private GameObject GrenadePrefab;
    [SerializeField] private float TimeBeforeExplosion;
        
    public override void PerformAbility()
    {
        base.PerformAbility();

        var spawnedGrenade = Instantiate(GrenadePrefab, GetStartPoint(), Quaternion.identity);
        spawnedGrenade.transform.position = GetStartPoint();
        Debug.LogWarning($"{spawnedGrenade.transform.position}");
        spawnedGrenade.GetComponent<Rigidbody>().AddForce(GetPerformDirection(), ForceMode.Impulse);
        spawnedGrenade.GetComponent<Grenade>().StartExplosionThroughTime(TimeBeforeExplosion);
    }
}