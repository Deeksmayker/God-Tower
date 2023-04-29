using UnityEngine;
using Zenject;

public class TimeControllerInstaller : MonoInstaller
{
    private TimeController _timeController;
    
    public override void InstallBindings()
    {
        _timeController = FindObjectOfType<TimeController>();
        
        Container.Bind<TimeController>().FromInstance(_timeController).AsSingle().NonLazy();
    }
}

