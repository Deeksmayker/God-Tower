using System;
using NTC.Global.Cache;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class HealthHud : MonoCache
{
    [SerializeField] private Slider healthSlider;
    
    [Inject] private PlayerUnit _player;

    private IHealthHandler _playerHealthHandler;

    private void Awake()
    {
        _playerHealthHandler = _player.Get<IHealthHandler>();

        if (_playerHealthHandler == null)
        {
            Debug.LogError("No health handler on player");
            healthSlider.value = 228;
            Destroy(this);
        }

        if (healthSlider != null)
        {
            healthSlider.value = _playerHealthHandler.GetHealth();
        }
    }

    protected override void OnEnabled()
    {
        _playerHealthHandler.OnHealthChanged += HandleHealthChange;
    }
    
    protected override void OnDisabled()
    {
        _playerHealthHandler.OnHealthChanged -= HandleHealthChange;
    }

    private void HandleHealthChange(float newValue)
    {
        if (healthSlider != null)
        {
            healthSlider.value = newValue;
        }
        else
        {
            Debug.Log("no health slider attached");
        }
    }
}
