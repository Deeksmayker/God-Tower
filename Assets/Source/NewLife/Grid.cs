using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

public class Grid : MonoBehaviour{
    public static Grid Instance;

    [SerializeField] private int fieldWidth, fieldHeight;
    [SerializeField] private GridBlock gridBlock;
    [SerializeField] private float blockSize;
    public GridBlock[] grid;
    public GridBlock PlayerGrid {get; private set;}
    
    private float _t;
    
    private Func<float, float>[] _functions = new Func<float, float>[] {(t) => t, (t) => t * t, (t) => Mathf.Sqrt(t)};
    private List<MoveBlockTask> _moveBlockTasks = new();
    
    private void Awake(){
        if (Instance && Instance != this){
            Destroy(this);
            return;
        }
        
        /*
        grid = FindObjectsOfType<GridBlock>();        
        for (int i = 0; i < grid.Length; i++){
            grid[i].index = i;
        }
        */
        
        Instance = this;
    }   
    
    private void Update(){
        if (Physics.Raycast(PlayerLocator.Instance.GetPlayerPos(), Vector3.down, out var hit, 100, Layers.Environment)){
            if (hit.transform.TryGetComponent<GridBlock>(out var block)){
                PlayerGrid = grid[block.index];
            }
        }
        
        /*
        _t += Time.deltaTime / 5;
        for (int i = 0; i < 20; i++){
            var originalPosition = new Vector3(grid[i].transform.position.x, 0, grid[i].transform.position.z);
            grid[i].transform.position = Vector3.Lerp(originalPosition, originalPosition + Vector3.up * 5, _t);
            
            originalPosition = new Vector3(grid[399-i].transform.position.x, 0, grid[399-i].transform.position.z);
            grid[399-i].transform.position = Vector3.Lerp(originalPosition, originalPosition + Vector3.up * 5, _t);
        }
        */
        
        for (int i = 0; i < _moveBlockTasks.Count; i++){
            var task = _moveBlockTasks[i];
            task.t += Time.deltaTime / task.duration;
            grid[task.blockIndex].transform.position = Vector3.Lerp(task.startPosition, task.endPosition, task.lerpFunction(task.t));
            
            if (task.t >= 1){
                _moveBlockTasks.RemoveAt(i);
            }
        }
    }
    
    public Vector3 GetMoveDirection(Vector3 startPosition, Vector3 wishDirection){
        //Now only horizontal move along grid, entity picks start height by themself
        wishDirection.y = 0;
        var currentBlockIndex = GetBlockIndexAtPosition(startPosition);
        
        if (IsWishPositionValid(startPosition, wishDirection, currentBlockIndex)){ //Original direction is cool
            return wishDirection;
        } else{
            var wishMoveDistance = wishDirection.magnitude;
            
            var possibleDirections = GetPossibleMoveDirections(wishDirection);
            for (int i = 0; i < possibleDirections.Length; i++){
                if (possibleDirections[i] != Vector3.zero && IsWishPositionValid(startPosition, possibleDirections[i] * wishMoveDistance, currentBlockIndex)){
                    return possibleDirections[i] * wishMoveDistance;
                }
            }
            
            return Vector3.zero;
        }
    }
    
    //@TODO consider gravity
    private bool IsWishPositionValid(Vector3 startPosition, Vector3 direction, int currentBlockIndex){
        if (Physics.Raycast(startPosition, direction.normalized, direction.magnitude, Layers.Environment)){
            return false;
        }
        return Physics.Raycast(startPosition + direction, Vector3.down, out var hit, 100, Layers.Environment)
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
    
    public GridBlock GetBlockAtPosition(Vector3 position, Vector3 gravity){
        if (Physics.Raycast(position, gravity, out var hit, 100, Layers.Environment) && hit.transform.TryGetComponent<GridBlock>(out var block)){
            return block;
        }
        return null;
    }
    
    public void MoveBlock(int index, float distance1, float time){
        _moveBlockTasks.Add(new MoveBlockTask(){blockIndex = index,
                                                duration = UnityEngine.Random.Range(time-1, time+1),
                                                startPosition = grid[index].transform.position,
                                                lerpFunction = _functions[UnityEngine.Random.Range(0, _functions.Length)]});
        _moveBlockTasks.Last().endPosition = _moveBlockTasks.Last().startPosition + Vector3.up * distance1;
    }
    
    [ContextMenu("Populate Area")]
    public void SpawnGrid(){
        for (int i = 0; i < grid.Length; i++){
            DestroyImmediate(grid[i], true);
        }
        
        grid = new GridBlock[fieldWidth * fieldHeight];
        for (int i = 0; i < fieldWidth; i++){
            for (int j = 0; j < fieldHeight; j++){
                grid[i * fieldWidth + j] = Instantiate(gridBlock);
                grid[i * fieldWidth + j].transform.position = new Vector3(
                                                   i * blockSize - blockSize * (fieldWidth/2),
                                                   -grid[i * fieldWidth + j].transform.localScale.y * 0.5f,
                                                   j * blockSize - blockSize * (fieldHeight/2));
                grid[i * fieldWidth + j].transform.parent = transform;
                grid[i * fieldWidth + j].index = i * fieldWidth + j;
                /*
                var collider = grid[i * fieldWidth + j].GetComponent<BoxCollider>();
                collider.center = new Vector3(collider.center.x, -grid[i * fieldWidth + j].transform.position.y/grid[i * fieldWidth + j].transform.localScale.y, collider.center.z);
                */
            }
        }
    }
}

public class MoveBlockTask{
    public int blockIndex;
    public float duration, t;
    public Vector3 endPosition;
    public Vector3 startPosition;
    public Func<float, float> lerpFunction;
}
