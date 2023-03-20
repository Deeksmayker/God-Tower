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
            _playerHands[i].OnAbilityPerformed += HandlePerformingAbility;
            _playerHands[i].OnAbilityCharging += HandleChargingAbility;
        }
    }

    protected override void OnDisabled()
    {
        for (var i = 0; i < _playerHands.Length; i++)
        {
            _playerHands[i].OnAbilityPerformed += HandlePerformingAbility;
            _playerHands[i].OnAbilityCharging -= HandleChargingAbility;
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
