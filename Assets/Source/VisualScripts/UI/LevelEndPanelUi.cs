using NTC.Global.Cache;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class LevelEndPanelUi : MonoCache
{
    [SerializeField] private int levelToLoad;
    [SerializeField] private GameObject levelEndPanel;
    [SerializeField] private TextMeshProUGUI timeTextMesh;

    [Inject] private LevelStatisticsManager _levelStatistics;
    [Inject] private PlayerInput _input;

    protected override void OnEnabled()
    {
        PlayerUnit.OnLevelEnd += HandleLevelEnded;
    }

    protected override void OnDisabled()
    {
        PlayerUnit.OnLevelEnd -= HandleLevelEnded;
    }

    protected override void Run()
    {
        if (!PlayerUnit.LevelEnded() || TimeController.Instance.IsPaused)
            return;

        if (_input.actions["RightAttack"].WasPressedThisFrame())
            GameManager.Instance.LoadScene(levelToLoad);
    }

    public void HandleLevelEnded()
    {
        levelEndPanel.SetActive(true);
        timeTextMesh.text = TimeSpan.FromSeconds(_levelStatistics.GetLevelTime()).ToString("mm':'ss':'fff");
    }
}