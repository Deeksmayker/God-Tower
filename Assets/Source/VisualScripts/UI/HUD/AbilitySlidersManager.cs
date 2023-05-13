using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using NTC.Global.Cache;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class AbilitySlidersManager : MonoCache
{
    [SerializeField] private Slider rightAbilitySlider;
    [SerializeField] private Slider leftAbilitySlider;

    [Inject] private PlayerUnit _player;
    private AbilitiesHandler abilitiesHandler;


    private Coroutine _checkingRightAbilityLifetime;
    private Coroutine _checkingLeftAbilityLifetime;
    
    private void Start()
    {
        abilitiesHandler = _player.Get<AbilitiesHandler>();
        
        rightAbilitySlider.gameObject.SetActive(false);
        leftAbilitySlider.gameObject.SetActive(false);
        
        abilitiesHandler.OnNewRightAbility += () =>
        {
            rightAbilitySlider.value =
                    rightAbilitySlider.maxValue = abilitiesHandler.GetRightAbility().GetMaxLifetime();
            
            if (_checkingRightAbilityLifetime != null) 
                StopCoroutine(_checkingRightAbilityLifetime);
            _checkingRightAbilityLifetime = StartCoroutine(
                StartCheckingAbilityLifetime(rightAbilitySlider, abilitiesHandler.GetRightAbility()));
            
            abilitiesHandler.GetRightAbility().OnDump += () =>
            {
                _checkingRightAbilityLifetime = null;
                rightAbilitySlider.gameObject.SetActive(false);
            };
        };
        abilitiesHandler.OnNewLeftAbility += () =>
        {
            leftAbilitySlider.value =
                    leftAbilitySlider.maxValue = abilitiesHandler.GetLeftAbility().GetMaxLifetime();
            
            if (_checkingLeftAbilityLifetime != null) 
                StopCoroutine(_checkingLeftAbilityLifetime);
            _checkingLeftAbilityLifetime = StartCoroutine(
                StartCheckingAbilityLifetime(leftAbilitySlider, abilitiesHandler.GetLeftAbility()));
            
            abilitiesHandler.GetLeftAbility().OnDump += () =>
            {
                _checkingLeftAbilityLifetime = null;
                leftAbilitySlider.gameObject.SetActive(false);
            };
        };
    }

    private IEnumerator StartCheckingAbilityLifetime(Slider slider, IActiveAbility activeAbility)
    {
        slider.gameObject.SetActive(true);
        
        while (slider.value > 0)
        {
            slider.value = activeAbility.GetRemainingLifetime();
            yield return new WaitForFixedUpdate();
        }

        slider.gameObject.SetActive(false);
    }
}
