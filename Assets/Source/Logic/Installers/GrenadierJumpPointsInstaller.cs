using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public class GrenadierJumpPointsInstaller : MonoInstaller
{
    private List<GrenadierJumpPoint> grenadierJumpPoints;
    
    public override void InstallBindings()
    {
        grenadierJumpPoints = FindObjectsOfType<GrenadierJumpPoint>().ToList();
        
        if (grenadierJumpPoints == null || grenadierJumpPoints.Count == 0)
            grenadierJumpPoints = FindObjectsOfType<GrenadierJumpPoint>().ToList();
        Container.Bind<List<GrenadierJumpPoint>>().FromInstance(grenadierJumpPoints).AsSingle().NonLazy();
    }
}
