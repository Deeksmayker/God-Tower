using NTC.Global.Cache;
using UnityEngine;

public abstract class StackedAbility : MonoCache
{
    protected int _stackedCount = 1;

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

    public void SetImpacter(IWorkWithStackAbilities impacter)
    {
        impacter.OnImpact += PerformOnImpact;
    }

    public void SpecifyStackedNumber(int stackedNumber)
    {
        _stackedCount = stackedNumber;
    }

    public abstract void PerformOnImpact(Vector3 position);
}