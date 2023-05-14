using System.Collections.Generic;
using System.Linq;
using Zenject;

public class RunnerMovePointsInstaller : MonoInstaller
{
    private List<RunnerMovePoint> runnerMovePoints;
    
    public override void InstallBindings()
    {
        runnerMovePoints = FindObjectsOfType<RunnerMovePoint>().ToList();
        
        if (runnerMovePoints == null || runnerMovePoints.Count == 0)
            runnerMovePoints = FindObjectsOfType<RunnerMovePoint>().ToList();
        Container.Bind<List<RunnerMovePoint>>().FromInstance(runnerMovePoints).AsSingle().NonLazy();
    }
}
