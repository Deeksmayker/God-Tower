using System;
using NTC.Global.Cache;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public class DeathPanelUi : MonoCache
{
    [SerializeField] private GameObject deathPanel;

    [Inject] private PlayerUnit _player;

    private IHealthHandler _playerHealthHandler;

    private void Awake()
    {
        _playerHealthHandler = _player.Get<IHealthHandler>();

        if (_playerHealthHandler == null)
        {
            Debug.LogError("No health handler on player");
                Destroy(this);
        }
    }
    
    protected override void OnEnabled()
    {
        _playerHealthHandler.OnDied += HandlePlayerDied;
    }
    
    protected override void OnDisabled()
    {
        _playerHealthHandler.OnDied -= HandlePlayerDied;
    }

    private void HandlePlayerDied()
    {
        if (deathPanel == null)
        {
            Debug.Log("no death panel attached");
            return;
        }

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        deathPanel.SetActive(true);
    }

    [ContextMenu("Reload game")]
    public void ReloadGameOnDeath()
    {
        SceneManager.LoadScene(0);
    }
}
