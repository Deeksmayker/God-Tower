using UnityEngine;

public class LevelRequiredObject : MonoBehaviour
{
    [SerializeField] private LevelData.Levels requiredLevel;

    private void Awake()
    {
        gameObject.SetActive(LevelsManager.GetLevelData(requiredLevel).Record != 0);
    }
}