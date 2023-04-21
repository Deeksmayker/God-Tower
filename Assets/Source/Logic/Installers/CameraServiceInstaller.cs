using UnityEngine;
using Zenject;

public class CameraServiceInstaller : MonoInstaller
{
    [SerializeField] private CameraService cameraService;

    public override void InstallBindings()
    {
        Container.Bind<CameraService>().FromInstance(cameraService).AsSingle().NonLazy();
    }

}
