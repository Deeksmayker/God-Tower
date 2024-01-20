using UnityEngine;
using UnityEngine.UI;

public class BulletTime : MonoBehaviour{
    [SerializeField] private float minTimeScale;
    [SerializeField] private float duration = 5f;
    [SerializeField] private Slider slider;
    
    private bool _passedDuration;
    private float _holdTime;
    
    private void Update(){
        if (!_passedDuration && _holdTime < duration && Input.GetKey(KeyCode.LeftShift)){
            Time.timeScale = Mathf.Lerp(Time.timeScale, minTimeScale, Time.unscaledDeltaTime * 4);
            _holdTime += Time.unscaledDeltaTime;
            _passedDuration = false;
        } else{
            Time.timeScale = Mathf.Lerp(Time.timeScale, 1, Time.unscaledDeltaTime * 10);
            _holdTime -= Time.unscaledDeltaTime;
            _passedDuration = true;
        }
        
        if (!Input.GetKey(KeyCode.LeftShift)){
            _passedDuration = false;
        }
        
        _holdTime = Mathf.Clamp(_holdTime, 0, duration);
        slider.value = 1f - (_holdTime / duration);
    }
}
