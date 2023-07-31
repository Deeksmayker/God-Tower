using NTC.Global.Cache;
using UnityEngine;

public class GridWaterPopulator : MonoCache
{
    [SerializeField] private GameObject waterPrefab;
    [SerializeField] private float prefabSize = 50;
    [SerializeField] private int gridSize = 10;

    private void Start()
    {
        
    }

    [ContextMenu("Spawn Water Blocks")]
    private void PopulateGrid()
    {


        for (var i = -gridSize / 2; i <= gridSize / 2; i++)
        {
            for (var j = -gridSize; j <= gridSize / 2; j++)
            {
                Instantiate(waterPrefab, new Vector3(i * prefabSize - 1f, 0, j * prefabSize - 1f), Quaternion.identity);
            }
        }
    }
}