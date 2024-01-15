using UnityEngine;

public class RealBallShooter : MonoBehaviour{
    [SerializeField] private RealBall ball;
    [SerializeField] private float timeToFullCharge = 3f;
    [SerializeField] private float ballReloadTime;
    
    private int _maxBallsCount = 3;
    private int _currentBallsCount = 3;
    
    private float _reloadTimer;
    
    private float _currentPower;
    private float _timeSinceAiming;
    
    private LineRenderer _lr;
    
    private void Awake(){
        _lr = GetComponent<LineRenderer>();
        _lr.positionCount = 0;
    }
    
    private void Update(){
        if (_currentBallsCount < _maxBallsCount){
            _reloadTimer += Time.deltaTime;
            if (_reloadTimer >= ballReloadTime){
                _currentBallsCount++;
                _reloadTimer -= ballReloadTime;
            }
        }
        if (_currentBallsCount == _maxBallsCount){
            _reloadTimer = 0;
        }
    
        if (_currentBallsCount > 0 && Input.GetMouseButton(1)){
            _timeSinceAiming = 0;
            _currentPower += Time.deltaTime / timeToFullCharge;
            _currentPower = Mathf.Clamp01(_currentPower);
        
            var imaginaryBall = Instantiate(ball);
            imaginaryBall.SetPowerMultiplier(Mathf.Sqrt(_currentPower));
            imaginaryBall.BallIsReal = false;
            var calculatedPositions = imaginaryBall.PredictPositions(Camera.main.transform.position + Camera.main.transform.forward * 2, Camera.main.transform.forward);
            
            _lr.positionCount = calculatedPositions.Length;
            _lr.SetPositions(calculatedPositions);
            Destroy(imaginaryBall.gameObject);
            
        } else if (Input.GetMouseButtonUp(1)){
            _lr.positionCount = 0;
        } else{
            _timeSinceAiming += Time.deltaTime;
            _currentPower -= Time.deltaTime / timeToFullCharge;
            _currentPower = Mathf.Clamp01(_currentPower);
        }
        
        if (_currentBallsCount > 0 && Input.GetKeyDown(KeyCode.F) && _timeSinceAiming < 0.5f){
            var spawnedBall = Instantiate(ball, Camera.main.transform.position + Camera.main.transform.forward * 2, Quaternion.identity);
            spawnedBall.SetPowerMultiplier(Mathf.Sqrt(_currentPower));
            _currentPower = 0;
            _currentBallsCount--;
        }

        if (Input.GetMouseButton(1)){
            //Collect balls
            var ballsInRadius = Physics.OverlapSphere(transform.position, 7, Layers.PlayerProjectile);
            for (int i = 0; i < ballsInRadius.Length; i++){
                if (ballsInRadius[i].TryGetComponent<RealBall>(out var ball) && ball.CanBeCollected()){
                    _currentBallsCount++;
                    _currentBallsCount = Mathf.Min(_currentBallsCount, _maxBallsCount);
                    Debug.Assert(_currentBallsCount > 0);
                    GetComponentInParent<IMover>().SetVerticalVelocity(30);
                    
                    CameraService.Instance.AddFovOnTime(30);
                    
                    Destroy(ball.gameObject);
                }
            }
        }
    }
    
}
