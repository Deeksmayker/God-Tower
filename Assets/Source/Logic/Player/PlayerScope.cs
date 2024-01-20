using UnityEngine;
using NTC.Global.Cache;

public class PlayerScope : MonoCache{
    [SerializeField] private GameObject scopeObject;
    
    protected override void LateRun(){
        var ray = Camera.main.ScreenPointToRay(new Vector2(Screen.width / 2, Screen.height / 2));
        
        if (Physics.Raycast(ray, out var hit, 500, Layers.Environment | Layers.EnemyHurtBox | Layers.EnemyBase)){
            scopeObject.transform.position = Vector3.Lerp(scopeObject.transform.position, hit.point, Time.deltaTime * 50);            
        } else{
            scopeObject.transform.position = ray.origin + ray.direction * 100;
        }
    }
}
