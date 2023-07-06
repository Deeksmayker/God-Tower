using NTC.Global.Cache;
using UnityEngine;

public class BaseAiController : MonoCache, IAiController
{
    [SerializeField] private AudioSource ambientAudioSource;

    [SerializeField] protected float baseReloadTime = 2;
    [SerializeField] protected float maxReloadTime = 0.5f;

    protected bool _targetDetected;
    protected bool _dead;

    protected float _timeDifficulty01;
    
    protected virtual void Start()
    {
        SetTargetDetected(!GetComponentInParent<EnemyGroup>());
    }

    protected override void OnEnabled()
    {
        if (TryGetComponent<IHealthHandler>(out var health))
        {
            health.OnStun += HandleDying;
            health.OnRevive += HandleRevive;
        }
    }

    protected override void OnDisabled()
    {
        if (TryGetComponent<IHealthHandler>(out var health))
        {
            health.OnStun -= HandleDying;
            health.OnRevive -= HandleRevive;
        }
    }

    public virtual void SetTargetDetected(bool value)
    {
        _targetDetected = value;

        if (value && ambientAudioSource)
        {
            ambientAudioSource.Play();
        }
    }

    public virtual bool CanAttack()
    {
        return true;
    }

    protected virtual void HandleDying()
    {
        _dead = true;
    }

    protected virtual void HandleRevive()
    {
        _dead = false;
    }

    public void SetTimeDifficulty01(float value)
    {
        _timeDifficulty01 = value;
    }

    public float GetTimeDifficulty01() => _timeDifficulty01;
}
