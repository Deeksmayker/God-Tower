using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public class GrenadierJumpPointsInstaller : MonoInstaller
{
    [SerializeField] private List<GrenadierJumpPoint> grenadierJumpPoints;
    
    public override void InstallBindings()
    {
        if (grenadierJumpPoints == null || grenadierJumpPoints.Count == 0)
            grenadierJumpPoints = FindObjectsOfType<GrenadierJumpPoint>().ToList();
        Container.Bind<List<GrenadierJumpPoint>>().FromInstance(grenadierJumpPoints).AsSingle().NonLazy();
    }
}
