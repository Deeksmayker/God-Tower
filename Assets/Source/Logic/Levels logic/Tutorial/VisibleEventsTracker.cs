using UnityEngine;
using UnityEngine.Events;

public class VisibleEventsTracker : MonoBehaviour
{
    public UnityEvent OnStun = new();
    public UnityEvent OnDied = new();
    public UnityEvent OnRevive = new();

    private void OnEnable()
    {
        if (TryGetComponent<IHealthHandler>(out var healthHandler))
        {
            healthHandler.OnStun += () => OnStun.Invoke();
            healthHandler.OnRevive += () => OnRevive.Invoke();
            healthHandler.OnDied += () => OnDied.Invoke();
        }
    }
}