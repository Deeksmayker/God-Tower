using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using NTC.Global.Cache;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;
using Zenject.SpaceFighter;

public class PlayerStatisticViewObserver : MonoCache
{
    [SerializeField] private PlayerStatisticView playerStatisticView;
    
    [SerializeField] private PlayerMovementController playerMovementController;
    [SerializeField] private PlayerHealthHandler playerHealthHandler;

    private bool _isShowed;

    protected override void Run()
    {
        UpdateView();
    }
    
    public void ShowPanel()
    {
        _isShowed = true;
        
        playerStatisticView.Show();
    }

    public void HidePanel()
    {
        _isShowed = false;
        
        playerStatisticView.Hide();
    }

    public bool Showed()
    {
        return _isShowed;
    }

    private void UpdateView()
    {
        playerStatisticView.SetGroundedStateText(playerMovementController.IsGrounded().ToString());
        playerStatisticView.SetVelocityVectorText(playerMovementController.GetVelocity().ToString());
        playerStatisticView.SetSpeedValueText(playerMovementController.Speed.ToString(CultureInfo.CurrentCulture));
        playerStatisticView.SetHpValueText(playerHealthHandler.GetHealth01().ToString(CultureInfo.CurrentCulture));
    }
}
