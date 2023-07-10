using NTC.Global.Cache;
using System;
using UnityEngine;

public abstract class StackedAbility : MonoCache
{
    protected int _stackedCount = 0;

    private float _timer;
    protected bool _performed;

    public event Action<Vector3> OnPerform;

    /*protected override void OnEnabled()
    {
        var impacter = GetComponentInParent<IWorkWithStackAbilities>();
        if (impacter == null)
        {
            Debug.LogError("No impacter on stacked ability");

        }

        impacter.OnImpact += PerformOnImpact;
    }*/

    /*protected override void OnDisabled()
    {
        var impacter = GetComponentInParent<IWorkWithStackAbilities>();
        if (impacter == null)
        {
            Destroy(this);
        }

        impacter.OnImpact -= PerformOnImpact;
    }*/

    protected override void Run()
    {
        if (_performed )
        {
            _timer += Time.deltaTime;
            if (_timer >= 2)
                Destroy(gameObject);
        }   
    }

    public void SetImpacter(IImpacter impacter)
    {
        impacter.OnImpact += PerformOnImpact;
    }

    public virtual void AddStackedCount()
    {
        _stackedCount++;
    }

    public virtual void PerformOnImpact(Vector3 position)
    {
        OnPerform?.Invoke(position);
    }

    public virtual AbilityTypes GetStackedAbilityType()
    {
        return AbilityTypes.None;
    }
}