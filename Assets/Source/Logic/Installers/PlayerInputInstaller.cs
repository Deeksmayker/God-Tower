using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class PlayerInputInstaller : MonoInstaller
{
    private PlayerInput playerInput;
    
    public override void InstallBindings()
    {
        playerInput = FindObjectOfType<PlayerInput>();
        
        Container.Bind<PlayerInput>().FromInstance(playerInput).AsSingle().NonLazy();
    }
}
