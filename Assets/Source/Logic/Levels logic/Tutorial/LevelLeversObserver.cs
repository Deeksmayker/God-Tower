using NTC.Global.Cache;
using UnityEngine;
using UnityEngine.Events;

public class LevelLeversObserver : MonoCache
{
    [SerializeField] private BaseHealthHandler[] connectedObjects;
    [SerializeField] private BaseHealthHandler[] connectedEnemies;

    private int _deadCount;

    public UnityEvent OnAllObjectsDied = new();

    protected override void OnEnabled()
    {
        for (var i = 0; i < connectedObjects.Length; i++)
        {
            //connectedObjects[i].OnStun += HandleObjectDie;
            //connectedObjects[i].OnRevive += HandleObjectRevive;
        }

        for (var i = 0; i < connectedEnemies.Length; i++)
        {
            //connectedEnemies[i].OnDied += HandleObjectDie;
        }
    }

    protected override void OnDisabled()
    {
        for (var i = 0; i < connectedObjects.Length; i++)
        {
            //connectedObjects[i].OnRevive -= HandleObjectRevive;
            //connectedObjects[i].OnStun -= HandleObjectDie;
        }

        for (var i = 0; i < connectedEnemies.Length; i++)
        {
            //connectedEnemies[i].OnDied -= HandleObjectDie;
        }
    }

    private void HandleObjectDie()
    {
        _deadCount++;
        if (_deadCount >= connectedObjects.Length + connectedEnemies.Length)
        {
            for (var i = 0; i < connectedObjects.Length; i++)
            {
                ////////////////////////////connectedObjects[i].SetNeedRecovery(false);
            }
            
            OnAllObjectsDied.Invoke();
        }
    }

    private void HandleObjectRevive()
    {
        _deadCount--;
    }
}
