using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ControlSettingsUi : MonoBehaviour
{
    [SerializeField] private GameObject waitingInputPanel;
    [SerializeField] private ControlRebindingButton[] controlRebindingButtons;

    private void Start()
    {
        for (var i = 0; i < controlRebindingButtons.Length; i++)
        {
            controlRebindingButtons[i].OnStartRebinding.AddListener(()=>waitingInputPanel.SetActive(true));
            controlRebindingButtons[i].OnEndRebinding.AddListener(()=>waitingInputPanel.SetActive(false));
        }
    }
}
