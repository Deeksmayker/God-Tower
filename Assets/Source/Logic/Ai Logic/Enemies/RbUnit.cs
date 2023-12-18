using UnityEngine;
using NTC.Global.Cache;

public class RbUnit : MonoCache{
    private Rigidbody[] _rbs;

    private void Awake(){
        _rbs = GetComponentsInChildren<Rigidbody>();
    }

    public void AddVelocity(Vector3 addedVelocity){
        for (var i = 0; i < _rbs.Length; i++){
            _rbs[i].velocity += addedVelocity;
        }
    }

    public void SetVelocity(Vector3 newVelocity){
        Debug.Log("SET");
        for (var i = 0; i < _rbs.Length; i++){
            _rbs[i].velocity = newVelocity;
        }
    }
}
