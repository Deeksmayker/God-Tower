using System;
using NTC.Global.Cache;
using NTC.Global.Pool;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class LaserAbility : DefaultActiveAbility
{
    [SerializeField] private bool _shootOnInput = true;

    [Header("Dump")]
    [SerializeField] private float spreadAngles;

    private LaserShooter _shooter;

    private void Awake()
    {
        _shooter = Get<LaserShooter>();
    }

    public override void PerformAbility(int count)
    {
        if (!_shootOnInput)
            return;

        var startPoint = directionTarget.position;

        var direction = GetPerformDirection();

        for (var i = 0; i < count; i++)
        {
            if (i == 0)
            {
                _shooter.ShootLaser(startPoint, direction);
                continue;
            }

            var randomNumberX = Random.Range(-spreadAngles, spreadAngles);
            var randomNumberY = Random.Range(-spreadAngles, spreadAngles);
            var randomNumberZ = Random.Range(-spreadAngles, spreadAngles);

            var spreadedDirection = Quaternion.Euler(randomNumberX, randomNumberY, randomNumberZ) * direction;

            _shooter.ShootLaser(startPoint, spreadedDirection);
        }

        base.PerformAbility(count);
    }

    public override AbilityTypes GetType()
    {
        return AbilityTypes.Laser;
    }
}
