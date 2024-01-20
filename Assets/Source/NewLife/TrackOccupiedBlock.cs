using UnityEngine;

public class TrackOccupiedBlock : MonoBehaviour{
    private GridBlock _currentBlock;

    private void Update(){
        //@TODO check gravity
        if (Physics.Raycast(transform.position, Vector3.down, out var hit, 100, Layers.Environment)
            && hit.transform.TryGetComponent<GridBlock>(out var block)){
            if (_currentBlock) _currentBlock.Occupied = false;
            _currentBlock = block;
            _currentBlock.Occupied = true;
        }
    }
}
