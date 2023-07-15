using NTC.Global.Cache;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class LevelEndPanelUi : MonoCache
{
    [SerializeField] private AudioClip newRecordClip;
    [SerializeField] private int levelToLoad;
    [SerializeField] private GameObject levelEndPanel;
    [SerializeField] private GameObject newRecordPanel;
    [SerializeField] private TextMeshProUGUI timeTextMesh;
    [SerializeField] private TextMeshProUGUI recordTextMesh;
    [SerializeField] private TextMeshProUGUI rankTextMesh;

    [Inject] private LevelStatisticsManager _levelStatistics;
    [Inject] private PlayerInput _input;

    private AudioSource _audioSource;

    protected override void OnEnabled()
    {
        _audioSource = GetComponent<AudioSource>();

        LevelStatisticsManager.OnLevelEnded += HandleLevelEnded;
        LevelStatisticsManager.OnNewRecord += HandleNewRecord;
    }

    protected override void OnDisabled()
    {
        LevelStatisticsManager.OnLevelEnded -= HandleLevelEnded;
        LevelStatisticsManager.OnNewRecord -= HandleNewRecord;
    }

    protected override void Run()
    {
        if (!PlayerUnit.LevelEnded() || TimeController.Instance.IsPaused)
            return;

        if (_input.actions["LeftAttack"].WasPressedThisFrame())
            GameManager.Instance.LoadScene(levelToLoad);
    }

    public void HandleLevelEnded()
    {
        levelEndPanel.SetActive(true);
        timeTextMesh.text = TimeSpan.FromSeconds(_levelStatistics.GetLevelTime()).ToString("mm':'ss':'fff");
        recordTextMesh.text = TimeSpan.FromSeconds(_levelStatistics.GetCurrentLevelData().Record).ToString("mm':'ss':'fff");
        rankTextMesh.text = _levelStatistics.GetCurrentLevelData().GetRankTextByTime(_levelStatistics.GetLevelTime(), true);
    }

    public void HandleNewRecord()
    {
        newRecordPanel.SetActive(true);
        recordTextMesh.text = TimeSpan.FromSeconds(_levelStatistics.GetCurrentLevelData().Record).ToString("mm':'ss':'fff");

        if (newRecordClip)
            _audioSource.PlayOneShot(newRecordClip);
    }
}