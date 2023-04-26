using UnityEngine;
using Zenject;

public class CameraServiceInstaller : MonoInstaller
{
    private CameraService cameraService;

    public override void InstallBindings()
    {
        cameraService = FindObjectOfType<CameraService>();
        
        Container.Bind<CameraService>().FromInstance(cameraService).AsSingle().NonLazy();
    }

}
