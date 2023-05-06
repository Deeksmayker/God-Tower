using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public Action OnSceneStartLoad;
    public Action OnSceneEndLoad;

    private PlayerData _playerData;
    private GameObject _playerGameObject;

    private Dictionary<int, List<int>> _definitionSceneDictionary;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(this);
        }

        _playerGameObject = FindObjectOfType<PlayerUnit>().gameObject;
        _playerData = new PlayerData();
        _definitionSceneDictionary = new Dictionary<int, List<int>>(){ {1, new List<int>(){1}} };
    }

    /// <summary>
    /// Загружает сцену с заданым индексом.
    /// </summary>
    /// <param name="sceneIndex"> Индекс загружаемой сцены. </param>
    public void LoadScene(int sceneIndex)
    {
        OnSceneStartLoad?.Invoke();
        
        SavePlayerData();
        SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Single);
    }

    /// <summary>
    /// Загружает сцену с заданой сложностью.
    /// </summary>
    /// <param name="definition"> Сложность загружаемой сцены. </param>
    public void LoadSceneByDefinition(int definition)
    {
        var sceneIndexes = _definitionSceneDictionary[definition];
        
        if (sceneIndexes != null && sceneIndexes.Count > 0)
            LoadScene(sceneIndexes[Random.Range(0, sceneIndexes.Count)]);
    }

    private void SavePlayerData()
    {
        _playerData.SetHealthPoint(_playerGameObject.GetComponent<BaseHealthHandler>().GetHealth());
    }
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        _playerGameObject = FindObjectOfType<PlayerUnit>().gameObject;
        _playerGameObject.GetComponent<BaseHealthHandler>().SetHealth(_playerData.GetHealthPoint());

        OnSceneEndLoad?.Invoke();
    }
}