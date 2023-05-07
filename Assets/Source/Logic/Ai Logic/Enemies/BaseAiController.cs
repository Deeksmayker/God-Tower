using NTC.Global.Cache;
using UnityEngine;

public class BaseAiController : MonoCache, IAiController
{
    [SerializeField] private AudioSource ambientAudioSource;
    
    protected bool _targetDetected;
    
    protected virtual void Start()
    {
        SetTargetDetected(!GetComponentInParent<GroupPlayerDetector>());
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
}
