using UnityEngine;
using System.Collections;

public class DieOnHit : MonoBehaviour{
    private bool _dead;

    private void OnEnable(){
        var victims = GetComponentsInChildren<ITakeHit>();
        for (int i = 0; i < victims.Length; i++){
            victims[i].OnTakeHit += Die;
        }
    }
    
    private void OnDisable(){
        var victims = GetComponentsInChildren<ITakeHit>();
        for (int i = 0; i < victims.Length; i++){
            victims[i].OnTakeHit -= Die;
        }
    }
    
    private void Die(float dmg){
    	if (_dead) return;
		_dead = true;

        var meshes = GetComponentsInChildren<MeshRenderer>();
        for (var i = 0; i < meshes.Length; i++){
            meshes[i].gameObject.AddComponent<Death>();
        }
        
        var colliders = GetComponentsInChildren<Collider>();
        for (var i = 0; i < colliders.Length; i++){
            colliders[i].enabled = false;
        }
        
        StartCoroutine(WaitAndDestroyMyself());
    }

    private IEnumerator WaitAndDestroyMyself(){
        yield return new WaitForSeconds(1);
        Destroy(gameObject);
    }

}
