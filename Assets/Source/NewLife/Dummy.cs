using UnityEngine;

public class Dummy : MonoBehaviour{
    private ITakeHit _hitTakers;
    
    private void OnEnable(){
        var hitTakers = GetComponentsInChildren<ITakeHit>();
        
        for (int i = 0; i < hitTakers.Length; i++){
            hitTakers[i].OnTakeHit += HandleHit;
        }
    }
    
    private void OnDisable(){
        var hitTakers = GetComponentsInChildren<ITakeHit>();
        
        for (int i = 0; i < hitTakers.Length; i++){
            hitTakers[i].OnTakeHit -= HandleHit;
        }
    }
    
    private void HandleHit(float dmg){
        var pos = Random.insideUnitSphere * 50;
        pos.y = Mathf.Clamp(pos.y, 1, 50);
        transform.position = pos;
        transform.rotation = Quaternion.LookRotation((FindObjectOfType<PlayerUnit>().transform.position - transform.position).normalized);
    }
}
