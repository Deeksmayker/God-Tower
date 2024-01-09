using UnityEngine;

public class BulletTime : MonoBehaviour{
    [SerializeField] private float minTimeScale;
    
    private void Update(){
        if (Input.GetKey(KeyCode.LeftShift)){
            Time.timeScale = Mathf.Lerp(Time.timeScale, minTimeScale, Time.deltaTime * 4);
        } else{
            Time.timeScale = Mathf.Lerp(Time.timeScale, 1, Time.deltaTime * 10);
        }
    }
}
