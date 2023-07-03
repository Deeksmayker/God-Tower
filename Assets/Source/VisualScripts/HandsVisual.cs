using Code.Global.Animations;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using NTC.Global.Cache;
using NTC.Global.Pool;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.XR;

public class HandsVisual : MonoCache
{
    [SerializeField] private VisualEffect dumpLoadedEffect;

    [Header("Abilities")]
    [ColorUsageAttribute(false, true)]
    [SerializeField] private Color noAbilityColor;
    [ColorUsageAttribute(false, true)]
    [SerializeField] private Color laserAbilityColor;
    [ColorUsageAttribute(false, true)]
    [SerializeField] private Color grenadeAbilityColor;
    [ColorUsageAttribute(false, true)]
    [SerializeField] private Color homingAbilityColor;

    [Header("Hand shader settings")]
    [SerializeField] private float minStep = 0.17f;
    [SerializeField] private float maxStep = 0.27f;
    [SerializeField] private float minSpeed = 0.01f;
    [SerializeField] private float maxSpeed = 1f;

    private Color _currentColor;

    private AbilitiesHandler _abilitiesHandler;

    private PlayerHand[] _playerHands;
    private MaterialPropertyBlock _propertyBlock;

    private void Awake()
    {
        _playerHands = GetComponentsInChildren<PlayerHand>();
        _propertyBlock = new MaterialPropertyBlock();
        _abilitiesHandler = GetComponentInParent<AbilitiesHandler>();   

        _currentColor = noAbilityColor;
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
            _playerHands[i].OnHandDumpLoaded += HandleAbilityDumpLoaded;
            _playerHands[i].OnHandNewAbilityStacked += HandleNewStackedAbility;
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
            _playerHands[i].OnHandDumpLoaded -= HandleAbilityDumpLoaded;
            _playerHands[i].OnHandNewAbilityStacked -= HandleNewStackedAbility;
        }
    }

    private void HandleEmptyHandAbility(PlayerHand hand)
    {
        HandleAbilityChangeOnHand(hand, AbilityTypes.None);
        hand.HandMeshRenderer.GetPropertyBlock(_propertyBlock);
        _propertyBlock.SetFloat("_Step", minStep);
        _propertyBlock.SetFloat("_Speed", minSpeed);
        hand.HandMeshRenderer.SetPropertyBlock(_propertyBlock);
    }

    private void HandleAbilityChangeOnHand(PlayerHand hand, AbilityTypes type)
    {
        //hand.HandAnimator.SetTrigger("StealAbility");
        ChangeHandColorCoverageByLifetime(hand);
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
            case AbilityTypes.Homing:
                ChangeHandAnimation(hand, AnimationConstants.HomingAbility);
                ChangeHandColor(hand, homingAbilityColor);
                return;
        }
    }

    private void HandleNewStackedAbility(PlayerHand hand, AbilityTypes type)
    {
        var color = GetColorByType(type);
        _currentColor /= 2;
        _currentColor *= Color.white;
        ChangeHandColor(hand, _currentColor * color);

        var t = Mathf.Clamp01(_abilitiesHandler.GetStackedAbilitiesCount() / 5f);

        hand.HandMeshRenderer.GetPropertyBlock(_propertyBlock);
        _propertyBlock.SetFloat("_Step", Mathf.Lerp(minStep, maxStep, t));
        _propertyBlock.SetFloat("_Speed", Mathf.Lerp(minSpeed, maxSpeed, t));
        hand.HandMeshRenderer.SetPropertyBlock(_propertyBlock);
    }

    private void HandleAbilityDumpLoaded(PlayerHand hand, AbilityTypes type)
    {
        var handTransform = hand.transform;
        hand.GetShaker()?.StartRapidShaking();
        /*switch (type)
        {
            case AbilityTypes.Laser:
                var effect1 = NightPool.Spawn(dumpLoadedEffect, handTransform.position,
                    handTransform.rotation);
                effect1.SetVector4("Color", laserAbilityColor);
                return;
            case AbilityTypes.Grenade:
                var effect2 = NightPool.Spawn(dumpLoadedEffect, handTransform.position,
                    handTransform.rotation);
                effect2.SetVector4("Color", grenadeAbilityColor);
                return;
            case AbilityTypes.Homing:
                var effect3 = NightPool.Spawn(dumpLoadedEffect, handTransform.position,
                    handTransform.rotation);
                effect3.SetVector4("Color", homingAbilityColor);
                return;
        }*/
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

        hand.GetComponentInChildren<Light>().color = color;

        _currentColor = color;
    }

    private Color GetColorByType(AbilityTypes type)
    {
        switch (type)
        {
            case AbilityTypes.Laser:
                return laserAbilityColor;
            case AbilityTypes.Grenade:
                return grenadeAbilityColor;
            case AbilityTypes.Homing:
                return homingAbilityColor;
        }

        return noAbilityColor;
    }

    private async UniTask ChangeHandColorCoverageByLifetime(PlayerHand hand)
    {
        var ability = hand.GetHandAbility();
        var t = 1f;
       
        while (t > 0 && hand.GetHandAbility() != null && hand.GetHandAbility() == ability)
        {
            hand.HandMeshRenderer.GetPropertyBlock(_propertyBlock);
            _propertyBlock.SetFloat("_ColorCoverage", t);
            hand.HandMeshRenderer.SetPropertyBlock(_propertyBlock);
            t = ability.GetRemainingLifetime() / ability.GetMaxLifetime();
            await UniTask.NextFrame();
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
