using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public class HomingEnemyMovePointsInstaller : MonoInstaller
{
    private List<HomingEnemyMovePoint> homingEnemyMovePoints;
    
    public override void InstallBindings()
    {
        homingEnemyMovePoints = FindObjectsOfType<HomingEnemyMovePoint>().ToList();
        
        if (homingEnemyMovePoints == null || homingEnemyMovePoints.Count == 0)
            homingEnemyMovePoints = FindObjectsOfType<HomingEnemyMovePoint>().ToList();
        Container.Bind<List<HomingEnemyMovePoint>>().FromInstance(homingEnemyMovePoints).AsSingle().NonLazy();
    }
}
