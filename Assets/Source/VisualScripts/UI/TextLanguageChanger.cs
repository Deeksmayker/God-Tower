using NTC.Global.Cache;
using TMPro;
using UnityEngine;

public class TextLanguageChanger : MonoCache
{
    [SerializeField, TextArea] private string ruText;
    [SerializeField, TextArea] private string enText;

    private TextMeshProUGUI _uiTextMesh;
    private TextMeshPro _textMesh;

    private void Awake()
    {
        _textMesh = GetComponent<TextMeshPro>();
        _uiTextMesh = GetComponent<TextMeshProUGUI>();
    }

    protected override void OnEnabled()
    {
        SetLanguage();

        LanguageManager.OnChangeLanguage += SetLanguage;
    }

    protected override void OnDisabled()
    {
        LanguageManager.OnChangeLanguage -= SetLanguage;
    }

    private void SetLanguage()
    {
        switch (LanguageManager.CurrentLanguage)
        {
            case "RU":
                if (_textMesh)
                    _textMesh.text = ruText;
                if (_uiTextMesh)
                    _uiTextMesh.text = ruText;
                return;
            case "EN":
                if (_textMesh)
                    _textMesh.text = enText;
                if (_uiTextMesh)
                    _uiTextMesh.text = enText;
                return;
        }
    }
}