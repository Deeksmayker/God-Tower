using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class GrenadierJumpPointsInstaller : MonoInstaller
{
    [SerializeField] private List<GrenadierJumpPoint> grenadierJumpPoints;
    
    public override void InstallBindings()
    {
        Container.Bind<List<GrenadierJumpPoint>>().FromInstance(grenadierJumpPoints).AsSingle().NonLazy();
    }
}
