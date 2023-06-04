using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class LevelStatisticsManagerInstaller : MonoInstaller
{
    private LevelStatisticsManager LevelStatisticsManager;

    public override void InstallBindings()
    {
        LevelStatisticsManager = FindObjectOfType<LevelStatisticsManager>();

        Container.Bind<LevelStatisticsManager>().FromInstance(LevelStatisticsManager).AsSingle().NonLazy();
    }
}