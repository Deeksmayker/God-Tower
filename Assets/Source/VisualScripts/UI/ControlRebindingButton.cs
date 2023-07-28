using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ControlRebindingButton : MonoBehaviour
{
    public Button Button;
    
    [HideInInspector] public UnityEvent OnStartRebinding;
    [HideInInspector] public UnityEvent OnEndRebinding;
    
    [SerializeField] private TextMeshProUGUI bindingText;
    [Space] 
    [SerializeField] private InputActionReference inputActionReference;
    
    private InputActionRebindingExtensions.RebindingOperation _rebindingOperation;

    private void OnEnable()
    {
        Button.onClick.AddListener(StartRebinding);
        
        bindingText.text = inputActionReference.action.controls[0].displayName;
    }

    private void OnDisable()
    {
        Button.onClick.RemoveListener(StartRebinding);
    }

    private void StartRebinding()
    {
        OnStartRebinding?.Invoke();
      

        _rebindingOperation = inputActionReference.action.PerformInteractiveRebinding()
            .OnMatchWaitForAnother(0.1f)
            .OnComplete(operation => {OnCompleteRebinding();})
            .Start();
    }

    private void OnCompleteRebinding()
    {
        _rebindingOperation.Dispose();
        bindingText.text = inputActionReference.action.controls[0].displayName;
        
        OnEndRebinding?.Invoke();
    }
}
