using NTC.Global.Cache;
using UnityEngine;
using UnityEngine.Events;

public class LevelLeversObserver : MonoCache
{
    [SerializeField] private LevelDamageTakerObject[] connectedObjects;

    private int _deadCount;

    public UnityEvent OnAllObjectsDied = new();

    protected override void OnEnabled()
    {
        for (var i = 0; i < connectedObjects.Length; i++)
        {
            connectedObjects[i].OnHurt.AddListener(HandleObjectDie);
            connectedObjects[i].OnRecovery.AddListener(HandleObjectRevive);
        }
    }

    private void HandleObjectDie()
    {
        _deadCount++;

        if (_deadCount >= connectedObjects.Length)
        {
            for (var i = 0; i < connectedObjects.Length; i++)
            {
                connectedObjects[i].SetNeedToRecovery(false);
            }
            
            OnAllObjectsDied.Invoke();
        }
    }

    private void HandleObjectRevive()
    {
        _deadCount--;
    }
}
