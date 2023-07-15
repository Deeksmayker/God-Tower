using NTC.Global.Cache;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

public class LevelStartTrigger : MonoCache
{
    [Inject] private LevelStatisticsManager _levelStatisticsManager;

    public UnityEvent OnEnter;
    public UnityEvent<Transform> OnEnterWithTransform;
    public UnityEvent OnExit;

    private void OnTriggerEnter(Collider other)
    {
        OnEnter.Invoke();
        OnEnterWithTransform.Invoke(transform);

        if (other.TryGetComponent<PlayerUnit>(out var player))
        {
            player.HandleLevelStarted();
            _levelStatisticsManager.HandleLevelStarted();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        OnExit.Invoke();
    }
}