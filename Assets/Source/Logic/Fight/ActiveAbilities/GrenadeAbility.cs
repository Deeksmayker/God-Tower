using System;
using Cinemachine.Utility;
using NTC.Global.Pool;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class GrenadeAbility : DefaultActiveAbility
{
    [Header("Grenade Settings")]
    [SerializeField] private Grenade GrenadePrefab;

    [SerializeField] private float throwPower = 10;
    [SerializeField] private float maxSpread = 30;
    [SerializeField] private int dumpedGrenadeCount = 3;

    protected override void OnEnabled()
    {
        OnDump += OnDumpGrenadeAbility;
    }
    
    protected override void OnDisabled()
    {
        OnDump -= OnDumpGrenadeAbility;
    }

    public override void PerformAbility()
    {
        base.PerformAbility();
        ThrowGrenades(1);
    }

    private void OnDumpGrenadeAbility()
    {
        ThrowGrenades(dumpedGrenadeCount);
    }
    
    /// <summary>
    /// Спавнит и придает ускорение гранатам, в количестве <paramref name="countSpawnedGrenade"/>.
    /// Если гранат больше одной, то придает каждой рандомный разброс.
    /// </summary>
    /// <param name="countSpawnedGrenade"> Количество выпускаемых гранат. </param>
    private void ThrowGrenades(int countSpawnedGrenade)
    {
        for (var i = 0; i < countSpawnedGrenade; i++)
        {
            var spawnedGrenade = Instantiate(GrenadePrefab, GetStartPoint(), directionTarget.rotation);
            
            spawnedGrenade.transform.Rotate(new Vector3(0,
                countSpawnedGrenade == 1 ? 0 : Random.Range(-maxSpread, maxSpread), 0));
            spawnedGrenade.GetComponent<Rigidbody>().velocity = spawnedGrenade.transform.forward * throwPower;
        }
    }
}