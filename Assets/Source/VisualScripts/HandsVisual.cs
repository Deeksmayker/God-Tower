using System;
using NTC.Global.Cache;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.XR;

public class HandsVisual : MonoCache
{
    [Header("Rapid hands shaking")]
    [SerializeField] private float rapidShakeDuration;
    [SerializeField] private float rapidShakeIntensity;
    [SerializeField] private float rapidShakeAmount;

    [Header("Durable hands shaking")]
    [SerializeField] private float durableTimeToMaxIntensity;
    [SerializeField] private float durableShakeIntensity;
    [SerializeField] private float durableShakeAmount;
    
    private PlayerHand[] _playerHands;

    private void Awake()
    {
        _playerHands = GetComponentsInChildren<PlayerHand>();
    }

    protected override void OnEnabled()
    {
        for (var i = 0; i < _playerHands.Length; i++)
        {
            _playerHands[i].OnHandAbilityPerformed += HandlePerformingAbility;
            _playerHands[i].OnHandAbilityCharging += HandleChargingAbility;
            _playerHands[i].OnHandNewAbility += HandleAbilityChangeOnHand;
            _playerHands[i].OnHandEmpty += HandleEmptyHandAbility;
        }
    }

    protected override void OnDisabled()
    {
        for (var i = 0; i < _playerHands.Length; i++)
        {
            _playerHands[i].OnHandAbilityPerformed -= HandlePerformingAbility;
            _playerHands[i].OnHandAbilityCharging -= HandleChargingAbility;
            _playerHands[i].OnHandNewAbility -= HandleAbilityChangeOnHand;
            _playerHands[i].OnHandEmpty -= HandleEmptyHandAbility;
        }
    }

    private void HandleEmptyHandAbility(PlayerHand hand)
    {
        HandleAbilityChangeOnHand(hand, AbilityTypes.None);
    }

    private void HandleAbilityChangeOnHand(PlayerHand hand, AbilityTypes type)
    {
        hand.Get<Animator>().SetTrigger("StealAbility");
        switch (type)
        {
            case AbilityTypes.None:
                hand.Get<Animator>().SetTrigger(AnimationConstants.NoAbility);
                return;
            case AbilityTypes.Laser:
                hand.Get<Animator>().SetTrigger(AnimationConstants.LaserAbility);
                return;
            case AbilityTypes.Grenade:
                hand.Get<Animator>().SetTrigger(AnimationConstants.GrenadeAbility);
                return;
        }
    }

    private void HandlePerformingAbility(PlayerHand hand)
    {
        hand.GetShaker()?.StopDurableShaking();
        hand.GetShaker()?.StartRapidShaking(rapidShakeDuration, rapidShakeIntensity, rapidShakeAmount);
    }

    private void HandleChargingAbility(PlayerHand hand)
    {
        hand.GetShaker()?.StartDurableShaking(durableTimeToMaxIntensity, durableShakeIntensity, durableShakeAmount);   
    }
}
