using UnityEngine;
using Zenject;

public class PlayerInstaller : MonoInstaller
{
    [SerializeField] private PlayerUnit playerUnit;
    
    public override void InstallBindings()
    {
        Container.Bind<PlayerUnit>().FromInstance(playerUnit).AsSingle().NonLazy();
    }
}