using UnityEngine;

public class GravtiyChecker : MonoBehaviour{
    private float _checkRadius = 1f;
    
    public Vector3 CurrentGravity {get; private set;}
    
    private void Awake(){
        CurrentGravity = Vector3.down;
    }
    
    private void Start(){
        _checkRadius *= transform.localScale.y;
    }

    private void Update(){
        var areas = Physics.OverlapSphere(transform.position, _checkRadius, Layers.Areas);
        var foundGravityArea = false;
        
        for (int i = 0; i < areas.Length; i++){
            if (areas[i].TryGetComponent<GravityArea>(out var gravityArea)){
                foundGravityArea = true;
                CurrentGravity = -gravityArea.transform.up;
            }
        }
        
        if (!foundGravityArea){
            CurrentGravity = Vector3.down;
        }
        
        transform.parent.rotation = Quaternion.LookRotation(transform.parent.forward, -CurrentGravity);
    }
}
