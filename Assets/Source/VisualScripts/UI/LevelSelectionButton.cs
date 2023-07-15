using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectionButton : MonoBehaviour
{
    [SerializeField] private LevelData.Levels level;
    [SerializeField] private TextMeshProUGUI recordTextMesh;
    [SerializeField] private TextMeshProUGUI rankTextMesh;

    private Button _button;

    private void OnEnable()
    {
        _button = GetComponent<Button>();

        var levelData = LevelsManager.GetLevelData(level);

        _button.interactable = levelData.Avaliable;

        recordTextMesh.text = TimeSpan.FromSeconds(levelData.Record).ToString("mm':'ss':'fff");
        rankTextMesh.text = levelData.GetRankTextByRecord(false);
    }
}