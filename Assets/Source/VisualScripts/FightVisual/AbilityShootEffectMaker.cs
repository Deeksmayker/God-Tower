using System;
using NTC.Global.Cache;
using NTC.Global.Pool;
using UnityEngine;
using UnityEngine.VFX;


public class AbilityShootEffectMaker : MonoCache
{
    [SerializeField] private VisualEffect effectOnShoot;

    private IActiveAbility _ability;
    private IMover _mover;
    
    private void Awake()
    {
        _ability = Get<IActiveAbility>();

        if (_ability is null)
        {
            Debug.LogWarning("No ability on effect maker");
            Destroy(gameObject);
        }

        _mover = GetComponentInParent<IMover>();
    }

    protected override void OnEnabled()
    {
        _ability.OnPerform += SpawnEffect;
    }
    
    protected override void OnDisabled()
    {
        _ability.OnPerform -= SpawnEffect;
    }

    private void SpawnEffect()
    {
        if (effectOnShoot is null)
            return;
        var effect = NightPool.Spawn(effectOnShoot, _ability.GetStartPoint(), _ability.GetRotationTargetTransform().rotation);
        effect.Play();
    }
}
