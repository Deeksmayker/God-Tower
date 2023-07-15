using NTC.Global.Cache;
using UnityEngine;

public class DifficultyRelatedObject : MonoCache
{
    [SerializeField] private bool appearOnEasy = true;
    [SerializeField] private bool appearOnNormal = true;

    private void Start()
    {
        if (DifficultyData.CurrentDifficulty == DifficultyData.Difficulties.Easy && !appearOnEasy)
        {
            gameObject.SetActive(false);
            return;
        }

        if (DifficultyData.CurrentDifficulty == DifficultyData.Difficulties.Normal && !appearOnNormal)
        {
            gameObject.SetActive(false);
            return;
        }
    }
}