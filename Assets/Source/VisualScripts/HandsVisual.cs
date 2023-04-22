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
            _playerHands[i].OnHandEndCharging += HandleEndChargingAbility;
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
            _playerHands[i].OnHandEndCharging -= HandleEndChargingAbility;
        }
    }

    private void HandleEmptyHandAbility(PlayerHand hand)
    {
        HandleAbilityChangeOnHand(hand, AbilityTypes.None);
    }

    private void HandleAbilityChangeOnHand(PlayerHand hand, AbilityTypes type)
    {
        hand.GetComponentInChildren<Animator>().SetTrigger("StealAbility");
        switch (type)
        {
            case AbilityTypes.None:
                hand.GetComponentInChildren<Animator>().SetTrigger(AnimationConstants.NoAbility);
                return;
            case AbilityTypes.Laser:
                hand.GetComponentInChildren<Animator>().SetTrigger(AnimationConstants.LaserAbility);
                return;
            case AbilityTypes.Grenade:
                hand.GetComponentInChildren<Animator>().SetTrigger(AnimationConstants.GrenadeAbility);
                return;
        }
    }

    private void HandlePerformingAbility(PlayerHand hand)
    {
        hand.GetShaker()?.StopDurableShaking();
        hand.GetShaker()?.StartRapidShaking();
    }

    private void HandleChargingAbility(PlayerHand hand)
    {
        hand.GetShaker()?.StartDurableShaking();   
    }

    private void HandleEndChargingAbility(PlayerHand hand)
    {
        hand.GetShaker()?.StopDurableShaking();
    }
}
