using UnityEngine;

public class RealBallShooter : MonoBehaviour{
    [SerializeField] private RealBall ball;
    [SerializeField] private float timeToFullCharge = 3f;
    
    private float _currentPower;
    
    private LineRenderer _lr;
    
    private void Awake(){
        _lr = GetComponent<LineRenderer>();
        _lr.positionCount = 0;
    }
    
    private void Update(){
        if (Input.GetMouseButton(1)){
            _currentPower += Time.deltaTime / timeToFullCharge;
            _currentPower = Mathf.Clamp01(_currentPower);
        
            var imaginaryBall = Instantiate(ball);
            imaginaryBall.SetPowerMultiplier(Mathf.Sqrt(_currentPower));
            imaginaryBall.BallIsReal = false;
            var calculatedPositions = imaginaryBall.PredictPositions(Camera.main.transform.position + Camera.main.transform.forward * 2, Camera.main.transform.forward);
            
            _lr.positionCount = calculatedPositions.Length;
            _lr.SetPositions(calculatedPositions);
            Destroy(imaginaryBall.gameObject);
            
            if (Input.GetKeyDown(KeyCode.F)){
                var spawnedBall = Instantiate(ball, Camera.main.transform.position + Camera.main.transform.forward * 2, Quaternion.identity);
                spawnedBall.SetPowerMultiplier(Mathf.Sqrt(_currentPower));
                _currentPower = 0;
                //spawnedBall.HandleKick(Camera.main.transform.forward);
            }
        } else if (Input.GetMouseButtonUp(1)){
            _lr.positionCount = 0;
        } else{
            _currentPower -= Time.deltaTime / timeToFullCharge;
            _currentPower = Mathf.Clamp01(_currentPower);
        }
        
        var ballsInRadius = Physics.OverlapSphere(transform.position, 7, Layers.PlayerProjectile);
        for (int i = 0; i < ballsInRadius.Length; i++){
            if (ballsInRadius[i].TryGetComponent<RealBall>(out var ball) && ball.CanBeCollected()){
                GetComponentInParent<IMover>().SetVerticalVelocity(50);
                Destroy(ball.gameObject);
            }
        }
    }
    
}
