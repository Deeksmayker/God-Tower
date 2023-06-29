﻿using System;
using Cysharp.Threading.Tasks.Triggers;
using DG.Tweening;
using NTC.Global.Cache;
using NTC.Global.Pool;
using UnityEngine;


public enum AbilityTypes
{
    None, 
    Laser,
    Grenade,
    Homing
}

public class AbilitiesHandler : MonoCache
{
    [SerializeField] private LayerMask layersToSteal;
    [SerializeField] private VfxTargetFly stealLineEffect;
    [SerializeField] private Transform camRotationTarget;
    [SerializeField] private Transform rightHandShootPoint, leftHandShootPoint;
    [SerializeField] private float stealDistance = 10;
    [SerializeField] private float stealRadius = 0.5f;
    [SerializeField] private float healBySteal = 20;

    private int _stackedAbilitiesCount;

    private IActiveAbility _leftAbility;
    private GameObject _currentAbilityObject;

    public event Action OnStealAbility;
    public event Action OnNewAbility;
    public event Action<StackedAbility> OnNewStackedAbility;

    protected override void OnEnabled()
    {
        OnNewAbility += HandleNewLeftAbility;
    }

    public void SetNewLeftAbility(IGiveAbility abilityGiver, bool isInfinite = false)
    {
/*        if (_leftAbility != null)
        {
            _leftAbility.RemoveAbility();
        }*/
        SetNewAbility(abilityGiver, leftHandShootPoint, isInfinite);
        Get<IHealthHandler>().AddHealth(healBySteal);
    }

    public void SetLeftAbilityInput(bool input)
    {
        _leftAbility?.SetInput(input);
    }

    public void SetLeftStealInput(bool input)
    {
        if (!input)
            return;

        var robbedAbility = CheckForStealAbility();
        if (robbedAbility != null)
        {
            SetNewLeftAbility(robbedAbility);
        }
    }

    public void RemoveAbilities()
    {
        _leftAbility?.RemoveAbility();
    }

    public void SetStealRadius(float newRadius)
    {
        stealDistance = newRadius;
    }

    private IGiveAbility CheckForStealAbility()
    {
        if (Physics.SphereCast(camRotationTarget.position - camRotationTarget.forward * stealRadius, stealRadius, camRotationTarget.forward, out var hit, stealDistance,
                layersToSteal))
        {
            if (hit.transform.TryGetComponent<IGiveAbility>(out var giver) && giver.CanGiveAbility())
            {
                var effect = NightPool.Spawn(stealLineEffect, rightHandShootPoint);
                effect.SetTarget(hit.transform.position);

                return giver;
            }
        }

        return null;
    }

    private void SetNewAbility(IGiveAbility abilityGiver, Transform shootPoint, bool isInfinite)
    {
        OnStealAbility?.Invoke();

        if (_leftAbility != null)
        {
            var stackedAbility = Instantiate(abilityGiver.GetStackedAbilityPrefab(), transform);
            _stackedAbilitiesCount++;
            stackedAbility.SpecifyStackedNumber(_stackedAbilitiesCount);

            OnNewStackedAbility?.Invoke(stackedAbility);

            if (_currentAbilityObject.TryGetComponent<IImpacter>(out var impacter))
            {
                stackedAbility.SetImpacter(impacter);
                return;
            }

            if (_currentAbilityObject.TryGetComponent<ISpawnImpacter>(out var impacterSpawner))
            {
                impacterSpawner.OnImpacterSpawned += stackedAbility.SetImpacter;
                return;
            }

            _leftAbility.RemoveAbility();
        }

        _currentAbilityObject = Instantiate(abilityGiver.GetAbilityPrefab(), transform);
        _currentAbilityObject.transform.position = shootPoint.position;
        _leftAbility = _currentAbilityObject.GetComponent<IActiveAbility>();
        _leftAbility.SetRotationTarget(camRotationTarget);
        _leftAbility.SetShootPoint(shootPoint);
        _leftAbility.SetInfinity(isInfinite || _leftAbility.IsInfinite());

        OnNewAbility?.Invoke();
    }

    private void HandleNewLeftAbility()
    {
        _leftAbility.OnEmpty += HandleLeftAbilityEmpty;
    }

    private void HandleLeftAbilityEmpty()
    {
        _leftAbility = null;
        _stackedAbilitiesCount = 0;
    }
    public IActiveAbility GetLeftAbility() => _leftAbility;

    public int GetStackedAbilitiesCount() => _stackedAbilitiesCount;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(camRotationTarget.position, stealRadius);
        Gizmos.DrawLine(camRotationTarget.position, camRotationTarget.forward * stealDistance);
    }
}
