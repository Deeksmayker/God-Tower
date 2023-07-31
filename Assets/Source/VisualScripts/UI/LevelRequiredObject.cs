using UnityEngine;

public class LevelRequiredObject : MonoBehaviour
{
    [SerializeField] private bool showAnyway;
    [SerializeField] private LevelData.Levels requiredLevel;

    private void Awake()
    {
        if (showAnyway)
        {
            gameObject.SetActive(true);
            return;
        }

        gameObject.SetActive(LevelsManager.GetLevelData(requiredLevel).Record != 0);
    }
}