using UnityEngine;

public class Grid : MonoBehaviour{
    public static Grid Instance;

    [SerializeField] private int fieldWidth, fieldHeight;
    [SerializeField] private GridBlock gridBlock;
    [SerializeField] private float blockSize;
    public GridBlock[] grid;
    public GridBlock PlayerGrid {get; private set;}
    
    private void Awake(){
        if (Instance && Instance != this){
            Destroy(this);
            return;
        }
        Instance = this;
    }   
    
    private void Update(){
        if (Physics.Raycast(PlayerLocator.Instance.GetPlayerPos(), Vector3.down, out var hit, 100, Layers.Environment)){
            if (hit.transform.TryGetComponent<GridBlock>(out var block)){
                PlayerGrid = grid[block.index];
            }
        }
    }
    
    public Vector3 GetMoveDirection(Vector3 startPosition, Vector3 wishDirection){
        //Now only horizontal move along grid, entity picks start height by themself
        wishDirection.y = 0;
        var currentBlockIndex = GetBlockIndexAtPosition(startPosition);
        
        if (IsWishPositionValid(startPosition + wishDirection, currentBlockIndex)){ //Original direction is cool
            return wishDirection;
        } else{
            var wishMoveDistance = wishDirection.magnitude;
            
            var possibleDirections = GetPossibleMoveDirections(wishDirection);
            for (int i = 0; i < possibleDirections.Length; i++){
                if (possibleDirections[i] != Vector3.zero && IsWishPositionValid(startPosition + possibleDirections[i] * wishMoveDistance, currentBlockIndex)){
                    return possibleDirections[i] * wishMoveDistance;
                }
            }
            
            return Vector3.zero;
        }
    }
    
    //@TODO consider gravity
    private bool IsWishPositionValid(Vector3 positionToCheckBlockBelow, int currentBlockIndex){
        return Physics.Raycast(positionToCheckBlockBelow, Vector3.down, out var hit, 100, Layers.Environment)
            && hit.transform.TryGetComponent<GridBlock>(out var block)
            && (!block.Occupied || block.index == currentBlockIndex);
    }
    
    private Vector3[] GetPossibleMoveDirections(Vector3 wishDirection){
        wishDirection = wishDirection.normalized;
        var directions = new Vector3[] 
                         {
                            new Vector3(Mathf.Ceil(wishDirection.x), 0, (int)wishDirection.z),
                            new Vector3((int)wishDirection.x, 0, Mathf.Ceil(wishDirection.z)),
                            new Vector3(Mathf.Ceil(wishDirection.x), 0, Mathf.Ceil(wishDirection.z)),
                            new Vector3(Mathf.Round(wishDirection.x), 0, (int)wishDirection.z),
                            new Vector3((int)wishDirection.x, 0, Mathf.Round(wishDirection.z)),
                            new Vector3(Mathf.Round(wishDirection.x), 0, Mathf.Round(wishDirection.z)),
                            new Vector3(1, 0, 0),
                            new Vector3(0, 0, 1),
                            new Vector3(-1, 0, 0),
                            new Vector3(0, 0, -1)
                         };
        return directions;
    }
    
    private int GetBlockIndexAtPosition(Vector3 position){
        //@TODO consider gravity
        if (Physics.Raycast(position, Vector3.down, out var hit, 100, Layers.Environment)
            && hit.transform.TryGetComponent<GridBlock>(out var block)){
                return block.index;
            }
        return -1;
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
                                                   0,
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
