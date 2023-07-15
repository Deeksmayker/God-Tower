using NTC.Global.Cache;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

public class LevelEndTrigger : MonoCache
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
            player.HandleLevelEnd(transform);
            _levelStatisticsManager.HandleLevelEnded();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        OnExit.Invoke();
    }
}