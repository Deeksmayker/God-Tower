using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class PlayerInputInstaller : MonoInstaller
{
    [SerializeField] private PlayerInput playerInput;
    
    public override void InstallBindings()
    {
        Container.Bind<PlayerInput>().FromInstance(playerInput).AsSingle().NonLazy();
    }
}
