using System.Collections;
using System.Collections.Generic;
using NTC.Global.Cache;
using TMPro;
using UnityEngine;

public class PlayerStatisticView : MonoCache
{
    [SerializeField] private TextMeshProUGUI groundedStateText;
    [SerializeField] private TextMeshProUGUI velocityVectorText;
    [SerializeField] private TextMeshProUGUI speedValueText;
    [SerializeField] private TextMeshProUGUI hpValueText;

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void SetGroundedStateText(string value)
    {
        groundedStateText.text = value;
    }

    public void SetSpeedValueText(string value)
    {
        speedValueText.text = value;
    }
    
    public void SetHpValueText(string value)
    {
        hpValueText.text = value;
    }
    
    public void SetVelocityVectorText(string value)
    {
        velocityVectorText.text = value;
    }
}
