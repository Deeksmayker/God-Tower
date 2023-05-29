using UnityEngine;
using Zenject;

public class PostProcessingControllerInstaller : MonoInstaller
{
    private PostProcessingController postProcessingController;

    public override void InstallBindings()
    {
        postProcessingController = FindObjectOfType<PostProcessingController>();
        
        Container.Bind<PostProcessingController>().FromInstance(postProcessingController).AsSingle().NonLazy();
    }
}
