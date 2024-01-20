using UnityEngine;

public class SnapToGridOnSpawn : MonoBehaviour{
    [SerializeField] private float additionalVerticalPosition;
    
    private void Start(){
        //@TODO consider gravity
        GridBlock block = Grid.Instance.GetBlockAtPosition(transform.position, Vector3.down);
        if (!block) return;
        transform.position = new Vector3(block.transform.position.x,
                          block.transform.position.y + block.transform.localScale.y * 0.5f + additionalVerticalPosition * transform.localScale.y,
                                         block.transform.position.z);
    }
}
