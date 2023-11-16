using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsWindow : MonoBehaviour
{
    [SerializeField] private Button backButton;

    [Header("Left buttons")]
    [SerializeField] private Button graphicsButton;
    [SerializeField] private Button managementButton;
    [SerializeField] private Button soundButton;
    [SerializeField] private Button languageButton;


    private void Start()
    {
        backButton.onClick.AddListener(BackToMenu);
    }

    private void OnDestroy()
    {
        backButton.onClick.RemoveListener(BackToMenu);
    }

    private void BackToMenu() => gameObject.SetActive(false);
}
