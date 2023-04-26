using System;
using Cysharp.Threading.Tasks;
using NTC.Global.Cache;
using UnityEngine;
using UnityEngine.Rendering.Universal;
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

    [Header("Abilities")]
    [ColorUsageAttribute(false, true)]
    [SerializeField] private Color noAbilityColor;
    [ColorUsageAttribute(false, true)]
    [SerializeField] private Color laserAbilityColor;
    [ColorUsageAttribute(false, true)]
    [SerializeField] private Color grenadeAbilityColor;

    private PlayerHand[] _playerHands;
    private MaterialPropertyBlock _propertyBlock;

    private void Awake()
    {
        _playerHands = GetComponentsInChildren<PlayerHand>();
        _propertyBlock = new MaterialPropertyBlock();
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
        hand.HandAnimator.SetTrigger("StealAbility");
        switch (type)
        {
            case AbilityTypes.None:
                ChangeHandAnimation(hand, AnimationConstants.NoAbility);
                ChangeHandColor(hand, noAbilityColor);
                return;
            case AbilityTypes.Laser:
                ChangeHandAnimation(hand, AnimationConstants.LaserAbility);
                ChangeHandColor(hand, laserAbilityColor);
                return;
            case AbilityTypes.Grenade:
                ChangeHandAnimation(hand, AnimationConstants.GrenadeAbility);
                ChangeHandColor(hand, grenadeAbilityColor);
                return;
        }
    }

    private async UniTask ChangeHandAnimation(PlayerHand hand, string abilityAnimationName)
    {
        hand.HandAnimator.SetBool(abilityAnimationName, true);
        await UniTask.Delay(500);
        hand.HandAnimator.SetBool(abilityAnimationName, false);
    }

    private void ChangeHandColor(PlayerHand hand, Color color)
    {
        hand.HandMeshRenderer.GetPropertyBlock(_propertyBlock);
        _propertyBlock.SetColor("_Color", color);
        hand.HandMeshRenderer.SetPropertyBlock(_propertyBlock);
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
