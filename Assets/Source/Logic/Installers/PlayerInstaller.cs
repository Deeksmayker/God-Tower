using UnityEngine;
using Zenject;

public class PlayerInstaller : MonoInstaller
{
    private PlayerUnit playerUnit;
    
    public override void InstallBindings()
    {
        playerUnit = FindObjectOfType<PlayerUnit>();
        
        Container.Bind<PlayerUnit>().FromInstance(playerUnit).AsSingle().NonLazy();
    }
}