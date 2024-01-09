using UnityEngine;

public class WindArea : MonoBehaviour{
    [SerializeField] private float windPower;
    
    public Vector3 GetDirection(){
        return transform.up * windPower;
    }
}
