using UnityEngine;

public class Grid : MonoBehaviour{
    public static Grid Instance;

    [SerializeField] private int fieldWidth, fieldHeight;
    [SerializeField] private GridBlock gridBlock;
    [SerializeField] private float blockSize;
    public GridBlock[] grid;
    public GridBlock PlayerGrid {get; private set;}
    
    private PlayerLocator _playerLocator;
    
    private void Awake(){
        if (Instance && Instance != this){
            Destroy(this);
            return;
        }
        Instance = this;
    
        _playerLocator = GetComponent<PlayerLocator>();
    }   
    
    private void Update(){
        if (Physics.Raycast(_playerLocator.GetPlayerPos(), Vector3.down, out var hit, 100, Layers.Environment)){
            if (hit.transform.TryGetComponent<GridBlock>(out var block)){
                PlayerGrid = grid[block.index];
            }
        }
    }
    
    [ContextMenu("Populate Area")]
    public void SpawnGrid(){
        for (int i = 0; i < grid.Length; i++){
            DestroyImmediate(grid[i], true);
        }
        
        grid = new GridBlock[fieldWidth * fieldHeight];
        for (int i = 0; i < fieldWidth; i++){
            for (int j = 0; j < fieldHeight; j++){
                grid[i * fieldWidth + j] = Instantiate(gridBlock, new Vector3(
                                                   i * blockSize - blockSize * (fieldWidth/2),
                                                   Random.Range(-0.2f, 0.2f),
                                                   j * blockSize - blockSize * (fieldHeight/2)),
                                        Quaternion.identity);
                grid[i * fieldWidth + j].transform.parent = transform;
                grid[i * fieldWidth + j].index = i * fieldWidth + j;
                var collider = grid[i * fieldWidth + j].GetComponent<BoxCollider>();
                collider.center = new Vector3(collider.center.x, -grid[i * fieldWidth + j].transform.position.y/grid[i * fieldWidth + j].transform.localScale.y, collider.center.z);
            }
        }
    }
}
